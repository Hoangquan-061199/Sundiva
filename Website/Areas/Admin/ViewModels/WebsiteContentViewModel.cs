using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class WebsiteContentViewModel
    {
        public List<WebsiteContentAdmin> ListItem { get; set; }
        public List<WebsiteContentAdmin> ListSpecialistItem { get; set; }
        public List<WebsiteContentAdmin> ListAddressItem { get; set; }
        public List<ProductAdmin> ListProduct { get; set; }
        public List<ProductAdmin> ListProductRelated { get; set; }
        public List<ProductAdmin> ListRelatedProduct { get; set; }
        public SearchModel SearchModel { get; set; }
        public WebsiteContent WebsiteContent { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
        public List<AlbumGalleryAdmin> ListAlbumGalleryAdmin { get; set; }
        public List<FileDownloadAdmin> ListFileDownloadAdmin { get; set; }
        public List<ModuleType> ListModuleTypeAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModule { get; set; }
        public List<WebsiteContentAdmin> ListRelated { get; set; }
        public List<SystemTagAdmin> SystemTagAdminLastest { get; set; }
        public List<SystemTagAdmin> SystemTagAdminTop { get; set; }
        public List<SystemTag> SystemTags { get; set; }
        public List<CommonJsonAdmin> commonJsonAdmins { get; set; }
        public List<CommonJsonAdmin> Position { get; set; }
        public List<CommonJsonAdmin> Area { get; set; }
        public string ValueSelected { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
        public string ModuleTypeCode { get; set; }
        public string Role { get; set; }
        public string UserId { get; set; }
        public List<SubContentItem> ListContentSubAdmin { get; set; }
    }
}
