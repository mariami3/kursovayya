using Microsoft.AspNetCore.Mvc;

namespace Magazin.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
