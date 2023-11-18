using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class AgenciesItem:BaseSimple
    {
        public string Name { get; set; }
        public string NameAscii { get; set; }
        public bool? IsShow { get; set; }
        public string Descreption { get; set; }
        public bool? IsDeleted { get; set; }
        public string Lang { get; set; }
        public int? OrderDisplay { get; set; }
        public DateTime? DateCreated { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescreption { get; set; }
        public string SEOKeyword { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Telephone { get; set; }
        public string UrlPicture { get; set; }
        public string AreaIds { get; set; }
        public string AlbumPictureJson { get; set; }
        public string WorkingTime { get; set; }
        public string TypeIds { get; set; }
        public string Type { get; set; }
    }
}
