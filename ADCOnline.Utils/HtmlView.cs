using System;
using System.Collections.Generic;
using System.Text;

namespace ADCOnline.Utils
{
    public class HtmlView
    {
        public static string Title(object title)
        {
            return "title=\"" + title + "\"";
        }
        public static string Alt(object alt)
        {
            return "alt=\"" + alt + "\"";
        }
        public static string Content(object content)
        {
            return "content=\"" + content + "\"";
        }
        public static string PlaceHolder(object content)
        {
            return "placeholder=\"" + content + "\"";
        }
        public static string WidthHeight(bool mobile, int w = 0, int h = 0, int wb = 0)
        {
            if (mobile == true && w > 0 && h > 0 && wb > 0)
            {
                return "width=\"" + wb + "\" height=\"" + Math.Round(Convert.ToDecimal(wb * h / w)) + "\"";
            }
            else if (w > 0 && h > 0)
                return "width=\"" + w + "\" height=\"" + h + "\"";
            else return string.Empty;
        }
    }
}
