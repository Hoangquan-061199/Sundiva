using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Website.Utils;
using ADCOnline.Simple.Json;
using System.IO;
using ADCOnline.Business.Implementation.ClientManager;

namespace Website.Infrastructure
{
    public class CheckOffWebsite : ActionFilterAttribute
    {
        private HttpRequest Request;
        private HttpResponse Response;
        private SystemConfigManager configManager;
        public CheckOffWebsite()
        {
            configManager = new SystemConfigManager(WebConfig.ConnectionString);
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = filterContext.HttpContext;
            Request = context.Request;
            Response = context.Response;            
            string lang = context.Request.Cookies["lang"] != null ? Utility.ValidString(context.Request.Cookies["lang"], "Code", true) : StaticEnum.DefaultLanguage;
            //string sql = string.Format("select * from SystemConfig where lang = '{0}'", lang);
            //var config = _dapperDA.Select<SystemConfigItem>(sql).FirstOrDefault();
            SystemConfigJson config = configManager.GetSystemConfigBase(lang);
            string access = GetAccess(context);
            string current = "ADC" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString();
            if (config != null && config.IsShow == false && (string.IsNullOrEmpty(access) || access != current))
            {
                filterContext.Result = new RedirectResult("/updating");
            }
        }
        public string GetAccess(HttpContext context) => context.Session.GetString("WebAccess");
        private static string ReadFile(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {

            }
            return fileContent;
        }
    }
}
