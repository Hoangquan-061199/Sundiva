using ADCOnline.Simple.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class ContactUsAdmin : BaseSimple
    {

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public int? Gender { get; set; }
        public DateTime? Dateofbirth { get; set; }
        public string LinkFile { get; set; }
        public string Code { get; set; }
        public string TypeOfQuestion { get; set; }
        public string YourQuestion { get; set; }
        public string Industry { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string CompanyAddress { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Nationality { get; set; }
        public string ProductName { get; set; }
        public string ProductLink { get; set; }
        public string ProductCode { get; set; }
        public string Number { get; set; }
        public string TypeCode { get; set; }
        public int ProductID { get; set; }
        public int ContentID { get; set; }
        public DateTime? Time { get; set; }
        public string Store { get; set; }
        public string City { get; set; }
        public string District { get; set; }

        public Product ProductItem { get; set; }
        public WebsiteContent ContentItem { get; set; }
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
