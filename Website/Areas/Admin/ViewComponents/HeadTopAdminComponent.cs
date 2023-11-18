using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using ADCOnline.Utils;
using ADCOnline.Simple.Admin;

namespace Areas.Admin.ViewComponents
{
    public class HeadTopAdminViewComponent : ViewComponent
    {
        private readonly MembershipDa aspnetMembershipDa;
        private readonly ContactUsDa _contactUsDa;
        private readonly CommentAdminDa _commentAdminDa;
        public HeadTopAdminViewComponent()
        {
            aspnetMembershipDa = new MembershipDa(WebConfig.ConnectionString);
            _contactUsDa = new ContactUsDa(WebConfig.ConnectionString);
            _commentAdminDa = new CommentAdminDa(WebConfig.ConnectionString);
        }
        public IViewComponentResult Invoke()
        {
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            Guid aGuid = new(userId);
            HomeAdminViewModel model = new()
            {
                Member = aspnetMembershipDa.GetAdminId(aGuid),
                CommentAdmins = _commentAdminDa.GetAllCommentNotIsApproved(new SearchModel { page = 1, pagesize = 15 }),
                CurrentLanguage = Lang()
            };
            return View(model);
        }
        private string Lang()
        {
            return Request.Cookies["lanad"] != null ? Utility.ValidString(Request.Cookies["lanad"], "Code", true) : StaticEnum.DefaultLanguage;
        }
    }
}
