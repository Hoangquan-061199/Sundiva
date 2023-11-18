using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Simple.Admin
{
    public class AlbumGalleryAdmin
    {
        public string ID { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumUrl { get; set; }
        public string AlbumAlt { get; set; }
        public int AlbumOrderDisplay { get; set; }
        public bool AlbumIsShow { get; set; }
        public int? AlbumType { get; set; }

    }
}
