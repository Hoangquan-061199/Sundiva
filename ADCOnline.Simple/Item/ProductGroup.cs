using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class ProductGroup : BaseSimple
    {
        public string Name { get; set; }
        public string ProductGroupCode { get; set; }
        public string GroupColorCode { get; set; }
        public string ProductIds { get; set; }
        public List<ProductItem> ProductItems { get; set; }
    }
}
