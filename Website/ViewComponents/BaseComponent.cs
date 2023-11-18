using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using Website.Utils;
using ADCOnline.Utils;

namespace Website.ViewComponents
{
    public abstract class BaseComponent : ViewComponent
    {
        #region  Session
        public int GetWebUserUserName()
        {
            return 0;
        }
        public string Lang() => Request.Cookies["lang"] != null ? Utility.ValidString(Request.Cookies["lang"], "Code", true) : StaticEnum.DefaultLanguage;
        public string Viewer()
        => Request.Cookies["viewer"] ?? string.Empty;
        public void SetWebUserID(int id)
        => HttpContext.Session.SetInt32("WebUserID", id);
        public int GetWebUserID() => 0;

        public void SetWebUserName(string userName)
        => HttpContext.Session.SetString("WebUserName", userName);
        public string GetFullNameMember()
        => HttpContext.Session.GetString("FullName");
        public int? GetIdMember() => HttpContext.Session.GetInt32("WebUserID") ?? 0;
        #endregion
        #region Xử lý file
        public string ReadFile(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
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
                string name = $"{WebConfig.PathServer}{path}/{fileName}";
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
            string name = $"{WebConfig.PathServer}{path}/{fileName}";
            FileInfo info = new(name);
            if (info.Exists)
            {
                info.Delete();
                return 1;
            }
            return 0;
        }
        #endregion
        #region Addlog
        public void AddLogError(string ex)
        {
            try
            {
                string content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}-{ex}";
                CreateAppendFile("LogClient.txt", content, "Logs");
            }
            catch
            {

            }
        }
        #endregion
    }
}