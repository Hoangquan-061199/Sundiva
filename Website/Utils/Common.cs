using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Mail;
using System.Text.RegularExpressions;
using ADCOnline.Utils;
using System.Threading.Tasks;

namespace Website.Utils
{
    public class Common
    {
        public static bool IsLocal(HttpRequest req)
        {
            ConnectionInfo connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    string ip = connection.LocalIpAddress.ToString();
                    if (connection.RemoteIpAddress.Equals(connection.LocalIpAddress) || "127.0.0.1" == ip)
                    {
                        return true;
                    }
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }
            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }
            return false;
        }
        #region xử lý file
        public static string ReadFile(string fileName, string path)
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
        public async static Task<string> ReadFileAsync(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = await File.ReadAllTextAsync(WebConfig.PathServer + path + "/" + fileName);
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
            string name = $"{WebConfig.PathServer}{path}/{fileName}";
            FileInfo info = new(name);
            if (info.Exists)
            {
                info.Delete();
                return 1;
            }
            return 0;
        }

        public static void CreateFileJson<T>(int i, T obj, string fileName, string path)
        {
            try
            {
                string output = JsonConvert.SerializeObject(obj, new NoColonIsoDateTimeConverter());
                CreateWriteFile(fileName + ".json", output, path);
            }
            catch
            {
                if (i < 5)
                {
                    i++;
                    CreateFileJson(i, obj, fileName, path);
                }

            }
        }
        public async static Task<bool> CreateFileJsonAsync<T>(int i, T obj, string fileName, string path)
        {
            try
            {
                string output = JsonConvert.SerializeObject(obj, new NoColonIsoDateTimeConverter());
                await CreateWriteFileAsync(fileName + ".json", output, path);
                return true;
            }
            catch
            {
                if (i < 5)
                {
                    i++;
                    await CreateFileJsonAsync(i, obj, fileName, path);
                }
                return false;
            }
        }
        public static void CreateWriteFile(string fileName, string content, string path)
        {
            try
            {
                string name = $"{WebConfig.PathServer}{path}/{fileName}";
                using StreamWriter sw = new StreamWriter(File.Open(name, FileMode.Create), Encoding.UTF8);
                try
                {
                    sw.WriteLine(content);
                }
                catch
                {
                }
                sw.Flush();
                sw.Close();
                sw.Dispose();              
            }
            catch
            {

            }
        }
        public async static Task CreateWriteFileAsync(string fileName, string content, string path)
        {
            try
            {
                string name = $"{WebConfig.PathServer}{path}/{fileName}";
                FileInfo info = new(name);
                using StreamWriter writer = info.CreateText();
                try
                {
                    await writer.WriteLineAsync(content);
                    await writer.FlushAsync();
                    writer.Close();
                }
                catch
                {
                    await writer.FlushAsync();
                    writer.Close();
                }
                writer.Dispose();
            }
            catch
            {

            }
        }

        public class NoColonIsoDateTimeConverter : IsoDateTimeConverter
        {
            public NoColonIsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffZ";
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is DateTime dateTime)
                {
                    string text = dateTime.ToString(DateTimeFormat);
                    text = text.Remove(text.Length - 3, 1);
                    writer.WriteValue(text);
                }
                else
                {
                    throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime");
                }
            }


        }

        #endregion
        #region addlogs
        public static void AddLogError(Exception ex)
        {
            try
            {
                string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + ex.Message + ex.StackTrace;
                CreateAppendFile("LogClient_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", content, "Logs");
            }
            catch
            {

            }

        }
        public static void AddLogEdit(string active, string id, string userEdit, string contentEdit)
        {
            try
            {
                string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-" + active + ",ID:" + id + ",UserName:" + userEdit + "," + contentEdit;
                CreateAppendFile("LogClientEdit_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", content, "Logs");
            }
            catch
            {


            }
        }

        #endregion
        #region phân trang
        public static string GetAjaxPage(int page, int totalRecord, int pageSize)
        {
            string html = string.Empty;
            if (totalRecord > pageSize)
            {
                int currentPage = page > 1 ? page : 1;
                html = ReadFile("PageAjax.htm", "html/Configuaration");
                int total = currentPage * pageSize;
                bool disabledNext = total >= totalRecord;
                bool disablePrevious = total <= pageSize;
                if (disablePrevious && disabledNext)
                {
                    html = html.Replace("[displayView]", "displayView hidden");
                }
                if (disablePrevious)
                {
                    html = html.Replace("[disabledPrevious]", "disabled hidden");
                    html = html.Replace("[LinkPrevious]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledPrevious]", "");
                }
                if (disabledNext)
                {
                    html = html.Replace("[disabledNext]", "disabled hidden");
                    html = html.Replace("[LinkNext]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledNext]", "");
                }
                int mode = totalRecord % pageSize;
                int doubleNext;
                if (mode == 0)
                {
                    doubleNext = totalRecord / pageSize;
                }
                else
                {
                    doubleNext = (totalRecord / pageSize) + 1;
                }
                html = html.Replace("[LinkPrevious]", (currentPage - 1).ToString());
                html = html.Replace("[LinkNext]", (currentPage + 1).ToString());
                html = html.Replace("[LinkFirst]", "1");
                html = html.Replace("[LinkLast]", doubleNext.ToString());
                StringBuilder link = new();
                int begin = 1;
                int end = 4;
                if (doubleNext < 4)
                {
                    end = doubleNext;
                }
                else
                {
                    if (currentPage >= 4)
                    {
                        begin = currentPage - 2;
                        end = currentPage + 2;
                    }
                    if (end >= doubleNext)
                    {
                        begin = doubleNext - 4;
                        end = doubleNext;
                    
                    }
                }
                for (int i = begin; i <= end; i++)
                {
                    if (i == currentPage)
                    {
                        link.Append("<span class=\"active-page\" data-page=\"0\">" + currentPage + "</span>");
                    }
                    else
                    {
                        link.Append("<div data-page=\"" + i.ToString() + "\">" + i + "</div>");
                    }
                }
                html = html.Replace("[LinkNumber]", link.ToString());
            }
            return html;
        }
        public static string GetHtmlPageLink(int page, int totalRecord, int pageSize, string seoUrl, string lang)
        {
            string html = string.Empty;
            if (totalRecord > pageSize)
            {
                int currentPage = page > 1 ? page : 1;
                html = ReadFile("PageNumber.htm", "html/Configuaration");
                int total = currentPage * pageSize;
                bool disabledNext = total >= totalRecord;
                bool disablePrevious = total <= pageSize;
                if (disablePrevious && disabledNext)
                {
                    html = html.Replace("[displayView]", "displayView hidden");
                }
                if (disablePrevious)
                {
                    html = html.Replace("[disabledPrevious]", "disabled hidden");
                    html = html.Replace("[LinkPrevious]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledPrevious]", "");
                }

                if (disabledNext)
                {
                    html = html.Replace("[disabledNext]", "disabled hidden");
                    html = html.Replace("[LinkNext]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledNext]", "");
                }

                int mode = totalRecord % pageSize;
                int doubleNext;
                if (mode == 0)
                {
                    doubleNext = totalRecord / pageSize;
                }
                else
                {
                    doubleNext = (totalRecord / pageSize) + 1;
                }
                if ((currentPage - 1) > 1)
                {
                    html = html.Replace("[LinkPrevious]", seoUrl + "/page/" + (currentPage - 1));
                }
                else
                {
                    html = html.Replace("[LinkPrevious]", seoUrl);
                }
                html = html.Replace("[LinkNext]", seoUrl + "/page/" + (currentPage + 1));
                html = html.Replace("[LinkFirst]", seoUrl);
                html = html.Replace("[LinkLast]", seoUrl + "/page/" + doubleNext);
                StringBuilder link = new();
                int begin = 1;
                int end = 4;
                if (doubleNext <= 4)
                {
                    end = doubleNext;
                }
                else
                {
                    if (currentPage >= 4)
                    {
                        begin = currentPage - 2;
                        end = currentPage + 2;
                    }
                    if (end >= doubleNext)
                    {
                        begin = doubleNext - 4;
                        end = doubleNext;
                    }
                }
                for (int i = begin; i <= end; i++)
                {
                    if (i == currentPage)
                    {
                        link.Append("<span class=\"active-page\">" + currentPage + "</span>");
                    }
                    else
                    {
                        link.Append("<a href=\"" + (i > 1 ? seoUrl + "/page/" + i : seoUrl) + "\">" + i + "</a>");
                    }
                }
                html = html.Replace("[LinkNumber]", link.ToString());
            }
            return html;
        }
        public static string GetHtmlPageLinkWithFilter(int page, int totalRecord, int pageSize, string seoUrl, bool haskeys)
        {
            string html = string.Empty;
            if (totalRecord > pageSize)
            {
                int currentPage = page > 1 ? page : 1;
                html = ReadFile("PageNumber.htm", "html/Configuaration");
                int total = currentPage * pageSize;
                bool disabledNext = total >= totalRecord;
                bool disablePrevious = total <= pageSize;
                if (disablePrevious && disabledNext)
                {
                    html = html.Replace("[displayView]", "displayView hidden");
                }
                if (disablePrevious)
                {
                    html = html.Replace("[disabledPrevious]", "disabled hidden");
                    html = html.Replace("[LinkPrevious]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledPrevious]", "");
                }

                if (disabledNext)
                {
                    html = html.Replace("[disabledNext]", "disabled hidden");
                    html = html.Replace("[LinkNext]", "javascript:");
                }
                else
                {
                    html = html.Replace("[disabledNext]", "");
                }

                int mode = totalRecord % pageSize;
                int doubleNext;
                if (mode == 0)
                {
                    doubleNext = totalRecord / pageSize;
                }
                else
                {
                    doubleNext = (totalRecord / pageSize) + 1;
                }
                if ((currentPage - 1) > 1)
                {
                    if (haskeys)
                    {
                        html = html.Replace("[LinkPrevious]", seoUrl + "&page=" + (currentPage - 1));
                    }
                    else
                    {
                        html = html.Replace("[LinkPrevious]", seoUrl + "?page=" + (currentPage - 1));
                    }
                }
                else
                {
                    html = html.Replace("[LinkPrevious]", seoUrl);
                }
                if (haskeys)
                {
                    html = html.Replace("[LinkNext]", seoUrl + "&page=" + (currentPage + 1));
                    html = html.Replace("[LinkFirst]", seoUrl);
                    html = html.Replace("[LinkLast]", seoUrl + "&page=" + doubleNext);
                }
                else
                {
                    html = html.Replace("[LinkNext]", seoUrl + "?page=" + (currentPage + 1));
                    html = html.Replace("[LinkFirst]", seoUrl);
                    html = html.Replace("[LinkLast]", seoUrl + "/page=" + doubleNext);
                }
                StringBuilder link = new();
                int begin = 1;
                int end = 8;
                if (doubleNext < 8)
                {
                    end = doubleNext;
                }
                else
                {
                    if (currentPage >= 8)
                    {
                        begin = currentPage - 2;
                        end = currentPage + 2;
                    }
                    if (end >= doubleNext)
                    {
                        begin = doubleNext - 4;
                        end = doubleNext;
                    }
                }
                for (int i = begin; i <= end; i++)
                {
                    if (i == currentPage)
                    {
                        link.Append("<li><a href=\"javascript:\" class=\"active-page\">" + currentPage + "</span></li>");
                    }
                    else
                    {
                        if (haskeys)
                        {
                            link.Append("<li><a href=\"" + (i > 1 ? seoUrl + "&page=" + i : seoUrl) + "\">" + i + "</a></li>");
                        }
                        else
                        {
                            link.Append("<li><a href=\"" + (i > 1 ? seoUrl + "?page=" + i : seoUrl) + "\">" + i + "</a></li>");
                        }
                    }
                }
                html = html.Replace("[LinkNumber]", link.ToString());
            }
            return html;
        }
        #endregion
        #region check mail
        public static bool check_gmail(string gmail)
        //Trả về True là gmail tồn tại
        {
            bool ketqua = false;
            try
            {
                TcpClient tClient = new("gmail-smtp-in.l.google.com", 25);
                const string crlf = "\r\n";
                NetworkStream netStream = tClient.GetStream();
                StreamReader reader = new(netStream);
                reader.ReadLine();
                byte[] dataBuffer = BytesFromString("HELO VietQuan" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                reader.ReadLine();
                dataBuffer = BytesFromString("MAIL FROM:<adcsoft1@gmail.com>" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                reader.ReadLine();
                dataBuffer = BytesFromString("RCPT TO:<" + gmail.Trim() + ">" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                string responseString = reader.ReadLine();
                ketqua = GetResponseCode(responseString) != 550;
                dataBuffer = BytesFromString("QUITE" + crlf);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                tClient.Close();
            }
            catch
            {

            }
            return ketqua;
        }
        private static byte[] BytesFromString(string str) => Encoding.ASCII.GetBytes(str);
        private static int GetResponseCode(string responseString) => int.Parse(responseString[..3]);
        #endregion       
        public static bool IsMobile(HttpContext context)
        {
            try
            {
                if (context.Request.Cookies["viewer"] == "mobile")
                {
                    return true;
                }
                else
                {
                    string agent = context.Request.Headers["User-Agent"].ToString();
                    Regex b = new(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Regex v = new(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    return b.IsMatch(agent) || v.IsMatch(agent[..4]);
                }
            }
            catch
            {
                return false;
            }
        }

        public static string Image(string urlImage, HttpContext context, int type = 0, int typemb = 0, string addInfo = "")
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(urlImage))
            {
                str = urlImage.Contains("http://") || urlImage.Contains("https://")
                    ? $"<img title=\"{addInfo}\" alt=\"{addInfo}\" data-original=\"{urlImage}\" class=\"lazy\" />"
                    : $"<img title=\"{addInfo}\" alt=\"{addInfo}\" data-original=\"{ImageUrl(urlImage, context, type, typemb)}\" class=\"lazy\" />";
            }
            return str;
        }
        public static string ImageWithSize(string urlImage, HttpContext context, int type = 0, int typemb = 0, string addInfo = "", int width = 0, int height = 0, int widthmb = 0, int heightmb = 0)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(urlImage))
            {
                str = urlImage.Contains("http://") || urlImage.Contains("https://")
                    ? $"<img alt=\"{addInfo}\" data-original=\"{urlImage}\" class=\"lazy\" width=\"{(IsMobile(context) ? widthmb : width)}\" height=\"{(IsMobile(context) ? heightmb : height)}\" />"
                    : $"<img alt=\"{addInfo}\" data-original=\"{ImageUrl(urlImage, context, type, typemb)}\" class=\"lazy\" width=\"{(IsMobile(context) ? widthmb : width)}\" height=\"{(IsMobile(context) ? heightmb : height)}\" />";
            }
            return str;
        }
        public static string ImageOwlLazy(string urlImage, HttpContext context, int type = 0, int typemb = 0, string addInfo = "", int width = 0, int height = 0, int widthmb = 0, int heightmb = 0)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(urlImage))
            {
                str = urlImage.Contains("http://") || urlImage.Contains("https://")
                    ? $"<img alt=\"{addInfo}\" data-src=\"{urlImage}\" class=\"owl-lazy\" width=\"{(IsMobile(context) ? widthmb : width)}\" height=\"{(IsMobile(context) ? heightmb : height)}\" />"
                    : $"<img alt=\"{addInfo}\" data-src=\"{ImageUrl(urlImage, context, type, typemb)}\" class=\"owl-lazy\" width=\"{(IsMobile(context) ? widthmb : width)}\" height=\"{(IsMobile(context) ? heightmb : height)}\" />";
            }
            return str;
        }
        public static string ImageWithSizeNoLazy(string urlImage, HttpContext context, int type = 0, int typemb = 0, string addInfo = "", int width = 0, int height = 0, int widthmb = 0, int heightmb = 0)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(urlImage))
            {
                str = urlImage.Contains("http://") || urlImage.Contains("https://")
                    ? $"<img alt=\"{addInfo}\" src=\"{urlImage}\" width=\"{width}\" height=\"{height}\" />"
                    : $"<img alt=\"{addInfo}\" src=\"{ImageUrl(urlImage, context, type, typemb)}\" width=\"{(IsMobile(context) ? widthmb : width)}\" height=\"{(IsMobile(context) ? heightmb : height)}\" />";
            }
            return str;
        }
        public static string ImageUrl(string urlImage, HttpContext context, int type, int typemb)
        {
            try{
                if(urlImage.Contains("http://") || urlImage.Contains("https://"))
                {
                    return urlImage;
                }
                string pathHost = context.Request.Host.Host;
                string path = Path.GetDirectoryName(urlImage);
                string file = Path.GetFileName(urlImage);
                string ext = Path.GetExtension(file);
                if (ext == ".svg" || ext == ".webp" || ext == ".gif")
                {
                    return urlImage;
                }
                if (!string.IsNullOrEmpty(urlImage))
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        path += "/";
                        path = path.Replace("\\", "/");
                    }
                    if (IsMobile(context))
                    {
                        if (typemb>0)
                        {
                            path = path.Replace("/Upload/", "/resize/" + typemb + "/");
                            return path + Path.GetFileNameWithoutExtension(file) + "x" + typemb + "x4" + Path.GetExtension(file);
                        }
                        return urlImage;
                    }
                    if (type>0)
                    {
                        path = path.Replace("/Upload/", "/resize/" + type + "/");
                        //if (pathHost == "localhost")
                        //{
                        //    return WebConfig.Website + path + Path.GetFileNameWithoutExtension(file) + "x" + type + "x4" + Path.GetExtension(file);
                        //}
                        return path + Path.GetFileNameWithoutExtension(file) + "x" + type + "x4" + Path.GetExtension(file);
                    }
                }
            }
            catch{

            }
            return urlImage;
        }
    }
}