using System;
namespace ADCOnline.Simple.Base
{
    public class Module
    {
        public int ID { get; set; }
        public string NameModule { get; set; }
        public string Tag { get; set; }
        public string Redirect { get; set; }
        public string ClassCss { get; set; }
        public int? Ord { get; set; }
        public int? ParentID { get; set; }
        public string Content { get; set; }
        public bool? IsShow { get; set; }
        public bool? CheckRole { get; set; }
        public string DataJson { get; set; }
    }
}
