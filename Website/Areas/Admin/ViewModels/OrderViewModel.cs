using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;
using ADCOnline.Simple.Json;

namespace Website.Areas.Admin.ViewModels
{
    public class OrderViewModel
    {
        public List<OrderAdmin> ListItem { get; set; }
        public List<ProductAdmin> ProductAdmins { get; set; }
        public List<OrderDetail> ListOrderDetail { get; set; }
        public List<Attributes> Attributes { get; set; }
        public OrderAdmin OrderAdmin { get; set; }
        public Order Order { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public Dictionary<string, CityJson> CityItems { get; set; }
        public Dictionary<string, DistrictJson> DistrictItems { get; set; }
        public Dictionary<string, WardJson> WardItems { get; set; }
        public List<StatisticalOrder> StatisticalOrders { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
    }
}
