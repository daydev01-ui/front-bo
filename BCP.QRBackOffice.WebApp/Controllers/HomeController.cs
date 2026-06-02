using BCP.Connector.Segurinet;
using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISegurinetConnector _segurinetConnector;
        public HomeController(
            ISegurinetConnector segurinetConnector)
        {
            _segurinetConnector = segurinetConnector;
        }

        public async Task<IActionResult> Index()
        {
            string? username = HttpContext.Session.GetString("username");
            string? role = HttpContext.Session.GetString("role");
            if (username is null || role is null)
            {
                return RedirectToAction("Index", "Auth");
            }
            ViewBag.Username = username;
            ViewBag.Role = role;
            return View();
        }
    }
}
