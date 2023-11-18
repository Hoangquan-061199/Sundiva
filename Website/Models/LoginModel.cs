using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Authentication;
namespace Website.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc nhập.")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Mật khẩu bắt buộc nhập")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        [Required]
        public string Token { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
