using System;
namespace ADCOnline.Simple.Base
{
    public class Attributes
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string NameAscii { get; set; }
        public string LinkUrl { get; set; }
        public string Code { get; set; }
        public string Lang { get; set; }
        public string Description { get; set; }
        public string ListModuleIds { get; set; }
        public string UrlPicture { get; set; }
        public string UrlPicture2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsAllowsFillter { get; set; }
        public bool? IsDisplayForHover { get; set; }
        public bool? IsShowOC { get; set; }
        public bool? IsShowContent { get; set; }
        public string ClassCss { get; set; }
        public int? OrderDisplay { get; set; }
        public int? OrderOC { get; set; }
        public string Type { get; set; }
        public double? Weight { get; set; }
        public int? TypeShow { get; set; }
    }
}
