using System;
namespace ADCOnline.Simple.Admin
{
    public class MemberAdmin : BaseSimple
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UrlPicture { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool? IsEmployer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActiveByEmail { get; set; }
        public string FaceBookId { get; set; }
        public string GoogleId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public string PassReal { get; set; }
    }
}
