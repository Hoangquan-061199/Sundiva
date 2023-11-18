using System;
namespace ADCOnline.Simple.Base
{
    public class Order
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public int? ProductID { get; set; }
        public string OrderCode { get; set; }
        public string Sellers { get; set; }
        public string TechnicalUsers { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
        public double? Quantity { get; set; }
        public decimal? Price { get; set; }
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
        public decimal? TotalVAT { get; set; }
        //extra
        public bool? IsBankReceive { get; set; }
        public bool? IsOtherReceive { get; set; }
        public bool? IsExportBill { get; set; }
        public string Addr { get; set; }
        public string FullNameReceive { get; set; }
        public string PhoneReceive { get; set; }
        public string FullNameBill { get; set; }
        public string PhoneBill { get; set; }
        public string EmailBill { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string Timereceivecheck { get; set; }
        public DateTime? Timereceive { get; set; }
        public string Paymentreceive { get; set; }
        public DateTime? DateofBirth { get; set; }
        public int? AreaAgencyIDParent { get; set; }
        public int? AreaAgencyIDChild { get; set; }
        public int? AgenciesID { get; set; }
        public string AreaAgencyIDParentName { get; set; }
        public string AreaAgencyIDChildName { get; set; }
        public string AgenciesIDName { get; set; }
        public decimal? DiscountModule { get; set; }
        public decimal? DiscountVoucher { get; set; }
        public decimal? DiscountCombo { get; set; }
        public decimal? TotalAfterSale { get; set; }
        public string VoucherCode { get; set; }
        public string NoteAdmin { get; set; }
        public string LogS { get; set; }
        public bool? IsCancerByCustomer { get; set; }
        public bool? IsPaymentTypeByCustomer { get; set; }
        public string EmailBill1 { get; set; }
        public string Zalo { get; set; }
        public string Facebook { get; set; }
        public Guid? EditorID { get; set; }
        public string EditorName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
