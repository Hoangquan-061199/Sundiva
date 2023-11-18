using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class SubItemViewModel
    {
        public List<SubItemAdmin> ListItem { get; set; }
        public List<SubItem> ListBaseItem { get; set; }
        public SubItem SubItem { get; set; }
        public Product Product { get; set; }
        public WebsiteContent WebsiteContent { get; set; }
        public WebsiteModule WebsiteModule { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<AlbumImageAdmin> AlbumImageAdmins { get; set; }
        public string StringBuilder { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
    }
}
