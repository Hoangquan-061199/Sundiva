using System;
namespace ADCOnline.Simple.Base
{
    public class Advertising
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string UrlPicture { get; set; }
        public string UrlPictureMobile { get; set; }
        public string AlbumPictureJson { get; set; }
        public string LinkUrl { get; set; }
        public string Video { get; set; }
        public string AlbumVideo { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int? OrderDisplay { get; set; }
        public int? ParentID { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PositionCode { get; set; }
        public string PositionIds { get; set; }
        public string DataJson { get; set; }
        public string ContentIds { get; set; }
        public int? Type { get; set; }
        public string ContentHtml { get; set; }
        public string PositionName { get; set; }
        public string Lang { get; set; }
        public string ModuleIds { get; set; }
    }
}
