using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Website.Utils;

namespace Website.Infrastructure
{
    public class CheckSession : ActionFilterAttribute
    {
        private HttpRequest Request;
        private HttpResponse Response;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            Request = context.Request;
            Response = context.Response;
            SessionBase session = new(context);
            if (string.IsNullOrEmpty(session.GetSessionUserId()))
            {
                filterContext.Result = new RedirectResult("/dang-nhap?returnUrl=" + Request.Host + Request.Path);
            }           
        }
    }
}