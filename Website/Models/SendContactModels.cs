using System;
using System.ComponentModel.DataAnnotations;

namespace Website.Models
{
    public class SendContactModels
    {
        public string Category { get; set; }
        public string Title { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string OrderCode { get; set; }
        [Required]
        public string Phone { get; set; }
        public string UrlFile { get; set; }
        public string Price { get; set; }
        public string Content { get; set; }
        public string Accessory { get; set; }
        public string PlateType { get; set; }
        public string Address { get; set; }
        public int? Gender { get; set; }
        public string TypeOfQuestion { get; set; }
        public string YourQuestion { get; set; }
        public string Industry { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string CompanyAddress { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Nationality { get; set; }
        public string PhoneCode { get; set; }
        public string Service { get; set; }
        public string ServicesName { get; set; }
        public string ServicesUrl { get; set; }
        public string Applicants { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Store { get; set; }
        public DateTime? Time { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProductName { get; set; }
        public string TypeCode { get; set; }
        public int ProductID { get; set; }
        public int ContentID { get; set; }
        public string ProductLink { get; set; }
        public string ProductCode { get; set; }
        public string Number { get; set; }
        public string AreaAgencyParentID { get; set; }
        public string AreaAgencyChildID { get; set; }
        [Required]
        public string Token { get; set; }
        public string Destination { get; set; }
        public string PointGo { get; set; }
        public int NumberElder { get; set; }
        public int NumberChildren { get; set; }
        public int NumberChildren2 { get; set; }
        public int NumberChildren3 { get; set; }
    }
}