using System;
namespace ADCOnline.Simple.Admin
{
    public class LogAdminAdmin : BaseSimple
    {
        public string Action { get; set; }
        public string Url { get; set; }
        public Guid? UserID { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Content { get; set; }
        public string UserLogin { get; set; }
        public string ClassControl { get; set; }
        public string NameModule { get; set; }
    }
}
