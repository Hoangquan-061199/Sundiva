using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class WebsiteModuleViewModel
    {
        public List<WebsiteModuleAdmin> ListItem { get; set; }
        public List<ModuleType> ListModuleType { get; set; }
        public List<ModulePosition> ListModulePosition { get; set; }
        public List<WebsiteContentAdmin> ListSelected { get; set; }
        public List<WebsiteContentAdmin> ListNote { get; set; }
        public List<WebsiteContentAdmin> ListFag { get; set; }
        public WebsiteModule WebsiteModule { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<AlbumGalleryAdmin> ListAlbumGalleryAdmin { get; set; }
        public List<AlbumImageAdmin> ListAlbumImageAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListRelated { get; set; }
        public string StringBuilder { get; set; }
        public string StringTradeBuilder { get; set; }
        public int Type { get; set; }
        public List<AttributesAdmin> AttributesAdmins { get; set; }
        public List<AdvertisingAdmin> AdvertisingAdmins { get; set; }
        public string AdvertisingIds { get; set; }
        public string ValuesSelected { get; set; }
        public string Code { get; set; }
        public string Types { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<Module> ListModule { get; set; }
    }
}
