using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class AttributeContent
    {
        public int AttributeID { get; set; }
        public int ContentID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }
        public decimal? Price { get; set; }
    }
}
