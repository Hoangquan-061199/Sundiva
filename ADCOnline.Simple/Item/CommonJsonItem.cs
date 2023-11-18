using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class CommonJsonItem
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string UrlPicture { get; set; }
        public double FromPrice { get; set; }
        public double ToPrice { get; set; }
        public string UrlImg { get; set; }
        public string Address { get; set; }
        public string AddressEn { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string ContentEn { get; set; }
        public string OrderDisplay { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ParentID { get; set; }
        public int? Order
        {
            get
            {
                return Convert.ToInt32(OrderDisplay);
            }
        } 
    }
}
