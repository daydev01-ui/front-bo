using BCP.Connector.Business;
using BCP.Connector.Report;
using BCP.QRBackOffice.Models;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Business.v1.Responses;
using BCP.QRBackOffice.Models.Connectors.Report.v1.Requests;
using BCP.QRBackOffice.Models.Connectors.Report.v1.Responses;
using BCP.QRBackOffice.Models.DTOs;
using BCP.QRBackOffice.Models.Options;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBusinessConnector _businessConnector;
        private readonly IReportConnector _reportConnector;

        public ReportController(
            IConfiguration configuration,
            IBusinessConnector businessConnector,
            IReportConnector reportConnector)
        {
            _configuration = configuration;
            _businessConnector = businessConnector;
            _reportConnector = reportConnector;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            string? channelName = HttpContext.Session.GetString("channelName");

            if (username is null || role is null || channelName is null)
            {
                return RedirectToAction("Index", "Auth");
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

            List<BusinessDTO> empresas = [];
            if (!response.HasError())
            {
                empresas = [.. response.Data.Select(b => new BusinessDTO
                {
                    BusinessCode = b.BusinessCode,
                    BusinessName = b.BusinessName
                })];
            }

            ViewBag.Username = username;
            ViewBag.Role = role;
            ViewBag.ChannelName = channelName;
            ViewBag.Empresas = empresas;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel(
            string? empresa,
            string? estado,
            string? fInicio,
            string? fFin)
        {
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            string? channelName = HttpContext.Session.GetString("channelName");

            if (username is null || role is null || channelName is null)
            {
                return RedirectToAction("Index", "Auth");
            }

            if (string.IsNullOrWhiteSpace(fInicio) || string.IsNullOrWhiteSpace(fFin))
            {
                return BadRequest("Las fechas de inicio y fin son requeridas.");
            }

            if (!DateTime.TryParseExact(fInicio, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(fFin, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime endDate))
            {
                return BadRequest("Formato de fecha inválido. Se esperaba dd/MM/yyyy.");
            }

            if (startDate > endDate)
            {
                return BadRequest("La fecha inicial no puede ser mayor que la fecha final.");
            }

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;

            ExcelReportRequest excelRequest = new()
            {

                StartDate = startDate.ToString("yyyyMMdd"),
                FinalDate = endDate.ToString("yyyyMMdd"),
                Currency = "ALL",

                BusinessCode = string.IsNullOrWhiteSpace(empresa) ? null : empresa,

            };

            ExcelReportResponse result = await _reportConnector.GetExcel(excelRequest, channel, username);

            if (!result.HasError && result.Bytes is { Length: > 0 })
            {
                return File(result.Bytes, result.ContentType!, result.FileName);
            }

            TempData["ErrorReporte"] = result.ErrorMessage ?? "No se pudo generar el reporte.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetAllEmpresas(GetAllReportRequest request)
        {
            string? username = HttpContext.Session.GetString("username");
            string? channelName = HttpContext.Session.GetString("channelName");

            if (username is null || channelName is null)
                return Unauthorized();

            Channel channel = _configuration.GetSection(channelName).Get<Channel>()!;

            GetAllReportRequest excelRequest = new()
            {
                PublicToken = channel.PublicToken,
                AppUserId = channel.AppUserId,
                BusinessCode = "0349"
            };

            Response<List<EmpresaResponse>> response = await _reportConnector.GetAllEmpresas(excelRequest, channel, username);

            if (!response.HasError())
            {
                var empresas = response.Data.Select(e => new {
                    id = e.Codigo,   
                    text = e.Nombre   
                });
                return Ok(empresas);
            }

            return Ok(new List<object>());
        }
    }
}
