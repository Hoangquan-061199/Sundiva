using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class CartItem
    {
        public Int32 ProductId { get; set; }//id san pham
        public int? Status { get; set; }//id san pham
        public double Quantity { get; set; } //so luong
        public string ProductName { get; set; } //ten san pham        
        public string ProductCode { get; set; }//ma san pham
        public string NameAscci { get; set; }//name ascii san pham
        public string ImageProduct { get; set; }//hinh anh san pham
        public decimal? PriceOrigin { get; set; }//gia goc
        public decimal PriceProduct { get; set; }//gia moi
        public decimal SumMoney { get; set; }//tong tien = so luong * gia (Quantity*PriceProduct)
        public string ModuleIds { get; set; }
        public string Type { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string ViewHome { get; set; }
        public IEnumerable<int> ModuleID { get; set; }
        public IEnumerable<AttributeItem> AttributeItems { get; set; }
        //sale chuong trinh khuyen mai
        public string PercentDiscountModule { get; set; }
        public decimal? DiscountModuleItem { get; set; }
        public decimal? TotalAfterSaleModule { get; set; }
        //sale voucher
        public string PercentDiscountVoucher { get; set; }
        public decimal? MoneyDiscountVoucher { get; set; }
        public decimal? DiscountVoucher { get; set; }
        public decimal? TotalAfterSaleVoucher { get; set; }
        public string PromotionText { get; set; }
        public string GiftIds { get; set; }
        public bool? IsVAT { get; set; }
        public decimal VATPrice { get; set; }
    }
}
