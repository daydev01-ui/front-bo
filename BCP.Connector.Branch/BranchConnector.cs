using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Net.Http.Headers;
using System.Text;

namespace BCP.Connector.Branch
{
    public interface IBranchConnector
    {
        Task<Response<BranchResponse>> Get(BranchGetRequest request, Channel channel, string username);
        Task<Response<List<BranchResponse>>> GetAll(BranchGetAllRequest request, Channel channel, string username);
        Task<Response<BranchRegisterResponse>> Register(BranchRegisterRequest request, Channel channel, string username);
        Task<Response<BranchUpdateResponse>> Update(BranchUpdateRequest request, Channel channel, string username);
    }

    public class BranchConnector : IBranchConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SecuritySegCript _securitySegCript;
        private readonly BaseUrl _baseUrl;
        private readonly RoutePath _routePath;

        public BranchConnector(
            IHttpClientFactory httpClientFactory,
            IOptions<BaseUrl> baseUrl,
            IOptions<RoutePath> routePath)
        {
            _httpClientFactory = httpClientFactory;
            _securitySegCript = new SecuritySegCript();
            _baseUrl = baseUrl.Value;
            _routePath = routePath.Value;
        }

        public async Task<Response<BranchResponse>> Get(BranchGetRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BranchResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetBranch)
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
                    response = JsonConvert.DeserializeObject<Response<BranchResponse>>(stringContent)!;
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
                logger.Information($"Request: {JsonConvert.SerializeObject(request)} | Response: {JsonConvert.SerializeObject(response)}");
                logger.Dispose();
            }
            return response;
        }

        public async Task<Response<List<BranchResponse>>> GetAll(BranchGetAllRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<List<BranchResponse>> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetAllBranch)
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
                    response = JsonConvert.DeserializeObject<Response<List<BranchResponse>>>(stringContent)!;
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
                logger.Information($"Request: {JsonConvert.SerializeObject(request)} | ResponseState: {response.State} | ResponseMessage: {response.Message}");
                logger.Dispose(); 
            }
            return response;
        }

        public async Task<Response<BranchRegisterResponse>> Register(BranchRegisterRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BranchRegisterResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiCompany);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.RegisterBranch)
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
                    response = JsonConvert.DeserializeObject<Response<BranchRegisterResponse>>(stringContent)!;
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

        public async Task<Response<BranchUpdateResponse>> Update(BranchUpdateRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BranchUpdateResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.UpdateBranch)
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
                    response = JsonConvert.DeserializeObject<Response<BranchUpdateResponse>>(stringContent)!;
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
