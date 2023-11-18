using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Json
{
    public class SitemapJson
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public int? Priority { get; set; } = 80;
        public string ChangeFrequency { get; set; }
        public DateTime? LastModified{get;set;}
        public DateTime? CreatedDate { get; set; }
        public string Code { get; set; }
        public int? ParentID { get; set; }
        public string ModuleIds { get; set; }
    }
}
