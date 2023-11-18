using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class ImportHistoryExcelWarehouseJson
    {
        public string Code { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Extention { get; set; }
        public int? ModuleID { get; set; }
    }
}
