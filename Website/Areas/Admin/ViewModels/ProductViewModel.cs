using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class ProductViewModel
    {
        public List<ProductAdmin> ListItem { get; set; }
        public List<SubItem> SubItems { get; set; }
        public List<WebsiteModuleAdmin> BrandAdmins { get; set; }
        public Product Product { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
        public List<ProductAdmin> ProductAdmins { get; set; }
        public List<ProductAdmin> ProductAdminGifts { get; set; }
        public List<FileDownloadAdmin> ListFileDownloadAdmin { get; set; }
        public List<ProductAdmin> ProductAdminReplaces { get; set; }
        public List<AlbumGalleryAdmin> ListAlbumGalleryAdmin { get; set; }
        public List<ColorTableAdmin> ListColorTableAdmin { get; set; }
        public List<ModuleType> ListModuleTypeAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModule { get; set; }
        public WebsiteModule WebsiteModule { get; set; }
        public SearchModel SearchModel { get; set; }
        public string ModuleId { get; set; }
        public string Lang { get; set; }
        public List<AttributesAdmin> ListAttributesAdmin { get; set; }
        public List<AttributesAdmin> AttributesAdmins { get; set; }
        public List<WebsiteContentAdmin> ListContentItem { get; set; }
        public List<WebsiteContentAdmin> ListContentDocumentItem { get; set; }
        public List<Attribute_WebsiteContent> Attribute_WebsiteContents { get; set; }
        public List<WebsiteContentAdmin> ListRelatedNewsItem { get; set; }
        public string ContentIds { get; set; }
        public List<ImportHistoryJson> ImportHistoryJsons { get; set; }
        public List<ImportHistoryExcelWarehouseJson> ImportHistoryExcelWarehouseJsons { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
        public int Total { get; set; }
        public int moduleParentID { get; set; }
        public string ValueSelected { get; set; }
        public int ParentID { get; set; }
        public List<SubContentItem> ListContentSubAdmin { get; set; }
        public List<CommonJsonAdmin> LitsItemTimeTours { get; set; } 
        public List<CommonJsonAdmin> LitsItemAddressStart { get; set; }
    }
}
