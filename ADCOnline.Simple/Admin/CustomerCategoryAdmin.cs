using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class CustomerCategoryAdmin : BaseSimple
    {
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public int? OrderDisplay { get; set; }
        public string Code { get; set; }
    }

}
