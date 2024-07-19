using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Website.ViewComponents
{
    public class BlockDevtoolComponent : BaseComponent
    {
        protected readonly IWebHostEnvironment HostEnvironment;

        public BlockDevtoolComponent(IWebHostEnvironment hostEnv)
        {
            this.HostEnvironment = hostEnv;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.IsDev = false;
            if (this.HostEnvironment.IsDevelopment())
            {
                ViewBag.IsDev = true;
            }
            return View();
        }
    }
}