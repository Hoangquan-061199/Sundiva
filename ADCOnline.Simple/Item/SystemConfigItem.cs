using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Item
{
    public class SystemConfigItem : BaseSimple
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Phone { get; set; }
        public string Hotline { get; set; }
        public string PhoneAdvice3 { get; set; }
        public string PhoneAdvice2 { get; set; }
        public string PhoneAdvice4 { get; set; }
        public string Fax1 { get; set; }
        public string Fax2 { get; set; }
        public string Fax3 { get; set; }
        public string Fax4 { get; set; }
        public string Email { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string BusinessLicence { get; set; }
        public string TwiterPage { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeyword { get; set; }
        public string GoogleAnalytics { get; set; }
        public string Headquarters5 { get; set; }
        public string Headquarters6 { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Google { get; set; }
        public string Youtube { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public bool? IsShow { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public DateTime? DateCreate { get; set; }
        public int? TotalVisitors { get; set; }
        public int? TotalApply { get; set; }
        public string MapGoogle1 { get; set; }
        public string MapGoogle2 { get; set; }
        public string Copyright { get; set; }
        public int? CountProductHot { get; set; }
        public string EmailCms { get; set; }
        public string PassEmailCms { get; set; }
        public bool? SSLEmail { get; set; }
        public string EmailAdmin { get; set; }
        public string Lang { get; set; }
        public string LinkFacebookPage { get; set; }
        public string LinkVideoHome { get; set; }
        public string Zalo { get; set; }
        public DateTime? TimeEnd { get; set; }
        public DateTime? TimeStart { get; set; }
        public string KeyApiMap { get; set; }
        public string CodeTgv { get; set; }
        public string ConfigPopupJson { get; set; }
        public Dictionary<string, string> ConfigPopup { get; set; }
    }
}
