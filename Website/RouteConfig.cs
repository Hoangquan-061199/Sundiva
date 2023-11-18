using Microsoft.AspNetCore.Builder;


namespace Website
{
    public class RouteConfig
    {
        public RouteConfig(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                 "Ajax",
                 "Ajax/{Controller}/{action}",
                  new[] { "Aladanh.Web.Controllers" }
               );
                routes.MapRoute(
                "AjaxAmp",
                "Ajax/{Controller}/{action}/amp",
                 new[] { "Aladanh.Web.Controllers" }
              );
                routes.MapRoute(
                    "Error404",
                    "Error",
                    new { controller = "Error", action = "Error404" });
                routes.MapRoute(
                    "updating",
                    "updating",
                    new { controller = "Maintenance", action = "Index" });
                routes.MapRoute(
                    "rss",
                    "rss",
                    new { controller = "Sitemap", action = "Rss" });
                routes.MapRoute(
                    "rss module",
                    "rss/{NameAscii}.rss",
                    new { controller = "Sitemap", action = "RssFeed" });               
                routes.MapRoute(
            "download file product",
                  "download-catalog/{id}",
                  new { controller = "Content", action = "DownloadCatalog" });
                routes.MapRoute(
            "tu khoa bai viet",
                  "tu-khoa-bai-viet/{nameAscii}",
                  new { controller = "Content", action = "TagsProducts" });
                routes.MapRoute(
            "tu khoa bai viet page",
                  "tu-khoa-bai-viet/{nameAscii}/page/{p}",
                  new { controller = "Content", action = "TagsProducts" });
                routes.MapRoute(
            "so sánh sản phẩm",
                  "{nameAscii}/{nameAsciiA}-vs-{nameAsciiB}",
                  new { controller = "Content", action = "Compare" });
                #region Tuyển dụng
                routes.MapRoute(
            "ung tuyen",
                  "ung-tuyen/{id}",
                  new { controller = "Content", action = "Apply" });
                #endregion    
                #region Đặt tour
                routes.MapRoute(
            "dat-tour",
                  "dat-tour/{id}",
                  new { controller = "Content", action = "BookTour" });
                #endregion    
                #region Order
                routes.MapRoute(
                 "thong-tin-dat-hang",
                 "thong-tin-dat-hang/{ordercode}",
                 new { controller = "Cart", action = "OrderInformation" });
                #endregion
                #region Stores 
               // routes.MapRoute(
               //  "Stores",
               //   "he-thong-phan-phoi",
               //   new { controller = "Stores", action = "AllStores" });
               // routes.MapRoute(
               //"StoresP",
               // "he-thong-phan-phoi/page/{p}",
               // new { controller = "Stores", action = "AllStores" });
                #endregion
                routes.MapRoute(
                   "sitemap",
                    "{code}.xml",
                    new { controller = "Sitemap", action = "Index" });
                routes.MapRoute(
                   "feed",
                    "feed",
                    new { controller = "Sitemap", action = "Feed" });                
                routes.MapRoute(
                    "thuong-hieu",
                    "thuong-hieu",
                    new { controller = "Content", action = "TradeMark" });
                
                routes.MapRoute(
                   "product quick view 1",
                    "{nameAscii}/quickview",
                    new { controller = "Content", action = "QuickView" });
                routes.MapRoute(
                   "product quick view 2",
                    "{nameAscii}/{nameAsciiC}/quickview",
                    new { controller = "Content", action = "QuickView" });
                routes.MapRoute(
                   "all content page",
                    "{nameAscii}/{nameAsciiC}/page/{p}",
                    new { controller = "Content", action = "Index" });
                routes.MapRoute(
                   "content page",
                    "{nameAscii}/page/{p}",
                    new { controller = "Content", action = "Index" });
                routes.MapRoute(
                  "Redirect301",
                   "{moduleId}/{contentId}/{nameAscii}",
                   new { controller = "Content", action = "Redirect301" });
                routes.MapRoute(
                  "Redirect301-2",
                   "{moduleId}/{contentId}/{nameAscii}/{nameAsciiC}",
                   new { controller = "Content", action = "Redirect301" });
                routes.MapRoute(
                   "module",
                    "{nameAscii}",
                    new { controller = "Content", action = "Index" });
                routes.MapRoute(
                  "content",
                   "{nameAscii}/{nameAsciiC}",
                   new { controller = "Content", action = "Index" });
                
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}