using System;
namespace ADCOnline.Simple.Base
{
    public class OrderDetail
    {
        public int ID { get; set; }
        public int? OrderID { get; set; }
        public int? ProductID { get; set; }
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
