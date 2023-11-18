using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Item
{
    public class AreaAgencyItem : BaseSimple
    {
        public string Name { get; set; }
        public bool? Show { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public string GroupLocation { get; set; }
        public string Lang { get; set; }
        public string Area { get; set; }
        public int? ParentID { get; set; }
        public bool? IsDeleted { get; set; }
        public int? Zoom { get; set; }
    }
}
