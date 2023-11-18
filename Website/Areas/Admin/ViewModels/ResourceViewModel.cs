using ADCOnline.Simple.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.ViewModels
{
    public class ResourceViewModel
    {
        public Dictionary<string, string> GetListItem { get; set; }
        public SearchModel SearchModel { get; set; }
        public string Lang { get; set; }
        public string TypeJson { get; set; }
        public string Keys { get; set; }
        public string Values { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}
