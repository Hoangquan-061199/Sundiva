using System.Configuration;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Website.Utils;
using ADCOnline.Business.Implementation.AdminManager;
using System;

namespace Website.Utils
{
    public class CheckLoginAdmin : ActionFilterAttribute
    {
        private HttpRequest Request;
        private HttpResponse Response; 
        private readonly MembershipDa aspnetMembershipDa = new MembershipDa(WebConfig.ConnectionString);
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            Request = context.Request;
            Response = context.Response;
            SessionBase session = new(context);
            if (string.IsNullOrEmpty(session.GetAdminUserId()))
            {
                filterContext.Result = new RedirectResult("/"+WebConfig.AdminAlias+"?returnUrl=" +Request.Host + Request.Path);
            }
            if (!string.IsNullOrEmpty(session.GetAdminUserId()))
            {
                Guid aGuid = new(session.GetAdminUserId());
                var memberShip = aspnetMembershipDa.GetId(aGuid);
                if (memberShip == null)
                {
                    filterContext.Result = new RedirectResult("/" + WebConfig.AdminAlias + "?returnUrl=" + Request.Host + Request.Path);
                }
            }
        }
    }
}