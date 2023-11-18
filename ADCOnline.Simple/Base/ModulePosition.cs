using System;
namespace ADCOnline.Simple.Base
{
    public class ModulePosition
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDisplay { get; set; }
        public string Code { get; set; }
        public string TypeView { get; set; }
        public string SqlModule { get; set; }
        public string SqlContent { get; set; }
        public string SqlContentOrderBy { get; set; }
        public string ModuleTypeCode { get; set; }
        public int? NumberCount { get; set; }
        public int? NumberContent { get; set; }
        public string ModuleIds { get; set; }
        public string UrlPicture { get; set; }
        public string UrlPictureMobile { get; set; }
        public string Icon { get; set; }
        public string Video { get; set; }
        public string LinkBanner { get; set; }
    }
}
