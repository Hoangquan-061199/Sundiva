using System.Collections.Generic;
using System.Text;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Json;

namespace Website.ViewModels
{
    public class CustomerViewModels
    {
        public List<OrderAdmin> ListOrders { get; set; }
        public OrderItem OrderItem { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public IEnumerable<AttributeItem> AttributeItems { get; set; }
        public CustomerItem CustomerItem { get; set; }
        public List<ProductAdmin> ListProductAdmins { get; set; }
        public List<ProductItem> ProductItems { get; set; }
        public List<WebsiteProductItemJson> ProductItemJson { get; set; }
        public List<OrderDetail> ListOrderDetails { get; set; }
        public List<CommentJson> CommentJsons { get; set; }
        public int? Total { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public StringBuilder GridHtml { get; set; }
        public IEnumerable<CommonJsonItem> Frames { get; set; }
    }
}