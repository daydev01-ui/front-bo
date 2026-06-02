using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Report.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Report.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;

namespace BCP.Connector.Report
{
    public interface IReportConnector
    {
        Task<ExcelReportResponse> GetExcel(ExcelReportRequest request, QRBackOffice.Models.Options.Channel channel, string username);
        Task<Response<List<EmpresaResponse>>>GetAllEmpresas(GetAllReportRequest request,QRBackOffice.Models.Options.Channel channel, string username);
    }

      public class ReportConnector : IReportConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SecuritySegCript _securitySegCript;
        private readonly BaseUrl _baseUrl;
        private readonly RoutePath _routePath;

        public ReportConnector(
            IHttpClientFactory httpClientFactory,
            IOptions<BaseUrl> baseUrl,
            IOptions<RoutePath> routePath)
        {
            _httpClientFactory = httpClientFactory;
            _securitySegCript = new SecuritySegCript();
            _baseUrl = baseUrl.Value;
            _routePath = routePath.Value;
        }

        public async Task<ExcelReportResponse> GetExcel(ExcelReportRequest request, QRBackOffice.Models.Options.Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            ExcelReportResponse result = new() { HasError = true, ErrorMessage = "Error de servicio." };

            try
            {
                // ── Autenticación Basic (igual que BusinessConnector) ───────
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{channel.Username}:{passwordChannel}"));

                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                // ── HttpClient ──────────────────────────────────────────────
                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOfficeQR);
                httpClient.Timeout = channel.Timeout;

                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetReportExcel)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                };
                httpRequest.Headers.Add("Correlation-Id", correlationId);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorization);

                // ── Llamada al servicio ─────────────────────────────────────
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                int status = (int)httpResponse.StatusCode;

                logger.Debug($"Response status: {status}");

                if (status > 199 && status < 300)
                {
                    // El servicio devuelve bytes directamente (mismo comportamiento que proyecto 1)
                    byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();

                    if (bytes.Length > 0)
                    {
                        // Leer Content-Type y Content-Disposition del response si vienen
                        string contentType = httpResponse.Content.Headers.ContentType?.MediaType
                            ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        string fileName = "REPORTE.xlsx";
                        if (httpResponse.Content.Headers.ContentDisposition?.FileNameStar != null)
                            fileName = httpResponse.Content.Headers.ContentDisposition.FileNameStar;
                        else if (httpResponse.Content.Headers.ContentDisposition?.FileName != null)
                            fileName = httpResponse.Content.Headers.ContentDisposition.FileName;

                        result = new ExcelReportResponse
                        {
                            Bytes = bytes,
                            ContentType = contentType,
                            FileName = fileName,
                            HasError = false
                        };
                    }
                    else
                    {
                        result.ErrorMessage = "El servicio respondió sin datos.";
                        logger.Error($"Request: {JsonConvert.SerializeObject(request)} | Respuesta vacía del servicio.");
                    }
                }
                else
                {
                    string errorContent = await httpResponse.Content.ReadAsStringAsync();
                    result.ErrorMessage = $"Error HTTP {status}.";
                    logger.Error($"Request: {JsonConvert.SerializeObject(request)} | StatusCode: {status} | Response: {errorContent}");
                }
            }
            catch (TaskCanceledException canceledException)
            {
                result.ErrorMessage = "Tiempo de espera agotado.";
                logger.Fatal($"Message: {canceledException.Message}, Exception: {JsonConvert.SerializeObject(canceledException)}");
            }
            catch (Exception exception)
            {
                result.ErrorMessage = "Error inesperado al generar el reporte.";
                logger.Fatal($"Message: {exception.Message}, Exception: {JsonConvert.SerializeObject(exception)}");
            }
            finally
            {
                logger.Information($"Request: {JsonConvert.SerializeObject(request)} | HasError: {result.HasError} | FileName: {result.FileName}");
                logger.Dispose();
            }

            return result;
        }
        public async Task<Response<List<EmpresaResponse>>> GetAllEmpresas(GetAllReportRequest request,QRBackOffice.Models.Options.Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<List<EmpresaResponse>> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{channel.Username}:{passwordChannel}"));

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOfficeQR);
                httpClient.Timeout = channel.Timeout;

                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetAllEmpresas)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                };
                httpRequest.Headers.Add("Correlation-Id", correlationId);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorization);

                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                string stringContent = await httpResponse.Content.ReadAsStringAsync();
                int status = (int)httpResponse.StatusCode;

                if (status > 199 && status < 300 || status > 399 && status < 500)
                {
                    response = JsonConvert.DeserializeObject<Response<List<EmpresaResponse>>>(stringContent)!;
                    if (response.State != "00")
                        logger.Error($"Response: {JsonConvert.SerializeObject(response)}");
                }
                else
                {
                    logger.Error($"StatusCode: {status} | Response: {stringContent}");
                }
            }
            catch (TaskCanceledException canceledException)
            {
                logger.Fatal($"Message: {canceledException.Message}");
                response = Helper.TimeoutError;
            }
            catch (Exception exception)
            {
                logger.Fatal($"Message: {exception.Message}");
                response = Helper.StatusException;
            }
            finally
            {
                logger.Information($"Response: {JsonConvert.SerializeObject(response)}");
                logger.Dispose();
            }

            return response;
        }
    }
}

