using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class OrderAdmin : BaseSimple
    {
        public int? CustomerID { get; set; }
        public string CustomerUserName { get; set; }
        public string CustomerFullName { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string OrderCode { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
        public DateTime? DeliveryTime { get; set; }
       public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsPayment { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PaymentType { get; set; }
        public string Address { get; set; }
        public decimal? TotalMoney { get; set; }
        //doji       
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string Paymentreceive { get; set; }
        public decimal? TotalAfterSale { get; set; }
        public decimal? TotalVAT { get; set; }
        public string NoteAdmin { get; set; }
        public Guid? EditorID { get; set; }
        public string EditorName { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
