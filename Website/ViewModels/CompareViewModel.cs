using System.Collections.Generic;
using ADCOnline.Simple.Item;

namespace Website.ViewModels
{
    public class CompareViewModel
    {
        public ProductDetail ProductOne { get; set; }
        public ProductDetail ProductTwo { get; set; }
        public ProductDetail ProductThree { get; set; }
        public IEnumerable<AttributeItem> AttributeItems { get; set; }
        public List<CommonJsonItem> AttributeTypes { get; set; }
        public WebsiteModulesItem WebsiteModulesItem { get; set; }
        public IEnumerable<ProductItem> ProductItems { get; set; }
    }
}