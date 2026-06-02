using BCP.Connector.Segurinet;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Segurinet.v1.Responses;
using BCP.QRBackOffice.Models.Options;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISegurinetConnector _segurinetConnector;

        public AuthController(
            ISegurinetConnector segurinetConnector,
            IConfiguration configuration)
        {
            _segurinetConnector = segurinetConnector;
            _configuration = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginParameter request)
        {
            Channel channel = _configuration.GetSection("MULT").Get<Channel>()!;
            request.PublicToken = channel.PublicToken;
            request.AppUserId = channel.AppUserId;
            request.Channel = channel.ChannelName;

            var response = new Response<SegurinetResponse> { };
            HttpContext.Session.SetString("username", "testUser");
            HttpContext.Session.SetString("role", "Administrador");
            HttpContext.Session.SetString("channelName", "MULT");
            return Ok();
            //Response<SegurinetResponse> response = await _segurinetConnector.Login(request);
            //if (!response.HasError())
            //{
            //    HttpContext.Session.SetString("username", response.Data.Matricula);
            //    List<bool> values = [.. response.Data.Roles.Values];
            //    if (values[1])
            //    {
            //        HttpContext.Session.SetString("role", "Administrador");
            //    }
            //    else if (values[2])
            //    {
            //        HttpContext.Session.SetString("role", "Lector");
            //    }
            //    else if (values[3])
            //    {
            //        HttpContext.Session.SetString("role", "Escritor");
            //    }
            //    return Ok();
            //}
            //return BadRequest(response);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}
