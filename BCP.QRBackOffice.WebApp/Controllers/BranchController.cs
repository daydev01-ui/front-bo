using BCP.Connector.Branch;
using BCP.Connector.Business;
using BCP.Connector.Notification;
using BCP.Connector.User;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses;
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
    public class BranchController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBusinessConnector _businessConnector;
        private readonly IBranchConnector _branchConnector;
        private readonly IUserConnector _userConnector;
        private readonly INotificationConnector _notificationConnector;

        public BranchController(
            IConfiguration configuration,
            IBusinessConnector businessConnector,
            IBranchConnector branchConnector,
            IUserConnector userConnector,
            INotificationConnector notificationConnector) 
        { 
            _configuration = configuration;
            _businessConnector = businessConnector;
            _branchConnector = branchConnector;
            _userConnector = userConnector;
            _notificationConnector = notificationConnector;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long businessCode)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (channelName is null || username is null || role is null)
            {
                return RedirectToAction("Index", "Auth");
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            BusinessGetRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = businessCode,
                Channel = channelName
            };
            Response<BusinessResponse> response = await _businessConnector.Get(request, channel, username);
            if (!response.HasError())
            {
                HttpContext.Session.SetString("businessCode", businessCode.ToString());
                HttpContext.Session.SetString("businessName", response.Data.BusinessName);
                ViewBag.TitleChannel = channelName switch
                {
                    "MULT" => "Multiplica [MULT]",
                    "CRDT" => "Credinet Web [CRDT]",
                    "MUBE" => "Extranet [MUBE]",
                    _ => "Multiplica [MULT]"
                };
                ViewBag.ChannelName = channelName;
                ViewBag.BusinessCode = businessCode;
                ViewBag.BusinessName = response.Data.BusinessName;
                ViewBag.Username = username;
                ViewBag.Role = role;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }

        [HttpPost]
        public async Task<ActionResult> GetAll()
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null || businessCode is null)
            {
                return Unauthorized(Response<List<BranchDTO>>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            BranchGetAllRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = Convert.ToInt64(businessCode),
                Channel = channel.ChannelName
            };

            Response<List<BranchResponse>> response = await _branchConnector.GetAll(request, channel, username);
            List<BranchDTO> branches = [];
            if (!response.HasError())
            {
                branches = [.. response.Data.Select(branch => new BranchDTO
                {
                    BranchCode = branch.BranchCode,
                    BranchName = branch.BranchName,
                    BranchCity = branch.BranchCity,
                    UserCode = branch.UserCode,
                    UserName = branch.UserName,
                    CreationUser = branch.CreationUser,
                    CreationDate = branch.CreationDate.ToString("dd MMM yyyy"),
                    ModificationUser = branch.ModificationUser,
                    ModificationDate = branch.ModificationDate.ToString("dd MMM yyyy"),
                    State = branch.State
                })];
                return Ok(branches);
            }
            if (response.State == "98")
            {
                return Ok(branches);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] BranchRegisterDTO request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? businessName = HttpContext.Session.GetString("businessName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null || businessCode is null || businessName is null)
            {
                return Unauthorized(Response<BranchDTO>.Status(Helper.Unauthorized));
            }
            
            request.BranchName = request.BranchName.Trim();
            request.FullName = request.FullName.Trim();
            request.UserName = request.UserName.Trim();
            request.UserPhone = 
                request.UserPhone.Trim() == string.Empty ? "99999999": request.UserPhone;
            request.CredentialsEmail = request.CredentialsEmail.Trim();
            request.NotificationsEmail = request.NotificationsEmail.Trim().Replace(";", ",");

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            Response<UserRegisterResponse> userRegisterResponse;
            string password = Helper.GeneratePassword();
            do
            {
                UserRegisterRequest userRegisterRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    BusinessCode = businessCode,
                    RoleCode = "1",
                    TypeUser = "IAM",
                    Channel = channelName,
                    Client = new RegisterClient
                    {
                        Name = request.FullName,
                        UserName = request.UserName,
                        Password = password,
                        Email = request.NotificationsEmail,
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
                BranchRegisterRequest branchRegisterRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    BranchName = request.BranchName,
                    BusinessCode = Convert.ToInt64(businessCode),
                    Channel = channel.ChannelName,
                    City = request.BranchCity,
                    CreationUser = username,
                    UserId = userRegisterResponse.Data.Client.Id
                };

                Response<BranchRegisterResponse> branchRegisterResponse = await _branchConnector.Register(branchRegisterRequest, channel, username);
                if (!branchRegisterResponse.HasError())
                {
                    EmailDetailsBranch details = new()
                    {
                        NombreDeEmpresa = businessName,
                        NombreDeSucursal = branchRegisterRequest.BranchName,
                        NombreCompleto = request.FullName,
                        RolDeUsuario = "Jefe de Agencia",
                        NombreDeUsuario = request.UserName,
                        ContrasenaDeUsuario = password
                    };
                    await _notificationConnector.SendAsync(details, request.CredentialsEmail, username);

                    BranchGetRequest branchGetRequest = new()
                    {
                        PublicToken = channel.PublicToken,
                        AppUserId = channel.AppUserId,
                        BranchCode = branchRegisterResponse.Data.BranchCode,
                        Channel = channel.ChannelName
                    };
                    Response<BranchResponse> branchGetResponse = await _branchConnector.Get(branchGetRequest, channel, username);
                    if (!branchGetResponse.HasError())
                    {
                        BranchDTO branch = new()
                        {
                            BranchCode = branchGetResponse.Data.BranchCode,
                            BranchName = request.FullName,
                            BranchCity = branchGetResponse.Data.BranchCity,
                            UserCode = branchGetResponse.Data.UserCode,
                            UserName = branchGetResponse.Data.UserName,
                            CreationUser = branchGetResponse.Data.CreationUser,
                            CreationDate = branchGetResponse.Data.CreationDate.ToString("dd MMM yyyy"),
                            ModificationUser = branchGetResponse.Data.ModificationUser,
                            ModificationDate = branchGetResponse.Data.ModificationDate.ToString("dd MMM yyyy"),
                            State = branchGetResponse.Data.State
                        };
                        return Ok(branch);
                    }
                    else
                    {
                        return BadRequest(branchGetResponse);
                    }
                }
                else
                {
                    BadRequest(branchRegisterResponse);
                }
            }
            return BadRequest(userRegisterResponse);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] BranchUpdateRequest request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
               return Unauthorized(Response<BranchDTO>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.Channel = channel.ChannelName;
            request.ModificationUser = username;
            request.BranchName = request.BranchName.Trim();

            Response<BranchUpdateResponse> response = await _branchConnector.Update(request, channel, username);
            if (!response.HasError())
            {
                BranchDTO branch = new()
                {
                    BranchCode = response.Data.BranchCode,
                    BranchName = response.Data.BranchName,
                    BranchCity = response.Data.BranchCity,
                    ModificationUser = response.Data.ModificationUser,
                    ModificationDate = DateTime.Now.ToString("dd MMM yyyy")
                };
                return Ok(branch);
            }
            return BadRequest(response);
        }
    }
}
