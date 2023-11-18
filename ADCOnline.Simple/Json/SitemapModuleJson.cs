using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Json
{
    public class SitemapModuleJson
    {
        public int ID { get; set; }        
        public string NameAscii { get; set; }
        public string LinkUrl { get; set; }       
        public int? ParentID { get; set; }       
        public string ModuleTypeCode { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsSitemap { get; set; }
        public int Level { get; set; }
    }
}
