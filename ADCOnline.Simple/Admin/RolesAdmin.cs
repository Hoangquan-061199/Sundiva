using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class RolesAdmin : BaseSimple
    {
        public Guid ApplicationId { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public string LoweredRoleName { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public string DataJson { get; set; }

        public string ModuleIds { get; set; }

        public string WebsiteModule { get; set; }

        public string WebsiteModuleIds { get; set; }

        public string Child { get; set; }

        public string RoleChild { get; set; }
    }
}
