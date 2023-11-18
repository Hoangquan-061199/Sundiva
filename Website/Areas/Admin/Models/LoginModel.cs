using Website.Utils;

namespace Website.Areas.Admin.Models
{
    public class LoginModel : GoogleReCaptchaModelBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PassCode { get; set; }
        public string ReturnUrl { get; set; }
    }
}
