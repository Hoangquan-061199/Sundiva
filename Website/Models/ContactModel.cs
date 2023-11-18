using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class ContactModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? Gender { get; set; }
        public string TypeOfQuestion { get; set; }
        public string YourQuestion { get; set; }
        public string Industry { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string Store { get; set; }
        public DateTime? Time { get; set; }
        public string CompanyAddress { get; set; }
        public string ZipCode { get; set; }
        public string Fax { get; set; }
        public string Nationality { get; set; }
        public string PhoneCode { get; set; }
        public string ServicesName { get; set; }
        public string ServicesUrl { get; set; }
        public int Applicants { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public string ProductLink { get; set; }
        public string ProductCode { get; set; }
    }
}