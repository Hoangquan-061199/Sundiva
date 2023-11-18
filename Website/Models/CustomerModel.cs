using Website.Utils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
namespace Website.Models
{
    public class CustomerModel
    {
        public string Fullname { get; set; }
        public string Id { get; set; }
        public string Phone { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Repass { get; set; }
        public string Oldpass { get; set; }
        public string Address { get; set; }
        public string Urlback { get; set; }
        public bool Receivesale { get; set; }
        public int? Gender { get; set; }
        public int? Yearbrth { get; set; }
        public int? Monthbrth { get; set; }
        public int? Daybrth { get; set; }
        public string Zalo { get; set; }
        public string Fb { get; set; }
        public string Gg { get; set; }
        //google
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}