using System;

namespace ADCOnline.Simple.Json
{
    public class ProductItemJson
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string _Name
        {
            get
            {
                if (!string.IsNullOrEmpty(ProductCode))
                    return Name + " " + ProductCode;
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        public string NameAscii { get; set; }
        public string _NameAscii
        {
            get
            {
                if (!string.IsNullOrEmpty(ModuleNameAscii))
                    return ModuleNameAscii + "/" + NameAscii;
                return NameAscii;
            }
            set
            {
                NameAscii = value;
            }
        }
        public string LinkUrl { get; set; }
        public string ModuleIds { get; set; }
        public string ModuleNameAscii
        {
            get; set;
        }
        public string UrlPicture { get; set; }
        public string ProductCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceOld { get; set; }
        public int? TypeSale { get; set; }
        public int? TypeSaleValue { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsBestSell { get; set; }
        public string PositionCode { get; set; }
        public string PositionIds { get; set; }
        public string ModuleViewIds { get; set; }
        public bool? IsShowPrice { get; set; }
        public string UrlLogoSale { get; set; }
        public string ViewHome { get; set; }
        public int? OrderDisplay { get; set; }
        public int? BrandID { get; set; }
        public string LogoBrand { get; set; }
        public string NameBrand { get; set; }
        public string HtmlSale { get; set; }
        public int? TotalOrder { get; set; }
        public int? Star { get; set; }
        public int? TotalRate { get; set; }
        public double? TotalStar { get; set; }
        public double AverageRate
        {
            get
            {
                if (TotalRate.HasValue && TotalStar.HasValue && TotalRate.Value > 0 && TotalStar.Value > 0)
                    return Math.Round(TotalStar.Value / TotalRate.Value, 1);
                else return 0;
            }
        }
    }
}
