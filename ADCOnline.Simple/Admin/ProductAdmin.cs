using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class ProductAdmin : BaseSimple
    {
        public string _Name;
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
        public string CurrentUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(LinkUrl))
                {
                    return ModuleNameAscii + "/" + LinkUrl + "/";
                }
                return ModuleNameAscii + "/" + NameAscii + "/";
            }
        }
        public string LinkUrl { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Video { get; set; }
        public string UrlPicture { get; set; }
        public string Lang { get; set; }
        public string AlbumPictureJson { get; set; }
        public string ColorTable { get; set; }
        public string ViewHome { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? PriceOld { get; set; }
        public int? TotalViews { get; set; }
        public int? TotalLike { get; set; }
        public Guid? CreatorID { get; set; }
        public string CreatorName { get; set; }
        public string ModuleIds { get; set; }
        public int? OrderDisplay { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string ModifiedName { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string ModuleNameAscii{get; set;}
        public bool IsShow { get; set; }
        public bool? IsShowPrice { get; set; }
        public bool? IsVAT { get; set; }
        public bool? IsSitemap { get; set; }
        public string IndexGoogle { get; set; }
        public int Status { get; set; }
        public int TotalOrder { get; set; }
        public string LinkFile { get; set; }
        public string ModuleNames { get; set; }
        public string Information { get; set; }
        public string ContentIds { get; set; }
        public int? TotalComment { get; set; }
        public int? TotalNotApproved { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsBestSell { get; set; }
        public int? TotalRated { get; set; }
        public int? TypeSale { get; set; }
        public double? TypeSaleValue { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountCombo { get; set; }
        public double? TotalStar { get; set; }
        public int? TotalRate { get; set; }
        public int? Amount { get; set; }
        public int? UnSatisfied { get; set; }
        public int? Satisfied { get; set; }
        public string AttributeProductIds { get; set; }
        public int _TotalRate
        {
            get
            {
                if (TotalRate.HasValue)
                    return TotalRate.Value;
                else return 0;
            }
        }
        public double AverageRate
        {
            get
            {
                if (TotalRate.HasValue && TotalStar.HasValue)
                    return Math.Round(TotalStar.Value / TotalRate.Value, 1);
                else return 0;
            }
        }
        public int? BrandId { get; set; }
        public List<int> AttrIds { get; set; }
        public List<ADCOnline.Simple.Base.Attributes> AttributesAdmins { get; set; }
        public List<ADCOnline.Simple.Admin.AttributesAdmin> ListAttributesAdmins { get; set; }
        public List<AttributeContent> AttributeContents { get; set; }
        public List<AlbumExcel> AlbumExcels { get; set; }
        public string GiftText { get; set; }
        public string AttachedProductIds { get; set; }
        public string GiftIds { get; set; }
        public string LogoBrand { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string NameBrand { get; set; }
        public string Canonical { get; set; }        
        public string Timespan
        {
            get
            {
                string span = "Vừa xong";
                TimeSpan Time = DateTime.Now - ModifiedDate.Value;
                var day = Math.Round(Time.TotalDays, 1);
                var hour = Math.Round(Time.TotalHours, 1);
                var minute = Math.Round(Time.TotalMinutes, 1);
                var seconds = Math.Round(Time.TotalSeconds, 1);
                if (day > 0 && day > 1)
                {
                    span = Math.Round(Time.TotalDays, 0).ToString() + " ngày";
                }
                else if (day > 0 && day < 1)
                {
                    span = Math.Round(Time.TotalHours, 0).ToString() + " giờ";
                }
                else
                {
                    if (hour > 0 && hour > 1)
                    {
                        span = Math.Round(Time.TotalHours, 0).ToString() + " giờ";
                    }
                    else if (hour > 0 && hour < 1)
                    {
                        span = Math.Round(Time.TotalMinutes, 0).ToString() + " phút";
                    }
                    else
                    {
                        if (minute > 0 && minute > 1)
                        {
                            span = Math.Round(Time.TotalMinutes, 0).ToString() + " phút";
                        }
                        else if (minute > 0 && minute < 1)
                        {
                            span = Math.Round(Time.TotalSeconds, 0).ToString() + " giây";
                        }
                        else
                        {
                            span = "Vừa xong";
                        }
                    }
                }
                return span;
            }
        }
        public string TypeView { get; set; }
        public int ParentID { get; set; }
    }
}
