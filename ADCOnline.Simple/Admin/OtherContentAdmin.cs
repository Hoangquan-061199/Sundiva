using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class OtherContentAdmin : BaseSimple
    {
        public string Name { get; set; }

        public int? OrderBy { get; set; }

        public string Code { get; set; }

        public string Content { get; set; }

        public bool? IsShow { get; set; }

        public bool? IsDeleted { get; set; }

        public int? OrderDisplay { get; set; }

        public string Lang { get; set; }

        public string ContentMobile { get; set; }

    }

}
