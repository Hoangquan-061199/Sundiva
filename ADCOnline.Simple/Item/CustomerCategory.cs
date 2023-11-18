using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class CustomerCategory : BaseSimple
    {
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDisplay { get; set; }
        public string Code { get; set; }
        public bool? IsShow { get; set; }
    }
}
