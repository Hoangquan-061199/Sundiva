using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage ="The old password is require.")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "The new password is require.")]
        [MinLength(6, ErrorMessage = "New password is at least 8 characters")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "The confirm password is require.")]
        public string ConfirmPassword { get; set; }
    }
}
