using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class CommentAdmin :BaseSimple
    {
        public int? Commentator { get; set; }
        public int? ContentID { get; set; }
        public int? ProductID { get; set; }
        public int? CustomerID { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UrlPicture { get; set; }
        public string MediaJson { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsApproved { get; set; }
        public string Act { get; set; }
        public int? ParentID { get; set; }
        public int? TotalReply { get; set; }
        public int? Rate { get; set; }
        public int? Good { get; set; }
        public int? Bad { get; set; }
        public string ContentName { get; set; }
        public string ContentNameAscii { get; set; }
        public string ContentModuleNameAscii { get; set; }
        public string ProductName { get; set; }
        public string ProductNameAscii { get; set; }
        public string ProductModuleNameAscii { get; set; }
        public string ProductCode { get; set; }
        public Guid AdminId { get; set; }
        public List<CommentAdmin> Replies { get; set; }
        public MembershipAdmin membershipAdmin { get; set; }
        public string Timespan
        {
            get
            {
                string span = "Vừa xong";
                TimeSpan Time = DateTime.Now - CreatedDate.Value;
                var day = Math.Round(Time.TotalDays, 1);
                var hour = Math.Round(Time.TotalHours, 1);
                var minute = Math.Round(Time.TotalMinutes, 1);
                var seconds = Math.Round(Time.TotalSeconds, 1);
                if (day > 0 && day > 1)
                {
                    span = "Khoảng " + Math.Round(Time.TotalDays, 0).ToString() + " ngày trước";
                }
                else if (day > 0 && day < 1)
                {
                    span = "Khoảng " + Math.Round(Time.TotalHours, 0).ToString() + " giờ trước";
                }
                else
                {
                    if (hour > 0 && hour > 1)
                    {
                        span = "Khoảng " + Math.Round(Time.TotalHours, 0).ToString() + " giờ trước";
                    }
                    else if (hour > 0 && hour < 1)
                    {
                        span = "Khoảng " + Math.Round(Time.TotalMinutes, 0).ToString() + " phút trước";
                    }
                    else
                    {
                        if (minute > 0 && minute > 1)
                        {
                            span = "Khoảng " + Math.Round(Time.TotalMinutes, 0).ToString() + " phút trước";
                        }
                        else if (minute > 0 && minute < 1)
                        {
                            span = "Khoảng " + Math.Round(Time.TotalSeconds, 0).ToString() + " giây trước";
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
    }
}
