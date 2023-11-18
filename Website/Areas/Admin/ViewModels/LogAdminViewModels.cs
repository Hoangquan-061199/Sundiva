using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using System.Collections.Generic;

namespace Website.Areas.Admin.ViewModels
{
    public class LogAdminViewModel
    {
        public List<LogAdminAdmin> ListItem { get; set; }
        public LogAdmin LogAdmin { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}

