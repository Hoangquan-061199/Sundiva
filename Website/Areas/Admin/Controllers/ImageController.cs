using ADCOnline.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Website.Utils;

namespace Website.Areas.Admin.Controllers
{
    public class ImageController : BaseController
    {
        private readonly string _systemRootPath;
        private readonly string _tempPath;
        private readonly string _filesRootPath;
        private readonly string _filesRootVirtual;
        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _lang = null;
        public ImageController(IWebHostEnvironment env)
        {
            // Setup CMS paths to suit your environment (we usually inject settings for these)
            _systemRootPath = env.ContentRootPath;
            _tempPath = _systemRootPath + "\\wwwroot\\Upload\\Temp";
            _filesRootPath = "/wwwroot/Upload";
            _filesRootVirtual = "/Upload";
        }
        [HttpPost]
        public ActionResult ConvertReSize(string path, string name)
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
                    #region resize
                    string newurl = AddTail(c + "/", Path.GetFileNameWithoutExtension(name), "x" + listsize[i] + "x4", Path.GetExtension(name), listsize[i]);
                    using Image filefrom = Image.FromFile(dest);
                    int height;
                    int width = ConvertUtil.ToInt32(Convert.ToInt32(listsize[i]));
                    if (filefrom.Width > width)
                    {
                        height = (int)Math.Round(Convert.ToDecimal(width * filefrom.Height / filefrom.Width));
                    }
                    else
                    {
                        width = filefrom.Width;
                        height = filefrom.Height;
                    }
                    Bitmap result = ResizeImage(filefrom, width, height);
                    ImageCodecInfo myImageCodecInfo;
                    Encoder myEncoder;
                    EncoderParameter myEncoderParameter;
                    EncoderParameters myEncoderParameters;
                    myImageCodecInfo = GetEncoderInfo(mimeType);
                    myEncoder = Encoder.ColorDepth;
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameter = new EncoderParameter(myEncoder, 24L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    result.Save(newurl, myImageCodecInfo, myEncoderParameters);
                    #endregion
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
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
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new(0, 0, width, height);
            Bitmap destImage = new(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
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
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        #region Utilities
        private string MakeVirtualPath(string path)
        {
            return !path.StartsWith(_filesRootPath) ? path : _filesRootVirtual + path.Substring(_filesRootPath.Length);
        }

        private string MakePhysicalPath(string path)
        {
            return !path.StartsWith(_filesRootVirtual) ? path : _filesRootPath + path.Substring(_filesRootVirtual.Length);
        }

        private string GetFilesRoot()
        {
            string ret = _filesRootPath;
            if (GetSetting("SESSION_PATH_KEY") != "" && HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY")) != null) ret = HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY"));
            ret = FixPath(ret);
            return ret;
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

        private static List<string> GetFiles(string path, string type)
        {
            List<string> ret = new();
            if (type == "#" || type == null) type = string.Empty;
            string[] files = Directory.GetFiles(path);
            foreach (string f in files) { if ((GetFileType(new FileInfo(f).Extension) == type) || (type == "")) ret.Add(f); }
            return ret;
        }

        private static string GetFileType(string ext)
        {
            string ret = "file";
            ext = ext.ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif") ret = "image";
            else if (ext == ".swf" || ext == ".flv") ret = "flash";
            return ret;
        }

        private void CheckPath(string path)
        {
            if (FixPath(path).IndexOf(GetFilesRoot()) != 0) throw new Exception("Access to " + path + " is denied");
        }

        private string FixPath(string path)
        {
            path = path.TrimStart('~');
            if (!path.StartsWith("/")) path = "/" + path;
            return _systemRootPath + path;
        }

        private static double LinuxTimestamp(DateTime d)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);
            return timeSpan.TotalSeconds;
        }

        private string GetSetting(string name)
        {
            string ret = string.Empty;
            if (_settings.ContainsKey(name)) ret = _settings[name];
            return ret;
        }

        private static string GetErrorRes(string msg) { return GetResultStr("error", msg); }

        private static string GetResultStr(string type, string msg)
        {
            return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"", "\\\"") + "\"}";
        }

        private string LangRes(string name) { return _lang.ContainsKey(name) ? _lang[name] : name; }

        private static string GetSuccessRes(string msg) { return GetResultStr("ok", msg); }

        private static string GetSuccessRes() { return GetSuccessRes(""); }

        private void CopyDir(string path, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            foreach (string f in Directory.GetFiles(path))
            {
                FileInfo file = new(f);
                if (!System.IO.File.Exists(Path.Combine(dest, file.Name))) System.IO.File.Copy(f, Path.Combine(dest, file.Name));
            }
            foreach (string d in Directory.GetDirectories(path)) CopyDir(d, Path.Combine(dest, new DirectoryInfo(d).Name));
        }

        private static string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            if (System.IO.File.Exists(Path.Combine(dir, ret)))
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
            if (!System.IO.File.Exists(f)) return false;
            else
            {
                try
                {
                    System.IO.File.Delete(f);
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

        private bool IsAjaxUpload()
        {
            return (!string.IsNullOrEmpty(HttpContext.Request.Query["method"]) && HttpContext.Request.Query["method"].ToString() == "ajax");
        }
        #endregion
    }
}