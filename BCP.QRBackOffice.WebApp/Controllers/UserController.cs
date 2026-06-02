using BCP.Connector.Business;
using BCP.Connector.User;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Responses;
using BCP.QRBackOffice.Models.Connectors.User.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.User.v1.Responses;
using BCP.QRBackOffice.Models.DTOs;
using BCP.QRBackOffice.Models.Options;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBusinessConnector _businessConnector;
        private readonly IUserConnector _userConnector;

        public UserController(
            IConfiguration configuration,
            IBusinessConnector businessConnector,
            IUserConnector userConnector)
        {
            _configuration = configuration;
            _businessConnector = businessConnector;
            _userConnector = userConnector;
        }

        public async Task<ActionResult> Index(long businessCode)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
                return RedirectToAction("Index", "Auth");
            }
            HttpContext.Session.SetString("businessCode", businessCode.ToString());
            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            BusinessGetRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = businessCode,
                Channel = channel.ChannelName
            };
            Response<BusinessResponse> response = await _businessConnector.Get(request, channel, username);
            if (!response.HasError())
            {
                ViewBag.TitleChannel = channelName switch
                {
                    "MULT" => "Multiplica [MULT]",
                    "CRDT" => "Credinet Web [CRDT]",
                    "MUBE" => "Extranet [MUBE]",
                    _ => "Multiplica [MULT]"
                };
                ViewBag.ChannelName = channelName;
                ViewBag.BusinessName = response.Data.BusinessName;
                ViewBag.Username = username;
                ViewBag.Role = role;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }

        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? businessCode = HttpContext.Session.GetString("businessCode");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null || businessCode is null)
            {
                return Unauthorized(Response<List<UserDTO>>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            UserGetAllRequest request = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = Convert.ToInt64(businessCode),
                Channel = channel.ChannelName
            };
            Response<List<UserResponse>> response = await _userConnector.GetAll(request, channel, username);
            List<UserDTO> users = [];
            if (!response.HasError())
            {
                users = [.. response.Data.Select(user => new UserDTO
                {
                    UserCode = user.UserCode,
                    UserName = user.UserName,
                    Access = user.Access,
                    Role = user.RoleId switch
                    {
                        1 => "Jefe de agencia",
                        2 => "Cajero",
                        3 => "Administrador",
                        _ => "Desconocido"
                    },
                    Email = user.Email,
                    CreationUser = user.CreationUser,
                    CreationDate = user.CreationDate.ToString("dd MMM yyyy"),
                    ModificationUser = user.ModificationUser,
                    ModificationDate = user.ModificationDate.ToString("dd MMM yyyy"),
                    State = user.State,
                })];
                return Ok(users);
            }
            if (response.State == "98")
            {
                return Ok(users);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Unlock([FromBody] UserGetRequest request)
        {
            string? channelName = HttpContext.Session.GetString("channelName");
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null || channelName is null)
            {
                return Unauthorized(Response<UserDTO>.Status(Helper.Unauthorized));
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.Channel = channel.ChannelName;
            Response<UserResponse> userGetResponse = await _userConnector.Get(request, channel, username);
            if (!userGetResponse.HasError())
            {
                UserUnlockRequest userUnlockRequest = new()
                {
                    PublicToken = channel.PublicToken,
                    AppUserId = channel.AppUserId,
                    ChannelHour = DateTime.Now.ToString("HH:mm:ss"),
                    ChannelDate = DateTime.Now.ToString("yyyyMMdd"),
                    BusinessCode = userGetResponse.Data.BusinessCode.ToString(),
                    Channel = channel.ChannelName,
                    Identifier = userGetResponse.Data.Identifier,
                    UserName = userGetResponse.Data.Access,
                    ModificationUser = username,
                    Status = "A"
                };

                Response<UserUnlockResponse> response = await _userConnector.Unlock(userUnlockRequest, channel, username);
                if (!response.HasError())
                {
                    return Ok(response.Data);
                }
                return BadRequest(response);
            }
            return BadRequest(userGetResponse);
        }
    }
}
