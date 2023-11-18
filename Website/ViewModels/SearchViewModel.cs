using System.Collections.Generic;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;

namespace Website.ViewModels
{
    public class SearchViewModel
    {
        public WebsiteModulesItem ModuleItem { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<WebsiteContentItem> ListArticle { get; set; }
        public IEnumerable<WebsiteContentItem> ListProject { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentSpecialistItem { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentAddressItem { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemAsync { get; set; }
        public IEnumerable<WebsiteContentItem> ListRecuiment { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemJson { get; set; }
        public IEnumerable<ProductItem> ListProducts { get; set; }
        public IEnumerable<WebsiteContentItem> ListAlbum { get; set; }
        public IEnumerable<WebsiteContentItem> ListDocument { get; set; }
        public IEnumerable<WebsiteContentItem> ListQA { get; set; }
        public IEnumerable<WebsiteContentItem> Listvideo { get; set; }
        public List<CommonJsonItem> Positions { get; set; }
        public List<CommonJsonItem> ListTimes { get; set; }
        public List<CommonJsonItem> ListAddressStart { get; set; }
        public List<CommonJsonItem> Areas { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModulesItem { get; set; }
        public int? Total{get;set;}
        public int? PageSize{get;set;}
        public string Keyword { get;set;}
        public int SpecialistId { get;set;}
        public int AddressId { get;set;}
        public int? Page { get; set; }
        public WebsiteModulesItem ModuleParentItem { get; set; }
        public SystemConfigJson SystemConfigItem { get; set; }
    }
}
