using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class ErrorController : BaseController
    {
        public IActionResult Index() => View();
        public ViewResult Error404() => View();
        public ViewResult Error404Amp() => View("~/Views/Amp/Error.cshtml");
    }
}