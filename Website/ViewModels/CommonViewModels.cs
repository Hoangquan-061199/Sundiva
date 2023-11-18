using System.Collections.Generic;
using ADCOnline.Simple.Item;

namespace Website.ViewModels
{
    public class CommonViewModels
    {
        public List<CommonJsonItem> ListCommon { get; set; }
        public string Code { get; set; }
        public List<CommonJsonItem> ListDepartures { get; set; }
        public List<CommonJsonItem> ListDestination { get; set; }
        public List<CommonJsonItem> ListPriceTour { get; set; }

    }
}