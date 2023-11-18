using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class ContactUsViewModel
    {
        public List<ContactUsAdmin> ListItem { get; set; }
        public Product ProductItem { get; set; }
        public WebsiteContent ContentItem { get; set; }
        public ContactUs ContactUs { get; set; }
        public string TypeCode { get; set; }
        public List<CommonJsonAdmin> Accessory { get; set; }
        public List<CommonJsonAdmin> PlateType { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public ADCOnline.Simple.Admin.SearchModel SearchModel { get; set; }
    }
}
