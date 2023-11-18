using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class MemberViewModel
    {
        public List<MemberAdmin> ListItem { get; set; }
        public Member Member { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}

