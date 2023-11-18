using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple
{
    public class OrderByItem : BaseSimple
    {
        public int OrderDisplay { get; set; }
        public string Key { get; set; }
        public string Price { get; set; }
        public string PriceOld { get; set; }
        public string Type { get; set; }
    }
}
