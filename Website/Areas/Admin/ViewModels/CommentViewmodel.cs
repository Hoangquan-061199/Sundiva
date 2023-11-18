using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.ViewModels
{
    public class CommentViewmodel
    {
        public List<CommentAdmin> ListItem { get; set; }
        public Comment Comment { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
        public WebsiteContent WebsiteContent { get; set; }
        public Product Product { get; set; }
        public MembershipAdmin Member { get; set; }
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
    }
}
