using Microsoft.AspNetCore.Builder;
using Website.Utils;

namespace Website
{
    public class RouteAdminConfig
    {
        public RouteAdminConfig(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Admin_default1",
                    template: WebConfig.AdminAlias + "/{controller=HomeAdmin}/{action=Index}"
                    );
                //   routes.MapRoute(
                //name: "areaRoute",
                //template: "{area:exists}/{controller=AccountAdmin}/{action=Login}");
            });
        }
    }
}
