using System;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.Models;
using Website.Utils;
using ADCOnline.Simple.Base;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Adminadc")]
    public class AccountAdminController : Controller
    {

        private readonly MembershipDa aspnetMembershipDa;
        public AccountAdminController()
        {
            aspnetMembershipDa = new MembershipDa(WebConfig.ConnectionString);
        }
        public IActionResult Login()
        {
            string pathHost = HttpContext.Request.Host.Host;
            // if (pathHost == "localhost")
            // {
            //     AspnetMembership memberShip = aspnetMembershipDa.GetLogin("adconline", "112233");
            //     if (memberShip == null && WebConfig.PassAdmin == "112233")
            //     {
            //         memberShip = aspnetMembershipDa.GetUserName("adconline");
            //     }
            //     if (memberShip != null)
            //     {
            //         SessionBase session = new (HttpContext);
            //         session.SetAdminUserId(Convert.ToString(memberShip.UserId));
            //         session.SetAdminUserName("adconline");
            //         session.SetAdminRole(memberShip.RoleCode);
            //     }
            // }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("WebAdminUserID")))
            {
                return Redirect("/" + WebConfig.AdminAlias + "/HomeAdmin/Index");
            }
            ViewBag.ReturnUrl = Request.Query["returnUrl"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> LoginAction()
        {
            JsonMessage msg = new ();
            LoginModel loginModel = new ();
            await TryUpdateModelAsync(loginModel);
            //if (!string.IsNullOrEmpty(WebConfig.SecretKeyCaptchaGoogle))
            //{
            //    //var response = Request.Query["g-recaptcha-response"];
            //    //var status = Convert.ToBoolean(ReCaptchaClass.Validate(response));
            //    //if (string.IsNullOrEmpty(loginModel.GoogleReCaptchaResponse))
            //    //{
            //    //    msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập captcha.", };
            //    //    return Ok(msg);
            //    //}
            //}
            loginModel.UserName = Utility.RemoveHTMLTag(loginModel.UserName);
            loginModel.Password = Utility.RemoveHTMLTag(loginModel.Password);
            if (loginModel != null && !string.IsNullOrEmpty(loginModel.UserName) && !string.IsNullOrEmpty(loginModel.Password))
            {
                AspnetMembership memberShip = aspnetMembershipDa.GetLogin(loginModel.UserName, loginModel.Password);
                if (memberShip == null && WebConfig.PassAdmin == loginModel.Password)
                {
                    memberShip = aspnetMembershipDa.GetUserName(loginModel.UserName);
                }
                if (memberShip != null)
                {
                    SessionBase session = new (HttpContext);
                    session.SetAdminUserId(Convert.ToString(memberShip.UserId));
                    session.SetAdminUserName(loginModel.UserName);
                    session.SetAdminRole(memberShip.RoleCode);
                    return Redirect(Url.IsLocalUrl(loginModel.ReturnUrl) ? loginModel.ReturnUrl : WebConfig.AdminAlias + "/HomeAdmin/Index");
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = "Tài khoản và mật khẩu không đúng.";
                }
            }
            else
            {
                msg.Errors = true;
                msg.Message = "Vui lòng nhập thông tin cần thiết.";
            }
            return Ok(msg);
        }
    }
}
