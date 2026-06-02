using BCP.Connector.Business;
using BCP.Connector.Notification;
using BCP.Connector.User;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Responses;
using BCP.QRBackOffice.Models.Connectors.Notification.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.User.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.User.v1.Responses;
using BCP.QRBackOffice.Models.DTOs;
using BCP.QRBackOffice.Models.Options;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class BusinessController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBusinessConnector _businessConnector;
        private readonly IUserConnector _userConnector;
        private readonly INotificationConnector _notificationConnector;

        public BusinessController(
            IConfiguration configuration,
            IBusinessConnector businessConnector,
            IUserConnector userConnector,
            INotificationConnector notificationConnector) 
        {
            _configuration = configuration;
            _businessConnector = businessConnector;
            _userConnector = userConnector;
            _notificationConnector = notificationConnector;
        }

        [HttpGet]
        public ActionResult Index(string channelName)
        {
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
                return RedirectToAction("Index", "Auth");
            }
            HttpContext.Session.SetString("channelName", channelName);
            ViewBag.TitleChannel = channelName switch
            {
                "MULT" => "Multiplica [MULT]",
                "CRDT" => "Credinet Web [CRDT]",
                "MUBE" => "Extranet [MUBE]",
                _ => "Multiplica [Mult]"
            };
            ViewBag.ChannelName = channelName;
            ViewBag.Username = username;
            ViewBag.Role = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
                return Unauthorized(Response<List<BusinessDTO>>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            BusinessGetAllRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessChannel = channel.ChannelName,
                Channel = channel.ChannelName
            };
            Response<List<BusinessResponse>> response = await _businessConnector.GetAll(request, channel, username);
            List<BusinessDTO> businesses = [];
            if (!response.HasError())
            {
                businesses = [.. response.Data.Select(business => new BusinessDTO
                {
                    BusinessCode = business.BusinessCode,
                    BusinessName = business.BusinessName,
                    Abbreviation = business.Abbreviation,
                    IsUserAdmin = business.IsUserAdmin,
                    CreationUser = business.CreationUser,
                    CreationDate = business.CreationDate.ToString("dd MMM yyyy"),
                    ModificationUser = business.ModificationUser,
                    ModificationDate = business.ModificationDate.ToString("dd MMM yyyy"),
                    State = business.State
                })];
                return Ok(businesses);
            }
            if(response.State == "98")
            {
                return Ok(businesses);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] BusinessRegisterRequest request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (channelName is null || username is null || role is null)
            {
                return Unauthorized(Response<BusinessDTO>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.CreationUser = username;
            request.Channel = channel.ChannelName;
            Response<BusinessRegisterResponse> businessRegisterResponse = await _businessConnector.Register(request, channel, username);
            if (!businessRegisterResponse.HasError())
            {
                BusinessGetRequest businessGetRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    Channel = channel.ChannelName,
                    BusinessCode = businessRegisterResponse.Data.BusinessCode
                };
                Response<BusinessResponse> businessGetResponse = await _businessConnector.Get(businessGetRequest, channel, username);
                if (!businessGetResponse.HasError())
                {
                    BusinessDTO business = new()
                    {
                        BusinessCode = businessGetResponse.Data.BusinessCode,
                        BusinessName = businessGetResponse.Data.BusinessName,
                        Abbreviation = businessGetResponse.Data.Abbreviation,
                        CreationUser = businessGetResponse.Data.CreationUser,
                        CreationDate = businessGetResponse.Data.CreationDate.ToString("dd MMM yyyy"),
                        ModificationUser = businessGetResponse.Data.ModificationUser,
                        ModificationDate = businessGetResponse.Data.ModificationDate.ToString("dd MMM yyyy"),
                        State = businessGetResponse.Data.State
                    };
                    return Ok(business);
                }
                return BadRequest(businessGetResponse);
            }
            return BadRequest(businessRegisterResponse);
        }

        public async Task<IActionResult> Update([FromBody] BusinessUpdateRequest request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
                return Unauthorized(Response<BusinessDTO>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.BusinessChannel = channel.ChannelName;
            request.ModificationUser = username;
            request.Channel = channel.ChannelName;

            Response<BusinessUpdateResponse> response = await _businessConnector.Update(request, channel, username);
            if (!response.HasError())
            {
                BusinessDTO business = new()
                {
                    BusinessCode = request.BusinessCode,
                    BusinessName = request.BusinessName,
                    ModificationUser = username,
                    ModificationDate = DateTime.Now.ToString("dd MMM yyyy")
                };
                return Ok(business);
            }
            return BadRequest(response);
        }

        public async Task<IActionResult> AddUserAdmin([FromBody] BusinessAddAdminDTO request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (channelName is null || username is null || role is null)
            {
                return Unauthorized(Response<BusinessDTO>.Status(Helper.Unauthorized));
            }

            request.BusinessName = request.BusinessName.Trim();
            request.FullName = request.FullName.Trim();
            request.UserName = request.UserName.Trim();
            request.UserPhone = 
                request.UserPhone == string.Empty ? "99999999" : request.UserPhone;

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            Response<UserRegisterResponse> userRegisterResponse;
            string password = Helper.GeneratePassword();
            do
            {
                UserRegisterRequest userRegisterRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    BusinessCode = request.BusinessCode.ToString(),
                    RoleCode = "3",
                    TypeUser = "IAM",
                    Channel = channel.ChannelName,
                    Client = new RegisterClient
                    {
                        Name = request.FullName,
                        UserName = request.UserName,
                        Password = password,
                        Email = request.CredentialsEmail.Replace(";", ","),
                        Cellphone = request.UserPhone,
                        DocumentNumber = request.DocumentNumber,
                        DocumentType = request.DocumentType,
                        DocumentExtension = request.DocumentExtension,
                        DocumentComplement = "00"
                    },
                    Device = new Device
                    {
                        Id = "01",
                        Type = "Desktop",
                        Name = "PC T",
                        OS = "Windows 11"
                    }
                };
                userRegisterResponse = await _userConnector.Register(userRegisterRequest, channel, username);
            } while (userRegisterResponse.State == "063");
            if (!userRegisterResponse.HasError())
            {
                BusinessAddUserAdminRequest businessAddUserAdminRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    BusinessCode = request.BusinessCode,
                    Channel = channel.ChannelName,
                    UpdateUser = username,
                    UserId = userRegisterResponse.Data.Client.Id
                };

                Response<BusinessAddUserAdminResponse> businessAddUserAdminResponse = await _businessConnector.AddUserAdmin(businessAddUserAdminRequest, channel, username);
                if (!businessAddUserAdminResponse.HasError())
                {
                    EmailDetailsUserAdmin details = new()
                    {
                        NombreDeEmpresa = request.BusinessName,
                        NombreCompleto = request.FullName,
                        RolDeUsuario = "Administrador",
                        NombreDeUsuario = request.UserName,
                        ContrasenaDeUsuario = password
                    };

                    await _notificationConnector.SendAsync(details, request.CredentialsEmail, username);
                    BusinessDTO business = new()
                    {
                        BusinessCode = request.BusinessCode,
                        IsUserAdmin = true,
                        ModificationUser = username,
                        ModificationDate = DateTime.Now.ToString("dd MMM yyyy")
                    };
                    return Ok(business);
                }
                else
                {
                    return BadRequest(businessAddUserAdminResponse);
                }
            }
            return BadRequest(userRegisterResponse);
        }
    }
}