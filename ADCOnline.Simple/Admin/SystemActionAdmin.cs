using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class SystemActionAdmin
    {
        public bool ViewFull { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Order { get; set; }
        public bool Active { get; set; }
        public bool Public { get; set; }
        public bool Sitemap { get; set; }
        public bool IsAdmin { get; set; }
        public string Action { get; set; }
        public string ActionText { get; set; }
    }
}
