using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class ModuleTypeViewModel
    {
        public List<ModuleTypeAdmin> ListItem { get; set; }
        public ModuleType ObjBase { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
