using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace ADCOnline.Utils
{
    public class ResizeImages
    {
        public static bool ResizeImage(string pathSave, string pathImage, string size, string name)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(pathImage);
                //String file = Convert.ToBase64String(bytes);
                //var contenttype = GetFileExtension(file);
                string[] listsize = size.Split(',');
                var ext = Path.GetExtension(name).ToLower();
                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".webp")
                {
                    for (int i = 0; i < listsize.Length; i++)
                    {
                        string newurl = AddTail(pathSave + "/", Path.GetFileNameWithoutExtension(name), "x" + listsize[i] + "x4", ext, listsize[i]);
                        using Image image = Image.Load(pathImage);
                        int height;
                        int width = ConvertUtil.ToInt32(Convert.ToInt32(listsize[i]));
                        if (image.Width > width)
                        {
                            height = (int)Math.Round(Convert.ToDecimal(width * image.Height / image.Width));
                        }
                        else
                        {
                            width = image.Width;
                            height = image.Height;
                        }
                        image.Mutate(x => x.Resize(width, height));
                        image.Save(newurl);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }


        }

        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                //case "AAAAF":
                //    return "mp4";
                //case "JVBER":
                //    return "pdf";
                //case "AAABA":
                //    return "ico";
                //case "UMFYI":
                //    return "rar";
                //case "E1XYD":
                //    return "rtf";
                //case "U1PKC":
                //    return "txt";
                //case "MQOWM":
                //case "77U/M":
                //    return "srt";
                default:
                    return string.Empty;
            }
        }

        public static string AddTail(string pathurl, string name, string tail, string extention, string s)
        {
            string str = string.Empty;
            try
            {
                string url = pathurl.Replace("/Upload/", "/resize/" + s + "/");
                string path = Path.GetDirectoryName(url);
                bool exists = Directory.Exists(url);
                if (!exists)
                    Directory.CreateDirectory(url);
                string link = url + name + tail + extention;
                return link;
            }
            catch
            {
            }
            return str;
        }
    }
}