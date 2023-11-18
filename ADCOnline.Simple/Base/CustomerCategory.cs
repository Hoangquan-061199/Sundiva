using System;
namespace ADCOnline.Simple.Base
{
    public class CustomerCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderDisplay { get; set; }
        public string Code { get; set; }
        public bool? IsShow { get; set; }

    }
}
