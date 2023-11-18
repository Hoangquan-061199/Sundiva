using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Website.Utils;
using ADCOnline.Utils;
using Website.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Website.Controllers
{
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 3600)]
    [CheckOffWebsite]
    public class BaseController : Controller
    {
        #region  Session
        public int? UserId => GetWebUserID();
        public string Email => GetEmailMember();
        public string GetAdminUserId()
        => HttpContext.Session.GetString("WebAdminUserID");
        public void SetWebUserID(int id)
        => HttpContext.Session.SetInt32("WebUserID", id);
        public int GetWebUserID() => HttpContext.Session.GetInt32("WebUserID") ?? 0;
        public void SetWebUserName(string userName)
        => HttpContext.Session.SetString("WebUserName", userName);
        public int GetWebUserUserName() => HttpContext.Session.GetInt32("WebUserName") ?? 0;
        public void SetWebFullName(string userName)
        => HttpContext.Session.SetString("FullName", userName);
        public int GetWebUserFullName() => HttpContext.Session.GetInt32("FullName") ?? 0;
        public string GetEmailMember() => HttpContext.Session.GetString("WebUserName");
        public void SetWebAddress(int id)
        => HttpContext.Session.SetInt32("WebAddress", id);
        public int GetWebAddress() => HttpContext.Session.GetInt32("WebAddress") ?? 0;
        public void SetWebBack(string back)
        => HttpContext.Session.SetString("Back", back);
        public string GetWebBack() => HttpContext.Session.GetString("Back") ?? string.Empty;
        #endregion
        #region Lang
        public string Lang() => Request.Cookies["lang"] != null ? Utility.RemoveHTMLTag(Request.Cookies["lang"]) : StaticEnum.DefaultLanguage;
        //public string Lang() => StaticEnum.DefaultLanguage;
        #endregion
        public string Viewer() => Request.Cookies["viewer"] != null ? Utility.ValidString(Request.Cookies["viewer"], "Code", true) : string.Empty;
        public void SetCookies(string key, string value, int? expireTime)
        {
            CookieOptions option = new()
            {
                Expires = expireTime.HasValue ? (DateTimeOffset?)DateTime.Now.AddMinutes(expireTime.Value) : (DateTimeOffset?)DateTime.Now.AddMilliseconds(10)
            };
            Response.Cookies.Append(key, value, option);
        }
        public void RemoveCookies(string key) => Response.Cookies.Delete(key);
        public string GetCookies(string key) => Request.Cookies[key];
        #region xử lý file
        public string ReadFile(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = System.IO.File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {

            }
            return fileContent;
        }
        public async Task<string> ReadFileAsync(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = await System.IO.File.ReadAllTextAsync(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {

            }
            return fileContent;
        }
        public static void CreateAppendFile(string fileName, string content, string path)
        {
            try
            {
                string name = WebConfig.PathServer + path + "/" + fileName;
                FileInfo info = new(name);
                if (info.Exists)
                {
                    using StreamWriter writer = info.AppendText();
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
                    using StreamWriter writer = info.CreateText();
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
        public static int DeleteFile(string fileName, string path)
        {
            string name = WebConfig.PathServer + path + "/" + fileName;
            FileInfo info = new(name);
            if (info.Exists)
            {
                info.Delete();
                return 1;
            }
            return 0;
        }
        public string GetContentType(string path)
        {
            Dictionary<string, string> types = GetMimeTypes();
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        public Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        #endregion
        #region Addlog
        public void AddLogError(string ex)
        {
            try
            {
                CreateAppendFile("LogClient.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + ex, "Logs");
            }
            catch
            {

            }
        }
        #endregion
    }
}
