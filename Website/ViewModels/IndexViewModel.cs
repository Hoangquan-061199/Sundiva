using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using System.Collections.Generic;

namespace Website.ViewModels
{
    public class IndexViewModel
    {
        public List<ModulePositionItem> ListPositionItem { get; set; }
        public List<WebsiteModulesItem> ListModuleManufacturers { get; set; }
        public List<WebsiteContentItem> ListContentItem { get; set; }
        public IEnumerable<WebsiteContentItem> LastestContentItems { get; set; }
        public SystemConfigJson SystemConfigItem { get; set; }
        public IEnumerable<WebsiteMenuItem> WebsiteMenuItems { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesItems { get; set; }
        public WebsiteModulesItem ModuleFormPopup { get; set; }
        public List<CommonJsonItem> Accessory { get; set; }
        public List<CommonJsonItem> CustomerOrder { get; set; }
        public List<CommonJsonItem> ListTechnology { get; set; }
        public List<CommonJsonItem> ListCurtomerReview { get; set; }
        public List<LanguageItem> LanguageItems { get; set; }
    }
}