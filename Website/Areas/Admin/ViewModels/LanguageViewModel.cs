using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class LanguageViewModel
    {
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<LanguageAdmin> ListItem { get; set; }
        public Language ObjBase { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
