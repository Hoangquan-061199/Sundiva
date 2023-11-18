using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class ProductDetail : BaseSimple
    {
        public string ModuleViewIds { get; set; }
        public string PositionCode { get; set; }
        public string ModuleTypeCode { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public double TotalNameAscii { get; set; }

        public string Name_
        {
            get
            {
                return _Name;
            }
        }

        //trường bắt buộc thêm_end

        private string _Name;

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductCode) && !_Name.Contains(ProductCode))
                    return _Name + " " + ProductCode;
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public string ProductCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string Model { get; set; }
        public string Warranty { get; set; }
        public string _NameAscii { get; set; }

        public string NameAscii
        {
            get
            {
                if (!string.IsNullOrEmpty(ModuleNameAscii))
                    return ModuleNameAscii + "/" + _NameAscii;
                return _NameAscii;
            }
            set
            {
                _NameAscii = value;
            }
        }

        public string LinkUrl { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Document { get; set; }
        public string Video { get; set; }
        public string UrlPicture { get; set; }
        public string UrlLogoSale { get; set; }
        public string PositionLogoSale { get; set; }
        public string AlbumPictureJson { get; set; }
        public string ColorTable { get; set; }
        public string ViewHome { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public string Title { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceOld { get; set; }
        public int? TypeSale { get; set; }
        public int? TypeSaleValue { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountCombo { get; set; }
        public int PercentSale { get; set; } = 0;

        public double AverageRate
        {
            get
            {
                if (TotalRate.HasValue && TotalStar.HasValue && TotalRate.Value > 0 && TotalStar.Value > 0)
                    return Math.Round(TotalStar.Value / TotalRate.Value, 1);
                else return 0;
            }
        }

        public int? TotalViews { get; set; }
        public int? TotalLike { get; set; }
        public bool? IsApproved { get; set; }
        public System.Guid? CreatorID { get; set; }
        public string CreatorName { get; set; }
        public int? CustomerID { get; set; }
        public string CustomerUserName { get; set; }
        public string ModuleIds { get; set; }
        public int? TotalOrder { get; set; }

        public int? RankCustomer { get; set; }
        public double? RankProduct { get; set; }
        public int? OrderDisplay { get; set; }
        public int? DeliveryTime { get; set; }
        public bool? IsAdv { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsShowPrice { get; set; }
        public bool? IsVAT { get; set; }
        public bool? IsDeleted { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string AdvIds { get; set; }
        public string CityIds { get; set; }
        public string DistrictIds { get; set; }
        public int? Status { get; set; }
        public string Code { get; set; }
        public string PackageDataJson { get; set; }
        public string QADataJson { get; set; }
        public string RequireDataJson { get; set; }
        public string CustomerDataJson { get; set; }
        public string ModuleNameAscii { get; set; }
        public string LinkDownload { get; set; }
        public bool? IsServeAtHome { get; set; }
        public int? DeliveryType { get; set; }
        public int? PaymentType { get; set; }
        public string DeliveryLocationJson { get; set; }
        public string AttributeProductIds { get; set; }
        public bool? IsOnline { get; set; }
        public bool? IsStopped { get; set; }
        public int? TotalFavorited { get; set; }
        public bool IsDenied { get; set; }
        public int? RateMark { get; set; }
        public int? Quantity { get; set; }
        public double OrderPrice { get; set; }
        public bool? IsAutoComment { get; set; }
        public string LinkFile { get; set; }
        public string Information { get; set; }
        public string Guide { get; set; }
        public string Advantage { get; set; }
        public string ContentVideo { get; set; }
        public string PreliminaryQuotes { get; set; }
        public string WarningContent { get; set; }
        public string ContentIds { get; set; }
        public int? TotalRated { get; set; }
        public bool? IsBestSell { get; set; }

        public double? TotalStar
        {
            get
            {
                return Rate5 * 5 + Rate4 * 4 + Rate3 * 3 + Rate2 * 2 + Rate1;
            }
        }

        public int? TotalRate { get; set; }
        public double TotalStars { get; set; }
        public int? Rate5 { get; set; }
        public int? Rate4 { get; set; }
        public int? Rate3 { get; set; }
        public int? Rate2 { get; set; }
        public int? Rate1 { get; set; }

        public int BestRate
        {
            get
            {
                if (Rate5.HasValue) return 5;
                else if (Rate4.HasValue) return 4;
                else if (Rate3.HasValue) return 3;
                else if (Rate2.HasValue) return 2;
                else if (Rate1.HasValue) return 1;
                else return 0;
            }
        }

        public int? Amount { get; set; }
        public string PriceText { get; set; }
        public string SaleTextMore { get; set; }
        public int? BrandID { get; set; }
        public string HtmlSale { get; set; }
        public string NameBrand { get; set; }
        public string LogoBrand { get; set; }

        public int _TotalRate
        {
            get
            {
                if (TotalRate.HasValue)
                    return TotalRate.Value;
                else return 0;
            }
        }

        public int? UnSatisfied { get; set; }
        public int? Satisfied { get; set; }

        public int? TotalSatisfied
        {
            get
            {
                return (Satisfied.HasValue ? Satisfied : 0) + (UnSatisfied.HasValue ? UnSatisfied : 0);
            }
        }

        public IEnumerable<AttributeItem> AttributeItem { get; set; }
        public List<AlbumGalleryItem> AlbumGalleryItems { get; set; }
        public List<ColorTableItem> ColorTableItems { get; set; }
        public List<ProductItem> ListWattageProducts { get; set; }
        public string TextNotePrice { get; set; }
        public int? YearManufacture { get; set; }
        public string GiftText { get; set; }
        public string AttachedProductIds { get; set; }
        public string PromotionText { get; set; }
        public string GiftIds { get; set; }
        public string ReplaceIds { get; set; }
        public string RelatedIds { get; set; }
        public string DocumentIds { get; set; }
        public string IndexGoogle { get; set; }
        public string Canonical { get; set; }
        public string TagValue { get; set; }
        public int? FramesID { get; set; }
        public string SubContent { get; set; }
        public int ParentID { get; set; }
        public List<SubItem> ListSubItems { get; set; }
        public string Address { get; set; }
        public string Times { get; set; }
        public string TimesValue { get; set; }
        public DateTime? TimeStart { get; set; }
        public string Shipping { get; set; }
        public string AddressId { get; set; }
        public int Star { get; set; }
    }
}