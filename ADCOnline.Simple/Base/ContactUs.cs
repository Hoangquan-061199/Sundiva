using System;
namespace ADCOnline.Simple.Base
{
    public class ContactUs
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }
        public string Number { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Accessory { get; set; }
        public string PlateType { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
        public int? Gender { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TypeOfQuestion { get; set; }
        public string YourQuestion { get; set; }
        public string Industry { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string CompanyAddress { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Nationality { get; set; }
        public string Code { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string ProductName { get; set; }
        public string ProductLink { get; set; }
        public string ProductCode { get; set; }
        public string TypeCode { get; set; }
        public int ProductID { get; set; }
        public int ContentID { get; set; }
        public DateTime? Time { get; set; }
        public string Store { get; set; }
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
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Destination { get; set; }
        public string PointGo { get; set; }
        public string Service { get; set; }
        public int NumberElder { get; set; }
        public int NumberChildren { get; set; }
        public int NumberChildren2 { get; set; }
        public int NumberChildren3 { get; set; }

    }
}
