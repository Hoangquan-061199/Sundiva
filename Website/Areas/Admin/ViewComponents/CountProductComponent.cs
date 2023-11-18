using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Utils;
using ADCOnline.Simple.Admin;

namespace Areas.Admin.ViewComponents
{
    public class CountProductComponent : ViewComponent
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ProductDa _productDa;
        public CountProductComponent()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
        }
        public IViewComponentResult Invoke(int id)
        {
            int count = 0;
            if (id > 0)
            {
                List<WebsiteModuleAdmin> child = _websiteModuleDa.GetListChidrent(id);
                count = _productDa.CountProductByModuleIds(string.Join(",", child.Select(x => x.ID.ToString())), Lang());
            }
            return View(count);
        }
        private string Lang() => Request.Cookies["lanad"] != null ? Utility.ValidString(Request.Cookies["lanad"], "Code", true) : StaticEnum.DefaultLanguage;
    }
}
