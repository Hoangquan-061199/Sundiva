using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class ModuleTypeAdmin : BaseSimple
    {
        public string Name { get; set; }

        public bool? IsDeleted { get; set; }

        public int? OrderDisplay { get; set; }

        public bool? IsShow { get; set; }

        public string Code { get; set; }
    }
}
