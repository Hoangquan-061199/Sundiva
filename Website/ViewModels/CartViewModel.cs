using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Json;

namespace Website.ViewModels
{
    public class CartViewModel
    {
        public List<CommonJsonItem> Frames { get; set; }
        public IList<CartItem> ListCartItem { get; set; }
        public IEnumerable<AreaAgencyItem> AreaAgencyItems { get; set; }
        public IEnumerable<AttributeItem> AttributeItems { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
        public string UrlBack { get; set; }
        public bool IsApplyOtherCampaign { get; set; }
        public bool CancelByCustomer { get; set; }
        public string VoucherCode { get; set; }
        public SystemConfig SystemConfig { get; set; }
        public CustomerItem CustomerItem { get; set; }
        public decimal TotalPriceCart { get; set; }
        public decimal TotalPriceCartAfterDisCount { get; set; }
        public decimal DisCountModule { get; set; }
        public decimal DisCountCombo { get; set; }
        public decimal TotalPriceCartAfterVoucher { get; set; }
        public decimal TotalPriceCartAfterAll { get; set; }
        public decimal DisCountVoucher { get; set; }
        public decimal TotalVAT { get; set; }
        public IEnumerable<OtherContentItem> OtherContentItems { get; set; }
        public WebsiteModulesItem WebsiteModulesItem { get; set; }
        public WebsiteModulesItem ModuleItem { get; set; }
        public Dictionary<string, CityJson> CityItems { get; set; }
        public Dictionary<string, DistrictJson> DistrictItems { get; set; }
        public Dictionary<string, WardJson> WardItems { get; set; }
    }
}