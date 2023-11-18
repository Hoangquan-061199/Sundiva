using System;
namespace ADCOnline.Simple.Item
{
    public class Attribute_WebsiteContentItem : BaseSimple
    {
        public int? ContentID { get; set; }
        public int? AttributeID { get; set; }
        public int? OrderDisplay { get; set; }
        public decimal? Price { get; set; }
        public string UrlPicture { get; set; }
    }
}
