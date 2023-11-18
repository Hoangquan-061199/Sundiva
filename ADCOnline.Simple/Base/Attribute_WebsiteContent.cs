using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Base
{
    public class Attribute_WebsiteContent
    {
        public int ID { get; set; }
        public int? ContentID { get; set; }
        public int? AttributeID { get; set; } 
        public int? OrderDisplay { get; set; }
        public decimal? Price { get; set; }
        public string UrlPicture { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
