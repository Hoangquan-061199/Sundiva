using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using System.Collections.Generic;

namespace Website.ViewModels
{
    public class FooterViewModel
    {
        public List<ModulePositionItem> ListPositionItem { get; set; }
        public SystemConfigJson SystemConfig { get; set; }
        public List<AreaJson> AreaJsons { get; set; }
        public List<WebsiteContentItem> ListContentItems{ get; set;}
        public OtherContentItem Payment { get; set; }
    }
}