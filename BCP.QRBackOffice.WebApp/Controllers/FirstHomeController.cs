using Microsoft.AspNetCore.Mvc;

namespace BCP.QRBackOffice.WebApp.Controllers
{
    public class FirstHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
