using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class UserAdminViewModel
    {
        public List<MembershipAdmin> ListItem { get; set; }
        public AspnetMembership Membership { get; set; }
        public MembershipAdmin MembershipAdmin { get; set; }
        public List<ActiveRole> ListActiveRole { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<Department> ListDepartment { get; set; }
        public List<RolesAdmin> ListRolesAdmin { get; set; }
        public List<ModuleAdmin> ListModuleAdmin { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
        public List<CommonJsonAdmin> CommonJsonAdmins { get; set; }
    }
}
