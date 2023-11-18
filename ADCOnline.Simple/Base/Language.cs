using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Base
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public int OrderDisplay { get; set; }
        public string UrlPicture { get; set; }
    }
}
