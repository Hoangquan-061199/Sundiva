using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class HomeAdminController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly MembershipDa aspnetMembershipDa;
        private readonly StatisticalDa _statisticalDa;
        public HomeAdminController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            aspnetMembershipDa = new MembershipDa(WebConfig.ConnectionString);
            _statisticalDa = new StatisticalDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                StatisticalAdmins = _statisticalDa.StatisticalAdmins(),
                ListModule = _moduleAdminDa.GetTabMenu(role, userId)
            };
            return View(model);
        }
        public ActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return Redirect("/" + WebConfig.AdminAlias + "/HomeAdmin/Index");
        }
        [HttpGet]
        public IActionResult ChangePassword() => View();
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> ChangePasswordAction()
        {
            JsonMessage msg = new();
            try
            {
                ChangePasswordModel model = new();
                await TryUpdateModelAsync(model);
                string userId = HttpContext.Session.GetString("WebAdminUserID");
                Guid aGuid = new(userId);
                AspnetMembership member = aspnetMembershipDa.GetId(aGuid);
                if (member != null)
                {
                    string newsha = Utility.CreatePasswordHash(model.NewPassword, member.PasswordSalt);
                    member.Password = newsha;
                    aspnetMembershipDa.Update(member, aGuid);
                    msg.Errors = false;
                    msg.Message = "Đổi mật khẩu thành công.";
                }
                else
                {
                    return Redirect("/Adminadc/AccountAdmin/Login");
                }
            }
            catch
            {
                msg.Errors = true;
                msg.Message = "Đổi mật khẩu thất bại.";
            }
            return Ok(msg);
        }
    }
}
