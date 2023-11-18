using Website.Utils;

namespace Website.Models
{
    public class PaymentModels : GoogleReCaptchaModelBase
    {
        public string fullname { get; set; }
        public string paymentmobile { get; set; }
        public string paymentemail { get; set; }
        public string paymentadd { get; set; }
        public string paymentype { get; set; }
        public string paymentfacebook { get; set; }
        public string paymentzalo { get; set; }
        public string gen { get; set; }
        public string note { get; set; }
        public string IsBankReceive { get; set; }
        public string IsOtherReceive { get; set; }
        public string IsExportBill { get; set; }
        public string addr { get; set; }
        public string FullNameReceive { get; set; }
        public string otherreceiveaf { get; set; }
        public string PhoneReceive { get; set; }
        public string FullNameBill { get; set; }
        public string PhoneBill { get; set; }
        public string EmailBill { get; set; }
        public string EmailBill1 { get; set; }
        public string CityID { get; set; }
        public string DistrictID { get; set; }
        public string WardID { get; set; }
        public string dateofbirth { get; set; }
        public string paymentreceive { get; set; }
        public string timereceivecheck { get; set; }
        public string timereceive { get; set; }
        public string AreaAgencyParentID { get; set; }
        public string AreaAgencyChildID { get; set; }
        public string store { get; set; }
        public string vouchercode { get; set; }
    }
}