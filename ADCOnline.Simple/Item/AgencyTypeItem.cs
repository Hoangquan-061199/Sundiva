using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class AgencyTypeItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDisplay { get; set; }
        public string UrlPicture { get; set; }
    }
}
