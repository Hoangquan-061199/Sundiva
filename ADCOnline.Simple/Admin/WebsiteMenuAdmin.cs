using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class WebsiteMenuAdmin : BaseSimple
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int? OrderDisplay { get; set; }
        public string UrlPicture { get; set; }
        public string UrlVideo { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsDeleted { get; set; }
        public string Lang { get; set; }
        public int? ParentID { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
    }
}
