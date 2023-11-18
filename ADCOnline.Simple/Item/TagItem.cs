using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Item
{
    [Serializable]
    public class TagItem : BaseSimple
    {
        public string Name { get; set; }
        public int? OrderBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public string NameAscii { get; set; }
        public bool? IsHome { get; set; }
        public int? Weight { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeyword { get; set; }
        public string SEOTitle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ContentIDs { get; set; }
    }
}
