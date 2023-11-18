using System.Collections.Generic;

namespace ADCOnline.Simple.Item
{
    public class SearchModel
    {
        public string q { get; set; }
        public int start { get; set; } = 0;
        public int page { get; set; } = 1;
        public int pagesize { get; set; } = 12;
        public int status { get; set; }
        public int type { get; set; }
        public int sort { get; set; }
        public string view { get; set; }
        public string wattage { get; set; }
        public string attr { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int moduleId { get; set; }
        public int brandId { get; set; }
        public string seoUrl { get; set; }
        public string lang { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string gia { get; set; }
        public string posCode { get; set; }
        public string des { get; set; }
        public string dpt { get; set; }
        public string prc { get; set; }
        public string userId { get; set; }
        public int contentId { get; set; }
        public int productId { get; set; }
        public int parentId { get; set; }
        public int customerId { get; set; }
        public string typeId { get; set; }
        public string nottypeId { get; set; }
        public decimal? price { get; set; }
        public string cityId { get; set; }
        public string cityPa { get; set; }
        public string areaIds { get; set; }
        public string districtId { get; set; }
        public string store { get; set; }
        public string code { get; set; }
        public string productids { get; set; }
        public string app { get; set; }
        public List<string> ListAttr { get; set; }
        public string hsx { get; set; }
        public string ordercode { get; set; }
        public string timeorder { get; set; }
        public string startorder { get; set; }
        public string toorder { get; set; }
        public string statusorder { get; set; }
        public string deleteNoti { get; set; }
        public string ispayment { get; set; }
        public string moduleIdsNoti { get; set; }
        public string productIdsNoti { get; set; }
        public string cateCustomerIds { get; set; }
        public string strWhere { get; set; }
        public string strOrder { get; set; }
        public int specialistid { get; set; }
        public int addressid { get; set; }
        public int moduleid { get; set; }
        public string TourType { get; set; }
        public string AddressStart { get; set; }
        public string Times { get; set; }

    }
}
