using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.ViewModels
{
    public class HomeAdminViewModel
    {
        public List<Module> ListModule { get; set; }
        public Module Module { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
        public MembershipAdmin Member { get; set; }
        public string CurrentLanguage { get; set; }
        public List<ContactUs> ContactUs { get; set; }
        public List<CommentAdmin> CommentAdmins { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<OrderAdmin> OrderAdmins { get; set; }
        public List<SitemapJson> SitemapJsons { get; set; }
        public SitemapJson SitemapJson { get; set; }
        public List<StatisticalAdmin> StatisticalAdmins { get; set; }
        public List<WebsiteModuleAdmin> WebsiteModuleAdmins { get; set; }
    }
}
