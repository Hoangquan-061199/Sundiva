using System.Collections.Generic;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Json;

namespace Website.Areas.Admin.ViewModels
{
    public class RedirectJsonViewModel
    {
        public List<RedirectJson> GetListItem { get; set; }
        public RedirectJson RedirectJsonAdmin { get; set; }
        public SearchModel SearchModel { get; set; }
        public List<ImportHistoryJson> ImportHistoryJsons { get; set; }
        public SystemActionAdmin SystemActionAdmin { get; set; }
    }
}
