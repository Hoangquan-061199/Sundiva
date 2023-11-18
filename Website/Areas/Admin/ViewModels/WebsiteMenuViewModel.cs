using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.ViewModels
{
    public class WebsiteMenuViewModel
    {
        public List<WebsiteMenuAdmin> ListItem { get; set; }
        public List<WebsiteModuleAdmin> ListModule { get; set; }
        public WebsiteMenu ObjBase { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
