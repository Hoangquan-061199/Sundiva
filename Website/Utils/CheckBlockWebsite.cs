using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Mvc;

namespace Website.Utils
{
    public class CheckBlockWebsite : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SystemConfigJson _config = JsonConvert.DeserializeObject<SystemConfigJson>(Common.ReadFile("SystemConfigvi.Json", "DataJson"));
            if (_config != null && _config.IsShow==false)
            {
                filterContext.Result = new RedirectResult("/comingsoon.html");
            }
        }
    }
}
