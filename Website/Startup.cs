using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Utils;
using WebMarkupMin.AspNetCore2;
using Website.Models;
using Microsoft.AspNetCore.Rewrite;
using Website.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;

namespace Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //runtime view *.cshtml
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.Configure<GoogleCaptchaConfig>(Configuration.GetSection("GoogleRecaptcha"));
            services.AddTransient(typeof(GoogleCapthaService));
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                int timeOut = ConvertUtil.ToInt32(WebConfig.SessionTimeout) == 0 ? 600000 : ConvertUtil.ToInt32(WebConfig.SessionTimeout);
                options.IdleTimeout = TimeSpan.FromSeconds(timeOut);
            });
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.BufferBodyLengthLimit = int.MaxValue;
                x.MultipartBoundaryLengthLimit = int.MaxValue;
                x.MemoryBufferThreshold = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddMvc(option => option.EnableEndpointRouting = false).AddXmlSerializerFormatters();
            services.AddMvc(option => option.EnableEndpointRouting = false).AddSessionStateTempDataProvider().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddWebMarkupMin(
              options =>
              {
                  options.AllowMinificationInDevelopmentEnvironment = true;
                  options.AllowCompressionInDevelopmentEnvironment = true;
              })
              .AddHtmlMinification(
                  options =>
                  {
                      options.MinificationSettings.RemoveRedundantAttributes = true;
                  })
              .AddHttpCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/error";
                    await next();
                }
            });
            app.UseMiddleware(typeof(BlockIP));
            var options = new RewriteOptions();
            options.Rules.Add(new NonWwwRule());
            app.UseRewriter(options);
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = r =>
                {
                    string path = r.File.PhysicalPath;
                    if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") || path.EndsWith(".svg") || path.EndsWith(".woff2") || path.EndsWith(".otf") || path.EndsWith(".ttf") || path.EndsWith(".webp"))
                    {
                        TimeSpan maxAge = new(7, 0, 0, 0);
                        r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds);
                    }
                }
            });
            app.UseSession();
            app.UseCookiePolicy();
            //app.UseWebMarkupMin();
            new WebConfig(Configuration, env);
            new RouteAdminConfig(app);
            new RouteConfig(app);
        }
    }
}