using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Website.Utils;
using System.IO;

namespace Website.Models
{
    public class NonWwwRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            HttpRequest req = context.HttpContext.Request;
            string path = HttpUtility.UrlDecode(req.Path + req.QueryString);
            if (!string.IsNullOrEmpty(path) || path!="/")
            {
                path = path.Replace(" ", "+").ToLower();
                List<RedirectJson> listredirect = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                if (listredirect != null && listredirect.Any(x => HttpUtility.UrlDecode(x.OldUrl).Replace(" ", "+").ToLower() == path))
                {
                    RedirectJson newurl = listredirect.FirstOrDefault(x => HttpUtility.UrlDecode(x.OldUrl).Replace(" ", "+").ToLower() == path);
                    if (newurl.TypeRedirect == "302")
                    {
                        Uri redirectURI = new(WebConfig.Website + newurl.NewUrl);
                        context.HttpContext.Response.Redirect(redirectURI.AbsoluteUri);
                        context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
                    }
                    else
                    {
                        Uri redirectURI = new(WebConfig.Website + newurl.NewUrl);
                        context.HttpContext.Response.Redirect(redirectURI.AbsoluteUri);
                        context.HttpContext.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                    }
                    context.Result = RuleResult.EndResponse;
                }
            }
            HostString currentHost = req.Host;
            if (currentHost.Host.StartsWith("www."))
            {
                HostString newHost = new(currentHost.Host.Substring(4));
                StringBuilder newUrl = new StringBuilder().Append("http://").Append(newHost).Append(req.PathBase).Append(req.Path).Append(req.QueryString);
                if (WebConfig.EnableHttps == true)
                {
                    newUrl = new StringBuilder().Append("https://").Append(newHost).Append(req.PathBase).Append(req.Path).Append(req.QueryString);
                }
                context.HttpContext.Response.Redirect(newUrl.ToString());
                context.Result = RuleResult.EndResponse;
            }
        }
        public string ReadFile(string fileName, string path)
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
