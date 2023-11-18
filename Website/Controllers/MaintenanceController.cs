using System;
using ADCOnline.Utils;
using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Website.Utils;

namespace Website.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly CacheUtils cacheUtils;
        public MaintenanceController(IDistributedCache distributedCache)
        {
            cacheUtils = new CacheUtils(distributedCache);
        }
        public IActionResult Index()
        {
            string current = "CTL" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString();
            string lang = HttpContext.Request.Cookies["lang"] != null ? Utility.ValidString(HttpContext.Request.Cookies["lang"], "Code", true) : StaticEnum.DefaultLanguage;
            SystemConfigJson config = cacheUtils.SystemConfigItem(lang);
            if ((config != null && config.IsShow == true) || GetAccess(HttpContext) == current)
            {
                return Redirect("/");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult Login()
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Truy cập thất bại"
            };
            try
            {
                var password = Request.Form["password"];
                string current = $"ADC{DateTime.Now.Year.ToString()}{DateTime.Now.Month.ToString()}{DateTime.Now.Day.ToString()}{DateTime.Now.Hour.ToString()}";
                if (!string.IsNullOrEmpty(password) && password == current)
                {
                    SetAccess(password);
                    msg.Errors = false;
                }
            }
            catch{}
            return Ok(msg);
        }
        public void SetAccess(string str) => HttpContext.Session.SetString("WebAccess", str);
        public string GetAccess(HttpContext context) => context.Session.GetString("WebAccess");
    }
}
