using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Json
{
    public class AreaJson
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; } 
        public List<AgencyJson> AgencyJsons { get; set; }
    }
}
