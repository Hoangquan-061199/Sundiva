using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Json
{
    public class CommentJson :BaseSimple
    {
        public string Content { get; set; }
        public string UrlPicture { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string Act { get; set; }
        public int? Rate { get; set; }
        public string Name { get; set; }
        public string ModuleNameAscii { get; set; }
        public string LinkUrl { get; set; }
        public string NameAscii { get; set; }
        public string UrlPictureProduct { get; set; }
    }
}
