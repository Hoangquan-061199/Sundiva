using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Json
{
    public class SitemapNode
    {
        public string Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }   
}
