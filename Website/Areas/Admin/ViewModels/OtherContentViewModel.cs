using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class OtherContentViewModel
    {
        public List<OtherContentAdmin> ListItem { get; set; }
        public OtherContent OtherContent { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
