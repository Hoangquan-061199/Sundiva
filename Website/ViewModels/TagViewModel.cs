using ADCOnline.Simple.Item;
using System.Collections.Generic;

namespace Website.ViewModels
{
    public class TagViewModel
    {
        public IEnumerable<WebsiteContentItem> ListContentItemAsync { get; set; }
        public IEnumerable<ProductItem> ListProductItemAsync { get; set; }
        public SearchModel SearchModel { get; set; }
        public TagItem TagItem { get; set; }
        public int? Total { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesItems { get; set; }
    }
}