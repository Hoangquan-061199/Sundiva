using System;
namespace ADCOnline.Simple.Base
{
    public class SystemTag
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? OrderBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public string NameAscii { get; set; }
        public bool? IsHome { get; set; }
        public int? Weight { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ContentIDs { get; set; }
    }
}
