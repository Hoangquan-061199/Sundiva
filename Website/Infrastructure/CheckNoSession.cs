using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Utils;

namespace Website.Infrastructure
{
    public class CheckNoSession : ActionFilterAttribute
    {
        private HttpRequest Request;
        private HttpResponse Response;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            Request = context.Request;
            Response = context.Response;
            SessionBase session = new(context);
            if (!string.IsNullOrEmpty(session.GetSessionUserId()))
            {
                filterContext.Result = new RedirectResult("/");
            }           
        }
    }
}