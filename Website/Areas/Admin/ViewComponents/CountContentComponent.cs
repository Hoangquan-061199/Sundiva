using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Simple.Admin;
using Website.Utils;

namespace Areas.Admin.ViewComponents
{
    public class CountContentComponent : ViewComponent
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly WebsiteContentDa _websiteContentDa;
        public CountContentComponent()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
        }
        public IViewComponentResult Invoke(int id)
        {
            int count = 0;
            if (id > 0)
            {
                List<WebsiteModuleAdmin> child = _websiteModuleDa.GetListChidrent(id);
                count = _websiteContentDa.CountContentyModuleIds(string.Join(",", child.Select(x => x.ID.ToString())), Lang());
            }
            return View(count);
        }
        private string Lang() => Request.Cookies["lanad"] != null ? Utility.ValidString(Request.Cookies["lanad"], "Code", true) : StaticEnum.DefaultLanguage;
    }
}
