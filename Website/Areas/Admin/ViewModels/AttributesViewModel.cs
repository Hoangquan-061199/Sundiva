using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System.Collections.Generic;


namespace Website.Areas.Admin.ViewModels
{
    public class AttributesViewModel
    {
        public List<AttributesAdmin> ListItem { get; set; }
        public Attributes Attributes { get; set; }
        public Attributes AttributesItem { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<WebsiteModule> ListWebsiteModule { get; set; }
        public ADCOnline.Simple.Admin.SearchModel SearchModel { get; set; }
        public Dictionary<string, string> ListAttrbuteCode { get; set; }
        public List<CommonJsonAdmin> ListAttributeType { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
    }
}