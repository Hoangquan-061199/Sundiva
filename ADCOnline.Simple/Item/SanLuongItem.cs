using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Simple.Item
{
    public class SanLuongItem : BaseSimple
    {
        public DateTime? Ngay { get; set; }
        public decimal? SanLuongNgay { get; set; }
        public int? ContentID { get; set; }
    }
}
