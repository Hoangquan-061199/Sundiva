using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ADCOnline.Simple.Item;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace ADCOnline.Utils
{
    public static class Utility
    {
        public static string RemoveSpecialCharacterSQLInjection(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[,|~|@|/|.|:|?|#|$|%|*|(|)|+|”|“|{|}|;|<|>|^|&|'|\"|!|`]", "", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }
        public static bool CheckBlackList(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return BlackListString.Any(x => str.ToLower().Contains(x));
                }
            }
            catch
            {
            }
            return false;
        }
        public static readonly List<string> BlackListString = new List<string>()
        {
            "replace", "script","css", "select", "from", "test", "pHqghUme", "php", "sql", "concat", "require", "gethostbyname", "socket",
            "import","md5", "base64", "decode", "assert", "bxss", "hex", "bxss", "windows", "http","src", "💡", "passwd", "nslookup", "noscript", "style", "title", "delete", "update", "textarea", "script",
            "response", "write", "dns", "log4j", "�", "receive", "pipe", "chr", "sleep", " or", "or ", "and ", " and", "waitfor", "delay", "xor", "sysdate", "foreach","for",
            "echo"
        };
        public static string RemoveSpecialCharacterSQLInjection2(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[~|/|.|:|?|#|$|%|*|(|)|+|”|“|{|}|;|<|>|^|&|'|\"|!|`]", " ", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }
        public static string RemoveSpecialCharacterNotDot(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[~|@|:|?|#|$|%|^|&|*|(|)|<|>|+|”|“|{|}|;|<|>|^|&|'|\"|!|`]", "", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }

        public static bool IsWebpImage(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".webp", StringComparison.OrdinalIgnoreCase);
        }

        public static SizeImages GetSizeImages(string pathServer, string imageUrl)
        {
            try
            {
                SizeImages imageSize = new SizeImages();
                if (string.IsNullOrEmpty(imageUrl) || imageUrl.EndsWith("svg"))
                {
                    imageSize.IsPicture = false;
                    imageSize.Width = 600;
                    imageSize.Height = 600;
                    return imageSize;
                }
                if (!imageUrl.Contains("http"))
                {
                    string url = pathServer + imageUrl;
                    if (!File.Exists(url))
                    {
                        imageSize.IsPicture = false;
                        imageSize.Width = 600;
                        imageSize.Height = 600;
                        return imageSize;
                    }
                    using (Image filefrom = Image.Load(url))
                    {
                        imageSize.IsPicture = true;
                        imageSize.Width = filefrom.Width;
                        imageSize.Height = filefrom.Height;
                    };
                }
                else
                {
                    var ischeck = isImageUlr(imageUrl);
                    if (!ischeck)
                    {
                        imageSize.IsPicture = false;
                        imageSize.Width = 600;
                        imageSize.Height = 600;
                        return imageSize;
                    }
                    using (var httpClient = new WebClient())
                    using (var imageStream = httpClient.OpenRead(imageUrl))
                    using (var image = Image.Load(imageStream))
                    {
                        imageSize.IsPicture = true;
                        imageSize.Width = image.Width;
                        imageSize.Height = image.Height;
                    }
                }
                return imageSize;
            }
            catch
            {
            }
            return null;
        }

        public static bool isImageUlr(string url)
        {
            HttpWebResponse response = null;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                /* A WebException will be thrown if the status of the response is not `200 OK` */
                return false;
            }
            finally
            {
                // Don't forget to close your response.
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public static string GetImage(string pathServer, string imageUrl, string name)
        {
            if(string.IsNullOrEmpty(imageUrl)) return "<span class=\"no-image\">No image</span>";
            if (imageUrl.EndsWith("svg"))
            {
                return $"<img src=\"{imageUrl}\" alt=\"{name}\" loading=\"lazy\">";
            }
            else
            {
                SizeImages image = GetSizeImages(pathServer, imageUrl);
                if (image.IsPicture)
                {
                    return $"<img src=\"{imageUrl}\" alt=\"{name}\" width=\"{image.Width}\" height=\"{image.Height}\" loading=\"lazy\">";
                }
                else
                {
                    return "<span class=\"no-image\">No image</span>";
                }
            }
        }

        public static string GetUrlPicture(string pathServer, string imageUrl)
        {
            string url = string.Empty;

            if (imageUrl.Contains("http"))
            {
                url = imageUrl;
                bool ischeck = isImageUlr(url);
                if (ischeck)
                {
                    url = imageUrl;
                }
                else
                {
                    url = "/html/style/images/image-no-image.webp";
                }
            }
            else
            {
                url = pathServer + imageUrl;
                if (File.Exists(pathServer + imageUrl))
                {
                    url = imageUrl;
                }
                else
                {
                    url = "/html/style/images/image-no-image.webp";
                }
            }
            return url;
        }
        public static string ValidString(string unicode, string code, bool removeHtml = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(unicode))
                {
                    switch (code)
                    {
                        case "301":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\w^\-:/.=?@]";
                                unicode = Regex.Replace(unicode, rex, "");
                                unicode = unicode.Replace("--", "-");
                                break;
                            }
                        case "link":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\w^\-:/.=?#&]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                        case "arrayint":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\d,]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                        case "arraycode":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\w,]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                        case "code":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[\W+]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                        case "title":
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\w\s^\-_()|/.,!&?%\[]]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                        default:
                            {
                                if (removeHtml == true)
                                {
                                    unicode = RemoveHTML(unicode);
                                }
                                string rex = @"[^\w\s]";
                                unicode = Regex.Replace(unicode, rex, "");
                                break;
                            }
                    }
                }
            }
            catch { }
            return unicode;
        }
        public static string ValidAddress(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[~|@|.|:|?|#|$|%|^|&|*|(|)|<|>|+|”|“|{|}|;|'|\"|!|`]", " ", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }
        public static bool ValidSpecial(string input)
        {
            try
            {
                string specialChar = @"!@#$%^&*()+=|\{}[];:'<>?/\""\";
                foreach (var item in specialChar)
                {
                    if (input.Contains(item)) return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool ContainsTextLowcaseUpcaseNumberSpecial(string input)
        {
            try
            {
                bool hasLower = false, hasUpcase = false, hasNumber = false, hasSpecial = false;
                for (int i = 0; i < input.Length && !(hasLower && hasUpcase && hasNumber && hasSpecial); i++)
                {
                    char c = input[i];
                    if (!hasLower) hasLower = char.IsUpper(c);
                    if (!hasUpcase) hasUpcase = char.IsUpper(c);
                    if (!hasNumber) hasNumber = char.IsUpper(c);
                    if (!hasSpecial) hasSpecial = char.IsUpper(c);
                }
                if (hasLower == true && hasUpcase == true && hasNumber == true && hasSpecial == true) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static string ValidNote(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[~|?|#|$|%|^|&|*|(|)|<|>|+|”|“|{|}|;|<|>|^|&|'|\"|!|`]", " ", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }
        public static bool CheckSpecial(string s)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (regexItem.IsMatch(s))
            {
                return false;
            }
            return true;
        }
        public static int CountWord(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return Regex.Matches(str, @"[A-Za-z0-9]+").Count;
            }
            return 0;
        }
        public static string FormatSize(Int64 bytes)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
        public static string Base64Encode(string plainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string RenDateFileName()
        {
            DateTime dt = DateTime.Now;
            string str = dt.ToString("MMddyyyyHHmmss");
            return str;
        }
        public static string FormatPhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
                return phone.Replace(" ", "").Replace("-", "").Replace(".", "").Replace(")", "").Replace("(", "");
            else return phone;
        }
        public async static Task<string> GetDataTemplate(string PathTemplate)
        {
            StreamReader sr = File.OpenText(PathTemplate);
            string Content = await sr.ReadToEndAsync();
            sr.Close();
            sr.Dispose();
            return Content;
        }
        public static string GetShortName(string str)
        {
            string s = string.Empty;
            try
            {
                string[] listName = str.Split(" ");
                foreach (string item in listName)
                {
                    if (s.Length < 2)
                    {
                        s += item[..1].ToUpper();
                    }
                }
            }
            catch
            {
                s = str[..2];
            }
            return s;
        }
        public static string AddAltImage(string response, string name)
        {
            if (!string.IsNullOrEmpty(response))
            {
                HtmlDocument doc = GetHtmlDocument(response);
                int i = 1;
                IEnumerable<HtmlNode> imageList = doc.DocumentNode.Descendants("img");
                foreach (HtmlNode imgTag in imageList)
                {
                    string altT = name + " " + i;
                    string a = string.Empty;
                    HtmlAttribute altOld = imgTag.Attributes["alt"];
                    if (altOld == null)
                    {
                        string original = imgTag.OuterHtml;
                        HtmlNode replacement = imgTag.Clone();
                        replacement.Attributes.Append("alt");
                        replacement.SetAttributeValue("alt", altT);
                        response = response.Replace(original, replacement.OuterHtml);

                    }
                    else
                    {
                        a = imgTag.Attributes["alt"].Value;
                        if (string.IsNullOrEmpty(a))
                        {
                            string original = imgTag.OuterHtml;
                            HtmlNode replacement = imgTag.Clone();
                            replacement.SetAttributeValue("alt", altT);
                            response = response.Replace(original, replacement.OuterHtml);
                        }
                    }
                    i++;
                }
            }
            return response;
        }
        #region Replace Html AMP
        public static string ReplaceWithLink(string tag, string response)
        {
            HtmlDocument doc = GetHtmlDocument(response);
            IEnumerable<HtmlNode> elements = doc.DocumentNode.Descendants(tag);
            if (!elements.Any()) return response;
            foreach (HtmlNode htmlNode in elements)
            {
                if (htmlNode.Attributes["data-link"] == null) continue;
                string dataLink = htmlNode.Attributes["data-link"].Value;
                HtmlNode paragraph = doc.CreateElement("p");
                string text = string.Format("[Embedded Link] {0}", dataLink);
                HtmlNode anchor = doc.CreateElement("a");
                anchor.InnerHtml = RemoveHTMLTag(text);
                anchor.Attributes.Add("href", dataLink);
                anchor.Attributes.Add("title", text);
                paragraph.InnerHtml = anchor.OuterHtml;
                string original = htmlNode.OuterHtml;
                string replacement = paragraph.OuterHtml;
                response = response.Replace(original, replacement);
            }
            return response;
        }
        public static string RemoveIframe(string content)
        {
            Match matchdec = Regex.Match(content, @"(?:<iframe[^>]*)(?:(?:\/>)|(?:>.*?<\/iframe>))", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            while (matchdec.Success)
            {
                if (matchdec.Groups.Count >= 1)
                {
                    string retval = matchdec.Groups[0].Value;
                    content = content.Replace(retval, "");
                }
                matchdec = matchdec.NextMatch();
            }
            string result = content;
            return result;

        }
        public static string RemoveStyle(string content)
        {
            Match matchdec = Regex.Match(content, @"()\s+style\s*=\s*([""']).*?\2(.*?)", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            while (matchdec.Success)
            {
                if (matchdec.Groups.Count >= 1)
                {
                    string retval = matchdec.Groups[0].Value;
                    content = content.Replace(retval, "");
                }
                matchdec = matchdec.NextMatch();
            }
            string result = content;
            return result;

        }
        public static string UpdateAmpImages(string response)
        {
            HtmlDocument doc = GetHtmlDocument(response);
            IEnumerable<HtmlNode> imageList = doc.DocumentNode.Descendants("img");
            const string ampImage = "amp-img";
            if (!imageList.Any()) return response;
            if (!HtmlNode.ElementsFlags.ContainsKey("amp-img"))
            {
                HtmlNode.ElementsFlags.Add("amp-img", HtmlElementFlag.Closed);
            }
            foreach (HtmlNode imgTag in imageList)
            {
                string h = string.Empty, w = string.Empty, hi = string.Empty;
                try
                {
                    h = imgTag.Attributes["style"].Value;
                    var lststyle = ListHelper.GetValuesArrayTagBySymbol(";", h);
                    if (ListHelper.GetValuesArrayTagBySymbol(":", lststyle[0])[0].Trim() == "height")
                    {
                        hi = ListHelper.GetValuesArrayTagBySymbol(":", lststyle[0])[1];
                    }
                    if (ListHelper.GetValuesArrayTagBySymbol(":", lststyle[0])[0].Trim() == "width")
                    {
                        w = ListHelper.GetValuesArrayTagBySymbol(":", lststyle[0])[1];
                    }
                    if (ListHelper.GetValuesArrayTagBySymbol(":", lststyle[1])[0].Trim() == "height")
                    {
                        hi = ListHelper.GetValuesArrayTagBySymbol(":", lststyle[1])[1];
                    }
                    if (ListHelper.GetValuesArrayTagBySymbol(":", lststyle[1])[0].Trim() == "width")
                    {
                        w = ListHelper.GetValuesArrayTagBySymbol(":", lststyle[1])[1];
                    }
                }
                catch { }
                string original = imgTag.OuterHtml;
                HtmlNode replacement = imgTag.Clone();
                replacement.Name = Utility.RemoveHTMLTag(ampImage);
                replacement.Attributes.Append("layout");
                replacement.SetAttributeValue("layout", "responsive");
                replacement.Attributes.Append("height");
                replacement.SetAttributeValue("height", hi);
                replacement.Attributes.Append("width");
                replacement.SetAttributeValue("width", w);
                replacement.Attributes.Remove("style");
                response = response.Replace(original, replacement.OuterHtml);
            }
            return response;
        }
        public static string RemoveImage(string content)
        {
            Match matchdec = Regex.Match(content, @"<img\s+[^>]*src=""([^""]*)""[^>]*>", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            while (matchdec.Success)
            {
                if (matchdec.Groups.Count >= 1)
                {
                    string retval = matchdec.Groups[0].Value;
                    content = content.Replace(retval, "");
                }
                matchdec = matchdec.NextMatch();
            }
            string result = content;
            return result;
        }
        #endregion
        private static HtmlDocument GetHtmlDocument(string htmlContent)
        {
            HtmlDocument doc = new()
            {
                OptionOutputAsXml = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            doc.LoadHtml(htmlContent);
            return doc;
        }
        //Context.Request.Headers["User-Agent"]
        public static bool IsMobile(string agent)
        {
            try
            {
                Regex b = new(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(agent) || v.IsMatch(agent[..4])))
                {
                    return true;
                }
            }
            catch
            {

            }

            return false;
        }
        public static bool IsTablet(string agent)
        {
            try
            {
                Regex tabletRegex = new Regex(@"(android(?!.*mobile))|(tablet)|(ipad)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (tabletRegex.IsMatch(agent))
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }
        public static string GetDomain(string Domain) => Domain.Contains("localhost") ? "" : Domain;
        public static bool IsValidDomainName(string domain) => Uri.CheckHostName(domain) == UriHostNameType.Unknown;
        public static string CheckLinkRedirect(string link, string linkUrl) => string.IsNullOrEmpty(linkUrl) ? "/" + link : linkUrl;

        public static bool CheckDecimal(string str) => decimal.TryParse(str, out decimal value);
        public static bool CheckNumber(string str) => int.TryParse(str, out int value);
        public static string TrimLength(string input, int maxLength)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Length > maxLength)
                {
                    maxLength -= "...".Length;
                    maxLength = input.Length < maxLength ? input.Length : maxLength;
                    bool isLastSpace = input[maxLength] == ' ';
                    string part = input.Substring(0, maxLength);
                    if (isLastSpace)
                        return part + "...";
                    int lastSpaceIndexBeforeMax = part.LastIndexOf(' ');
                    if (lastSpaceIndexBeforeMax == -1)
                        return part + "...";
                    return string.Concat(input.AsSpan(0, lastSpaceIndexBeforeMax), "...");
                }
            }
            return input;
        }
        public static string ConvertRewrite(string unicode)
        {
            try
            {
                if (!string.IsNullOrEmpty(unicode))
                {
                    unicode = NewUnicodeToAscii(unicode);
                    unicode = unicode.ToLower().Trim();
                    unicode = Regex.Replace(unicode, @"\s+", " ");
                    unicode = Regex.Replace(unicode, "[\\s]", "-");
                    //Unicode = UnicodeToAscii(Unicode);
                    unicode = Regex.Replace(unicode, @"-+", "-");
                    unicode = unicode.Replace("®", "");
                    unicode = unicode.Replace("™", "");
                }
            }
            catch
            {
            }
            return unicode;
        }


        /// <summary>
        /// create by BienLV 02-04-2014
        /// remove special character 
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public static string NewUnicodeToAscii(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[á|à|ả|ã|ạ|â|ă|ấ|ầ|ẩ|ẫ|ậ|ắ|ằ|ẳ|ẵ|ặ]", "a", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ]", "e", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự]", "u", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[í|ì|ỉ|ĩ|ị]", "i", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[ó|ò|ỏ|õ|ọ|ô|ơ|ố|ồ|ổ|ỗ|ộ|ớ|ờ|ở|ỡ|ợ]", "o", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[đ|Đ]", "d", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[ý|ỳ|ỷ|ỹ|ỵ|Ý|Ỳ|Ỷ|Ỹ|Ỵ]", "y", RegexOptions.IgnoreCase);
                unicode = Regex.Replace(unicode, "[,|~|@|/|.|:|?|#|$|%|&|*|(|)|+|”|“|'|\"|!|`|–]", "", RegexOptions.IgnoreCase);
            }
            catch { }
            return unicode;
        }
        public static bool IsValidEmail(string email)
        {
            string trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                System.Net.Mail.MailAddress addr = new(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// create by BienLV 00-04-2014
        /// change special character to "-"
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacter(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[,|~|@|/|.|:|?|#|$|%|&|*|(|)|+|”|“|'|\"|!|`|–]", "-", RegexOptions.IgnoreCase);
                return unicode;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string RemoveValidFullname(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[,|~|@|/|.|:|?|#|$|%|&|*|(|)|+|”|“|'|\"|!|`|–]", " ", RegexOptions.IgnoreCase);
                return unicode.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string RemoveValidUserName(string unicode)
        {
            try
            {
                unicode = Regex.Replace(unicode, "[`|~|#|$|%|^|&|*|(|)|+|=|\"|{|}|:|/|”|“|'|?|>|<|`|–]", "", RegexOptions.IgnoreCase);
                return unicode;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string RemoveValidNumber(string unicode)
        {
            try
            {
                Regex digitsOnly = new(@"[^\d]");
                return digitsOnly.Replace(unicode, "");
            }
            catch
            {
                return string.Empty;
            }
        }


        public static string CreateSaltKey(int size)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
        public static string GetRandom()
        {
            StringBuilder builder = new();
            //builder.Append(RandomString(4,true));
            builder.Append(RandomNumber(1000000, 9999999));
            //builder.Append(RandomString(2,false));
            return builder.ToString();
        }
        public static string GetRandom(int from, int to)
        {
            StringBuilder builder = new();
            //builder.Append(RandomString(4,true));
            builder.Append(RandomNumber(from, to));
            //builder.Append(RandomString(2,false));
            return builder.ToString();
        }
        public static string RandomCode()
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new(1970, 1, 1);
            DateTime d2 = now.ToUniversalTime();
            TimeSpan ts = new(d2.Ticks - d1.Ticks);
            string result = (Math.Round(ts.TotalSeconds, 0)).ToString() + Utility.RandomNumber(100, 999);
            return result;
        }
        public static double DateTimeToJson(DateTime dt)
        {
            DateTime d1 = new(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }
        //1562402283
        public static DateTime JsonToDateTime(string dt)
        {
            DateTime d1 = new(1970, 1, 1);
            long d = (long)ConvertUtil.ToDouble(dt) + d1.Ticks;
            DateTime myDate = new(d);
            return myDate;
        }
        public static int RandomNumber(int min, int max)
        {
            Random random = new();
            return random.Next(min, max);
        }
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new();
            Random random = new();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public static string CreatePasswordHash(string password, string saltkey)
        {
            byte[] salt = Convert.FromBase64String(saltkey);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            return hashed;
        }


        /// <summary>
        /// Hàm tạo mã md5
        /// </summary>
        /// <param name="str">xâu cần mã hóa</param>
        /// <Modified>        
        ///	Name		Date		    Comment 
        /// DongDT      11/12/2013     Thêm string mã hóa: ADCOnline@2014
        /// </Modified>
        public static string RemoveHTML(string source) => string.IsNullOrEmpty(source) == false ? Regex.Replace(source, "<.*?>", "") : string.Empty;

        /// <summary>
        /// create by BienLV 17-01-2014
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveASCII(string source) => Regex.Replace(RemoveHTML(source), @"\t*\n*\r*\s*", "");
        public static string ConvertDateTimeVi(DateTime? value)
        {
            if (value.HasValue)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-Us");
                return value?.ToString("dd/MM/yyyy HH:mm");
            }
            else
                return string.Empty;
        }

        public static string ConvertDateVi(DateTime? value)
        {
            if (value.HasValue)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-Us");
                return value.Value.ToString("dd/MM/yyyy");
            }
            else
                return string.Empty;
        }
        public static string ConvertDateMMddyyyy(string ddmmyyyy)
        {
            if (!string.IsNullOrEmpty(ddmmyyyy))
            {
                List<string> lst = new();
                string[] t = ddmmyyyy.ToString().Split('/');
                for (int i = 0; i < t.Length; i++)
                {
                    lst.Add(t[i]);
                }
                return lst[1] + "/" + lst[0] + "/" + lst[2];
            }
            else
                return string.Empty;
        }
        public static List<int> StringToListInt(string array)
        {
            List<int> lst = new();
            try
            {
                if (!string.IsNullOrEmpty(array))
                {
                    array = array.Trim(',');
                    lst = array.Split(',').Select(int.Parse).ToList();
                }
            }
            catch { }
            return lst;
        }
        public static string DisplayShowName(string Name, bool show) => show == false ? "<del>" + Name + "</del>" : "<span>" + Name + "</span>";
        public static string DisplayShowTitle(string Name, string Title) => !string.IsNullOrEmpty(Title) ? Title : Name;
        public static string FormatNumber(object val, int comma = 0)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            string format = "N" + comma;
            string result = string.Format("{0:" + format + "}", val);
            return result;
        }
        public static string GetFormatPriceType(decimal? value, Int16? type, string content, bool? showprice = true)
        {
            switch (type)
            {
                case 1:
                    if (value.HasValue)
                    {
                        if (value.Value == 0)
                            return content;
                        else if (showprice.HasValue && showprice == false)
                            return content;
                        else
                            return String.Format("{0:0,0}", value).Replace(".", ",") + "đ";
                    }
                    else
                        return content;
                case 2:
                    return "$" + value;
                case 3:
                    return value.HasValue ? value.Value == 0 ? content : string.Format("{0:0,0}", value).Replace(".", ",") : content;
                case 4:
                    return value.HasValue
                        ? value.Value switch
                        {
                            0 => content,
                            _ => showprice.HasValue && showprice == false ? content : String.Format("{0:0,0}", value).Replace(".", ","),
                        }
                        : content;
                default: return string.Empty;
            }
        }
        public static string CreateRandomKey(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[size];
            Random random = new();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string finalString = new(stringChars);
            // Has 18 character.
            string tickOfTime = DateTime.Now.Ticks.ToString();
            // Has 2 character.
            string date = string.Format("{0:ss}", DateTime.Now);
            date = date.Replace(":", String.Empty);
            // Has 35 character.
            string key = finalString + DateTime.Now.Ticks.ToString();
            return key;
        }
        public static string CreateRandomTransaction(int len) => Guid.NewGuid().ToString()[..len];
        public static string Link(string moduleAscii, string nameAscii, string redirect)
        {
            if (!string.IsNullOrEmpty(redirect))
            {
                return redirect;
            }
            if (!string.IsNullOrEmpty(moduleAscii) && !string.IsNullOrEmpty(nameAscii))
            {
                return "/" + moduleAscii + "/" + nameAscii;
            }
            if (!string.IsNullOrEmpty(moduleAscii))
            {
                moduleAscii = moduleAscii.TrimEnd('/');
            }
            if (!string.IsNullOrEmpty(nameAscii))
            {
                nameAscii = nameAscii.TrimEnd('/');
            }
            if (!string.IsNullOrEmpty(nameAscii))
            {
                string moduleSub = nameAscii[..1];
                if (moduleSub == "/")
                {
                    return nameAscii;
                }
                return "/" + nameAscii;
            }
            if (!string.IsNullOrEmpty(moduleAscii))
            {
                string moduleSub = moduleAscii[..1];
                if (moduleSub == "/")
                {
                    return moduleAscii;
                }
                return "/" + moduleAscii;
            }
            return string.Empty;
        }

        public static string LinkMenu(string nameAscii, string linkurl, string redirect)
        {
            if (!string.IsNullOrEmpty(redirect))
            {
                redirect = redirect.TrimEnd('/');
                return redirect;
            }
            if (!string.IsNullOrEmpty(linkurl))
            {
                return linkurl.TrimEnd('/');
            }
            if (!string.IsNullOrEmpty(nameAscii))
            {
                return nameAscii.TrimEnd('/');
            }
            return string.Empty;
        }
        public static string LinkAdmin(string moduleAscii, string nameAscii, string redirect)
        {
            if (!string.IsNullOrEmpty(redirect))
            {
                return redirect;
            }
            if (!string.IsNullOrEmpty(moduleAscii) && !string.IsNullOrEmpty(nameAscii))
            {
                string moduleSub = moduleAscii[..1];
                if (moduleSub == "/")
                {
                    return moduleAscii + "/" + nameAscii;
                }
                return "/" + moduleAscii + "/" + nameAscii;
            }
            if (!string.IsNullOrEmpty(nameAscii))
            {
                string moduleSub = nameAscii[..1];
                if (moduleSub == "/")
                {
                    return nameAscii;
                }
                return "/" + nameAscii;
            }
            if (!string.IsNullOrEmpty(moduleAscii))
            {
                string moduleSub = moduleAscii[..1];
                if (moduleSub == "/")
                {
                    return moduleAscii;
                }
                return "/" + moduleAscii;
            }
            return "javascript:void(0)";
        }
        public static List<string> StringToListString(string array)
        {
            List<string> lst = new();
            try
            {
                if (!string.IsNullOrEmpty(array))
                {
                    array = array.Trim(',');
                    lst = array.Split(',').ToList();
                }
            }
            catch { }
            return lst;
        }
        public static List<string> FolderToListString(string array)
        {
            List<string> lst = new();
            try
            {
                if (!string.IsNullOrEmpty(array))
                {
                    array = array.Trim('/');
                    lst = array.Split('/').ToList();
                }
            }
            catch { }
            return lst.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        //Lấy ra phần tử trước hoặc sau.
        // isLast: là lấy đằng sau.
        //lenght: Số lượng
        public static string SubString(string val, string split, int lenght, bool isLast = false)
        {
            //s.Substring(s.LastIndexOf('/') + 1);
            string result = val;
            try
            {
                List<string> vals = val.Split(char.Parse(split)).ToList();
                if (vals != null && vals.Count > lenght)
                {
                    if (isLast)
                    {
                        int len = vals.Count - lenght;
                        for (int i = 0; i < len; i++)
                        {
                            vals.RemoveAt(0);
                        }
                        result = string.Join(",", vals);
                    }
                    else
                    {
                        int len = vals.Count - lenght;
                        for (int i = 0; i < len; i++)
                        {
                            vals.RemoveAt(vals.Count - 1);
                        }
                        result = string.Join(",", vals);
                    }
                }
            }
            catch
            {

            }
            return result;
        }

        public static string RemoveHTMLTag(string source) => !string.IsNullOrEmpty(source) ? Regex.Replace(source, "<.*?>", "") : string.Empty;
        public static string ReplaceSpecialText(string source)
        {
            if (string.IsNullOrEmpty(source) == false)
            {
                source = source.Replace("&amp;", "&");
                source = source.Replace("&nbsp;", " ");
                source = source.Replace("&gt;", ">");
                source = source.Replace("&lt;", "<");
                source = source.Replace("&quot;", "“");
                return source;
            }
            return string.Empty;
        }

        public static string FormatMobileView(string phone)
        {
            string mobileView = string.Empty;
            if (!string.IsNullOrEmpty(phone))
            {
                string phone4 = string.Empty;
                string phone1 = phone.Substring(0, 4);
                string phone2 = phone.Substring(4, 3);
                string phone3 = phone.Substring(7, 3);
                if (phone.Length > 10)
                {
                    phone4 = phone.Substring(10);
                }
                mobileView = string.Format("{0} {1} {2} {3}", phone1, phone2, phone3, phone4);
            }
            return mobileView;
        }
        public static string GetUrl(string seoUrl) => "/" + seoUrl;
        public static bool IsAmp(string linkUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(linkUrl))
                {
                    linkUrl = linkUrl.Trim('/');
                    string[] links = linkUrl.Split('/');
                    if (links[0].ToUpper() == "AMP" || links[^1].ToUpper() == "AMP")
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public static bool IsNumber(string pText)
        {
            try
            {
                Regex regex = new(@"^[-+]?[0-9]*\.?[0-9]+$");
                return regex.IsMatch(pText);
            }
            catch
            {
                return false;
            }
        }
        public static bool IsArrIds(string pText)
        {
            try
            {
                Regex regex = new(@"^[0-9\,\/]+$");
                return regex.IsMatch(pText);
            }
            catch
            {
                return false;
            }
        }
        public static string ReplaceHttpToHttps(string url, bool enable) => enable ? url.Replace("http://", "https://") : url;

        public static string ReplaceAMP(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = RemoveImage(content);
                content = ReplaceWithLink("script", content);
                content = RemoveIframe(content);
                content = RemoveStyle(content);
            }
            return content;
        }

        public static string AddTail(string str, string tail)
        {
            try
            {
                int index = str.LastIndexOf('.');
                string url = str[..index];
                string tailName = str[^(str.Length - index)..];
                string link = url + tail + tailName;
                return link;
            }
            catch
            {
            }
            return str;
        }

        public static string FormatDate(DateTime? date)
        {
            try
            {
                if (date != null)
                {
                    return string.Format("{0:MMMM dd, yyyy}", date);
                }
            }
            catch { }
            return null;
        }

        public static string FormatDateTime(DateTime? date) => date?.ToString("dd/MM/yyyy HH:mm");
        public static string BytesToString(byte[] bytes)
        {
            using MemoryStream stream = new(bytes);
            using StreamReader streamReader = new(stream);
            return streamReader.ReadToEnd();
        }
    }
}
