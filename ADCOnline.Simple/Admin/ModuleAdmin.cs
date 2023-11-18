using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class ModuleAdmin : BaseSimple
    {
        public string NameModule { get; set; }
        public string Tag { get; set; }
        public string ClassCss { get; set; }
        public int? Ord { get; set; }
        public int? ParentID { get; set; }
        public string Content { get; set; }
        public string DataJson { get; set; }
        public bool? IsShow { get; set; }
        public bool? CheckRole { get; set; }
    }
}
