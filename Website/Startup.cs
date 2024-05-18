using ADCOnline.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore6;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;
using Website.Infrastructure;
using Website.Models;
using Website.Utils;
using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using WmmThrowExceptionLogger = WebMarkupMin.Core.Loggers.ThrowExceptionLogger;

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
            //services.AddRazorPages().AddRazorRuntimeCompilation();
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
            // Add WebMarkupMin services to the services container.
            //services.AddWebMarkupMin(
            //options =>
            //{
            //    options.AllowMinificationInDevelopmentEnvironment = true;
            //    options.AllowCompressionInDevelopmentEnvironment = true;
            //}
            //).AddHtmlMinification(option =>
            //{
            //    option.MinificationSettings.RemoveRedundantAttributes = true;
            //}).AddXmlMinification().AddHttpCompression();

            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.AllowCompressionInDevelopmentEnvironment = true;
            })
                .AddHtmlMinification(options =>
                {
                    HtmlMinificationSettings settings = options.MinificationSettings;
                    settings.RemoveRedundantAttributes = true;
                    settings.RemoveHttpProtocolFromAttributes = false;
                    settings.RemoveHttpsProtocolFromAttributes = false;

                    options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddHttpCompression(options =>
                {
                    options.CompressorFactories = new List<ICompressorFactory>
                    {
            new BuiltInBrotliCompressorFactory(new BuiltInBrotliCompressionSettings
            {
                Level = CompressionLevel.Fastest
            }),
            new DeflateCompressorFactory(new DeflateCompressionSettings
            {
                Level = CompressionLevel.Fastest
            }),
            new GZipCompressorFactory(new GZipCompressionSettings
            {
                Level = CompressionLevel.Fastest
            })
                    };
                })
                ;
            // Override the default logger for WebMarkupMin.
            //services.AddSingleton<IWmmLogger, WmmThrowExceptionLogger>();
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
                context.Response.Headers.Add("Content-Security-Policy",
                    //"script-src 'self' 'unsafe-inline' 'nonce-rAnd0m' www.google-analytics.com cdn.jsdelivr.net cdn.linearicons.com www.googletagmanager.com connect.facebook.net www.facebook.com www.google.com www.gstatic.com;" +                    
                    //"style-src 'self' 'unsafe-inline' www.google-analytics.com cdn.jsdelivr.net fonts.googleapis.com cdn.linearicons.com connect.facebook.com;" +
                    "font-src 'self' fonts.googleapis.com fonts.gstatic.com cdn.linearicons.com;" +
                    "frame-src 'self' td.doubleclick.net www.googletagmanager.com connect.facebook.net www.facebook.com www.google.com www.gstatic.com www.youtube.com web.facebook.com;" +
                    //"connect-src 'self' www.google-analytics.com www.googletagmanager.com connect.facebook.net www.facebook.com www.google.com www.gstatic.com www.youtube.com www.google.com.vn/pagead/1p-user-list/998208113 stats.g.doubleclick.net/j/collect;" +
                    "object-src 'none';");
                    //"img-src 'self' www.googletagmanager.com connect.facebook.net www.facebook.com www.google.com www.gstatic.com www.google-analytics.com/collect www.google.com.vn/ads/ga-audiences www.google.com.vn/pagead/1p-user-list/6145898751/ www.google.com.vn/pagead/1p-user-list/998208113/;");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
                context.Response.Headers.Add("Cross-Origin-Resource-Policy", "cross-origin");
                context.Response.Headers.Add("Cross-Origin-Opener-Policy", "cross-origin");
                context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "unsafe-none");
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(10)
                };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
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
                        TimeSpan maxAge = new(365, 0, 0, 0);
                        r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds);
                    }
                }
            });
            app.UseSession();
            app.UseCookiePolicy();
            app.UseWebMarkupMin();
            new WebConfig(Configuration, env);
            new RouteAdminConfig(app);
            new RouteConfig(app);
        }
    }
}