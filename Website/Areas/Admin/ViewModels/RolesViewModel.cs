using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class RolesViewModel
    {
        public List<RolesAdmin> ListItem { get; set; }
        public AspnetRoles Roles { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}
