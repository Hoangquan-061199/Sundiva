using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class OrderDetailItem : BaseSimple
    {
        public int? OrderID { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string NameAscii { get; set; }
        public string ModuleNameAscii { get; set; }
        public string LinkUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? Price { get; set; }
        public double Quantity { get; set; }
        public string AttrIds { get; set; }
        public string AttrNames { get; set; }
        public string Type { get; set; }
        public decimal? SumMoney { get; set; }
        public decimal? SumMoneyAfterSaleModule { get; set; }
        public string PromotionText { get; set; }
        public string GiftIds { get; set; }
        public decimal? VATPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsVAT { get; set; }
    }
}
