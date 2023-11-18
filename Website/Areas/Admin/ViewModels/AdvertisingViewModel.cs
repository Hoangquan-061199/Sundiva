using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class AdvertisingViewModel
    {
        public List<AdvertisingAdmin> ListItem { get; set; }
        public List<Advertising> ListBaseItem { get; set; }
        public Advertising Advertising { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<ModulePosition> ListModulePosition { get; set; }
        public List<ModulePositionAdmin> ListModulePositionAdmin { get; set; }
        public List<AlbumGalleryAdmin> ListAlbumGalleryAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<Module> ListModule { get; set; }
    }
}
