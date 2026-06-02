using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Net.Http.Headers;
using System.Text;

namespace BCP.Connector.Segurinet
{
    public interface ISegurinetConnector
    {
        Task<Response<SegurinetResponse>> Login(LoginParameter request);
    }

    public class SegurinetConnector : ISegurinetConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly SecuritySegCript _securitySegCript;
        private readonly BaseApiOptions _baseApiOptions;

        public SegurinetConnector(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            BaseApiOptions baseApiOptions)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _securitySegCript = new SecuritySegCript();
            _baseApiOptions = baseApiOptions;
        }

        public async Task<Response<SegurinetResponse>> Login(LoginParameter request)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{request.Username}");
            Response<SegurinetResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, _configuration["Mult:Password"], _configuration["Mult:SegCryptName"]);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _configuration["Mult:Username"], passwordChannel)));

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseApiOptions.Url);
                httpClient.Timeout = _baseApiOptions.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _baseApiOptions.Url)
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
                    response = JsonConvert.DeserializeObject<Response<SegurinetResponse>>(stringContent)!;
                    if (response.State != "00")
                        logger.Error($"Request: {JsonConvert.SerializeObject(request)}, Response: {JsonConvert.SerializeObject(response)}");
                }
                else
                {
                    logger.Error($"Request.Url: {"_apiOptions.Url"}, Response.Content: {stringContent}");
                    logger.Error($"Request: {JsonConvert.SerializeObject(request)}, Response: {JsonConvert.SerializeObject(response)}");
                }
            }
            catch (TaskCanceledException canceledException)
            {
                logger.Fatal($"Message: {canceledException.Message}, Exception: {JsonConvert.SerializeObject(canceledException)}");
                response = Helper.TimeoutError;
            }
            catch (Exception exception)
            {
                logger.Fatal($"Message: {exception.Message}, Exception: {JsonConvert.SerializeObject(exception)}");
                response = Helper.StatusException;
            }
            finally {
                logger.Information($"Request: {JsonConvert.SerializeObject(request)}, Response: {JsonConvert.SerializeObject(response)}");
                logger.Dispose(); 
            }
            return response;
        }
    }
}
