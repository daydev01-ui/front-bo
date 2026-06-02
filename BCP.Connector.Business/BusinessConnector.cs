using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Net.Http.Headers;
using System.Text;

namespace BCP.Connector.Business
{
    public interface IBusinessConnector
    {
        Task<Response<BusinessResponse>> Get(BusinessGetRequest request, Channel channel, string username);
        Task<Response<List<BusinessResponse>>> GetAll(BusinessGetAllRequest request, Channel channel, string username);
        Task<Response<BusinessRegisterResponse>> Register(BusinessRegisterRequest request, Channel channel, string username);
        Task<Response<BusinessUpdateResponse>> Update(BusinessUpdateRequest request, Channel channel, string username);
        Task<Response<BusinessAddUserAdminResponse>> AddUserAdmin(BusinessAddUserAdminRequest request, Channel channel, string username);
    }

    public class BusinessConnector : IBusinessConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SecuritySegCript _securitySegCript;
        private readonly BaseUrl _baseUrl;
        private readonly RoutePath _routePath;

        public BusinessConnector(
            IHttpClientFactory httpClientFactory,
            IOptions<BaseUrl> baseUrl,
            IOptions<RoutePath> routePath)
        {
            _httpClientFactory = httpClientFactory;
            _securitySegCript = new SecuritySegCript();
            _baseUrl = baseUrl.Value;
            _routePath = routePath.Value;
        }

        public async Task<Response<BusinessResponse>> Get(BusinessGetRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BusinessResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetBusiness)
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
                    response = JsonConvert.DeserializeObject<Response<BusinessResponse>>(stringContent)!;
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

        public async Task<Response<List<BusinessResponse>>> GetAll(BusinessGetAllRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<List<BusinessResponse>> response = Helper.ServiceError;
            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiCompany);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.GetAllBusiness)
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
                    response = JsonConvert.DeserializeObject<Response<List<BusinessResponse>>>(stringContent);
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
                logger.Information($"Request: {JsonConvert.SerializeObject(request)} | ResponseState: {response.State} | responseMessage: {response.Message}");
                logger.Dispose(); 
            }
            return response;
        }

        public async Task<Response<BusinessRegisterResponse>> Register(BusinessRegisterRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BusinessRegisterResponse> response = Helper.ServiceError;
            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiCompany);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.RegisterCompany)
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
                    response = JsonConvert.DeserializeObject<Response<BusinessRegisterResponse>>(stringContent);
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

        public async Task<Response<BusinessUpdateResponse>> Update(BusinessUpdateRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BusinessUpdateResponse> response = Helper.ServiceError;
            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiBackOffice);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.UpdateBusiness)
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
                    response = JsonConvert.DeserializeObject<Response<BusinessUpdateResponse>>(stringContent);
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

        public async Task<Response<BusinessAddUserAdminResponse>> AddUserAdmin(BusinessAddUserAdminRequest request, Channel channel, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<BusinessAddUserAdminResponse> response = Helper.ServiceError;
            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, channel.Password, channel.SegCryptName);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",channel.Username, passwordChannel)));
                logger.Debug($"Channel: {JsonConvert.SerializeObject(channel)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_baseUrl.ApiCompany);
                httpClient.Timeout = channel.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _routePath.AddUserAdminCompany)
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
                    response = JsonConvert.DeserializeObject<Response<BusinessAddUserAdminResponse>>(stringContent);
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
            finally
            {
                logger.Information($"Request: {JsonConvert.SerializeObject(request)}, Response: {JsonConvert.SerializeObject(response)}");
                logger.Dispose();
            }
            return response;
        }
    }
}
