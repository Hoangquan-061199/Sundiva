using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class SystemTagViewModel
    {
        public List<SystemTagAdmin> ListItem { get; set; }
        public SystemTag ObjBase { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
        public string ValueSelected { get; set; }
    }
}
