using System;

namespace ADCOnline.Simple.Base
{
    public class WebsiteContent
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string NameAscii { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Video { get; set; }
        public string UrlPicture { get; set; }
        public string AlbumPictureJson { get; set; }
        public string ViewHome { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public decimal? Price { get; set; }
        public string Salary { get; set; }
        public string Experience { get; set; }
        public string Rank { get; set; }
        public string Degree { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? TotalViews { get; set; }
        public bool? IsApproved { get; set; }
        public Guid? CreatorID { get; set; }
        public string CreatorName { get; set; }
        public string ModuleIds { get; set; }
        public int? OrderDisplay { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsInternal { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DateIssued { get; set; }
        public string ModifiedName { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string TagValue { get; set; }
        public string AdvIds { get; set; }
        public string ModuleNameAscii { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int? CustomerId { get; set; }
        public int? Status { get; set; }
        public bool? ManualSeo { get; set; }
        public string Lang { get; set; }
        public string RelatedIds { get; set; }
        public string LinkMap { get; set; }
        public string IMap { get; set; }
        public string LinkFile { get; set; }
        public string LinkDownload { get; set; }
        public int? Star { get; set; }
        public string IndexGoogle { get; set; }
        public string Canonical { get; set; }
        public bool? IsSitemap { get; set; }
        public string Position { get; set; }
        public string Code { get; set; }
        public string Area { get; set; }
        public string Phone { get; set; }
        public string Hotline { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string TypeView { get; set; }
        public string Mission { get; set; }
        public string Strengths { get; set; }
        public string SpecialistID { get; set; }
        public string AddressID { get; set; }
        public string SubContent { get; set; }
    }
}