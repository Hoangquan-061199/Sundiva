using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class StatisticsAdmin : BaseSimple
    {
        public string Id { get; set; }
        public string Product { get; set; }
        public int Sold { get; set; }
        public DateTime? Date { get; set; }
        public int Order { get; set; }
        public int View { get; set; }
    }
}
