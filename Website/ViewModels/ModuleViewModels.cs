using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using Website.Models;

namespace Website.ViewModels
{
    public class ModuleViewModels
    {
        public ModulePositionItem PositionItem { get; set; }
        public List<ModulePositionItem> ListPositionItem { get; set; }
        public List<AdvertisingItem> Logo { get; set; }
        public AdvertisingItem BannerRight { get; set; }
        public IEnumerable<AdvertisingItem> BannerGeneral { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleItem { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleChildItem { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleItemParents { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesItems { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesItemsRight { get; set; }
        public IEnumerable<WebsiteModulesItem> WebsiteModulesAllItems { get; set; }
        public WebsiteModulesItem ModuleItem { get; set; }
        public IEnumerable<WebsiteModulesItem> BreadcrumbList { get; set; }
        public WebsiteModulesItem WebsiteModulesItem { get; set; }
        public WebsiteModulesItem ModuleProductTypeII { get; set; }
        public List<string> ListWattages { get; set; }
        public WebsiteModulesItem ModuleContact { get; set; }
        public WebsiteModulesItem moduleContactUs { get; set; }
        public WebsiteModulesItem ModuleParentItem { get; set; }
        public List<SystemTag> ListSystemTags { get; set; }
        public List<WebsiteModulesJson> ListModuleJsonItem { get; set; }
        public List<ProductGroup> ListProductGroup { get; set; }
        public List<ProductItem> ListProductModels { get; set; }
        public IEnumerable<ProductItem> ListProductItem { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemHot { get; set; }
        public IEnumerable<ProductItem> ListProductItemAsync { get; set; }
        public List<ProductItem> ListProductNew { get; set; }
        public List<ProductItem> ListProductSeen { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemJson { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductItemJson2 { get; set; }
        public List<WebsiteProductItemJson> ListProductBestSaleJson { get; set; }
        public CustomerItem CustomerItem { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItem { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemAsync { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemView { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItemVideo { get; set; }
        public List<WebsiteContentItem> ListContentItemHot { get; set; }
        public IEnumerable<WebsiteContentItem> Items { get; set; }
        public WebsiteContentItem WebsiteContentItem { get; set; }
        public WebsiteContentItem ContentItem { get; set; }
        public List<CustomerItem> ListCustomerItems { get; set; }
        public SystemConfigJson SystemConfig { get; set; }
        public SystemConfigJson SystemConfigItem { get; set; }
        public OtherContentItem OtherContentItem { get; set; }
        public OtherContentItem Schema { get; set; }
        public List<TagItem> ListTagItems { get; set; }
        public TagItem TagItem { get; set; }
        public ProductDetail ProductItem { get; set; }
        public int NumberMenu { get; set; }
        public int? Total { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public List<int> Active { get; set; }
        public StringBuilder GridHtml { get; set; }
        public string ContentIds { get; set; }
        public string Code { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double AverageRate { get; set; }
        public IEnumerable<CommentItem> RateItems { get; set; }
        public List<CommonJsonItem> CommonJsonItems { get; set; }
        public List<CommonJsonItem> ListTechnology { get; set; }
        public List<CommonJsonItem> ListServiceCars { get; set; }
        public List<CommonJsonItem> ListAboutConvenient { get; set; }
        public SearchModel SearchModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public List<ADCOnline.Simple.Item.SubItem> SubItems { get; set; }
        public List<AttributeItem> AttributeItems { get; set; }
        public List<AttributeItem> AttributeItemsRight { get; set; }
        public List<AttributeItem> AttributeItemsContent { get; set; }
        public List<CityJson> CityJsons { get; set; }
        public ContactModel ContactModel { get; set; }
        public IEnumerable<WebsiteContentItem> ListTeamViews { get; set; }
        public List<WebsiteModulesItem> ListModuleProducts { get; set; }
        public List<WebsiteModulesItem> ListModuleApplications { get; set; }
        public List<WebsiteModulesItem> ListModuleManufacturers { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleItems { get; set; }
        public List<AdvertisingItem> AdvertisingItems { get; set; }
        public List<WebsiteModulesItem> ListModulesItemTour { get; set; }
    }
}