using System;

namespace ADCOnline.Simple.Admin
{
    public class SearchModel
    {
        public string keyword { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleIds { get; set; }
        public int? BrandId { get; set; }
        public int? ID { get; set; }
        public int? contentId { get; set; }
        public int? customerId { get; set; }
        public int? productId { get; set; }
        public int? parentId { get; set; }
        public string ItemID { get; set; }
        public string Show { get; set; }
        public string TypeId { get; set; }
        public string Status { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public bool IsExcel { get; set; }
        public bool IsPdf { get; set; }
        public bool IsCsv { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string startwith { get; set; }
        public int day{get;set; }
        public int month{get;set; }
        public int year{get;set; }
        public string type { get; set; }
        public int? sort { get; set; }
        public string lang { get; set; }
        public string voucher { get; set; }
        public string company { get; set; }
        public string position { get; set; }
        public string approal { get; set; }
        public Guid UserId { get; set; }
        public string CityID { get; set; }
        public string DistrictID { get; set; }
        public string WardID { get; set; }
        public string ExportBill { get; set; }
        public string paymenttype { get; set; }
        public string dateb { get; set; }
        public string monthb { get; set; }
        public string yearb { get; set; }
        public string service { get; set; }
        public string selected { get; set; }
        public string unselected { get; set; }
    }
}
