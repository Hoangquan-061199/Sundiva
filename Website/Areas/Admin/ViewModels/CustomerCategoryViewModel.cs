using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.ViewModels
{
    public class CustomerCategoryViewModel
    {
        public List<CustomerCategoryAdmin> ListItem { get; set; }
        public CustomerCategory CustomerCategory { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}
