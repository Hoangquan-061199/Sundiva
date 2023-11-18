using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Website.Utils;
using ADCOnline.Business.Implementation.AdminManager;
using Website.Areas.Admin.ViewModels;

namespace Areas.Admin.ViewComponents
{
    public class MenuLeftAdminViewComponent : ViewComponent
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        public MenuLeftAdminViewComponent()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }

        public IViewComponentResult Invoke()
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetMenu(role, userId)
            };
            return View(model);
        }
    }
}
