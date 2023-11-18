using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Website.Utils;
using ADCOnline.Utils;
using System.Drawing;
using System.Text;

namespace Website.Controllers
{
    [Route("api/[controller]/[action]"), Produces("application/json")]
    [CheckLoginAdmin]
    public class TinyMceController : Controller
    {
        private readonly string _systemRootPath;
        private readonly string _tempPath;
        private readonly string _filesRootPath;
        private readonly string _filesRootVirtual;
        private Dictionary<string, string> _settings;
        private Dictionary<string, string> _lang = null;

        public TinyMceController(IWebHostEnvironment env)
        {
            // Setup CMS paths to suit your environment (we usually inject settings for these)
            _systemRootPath = env.ContentRootPath;
            _tempPath = _systemRootPath + "\\wwwroot\\Upload\\Temp";
            _filesRootPath = "/wwwroot/Upload";
            _filesRootVirtual = "/Upload";
            // Load Fileman settings
            LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(_systemRootPath + "/wwwroot/lib/tinymce/conf.json"));
            string langFile = _systemRootPath + "/wwwroot/lib/tinymce/lang/" + GetSetting("LANG") + ".json";
            if (!System.IO.File.Exists(langFile)) langFile = _systemRootPath + "/wwwroot/lib/tinymce/lang/en.json";
            _lang = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(langFile));
        }

        // GET api/RoxyFileman - test entry point//]
        [AllowAnonymous, Produces("text/plain"), ActionName("")]
        public string Get() { return "RoxyFileman - access to API requires Authorisation"; }

        #region API Actions
        public IActionResult DIRLIST(string type)
        {
            try
            {
                DirectoryInfo d = new(GetFilesRoot());
                if (!d.Exists) throw new Exception("Invalid files root directory. Check your configuration.");
                ArrayList dirs = ListDirs(d.FullName);
                dirs.Insert(0, d.FullName);
                string localPath = _systemRootPath;
                string result = string.Empty;
                for (int i = 0; i < dirs.Count; i++)
                {
                    string dir = (string)dirs[i];
                    result += (result != string.Empty ? "," : string.Empty) + "{\"p\":\"" + MakeVirtualPath(dir.Replace(localPath, string.Empty).Replace("\\", "/")) + "\",\"f\":\"" + GetFiles(dir, type).Count.ToString() + "\",\"d\":\"" + Directory.GetDirectories(dir).Length.ToString() + "\"}";
                }
                return Content("[" + result + "]", "application/json");
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult FILESLIST(string d, string type)
        {
            try
            {
                d = MakePhysicalPath(d);
                CheckPath(d);
                string fullPath = FixPath(d);
                List<string> files = GetFiles(fullPath, type);
                StringBuilder result = new();
                for (int i = 0; i < files.Count; i++)
                {
                    FileInfo f = new(files[i]);
                    int w = 0, h = 0;
                    result.Append(result.ToString() != string.Empty ? "," : string.Empty);
                    result.Append('{');
                    result.Append("\"p\":\"" + MakeVirtualPath(d) + "/" + f.Name + "\"");
                    result.Append(",\"t\":\"" + Math.Ceiling(LinuxTimestamp(f.LastWriteTime)).ToString() + "\"");
                    result.Append(",\"s\":\"" + f.Length.ToString() + "\"");
                    result.Append(",\"w\":\"" + w.ToString() + "\"");
                    result.Append(",\"h\":\"" + h.ToString() + "\"");
                    result.Append('}');
                }
                return Content("[" + result.ToString() + "]");
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult COPYDIR(string d, string n)
        {
            try
            {
                d = MakePhysicalPath(d);
                n = MakePhysicalPath(n);
                CheckPath(d);
                CheckPath(n);
                DirectoryInfo dir = new(FixPath(d));
                DirectoryInfo newDir = new(FixPath(n + "/" + dir.Name));
                if (!dir.Exists) throw new Exception(LangRes("E_CopyDirInvalidPath"));
                else if (newDir.Exists) throw new Exception(LangRes("E_DirAlreadyExists"));
                else CopyDir(dir.FullName, newDir.FullName);
                return Content(GetSuccessRes());
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult COPYFILE(string f, string n)
        {
            try
            {
                f = MakePhysicalPath(f);
                CheckPath(f);
                FileInfo file = new(FixPath(f));
                n = FixPath(n);
                if (!file.Exists) throw new Exception(LangRes("E_CopyFileInvalisPath"));
                else
                {
                    try
                    {
                        System.IO.File.Copy(file.FullName, Path.Combine(n, MakeUniqueFilename(n, file.Name)));
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_CopyFile")); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult CREATEDIR(string d, string n)
        {
            try
            {
                d = MakePhysicalPath(d);
                CheckPath(d);
                d = FixPath(d);
                if (!Directory.Exists(d)) throw new Exception(LangRes("E_CreateDirInvalidPath"));
                else
                {
                    try
                    {
                        n = Utility.ConvertRewrite(n);
                        d = Path.Combine(d, n);
                        if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_CreateDirFailed")); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult DELETEDIR(string d)
        {
            try
            {
                d = MakePhysicalPath(d);
                CheckPath(d);
                d = FixPath(d);
                if (!Directory.Exists(d)) throw new Exception(LangRes("E_DeleteDirInvalidPath"));
                else if (d == GetFilesRoot()) throw new Exception(LangRes("E_CannotDeleteRoot"));
                else if (Directory.GetDirectories(d).Length > 0 || Directory.GetFiles(d).Length > 0) throw new Exception(LangRes("E_DeleteNonEmpty"));
                else
                {
                    try
                    {
                        Directory.Delete(d);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_CannotDeleteDir")); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }
        public IActionResult DELETEFILE(string f)
        {
            //var d = "/wwwroot/resize/";
            List<string> list = Utility.FolderToListString(f);
            int size = list.Count;
            string name = list[(size - 1)];
            try
            {
                f = MakePhysicalPath(f);
                CheckPath(f);
                f = FixPath(f);
                string path = f.Replace("/" + name, "/");
                if (!System.IO.File.Exists(f)) throw new Exception(LangRes("E_DeleteFileInvalidPath"));
                else
                {
                    try
                    {
                        System.IO.File.Delete(f);
                        #region delete resize
                        string sizes = WebConfig.Sizes;
                        string[] listsize = sizes.Split(',');
                        foreach (string item in listsize)
                        {
                            string resizePath = path.Replace("/Upload/", "/resize/" + item + "/");
                            string pathmobile = resizePath + Path.GetFileNameWithoutExtension(name) + "x" + item + "x4" + Path.GetExtension(name);
                            if (System.IO.File.Exists(pathmobile))
                            {
                                System.IO.File.Delete(pathmobile);
                            }
                        }
                        #endregion
                        return Content(GetSuccessRes());
                    }
                    catch (Exception ex) { throw new Exception(LangRes("E_DeletеFile") + ex.Message); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public ActionResult DOWNLOAD(string f)
        {
            try
            {
                f = MakePhysicalPath(f);
                CheckPath(f);
                FileInfo file = new FileInfo(FixPath(f));
                if (file.Exists)
                {
                    string contentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(file.FullName, out contentType);
                    return PhysicalFile(file.FullName, contentType ?? "application/octet-stream", file.Name);
                }
                else return NotFound();
            }
            catch (Exception ex) { return Json(GetErrorRes(ex.Message)); }
        }

        public ActionResult DOWNLOADDIR(string d)
        {
            try
            {
                d = MakePhysicalPath(d);
                d = FixPath(d);
                if (!Directory.Exists(d)) throw new Exception(LangRes("E_CreateArchive"));
                string dirName = new FileInfo(d).Name;
                string tmpZip = _tempPath + "/" + dirName + ".zip";
                if (System.IO.File.Exists(tmpZip)) System.IO.File.Delete(tmpZip);
                ZipFile.CreateFromDirectory(d, tmpZip, CompressionLevel.Fastest, true);
                return PhysicalFile(tmpZip, "application/zip", dirName + ".zip");
            }
            catch (Exception ex) { return Json(GetErrorRes(ex.Message)); }
        }

        public IActionResult MOVEDIR(string d, string n)
        {
            try
            {
                d = MakePhysicalPath(d);
                n = MakePhysicalPath(n);
                CheckPath(d);
                CheckPath(n);
                DirectoryInfo source = new(FixPath(d));
                DirectoryInfo dest = new(FixPath(Path.Combine(n, source.Name)));
                if (dest.FullName.IndexOf(source.FullName) == 0) throw new Exception(LangRes("E_CannotMoveDirToChild"));
                else if (!source.Exists) throw new Exception(LangRes("E_MoveDirInvalisPath"));
                else if (dest.Exists) throw new Exception(LangRes("E_DirAlreadyExists"));
                else
                {
                    try
                    {
                        source.MoveTo(dest.FullName);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_MoveDir") + " \"" + d + "\""); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult MOVEFILE(string f, string n)
        {
            try
            {
                f = MakePhysicalPath(f);
                n = MakePhysicalPath(n);
                CheckPath(f);
                CheckPath(n);
                FileInfo source = new(FixPath(f));
                FileInfo dest = new(FixPath(n));
                if (!source.Exists) throw new Exception(LangRes("E_MoveFileInvalisPath"));
                else if (dest.Exists) throw new Exception(LangRes("E_MoveFileAlreadyExists"));
                else if (!CanHandleFile(dest.Name)) throw new Exception(LangRes("E_FileExtensionForbidden"));
                else
                {
                    try
                    {
                        source.MoveTo(dest.FullName);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_MoveFile") + " \"" + f + "\""); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult RENAMEDIR(string d, string n)
        {
            try
            {
                d = MakePhysicalPath(d);
                CheckPath(d);
                DirectoryInfo source = new(FixPath(d));
                DirectoryInfo dest = new(Path.Combine(source.Parent.FullName, n));
                if (source.FullName == GetFilesRoot()) throw new Exception(LangRes("E_CannotRenameRoot"));
                else if (!source.Exists) throw new Exception(LangRes("E_RenameDirInvalidPath"));
                else if (dest.Exists) throw new Exception(LangRes("E_DirAlreadyExists"));
                else
                {
                    try
                    {
                        source.MoveTo(dest.FullName);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception) { throw new Exception(LangRes("E_RenameDir") + " \"" + d + "\""); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        public IActionResult RENAMEFILE(string f, string n)
        {
            try
            {
                f = MakePhysicalPath(f);
                CheckPath(f);
                FileInfo source = new(FixPath(f));
                FileInfo dest = new(Path.Combine(source.Directory.FullName, n));
                if (!source.Exists) throw new Exception(LangRes("E_RenameFileInvalidPath"));
                else if (!CanHandleFile(n)) throw new Exception(LangRes("E_FileExtensionForbidden"));
                else
                {
                    try
                    {
                        source.MoveTo(dest.FullName);
                        return Content(GetSuccessRes());
                    }
                    catch (Exception ex) { throw new Exception(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + f + "\""); }
                }
            }
            catch (Exception ex) { return Content(GetErrorRes(ex.Message)); }
        }

        [HttpPost, Produces("text/plain")]
        public string UPLOAD(string d)
        {
            Common.AddLogEdit("upload File: vào:" + d, null, null, null);
            try
            {
                d = MakePhysicalPath(d);
                CheckPath(d);
                d = FixPath(d);
                string res = GetSuccessRes();
                bool hasErrors = false;
                try
                {
                    foreach (IFormFile file in HttpContext.Request.Form.Files)
                    {
                        if (file.Length <= 0)
                        {
                            hasErrors = true;
                            return "<script>parent.fileUploaded(Chưa nhập file!);</script>";
                        };
                        if (!ImageExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                        {
                            hasErrors = true;
                            return "<script>parent.fileUploaded(File không đúng định dạng!);</script>";
                        }
                        if (!FileType.Contains(file.ContentType))
                        {
                            hasErrors = true;
                            return "<script>parent.fileUploaded(File không đúng định dạng!);</script>";
                        }
                        if (file.Length > 200 * 1024 * 1024)
                        {
                            hasErrors = true;
                            res = GetSuccessRes(LangRes("E_UploadNotAll"));
                            return "<script>parent.fileUploaded(" + res + ");</script>";
                            //return "<script>parent.fileUploaded(File quá lớn!);</script>";
                        }
                        // Common.AddLogEdit("upload File: vào:"+file.FileName, null,null,null);
                        if (CanHandleFile(file.FileName))
                        {
                            FileInfo f = new(file.FileName);
                            string filename = MakeUniqueFilename(d, f.Name);
                            string dest = Path.Combine(d, filename);
                            //string webPFileName = Utility.ConvertRewrite(Path.GetFileNameWithoutExtension(file.FileName)) + ".webp";
                            //string webPImagePath = Path.Combine(d, webPFileName);
                            //var width = 425;
                            //string fileNameMobile = Utility.ConvertRewrite(Path.GetFileNameWithoutExtension(file.FileName)) + "-mb" + Path.GetExtension(file.FileName);
                            //var destMobile = Path.Combine(d, fileNameMobile);
                            #region image default
                            using (FileStream saveFile = new(dest, FileMode.Create))
                            {
                                file.CopyTo(saveFile);
                                var uploadedImage = Image.FromStream(saveFile);
                                saveFile.Close();
                                saveFile.Dispose();
                                uploadedImage.Dispose();
                            }
                            #endregion                           
                        }
                        else
                        {
                            hasErrors = true;
                            res = GetSuccessRes(LangRes("E_UploadNotAll"));
                        }
                    }
                }
                catch (Exception ex) { res = GetErrorRes(ex.Message); }
                if (IsAjaxUpload())
                {
                    if (hasErrors) res = GetErrorRes(LangRes("E_UploadNotAll"));
                    return res;
                }
                else return "<script>parent.fileUploaded(" + res + ");</script>";
            }
            catch (Exception ex)
            {
                if (!IsAjaxUpload()) return "<script>parent.fileUploaded(" + GetErrorRes(LangRes("E_UploadNoFiles")) + ");</script>";
                else return GetErrorRes(ex.Message);
            }
        }
        public static readonly List<string> ImageExtensions = new()
        {
            ".jpg", ".jpeg", ".bmp", ".gif", ".webp", ".png", ".jped", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv", ".zip", ".rar", ".svg", ".mp4", ".mp3", ".avi",
            ".mov",".flv","wwmv", "mpeg-4", "mpeg-2"
        };
        public static readonly List<string> FileType = new()
        {
            "image/png", "image/gif", "image/avif", "image/apng", "image/jpeg", "image/svg+xml", "image/webp", "image/vnd.microsoft.icon",
            "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/csv", "application/x-7z-compressed", "application/vnd.rar",  "audio/mpeg",
            "video/mp4", "video/x-msvideo", "audio/wav","video/mpeg"
        };
        [HttpPost]
        public ActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;
            if (!ImageExtensions.Contains(Path.GetExtension(upload.FileName).ToLower()))
            {
                string NotImageMessage = "please choose a picture";
                dynamic NotImage = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + NotImageMessage + "\"}}");
                return Json(NotImage);
            }
            if (!FileType.Contains(upload.ContentType))
            {
                string NotImageMessage = "please choose a picture";
                dynamic NotImage = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + NotImageMessage + "\"}}");
                return Json(NotImage);
            }
            string fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
            Image image = Image.FromStream(upload.OpenReadStream());
            int width = image.Width;
            int height = image.Height;
            if ((width > 3000) || (height > 5000))
            {
                string DimensionErrorMessage = "Custom Message for error";
                dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + DimensionErrorMessage + "\"}}");
                return Json(stuff);
            }

            if (upload.Length > 110 * 1024 * 1024)
            {
                string LengthErrorMessage = "Custom Message for error";
                dynamic stuff = JsonConvert.DeserializeObject("{ 'uploaded': 0, 'error': { 'message': \"" + LengthErrorMessage + "\"}}");
                return Json(stuff);
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads/images/ck", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);
            }
            string url = $"{"/Uploads/images/ck/"}{fileName}";
            string successMessage = "image is uploaded successfully";
            dynamic success = JsonConvert.DeserializeObject("{ 'uploaded': 1,'fileName': \"" + fileName + "\",'data': \"" + url + "\", 'error': { 'message': \"" + successMessage + "\"}}");
            return Content(url);
        }
        #endregion
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
            if (GetSetting("SESSION_PATH_KEY") != string.Empty && HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY")) != null) ret = HttpContext.Session.GetString(GetSetting("SESSION_PATH_KEY"));
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

        private List<string> GetFiles(string path, string type)
        {
            List<string> ret = new();
            if (type == "#" || type == null) type = string.Empty;
            string[] files = Directory.GetFiles(path);
            foreach (string f in files) { if ((GetFileType(new FileInfo(f).Extension) == type) || (type == string.Empty)) ret.Add(f); }
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

        private double LinuxTimestamp(DateTime d)
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

        private string GetErrorRes(string msg) { return GetResultStr("error", msg); }

        private string GetResultStr(string type, string msg)
        {
            return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"", "\\\"") + "\"}";
        }

        private string LangRes(string name) { return _lang.ContainsKey(name) ? _lang[name] : name; }

        private string GetSuccessRes(string msg) { return GetResultStr("ok", msg); }

        private string GetSuccessRes() { return GetSuccessRes(string.Empty); }

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

        private string MakeUniqueFilename(string dir, string filename)
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
            string ext = file.Extension.Replace(".", string.Empty).ToLower();
            string setting = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (setting != string.Empty)
            {
                ArrayList tmp = new();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext)) ret = true;
            }
            setting = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (setting != string.Empty)
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
