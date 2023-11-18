using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class SystemConfigViewModel
    {
        public List<SystemConfigAdmin> ListItem { get; set; }
        public SystemConfig SystemConfig { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public Dictionary<string, string> ConfigPopup { get; set; }
    }
}
