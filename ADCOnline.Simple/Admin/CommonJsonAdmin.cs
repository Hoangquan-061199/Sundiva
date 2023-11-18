using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Admin
{
    public class CommonJsonAdmin
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string NameEn { get; set; }
        public double FromPrice { get; set; }
        public double ToPrice { get; set; }
        public string UrlImg { get; set; }
        public int sort { get; set; }
        public bool? show { get; set; }
        public string code { get; set; }
        public string Address { get; set; }
        public string AddressEn { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string ContentEn { get; set; }
        public string OrderDisplay { get; set; }
        public string ParentID { get; set; }
        public int Order
        {
            get
            {
                if (!string.IsNullOrEmpty(OrderDisplay))
                {
                    return Convert.ToInt32(OrderDisplay);
                }
                else
                {
                    return 0;
                }
            }
        }
        public DateTime CreatedDate { get; set; }
    }
}
