using System.Collections.Generic;
using ADCOnline.Simple.Admin;

namespace Website.Areas.Admin.ViewModels
{
    public class ContentDataJsonViewModel
    {
        public List<CommonJsonAdmin> GetListItem { get; set; }
        public List<CommonJsonAdmin> CityJson { get; set; }
        public List<CommonJsonAdmin> PhoneRegister { get; set; }
        public CommonJsonAdmin CommonJsonAdmin { get; set; }
        public string TypeJson { get; set; }
        public string Lang { get; set; }
        public SearchModel SearchModel { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}
