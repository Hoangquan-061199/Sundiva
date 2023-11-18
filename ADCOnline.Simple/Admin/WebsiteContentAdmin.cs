using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class WebsiteContentAdmin : BaseSimple
    {
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
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
        public int? TotalViews { get; set; }
        public bool? IsApproved { get; set; }
        public Guid? CreatorID { get; set; }
        public string CreatorName { get; set; }
        public string ModuleIds { get; set; }
        public int? OrderDisplay { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DateIssued { get; set; }
        public DateTime? PublishDate { get; set; }
        public string ModifiedName { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string AdvIds { get; set; }
        public string ModuleNameAscii { get; set; }
        public string ModuleNames { get; set; }
        public string ModuleName { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CustomerId { get; set; }
        public int? Status { get; set; }
        public bool? ManualSeo { get; set; }
        public bool? IsSitemap { get; set; }
        public string ModuleType { get; set; }
        public string LinkMap { get; set; }
        public string IMap { get; set; }
        public int? Star { get; set; }
        public int? TotalComment { get; set; }
        public string Canonical { get; set; }
        public string Timespan
        {
            get
            {
                var date = new DateTime();
                if (CreatedDate.HasValue)
                {
                    date = CreatedDate.Value;
                }
                var span = "Vừa xong";
                TimeSpan Time = DateTime.Now - date;
                var day = Math.Round(Time.TotalDays, 1);
                var hour = Math.Round(Time.TotalHours, 1);
                var minute = Math.Round(Time.TotalMinutes, 1);
                var seconds = Math.Round(Time.TotalSeconds, 1);
                if (day > 0 && day > 1)
                {
                    span = Math.Round(Time.TotalDays, 0).ToString() + " ngày trước";
                }
                else if (day > 0 && day < 1)
                {
                    span = Math.Round(Time.TotalHours, 0).ToString() + " giờ trước";
                }
                else
                {
                    if (hour > 0 && hour > 1)
                    {
                        span = Math.Round(Time.TotalHours, 0).ToString() + " giờ trước";
                    }
                    else if (hour > 0 && hour < 1)
                    {
                        span = Math.Round(Time.TotalMinutes, 0).ToString() + " phút trước";
                    }
                    else
                    {
                        if (minute > 0 && minute > 1)
                        {
                            span = Math.Round(Time.TotalMinutes, 0).ToString() + " phút trước";
                        }
                        else if (minute > 0 && minute < 1)
                        {
                            span = Math.Round(Time.TotalSeconds, 0).ToString() + " giây trước";
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
        public string SpecialistID { get; set; }
    }
}
