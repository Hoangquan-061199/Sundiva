using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Base
{
    public class Comment
    {
        public int ID { get; set; }
        public int? Commentator{get;set;}
        public int? ContentID { get; set; }
        public int? ProductID { get; set; }
        public int? CustomerID { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Phone { get; set; }
        public string UrlPicture { get; set; }
        public string MediaJson { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsApproved { get; set; }
        public string Act { get; set; }
        public int? ParentID { get; set; }
        public int? Rate { get; set; }
        public int? Good { get; set; }
        public int? Bad { get; set; }
        public Guid? AdminId { get; set; }
    }
}
