using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class MembershipAdmin : BaseSimple
    {
        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int DepartmentID { get; set; }
        public string Password { get; set; }
        public int PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string MobilePIN { get; set; }
        public string Email { get; set; }
        public string LoweredEmail { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        public string Comment { get; set; }
        public string FullName { get; set; }
        public string ModuleIds { get; set; }
        public string RoleCode { get; set; }
        public string DataJson { get; set; }
        public string WebsiteModule { get; set; }
        public string WebsiteModuleIds { get; set; }
        public string Child { get; set; }
        public string RoleOld { get; set; }
        public string Company { get; set; }
    }
}
