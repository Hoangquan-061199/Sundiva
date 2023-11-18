using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class AccountModel
    {
        public CustomerItem CustomerItem { get; set; }
        public Customer Customer { get; set; }
        public WebsiteModulesItem ModuleItem { get; set; }
    }
}
