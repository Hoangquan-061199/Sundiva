using ADCOnline.Simple.Json;
using System;
using System.Collections.Generic;

namespace ADCOnline.Simple.Item
{
    public class WebsiteModulesItem : BaseSimple
    {
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string NameAscii { get; set; }
        public string LinkUrl { get; set; }
        public string UrlPicture { get; set; }
        public string Icon { get; set; }
        public string Background { get; set; }
        public string AlbumPictureJson { get; set; }
        public string AlbumImageJson { get; set; }
        public string Description { get; set; }
        public string ShortContent { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string LinkFile { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public string Rel { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDisplay { get; set; }
        public int? OrderHome { get; set; }
        public int? OrderTree { get; set; }
        public string PositionCode { get; set; }
        public string ModuleTypeCode { get; set; }
        public string TypeView { get; set; }
        public string TypeViewMenu { get; set; }
        public string TypeViewChild { get; set; }
        public string Code { get; set; }
        public string Video { get; set; }
        public string ViewHome { get; set; }
        public int? TotalViews { get; set; }
        public bool? IsApproved { get; set; }
        public Guid? CreatorID { get; set; }
        public string CreatorName { get; set; }
        public string ModuleIds { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string AdvIds { get; set; }
        public string CityNamAscii { get; set; }
        public string DistrictNamAscii { get; set; }
        public string ModuleNameAscii { get; set; }
        public int? CustomerId { get; set; }
        public string SuggestContentIDs { get; set; }
        public string RelatedContentIDs { get; set; }
        public DateTime? PublishTime { get; set; }
        public int? Status { get; set; }
        public string Comment { get; set; }
        public string LayoutCode { get; set; }
        public string RelateIds { get; set; }
        public string NoteIds { get; set; }
        public string Faqs { get; set; }
        public string Lang { get; set; }
        public bool? IsTableOfContent { get; set; }
        public bool? IsLogin { get; set; }
        public string AttributeModuleIds { get; set; }
        public IEnumerable<WebsiteContentItem> ListContentItem { get; set; }
        public IEnumerable<WebsiteModulesItem> ListModuleChild { get; set; }
        public List<ProductItem> ListProductItem { get; set; }
        public IEnumerable<WebsiteProductItemJson> ListProductJson { get; set; }
        public int? PercentSaleProduct { get; set; }
        public string AttributeModuleIdsCal { get; set; }
        public List<AlbumImageItem> AlbumImageItems { get; set; }
        public List<AlbumGalleryItem> AlbumGalleryItems { get; set; }
        public List<AttributeItem> AttributeItemsRight { get; set; }
        public int? SaleValue { get; set; }
        public int? PromotionID { get; set; }
        public string IndexGoogle { get; set; }
        public string Canonical { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundHeader { get; set; }
        public string BackgroundIndex { get; set; }
        public string BackgroundFooter { get; set; }
    }
}
