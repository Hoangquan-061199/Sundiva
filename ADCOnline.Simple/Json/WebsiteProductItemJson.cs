using System;

namespace ADCOnline.Simple.Json
{
    public class WebsiteProductItemJson
    {
        public int ID { get; set; }
        public int? TotalRecord { get; set; }
        public string Name { get; set; }
        public string Title{ get; set; }
        public string AlbumPictureJson { get; set; }
        public string _Name { get; set; }
        public string NameAscii { get; set; }
        public string _NameAscii { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string LinkUrl { get; set; }
        public string ModuleIds { get; set; }
        public string ModuleNameAscii { get;set; }
        public string SeoDescription { get;set; }
        public string UrlPicture { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? Price2 { get; set; }
        public decimal? PriceOld { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? TypeSale { get; set; }
        public int? TypeSaleValue { get; set; }
        public bool? IsBestSell { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string PositionCode { get; set; }
        public string PositionIds { get; set; }
        public bool? IsShowPrice { get; set; }
        public string UrlLogoSale { get; set; }
        public string PositionLogoSale { get; set; }
        public string HtmlSale { get; set; }
        public int? BrandID { get; set; }
        public string LogoBrand { get; set; }
        public string NameBrand { get; set; }
        public string ViewHome { get; set; }
        public int? OrderDisplay { get; set; }
        public int? YearManufacture { get; set; }
        public int Star { get; set; }
        public string AllModule { get; set; }
        public string PromotionText { get; set; }
        public string ModuleName { get; set; }
        public string Wattage { get; set; }
        public int? TotalOrder { get; set; }
        public int? TotalRate { get; set; }
        public double? TotalStar { get; set; }
        public int? FramesID { get; set; }
        public double AverageRate
        {
            get
            {
                if (TotalRate.HasValue && TotalStar.HasValue && TotalRate.Value > 0 && TotalStar.Value > 0)
                    return Math.Round(TotalStar.Value / TotalRate.Value, 1);
                else return 0;
            }
        }
        public int? Quantity { get; set; }
        public DateTime? TimeStart { get; set; }
        public string Address { get; set; }
        public string AddressId { get; set; }
        public string Times { get; set; }
        public string TimesValue { get; set; }
    }
}
