using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class DepartmentViewModel
    {
        public List<DepartmentAdmin> ListItem { get; set; }
        public Department Department { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
