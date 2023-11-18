using ADCOnline.Simple.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
   public class WebsiteModuleAdmin : BaseSimple
    {
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }        
        public string NameAscii { get; set; }
        public string LinkUrl { get; set; }
        public string UrlPicture { get; set; }
        public string Icon { get; set; }
        public string Background { get; set; }
        public string Rel { get; set; }
        public string AlbumPictureJson { get; set; }
        public string AlbumImageJson { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Content2 { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public string ViewHome { get; set; }
        public bool IsShow { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSitemap { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int OrderDisplay { get; set; }
        public int? OrderHome { get; set; }
        public string PositionCode { get; set; }
        public string ModuleTypeCode { get; set; }
        public string ModuleType { get; set; }
        public int? TotalProduct { get; set; }
        public int? TotalContent { get; set; }
        public string Lang { get; set; }
        public string LayoutCode { get; set; }
        public string RelateIds { get; set; }
        public string NoteIds { get; set; }
        public int? TotalViews { get; set; }
        public int? TotalContents { get; set; }
        public int? TotalProducts { get; set; }
        public int? TotalAttributes { get; set; }
        public string Faqs { get; set; }
        public bool? IsTableOfContent { get; set; }
        public bool? IsLogin { get; set; }
        public string Video { get; set; }
        public string AttributeModuleIds { get; set; }
        public string TypeView { get; set; }
        public int? PercentSaleProduct { get; set; }
        public string HotlineProduct { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AttributeModuleIdsCal { get; set; }
        public string PriceTextHotline { get; set; }
        public int? PromotionID { get; set; }
        public string Canonical { get; set; }
        public List<AttributesAdmin> Attributes { get; set; }
        public List<ProductAdmin> ProductAdmins { get; set; }
        public List<WebsiteContentAdmin> WebsiteContentAdmins { get; set; }
        public List<WebsiteModuleAdmin> WebsiteModuleAdmins { get; set; }
    }
}
