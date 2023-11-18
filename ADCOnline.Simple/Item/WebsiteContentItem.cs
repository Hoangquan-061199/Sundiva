using ADCOnline.Simple.Admin;
using System;
using System.Collections.Generic;

namespace ADCOnline.Simple.Item
{
    public class WebsiteContentItem : BaseSimple
    {
        public DateTime? CreatedDate { get; set; }
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

        public string ModuleName { get; set; }
        public string ModuleNameAscii { get; set; }

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

        public string ModuleTypeCode { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public double TotalName { get; set; }
        public double TotalDes { get; set; }
        public double TotalContent { get; set; }
        public double TotalComment { get; set; }
        public int? TotalViews { get; set; }
        public string Description { get; set; }
        public string PositionCode { get; set; }
        public string Content { get; set; }
        public string Video { get; set; }
        public string UrlPicture { get; set; }
        public string ModuleViewIds { get; set; }
        public string AlbumPictureJson { get; set; }
        public string ViewHome { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsInternal { get; set; }
        public Guid? CreatorID { get; set; }
        public string CommentCreator { get; set; }
        public string CreatorName { get; set; }
        public string LinkUrl { get; set; }
        public string ModuleIds { get; set; }
        public int? OrderDisplay { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public string DataJson { get; set; }
        public string Tags { get; set; }
        public string TagValue { get; set; }
        public string AdvIds { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceOld { get; set; }
        public string Salary { get; set; }
        public string Experience { get; set; }
        public string Rank { get; set; }
        public string Degree { get; set; }
        public string SuggestContentIDs { get; set; }
        public string RelatedContentIDs { get; set; }
        public DateTime? PublishTime { get; set; }
        public DateTime? MatchTime { get; set; }
        public DateTime? DateIssued { get; set; }
        public int? Status { get; set; }
        public string Comment { get; set; }
        public bool? ManualSeo { get; set; }
        public string RelatedIds { get; set; }
        public string LinkMap { get; set; }
        public string LinkDownload { get; set; }
        public string IMap { get; set; }
        public string LinkFile { get; set; }
        public int? Star { get; set; }
        public string Position { get; set; }
        public string Area { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Hotline { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Website { get; set; }
        public string IndexGoogle { get; set; }
        public string Canonical { get; set; }
        public string OldUrl { get; set; }
        public string Document { get; set; }
        public List<AlbumGalleryItem> AlbumGalleryItems { get; set; }
        public List<FileDownloadAdmin> FileDownloadAdmins { get; set; }

        public string Timespan
        {
            get
            {
                var span = "Vừa xong";
                TimeSpan Time = DateTime.Now - CreatedDate.Value;
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

        public string Mission { get; set; }
        public string Strengths { get; set; }
        public string DoctorTeam { get; set; }
        public string Achievements { get; set; }
        public string AchievementDoctor { get; set; }
        public string ExpertiseDoctor { get; set; }
        public string SpecialistID { get; set; }
        public string SubContent { get; set; }
        public string TypeView { get; set; }
    }
}