using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Utils
{
    public abstract class GoogleReCaptchaModelBase
    {
        [Required]
        [GoogleReCaptchaValidation]
        [BindProperty(Name = "g-recaptcha-response")]
        public String GoogleReCaptchaResponse { get; set; }
    }
}
