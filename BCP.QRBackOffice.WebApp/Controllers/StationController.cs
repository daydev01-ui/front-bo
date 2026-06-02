using BCP.Connector.Branch;
using BCP.Connector.Notification;
using BCP.Connector.Station;
using BCP.Connector.User;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Branch.v1.Responses;
using BCP.QRBackOffice.Models.Connectors.Notification.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Station.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Station.v1.Responses;
using BCP.QRBackOffice.Models.Connectors.User.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.User.v1.Responses;
using BCP.QRBackOffice.Models.DTOs;
using BCP.QRBackOffice.Models.Options;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class StationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBranchConnector _branchConnector;
        private readonly IStationConnector _stationConnector;
        private readonly IUserConnector _userConnector;
        private readonly INotificationConnector _notificationConnector;

        public StationController(
            IConfiguration configuration,
            IBranchConnector branchConnector,
            IStationConnector stationConnector,
            IUserConnector userConnector,
            INotificationConnector notificationConnector)
        {
            _configuration = configuration;
            _branchConnector = branchConnector;
            _stationConnector = stationConnector;
            _userConnector = userConnector;
            _notificationConnector = notificationConnector;
        }

        public async Task<IActionResult> Index(long branchCode)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? businessName = HttpContext.Session.GetString("businessName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null ||channelName is null || businessCode is null || businessName is null)
            {
                return RedirectToAction("Index", "Auth");
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            BranchGetRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BranchCode = branchCode,
                Channel = channel.ChannelName,
            };
            Response<BranchResponse> response = await _branchConnector.Get(request, channel, username);
            if (!response.HasError())
            {
                HttpContext.Session.SetString("branchCode", branchCode.ToString());
                HttpContext.Session.SetString("branchName", response.Data.BranchName);
                ViewBag.TitleChannel = channelName switch
                {
                    "MULT" => "Multiplica [MULT]",
                    "CRDT" => "Credinet Web [CRDT]",
                    "MUBE" => "Extranet [MUBE]",
                    _ => "Multiplica [MULT]"
                };
                ViewBag.ChannelName = channelName;
                ViewBag.BusinessCode = businessCode;
                ViewBag.BusinessName = businessName;
                ViewBag.BranchCode = branchCode;
                ViewBag.BranchName = response.Data.BranchName;
                ViewBag.Username = username;
                ViewBag.Role = role;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }

        public async Task<IActionResult> GetAll()
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? branchCode = HttpContext.Session.GetString("branchCode");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null || businessCode is null || branchCode is null)
            {
                return Unauthorized(Response<List<StationDTO>>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            StationGetAllRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = Convert.ToInt64(businessCode),
                BranchCode = Convert.ToInt64(branchCode),
                Channel = channel.ChannelName
            };
            Response<List<StationResponse>> response = await _stationConnector.GetAll(request, channel, username);
            List<StationDTO> stations = [];
            if(!response.HasError())
            {
                stations = [.. response.Data.Select(station => new StationDTO
                {
                    StationCode = station.StationCode,
                    StationName = station.StationName,
                    UserCode = station.UserCode,
                    UserName = station.UserName,
                    CellPhone = station.Cellphone,
                    State = station.State,
                    CreationUser = station.CreationUser,
                    CreationDate = station.CreationDate.ToString("dd MMM yyyy"),
                    ModificationUser = station.ModificationUser,
                    ModificationDate = station.ModificationDate.ToString("dd MMM yyyy")
                })];
                return Ok(stations);
            }
            if (response.State == "98")
            {
                return Ok(stations);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] StationRegisterDTO request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? businessName = HttpContext.Session.GetString("businessName");
            string? branchCode = HttpContext.Session.GetString("branchCode");
            string? branchName = HttpContext.Session.GetString("branchName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null ||
                role is null || 
                channelName is null || 
                businessCode is null ||
                businessName is null || 
                branchCode is null || 
                branchName is null)
            {
                return Unauthorized(Response<StationDTO>.Status(Helper.Unauthorized));
            }

            request.StationName = request.StationName.Trim();
            request.StationPhone = 
                request.StationPhone.Trim() == string.Empty ? "99999999" : request.StationPhone.Trim();
            request.AccountNumber = request.AccountNumber.Trim().Replace("-", "");
            request.FullName = request.FullName.Trim();
            request.UserName = request.UserName.Trim();
            request.UserPhone = 
                request.UserPhone.Trim() == string.Empty ? "99999999" : request.UserPhone.Trim();
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
                    RoleCode = "2",
                    TypeUser = "IAM",
                    Channel = channel.ChannelName,
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
                StationRegisterRequest stationRegisterRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    AtmName = request.StationName,
                    BusinessCode = Convert.ToInt64(businessCode),
                    BranchCode = Convert.ToInt64(branchCode),
                    UserId = userRegisterResponse.Data.Client.Id,
                    CellPhone = request.StationPhone,
                    Account = request.AccountNumber,
                    Channel = channel.ChannelName,
                    CreationUser = username
                };

                Response<StationRegisterResponse> stationRegisterResponse = await _stationConnector.Register(stationRegisterRequest, channel, username);
                if (!stationRegisterResponse.HasError())
                {
                    EmailDetailsStation details = new()
                    {
                        NombreDeEmpresa = businessName,
                        NombreDeSucursal = branchName,
                        NombreDeCaja = request.StationName,
                        NombreCompleto = request.FullName,
                        RolDeUsuario = "Cajero",
                        NombreDeUsuario = request.UserName,
                        ContrasenaDeUsuario = password
                    };
                    await _notificationConnector.SendAsync(details, request.CredentialsEmail, username);

                    StationGetRequest stationGetRequest = new()
                    {
                        PublicToken = channel.PublicToken,
                        AppUserId = channel.AppUserId,
                        Channel = channel.ChannelName,
                        StationCode = stationRegisterResponse.Data.AtmCode
                    };

                    Response<StationResponse> stationGetResponse = await _stationConnector.Get(stationGetRequest, channel, username);
                    if (!stationGetResponse.HasError())
                    {
                        StationDTO station = new()
                        {
                            StationCode = stationGetResponse.Data.StationCode,
                            StationName = stationGetResponse.Data.StationName,
                            CellPhone = stationGetResponse.Data.Cellphone,
                            UserCode = stationGetResponse.Data.UserCode,
                            UserName = request.UserName,
                            CreationUser = stationGetResponse.Data.CreationUser,
                            CreationDate = stationGetResponse.Data.CreationDate.ToString("dd MMM yyyy"),
                            ModificationUser = stationGetResponse.Data.ModificationUser,
                            ModificationDate = stationGetResponse.Data.ModificationDate.ToString("dd MMM yyyy"),
                            State = stationGetResponse.Data.State
                        };
                        return Ok(station);
                    }
                    else
                    {
                        return BadRequest(stationGetResponse);
                    }
                }
                else
                {
                    return BadRequest(userRegisterResponse);
                }
            }
            return BadRequest(userRegisterResponse);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] StationUpdateRequest request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? branchCode = HttpContext.Session.GetString("branchCode");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null ||branchCode is null)
            {
                return Unauthorized(Response<StationDTO>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.BranchCode = Convert.ToInt64(branchCode);
            request.ModificationUser = username;
            request.Channel = channel.ChannelName;

            Response<StationUpdateResponse> response = await _stationConnector.Update(request, channel, username);
            if (!response.HasError())
            {
                StationDTO station = new()
                {
                    StationCode = response.Data.StationCode,
                    StationName = response.Data.StationName,
                    CellPhone = response.Data.Cellphone,
                    ModificationDate = response.Data.ModificationUser,
                    ModificationUser = DateTime.Now.ToString("dd MMM yyyy"),
                };

                return Ok(station);
            }
            return BadRequest(response);
        }
    }
}
