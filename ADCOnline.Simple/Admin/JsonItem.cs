using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class JsonItem
    {
        public string Type { get; set; }
        public List<OrderByItem> OrderByItems { get; set; }
    }
}
