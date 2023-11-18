using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class SubItemAdmin : BaseSimple
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string UrlPicture { get; set; }
        public string ModuleTypeCode { get; set; }
        public string Code { get; set; }
        public int? OrderDisplay { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ContentID { get; set; }
        public int? ProductID { get; set; }
        public int? ModuleID { get; set; }
        public string Lang { get; set; }
        public string AlbumImageJson { get; set; }
    }
}
