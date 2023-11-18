using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;
using ADCOnline.Simple.Item;

namespace Website.Areas.Admin.ViewModels
{
    public class CustomerViewModel
    {
        public List<CustomerAdmin> ListItem { get; set; }
        public List<CustomerCategoryAdmin> CustomerCategoryAdmins { get; set; }
        public List<OrderAdmin> ListOrders { get; set; }
        public List<ProductAdmin> ListProductAdmins { get; set; }
        public List<Product> ListProducts { get; set; }
        public List<OrderDetail> ListOrderDetails { get; set; }
        public Customer Customer { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public ADCOnline.Simple.Admin.SearchModel SearchModel { get; set; }
        //cart
        public IList<CartItem> ListCartItem { get; set; }
        public decimal TotalPriceCart { get; set; }
        public decimal TotalPriceCartAfterDisCount { get; set; }
        public decimal DisCountModule { get; set; }
        public decimal TotalPriceCartAfterVoucher { get; set; }
        public decimal TotalPriceCartAfterAll { get; set; }
        public decimal DisCountVoucher { get; set; }
        public List<Module> ListModule { get; set; }
        public List<WebsiteModuleAdmin> ListWebsiteModuleAdmin { get; set; }
    }
}
