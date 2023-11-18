using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System.Collections.Generic;


namespace Website.Areas.Admin.ViewModels
{
    public class ModuleViewModel
    {
        public List<ModuleAdmin> ListItem { get; set; }
        public Module Module { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public string StringBuilder { get; set; }
        public bool SelectMutil { get; set; }
        public int Type { get; set; }
    }
}