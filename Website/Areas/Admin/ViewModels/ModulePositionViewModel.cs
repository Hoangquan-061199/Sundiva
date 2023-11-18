using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class ModulePositionViewModel
    {
        public List<ModulePositionAdmin> ListItem { get; set; }
        public List<ModulePosition> ListBaseItem { get; set; }
        public ModulePosition ModulePosition { get; set; }
        public List<ModuleType> ListModuleType { get; set; }
        public List<WebsiteModule> ListWebsiteModule { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public string StringBuilder { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
