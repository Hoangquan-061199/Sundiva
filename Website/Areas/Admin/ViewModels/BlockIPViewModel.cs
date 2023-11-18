using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System.Collections.Generic;
namespace Website.Areas.Admin.ViewModels
{
    public class BlockIPViewModel
    {
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public List<string> AddressIp { get; set; }
    }
}
