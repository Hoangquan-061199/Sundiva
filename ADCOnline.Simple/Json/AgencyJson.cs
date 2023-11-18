using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Json
{
    public class AgencyJson
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NameAscii { get; set; }
        public string Descreption { get; set; }
        public int? OrderDisplay { get; set; }
        public string UrlPicture { get; set; }
        public string UrlBanner { get; set; }
        public string GoogleMap { get; set; }
        public string AreaIds { get; set; }
    }
}
