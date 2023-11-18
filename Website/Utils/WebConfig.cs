using ADCOnline.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Website.Utils
{

    public class WebConfig
    {
        public static string WebsiteName { get; set; }
        public const string AdminAlias = "Adminadc";
        public static string FacebookAppId { get; set; }
        public static string GoogleId { get; set; }
        public static string SiteKeyCaptchaGoogle { get; set; }
        public static string SecretKeyCaptchaGoogle { get; set; }
        public static string SessionTimeout { get; set; }
        public static string ConnectionString { get; set; }
        private IConfiguration Configuration { get; set; }
        public static string PathServer { get; set; }
        public static int CachingExpireMinute { get; set; }
        public static string RedisConnection { get; set; }
        public static string PassAdmin { get; set; }
        public static string CacheProject { get; set; }
        public static string Website { get; set; }
        public static string WebsiteGenerator { get; set; }
        public static string WebsiteDescription { get; set; }
        public static bool EnableCache { get; set; }
        public static bool EnableHttps { get; set; }
        public static string Sizes { get; set; }
        public static string Version { get; set; }
        public static bool DebugSend { get; set; }
        public static string ApiGold { get; set; }
        public static string KeyApi { get; set; }
        public static string KeyCapcha { get; set; }
        public static string SecretKey { get; set; }
        public static string SecretSalt { get; set; }
        public static bool ShowTool { get; set; }
        public static string GoogleTranslateDomain { get; set; }
        public static string GoogleTranslateDomain2 { get; set; }
        public WebConfig(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
            WebsiteName = Configuration["ConfigApp:WebSiteName"];
            SessionTimeout = Configuration["ConfigApp:SessionTimeout"];
            PathServer = hostingEnvironment.WebRootPath+"/";
            CachingExpireMinute = ConvertUtil.ToInt32(Configuration["ConfigApp:CachingExpireMinute"]);
            RedisConnection = Configuration["ConnectionStrings:RedisConnection"];
            PassAdmin = Configuration["ConfigApp:PassAdmin"];
            CacheProject = Configuration["ConfigApp:CacheProject"];
            Website = Configuration["ConfigApp:Website"];
            WebsiteGenerator = Configuration["ConfigApp:WebsiteGenerator"];
            WebsiteDescription = Configuration["ConfigApp:WebsiteDescription"];
            EnableCache = ConvertUtil.ToBool(Configuration["ConfigApp:EnableCache"]);
            EnableHttps = ConvertUtil.ToBool(Configuration["ConfigApp:EnableHttps"]);
            Sizes = Configuration["ConfigApp:Sizes"];
            Version = Configuration["ConfigApp:Version"];
            DebugSend = ConvertUtil.ToBool(Configuration["ConfigApp:DebugSend"]);
            ShowTool = ConvertUtil.ToBool(Configuration["ConfigApp:ShowTool"]);
            ApiGold = Configuration["ConfigApp:ApiGold"];
            KeyApi = Configuration["ConfigApp:KeyApi"];
            KeyCapcha = Configuration["ConfigApp:KeyCapcha"];
            FacebookAppId = Configuration["ConfigApp:FacebookAppId"];
            GoogleId = Configuration["ConfigApp:GoogleId"];
            SiteKeyCaptchaGoogle = Configuration["ConfigApp:SiteKeyCaptchaGoogle"];
            SecretKeyCaptchaGoogle = Configuration["ConfigApp:SecretKeyCaptchaGoogle"];
            SecretKey = Configuration["ConfigApp:SecretKey"];
            SecretSalt = Configuration["ConfigApp:SecretSalt"];
            GoogleTranslateDomain = Configuration["ConfigApp:DomainGoogleTranslate"];
            GoogleTranslateDomain2 = Configuration["ConfigApp:DomainGoogleTranslate2"];
        }
    }
}