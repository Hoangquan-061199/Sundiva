using ADCOnline.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Website.Utils
{
    public class ResizeImage
    {
        private string _systemRootPath;
        private string _tempPath;
        private string _filesRootPath;
        private string _filesRootVirtual;
        private Dictionary<string, string> _lang = null;
        public ResizeImage(IWebHostEnvironment env)
        {
            _systemRootPath = env.ContentRootPath;
            _tempPath = _systemRootPath + "\\wwwroot\\Upload\\Temp";
            _filesRootPath = "/wwwroot/Upload";
            _filesRootVirtual = "/Upload";
        }
        public bool ConvertReSize(string path, string name)
        {
            string dpath = "/wwwroot/resize/";
            string c = string.Empty;
            Image fileImage;
            try
            {
                string d = MakePhysicalPath(dpath);
                d = FixPath(d);
                c = MakePhysicalPath(path);
                c = FixPath(c);
                string res = GetSuccessRes();
                FileInfo f = new(name);
                RemoveExist(dpath + name);
                string dest = Path.Combine(c, name);
                string size = WebConfig.Sizes;
                string[] listsize = size.Split(',');
                fileImage = Image.FromFile(dest);
                ImageFormat format = fileImage.RawFormat;
                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().First(x => x.FormatID == format.Guid);
                string mimeType = codec.MimeType;
                fileImage.Dispose();
                for (int i = 0; i < listsize.Length; i++)
                {
                    string newurl = AddTail(c + "/", Path.GetFileNameWithoutExtension(name), "x" + listsize[i] + "x4", Path.GetExtension(name), listsize[i]);
                    using var filefrom = Image.FromFile(dest);
                    int height = ConvertUtil.ToInt32(Convert.ToInt32(listsize[i]));
                    int width = ConvertUtil.ToInt32(Convert.ToInt32(listsize[i]));
                    double hRatio = (height * 100.0) / filefrom.Height;
                    double wRatio = (width * 100.0) / filefrom.Width;
                    if (hRatio > wRatio)
                    {
                        height = (int)Math.Round((wRatio * filefrom.Height) / 100);
                    }
                    else if (hRatio < wRatio)
                    {
                        width = (int)Math.Round((hRatio * filefrom.Width) / 100);
                    }
                    else
                    {
                        width = filefrom.Width;
                        height = filefrom.Height;
                    }
                    Bitmap result = ResizeImages(filefrom, width, height);
                    result.Save(newurl);
                }
                return true;
            }
            catch (Exception e)
            {
                AddLogError(e);
                return false;
            }
        }
        public static string AddTail(string pathurl, string name, string tail, string extention, string s)
        {
            string str = string.Empty;
            try
            {
                string url = pathurl;
                url = url.Replace("/Upload/", "/resize/" + s + "/");
                var path = Path.GetDirectoryName(url);
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
        public static Bitmap ResizeImages(Image image, int width, int height)
        {
            Rectangle destRect = new(0, 0, width, height);
            Bitmap destImage = new(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
                graphics.Dispose();
            }
            image.Dispose();
            return destImage;
        }
        public void AddLogError(Exception ex)
        {
            try
            {
                string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + ex.Message + ex.StackTrace;
                CreateAppendFile("LogAdmin.txt", content, "Logs");
            }
            catch
            {

            }
        }
        public static void CreateAppendFile(string fileName, string content, string path)
        {
            try
            {
                string name = WebConfig.PathServer + path + "/" + fileName;
                FileInfo info = new(name);
                if (info.Exists)
                {
                    using var writer = info.AppendText();
                    try
                    {
                        writer.WriteLine(content);
                        writer.Flush();
                        writer.Close();
                    }
                    catch
                    {
                        writer.Flush();
                        writer.Close();
                    }
                }
                else
                {
                    using var writer = info.CreateText();
                    try
                    {
                        writer.WriteLine(content);
                        writer.Flush();
                        writer.Close();
                    }
                    catch
                    {
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch
            {

            }
        }
        private string MakeVirtualPath(string path)
        {
            return !path.StartsWith(_filesRootPath) ? path : _filesRootVirtual + path.Substring(_filesRootPath.Length);
        }

        private string MakePhysicalPath(string path)
        {
            return !path.StartsWith(_filesRootVirtual) ? path : _filesRootPath + path.Substring(_filesRootVirtual.Length);
        }

        private ArrayList ListDirs(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            ArrayList ret = new();
            foreach (string dir in dirs)
            {
                ret.Add(dir);
                ret.AddRange(ListDirs(dir));
            }
            return ret;
        }

        private List<string> GetFiles(string path, string type)
        {
            List<string> ret = new();
            if (type == "#" || type == null) type = string.Empty;
            string[] files = Directory.GetFiles(path);
            foreach (string f in files) { if ((GetFileType(new FileInfo(f).Extension) == type) || (type == "")) ret.Add(f); }
            return ret;
        }

        private string GetFileType(string ext)
        {
            string ret = "file";
            ext = ext.ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif") ret = "image";
            else if (ext == ".swf" || ext == ".flv") ret = "flash";
            return ret;
        }


        private string FixPath(string path)
        {
            path = path.TrimStart('~');
            if (!path.StartsWith("/")) path = "/" + path;
            return _systemRootPath + path;
        }

        private double LinuxTimestamp(DateTime d)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);
            return timeSpan.TotalSeconds;
        }
        //private Dictionary<string, string> _settings;
        private string GetSetting(string name)
        {
            Dictionary<string, string> _settings = new();
            string ret = string.Empty;
            if (_settings.ContainsKey(name)) ret = _settings[name];
            return ret;
        }

        private string GetErrorRes(string msg) { return GetResultStr("error", msg); }

        private string GetResultStr(string type, string msg) => "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"", "\\\"") + "\"}";

        private string LangRes(string name) { return _lang.ContainsKey(name) ? _lang[name] : name; }

        private string GetSuccessRes(string msg) { return GetResultStr("ok", msg); }

        private string GetSuccessRes() { return GetSuccessRes(""); }

        private void CopyDir(string path, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            foreach (string f in Directory.GetFiles(path))
            {
                FileInfo file = new(f);
                if (!File.Exists(Path.Combine(dest, file.Name))) System.IO.File.Copy(f, Path.Combine(dest, file.Name));
            }
            foreach (string d in Directory.GetDirectories(path)) CopyDir(d, Path.Combine(dest, new DirectoryInfo(d).Name));
        }

        private string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            if (File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Utility.ConvertRewrite(Path.GetFileNameWithoutExtension(filename)) + "-copy" + i.ToString() + Path.GetExtension(filename);
            }
            else
            {
                ret = Utility.ConvertRewrite(Path.GetFileNameWithoutExtension(filename)) + Path.GetExtension(filename);
            }
            return ret;
        }
        private bool RemoveExist(string f)
        {
            f = MakePhysicalPath(f);
            f = FixPath(f);
            if (!File.Exists(f)) return false;
            else
            {
                try
                {
                    File.Delete(f);
                    return true;
                }
                catch { return false; }
            }
        }

        private bool CanHandleFile(string filename)
        {
            bool ret = false;
            FileInfo file = new(filename);
            string ext = file.Extension.Replace(".", "").ToLower();
            string setting = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext)) ret = true;
            }
            setting = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext)) ret = false;
            }
            return ret;
        }
    }
}