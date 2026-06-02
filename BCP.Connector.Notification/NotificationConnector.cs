using BCP.Framework;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Notification.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Notification.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QRBackoffice.Intranet.Security;
using System.Net.Http.Headers;
using System.Text;

namespace BCP.Connector.Notification
{
    public interface INotificationConnector
    {
        Task<Response<NotificationResponse>> SendAsync<TEmailData>(TEmailData request, string emails, string username);
    }

    public class NotificationConnector : INotificationConnector
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly SecuritySegCript _securitySegCript;
        private readonly NotificationOptions _notificationOptions;

        public NotificationConnector(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            NotificationOptions notificationOptions) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _securitySegCript = new SecuritySegCript();
            _notificationOptions = notificationOptions;
        }

        public async Task<Response<NotificationResponse>> SendAsync<TEmailData>(TEmailData request, string emails, string username)
        {
            string correlationId = Guid.NewGuid().ToString("N", null);
            ILogger logger = Logger.CreateContext($"{correlationId[..12]}_{username}");
            Response<NotificationResponse> response = Helper.ServiceError;

            try
            {
                string passwordChannel = _securitySegCript.EncryptDecrypt(false, _configuration["Mult:Password"], _configuration["Mult:SegCryptName"]);
                string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _configuration["Mult:Username"], passwordChannel)));

                NotificationRequest<TEmailData> notificationRequest = new()
                {
                    Target = new Target { Push = false, WhatsApp = false, Sms = false, Email = true },
                    SendType = 0,
                    Clients =
                    [
                        new() {Email = [.. emails.Split(';')], Idc = "", PhoneNumber = "" }
                    ],
                    Groups = [""],
                    Application = _notificationOptions.Application,
                    Title = _notificationOptions.PushTitle,
                    Message = _notificationOptions.PushMessage,
                    Image = "",
                    Data = new Data<TEmailData>
                    {
                        Alert = "Success",
                        EmailName = _notificationOptions.EmailName,
                        EmailFrom = _notificationOptions.EmailFrom,
                        WhatsAppMessage = "",
                        EmailDetails = request
                    },
                    Test = false,
                    PublicToken = _configuration["Mult:PublicToken"],
                    AppUserId = _configuration["Mult:AppUserId"]
                };
                logger.Debug($"RequestNotification: {JsonConvert.SerializeObject(notificationRequest)}");

                HttpClient httpClient = _httpClientFactory.CreateClient("HTTPClientWithTrustedOrUntrustedSSL");
                httpClient.BaseAddress = new Uri(_notificationOptions.Url);
                httpClient.Timeout = _notificationOptions.Timeout;
                HttpRequestMessage httpRequest = new(HttpMethod.Post, _notificationOptions.Url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(notificationRequest), Encoding.UTF8, "application/json"),
                };
                httpRequest.Headers.Add("Correlation-Id", correlationId);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorization);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                string stringContent = await httpResponse.Content.ReadAsStringAsync();
                int status = (int)httpResponse.StatusCode;
                if (status > 199 && status < 300 || status > 399 && status < 500)
                {
                    response = JsonConvert.DeserializeObject<Response<NotificationResponse>>(stringContent)!;
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
