using System.Collections.Generic;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;

namespace Website.ViewModels
{
    public class ContentViewModels
    {
        public IEnumerable<WebsiteContentItem> ListContentItem { get; set; }
        public List<SystemTag> ListSystemTags { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentHightlightsItem { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemView { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleItems { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemNew { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemVideoo { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemProjectHot { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemServiceHot { get; set; }
        public List<WebsiteContentItem> RelatedContent { get; set; }
        public IEnumerable<WebsiteModulesItem> BreadcrumbList { get; set; }
        public List<WebsiteContentItem> RelatedDocumentContent { get; set; }
        public List<WebsiteContentItem> RelatedItems { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItem2 { get; set; }
        public List<WebsiteContentItem> ListContentItemProjects { get; set; }
        public WebsiteContentItem ContentItem { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItem { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemNew { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductHot { get; set; }
        public List<ColorTableItem> ColorTableItems { get; set; }
        public IEnumerable<WebsiteProductItemJson> RelatedProduct { get; set; }
        public List<WebsiteProductItemJson> ListProductCompare { get; set; }
        public List<WebsiteProductItemJson> ListProductSeen { get; set; }
        public List<WebsiteModulesItem> ListModuleProducts { get; set; }
        public IEnumerable<ProductItem> ListProductGroup { get; set; }
        public IEnumerable<ProductItem> ListProductAttach { get; set; }
        public IEnumerable<ProductItem> Gifts { get; set; }
        public IEnumerable<ProductItem> Replaces { get; set; }
        public IEnumerable<AdvertisingItem> AdvertisingItems { get; set; }
        public IEnumerable<ADCOnline.Simple.Item.SubItem> SubItems { get; set; }
        public IEnumerable<TagItem> TagItems { get; set; }
        public SystemConfigJson SystemConfigJson { get; set; }
        public List<WebsiteContentItem> ListQA { get; set; }

        // public CustomerItem CustomerItem { get; set; }
        public List<AttributeItem> AttributeItemsContent { get; set; }
        public List<ProductItem> ListProductSub { get; set; }
        public ProductDetail ProductItem { get; set; }
        public WebsiteModulesItem WebsiteModulesItem { get; set; }
        public WebsiteModulesItem moduleContactUs { get; set; }
        public List<FileDownloadAdmin> ListFileDownload { get; set; }
        public WebsiteModulesItem ModuleItem { get; set; }
        public List<ProductItem> ListProductModels { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModulesItem { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModulesProductItems { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesItems { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleItemParents { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleManufacturers { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemHot { get; set; }
        public Order Order { get; set; }
        public CustomerItem CustomerItem { get; set; }
        public IEnumerable<CommentItem> CommentItems { get; set; }
        public IEnumerable<CommentItem> RateItems { get; set; }
        public int TotalComment { get; set; }
        public int TotalRate { get; set; }
        public int Pagesize { get; set; }
        public IEnumerable<CommonJsonItem> Frames { get; set; }
        public IEnumerable<CommonJsonItem> Areas { get; set; }
        public IEnumerable<AttributeItem> AttributeItems { get; set; }
        public List<AttributeItem> AttributeItemCals { get; set; }
        public List<Attribute_WebsiteContentItem> AttributeWebsiteContentItems { get; set; }
        public OtherContentItem Schema { get; set; }
        public SystemConfigJson SystemConfigItem { get; set; }
        public List<AdvertisingItem> ListAds { get; set; }
    }
}