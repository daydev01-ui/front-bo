using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Station.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Station.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Net.Http.Headers;
using System.Text;

namespace BCP.Connector.Station
{
    public interface IStationConnector
    {
        Task<Response<StationResponse>> Get(StationGetRequest request, Channel channel, string username);
        Task<Response<List<StationResponse>>> GetAll(StationGetAllRequest request, Channel channel, string username);
        Task<Response<StationRegisterResponse>> Register(StationRegisterRequest request, Channel channel, string username);
        Task<Response<StationUpdateResponse>> Update(StationUpdateRequest request, Channel channel, string username);
    }

    public class StationConnector : IStationConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SecuritySegCript _securitySegCript;
        private readonly BaseUrl _baseUrl;
        private readonly RoutePath _routePath;

        public StationConnector(
            IHttpClientFactory httpClientFactory,
            IOptions<BaseUrl> baseUrl,
            IOptions<RoutePath> routePath)
        {
            _httpClientFactory = httpClientFactory;
            _securitySegCript = new SecuritySegCript();
            _baseUrl = baseUrl.Value;
            _routePath = routePath.Value;
        }

        public async Task<Response<StationResponse>> Get(StationGetRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<StationResponse> response = Helper.ServiceError;

            try
            {

                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetStation)
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
                    response = JsonConvert.DeserializeObject<Response<StationResponse>>(stringContent)!;
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

        public async Task<Response<List<StationResponse>>> GetAll(StationGetAllRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<List<StationResponse>> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetAllStation)
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
                    response = JsonConvert.DeserializeObject<Response<List<StationResponse>>>(stringContent)!;
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

        public async Task<Response<StationRegisterResponse>> Register(StationRegisterRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<StationRegisterResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiCompany);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.RegisterStation)
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
                    response = JsonConvert.DeserializeObject<Response<StationRegisterResponse>>(stringContent)!;
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

        public async Task<Response<StationUpdateResponse>> Update(StationUpdateRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<StationUpdateResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.UpdateStation)
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
                    response = JsonConvert.DeserializeObject<Response<StationUpdateResponse>>(stringContent)!;
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
