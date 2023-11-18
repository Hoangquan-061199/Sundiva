using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class PartialView : BaseController
    {
        public IActionResult Login(string type)
        {
            ViewBag.Type = type;
            return View();
        }
        public IActionResult Register() => View();
        public IActionResult LostPass() => View();
        public IActionResult Reset() => View();
    }
}
