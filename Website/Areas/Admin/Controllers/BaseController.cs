using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Website.Models;
using Website.Utils;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;

namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route(WebConfig.AdminAlias + "/[controller]/[action]")]
    [CheckLoginAdmin]
    public class BaseController : Controller
    {
        public SystemActionAdmin SystemActionAdmin;
        public SessionBase session;
        private readonly WebsiteModuleDa _websiteModuleDa = new(WebConfig.ConnectionString);
        private readonly ProductDa _productDa = new(WebConfig.ConnectionString);
        private readonly WebsiteContentDa _websiteContentDa = new(WebConfig.ConnectionString);
        private readonly DapperDA _dapperDA = new(WebConfig.ConnectionString);
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string conttoller = filterContext.RouteData.Values["controller"].ToString();
            string CtrCode = conttoller.ToLower();
            string ctrCodeCheck = "/" + WebConfig.AdminAlias + "/" + CtrCode;
            if (Request != null)
            {
                session = new SessionBase(HttpContext);
                string callingUrl = Request.Headers["Referer"].ToString();
                //var isLocal = Url.IsLocalUrl(callingUrl);
                //if (isLocal)
                //{
                //    if (string.IsNullOrEmpty(session.GetAdminUserId()))
                //    {
                //        session.SetAdminUserId("DB87C597-491B-45F0-80EA-08CEFA19D681");
                //    }
                //    if (string.IsNullOrEmpty(session.GetAdminRole()))
                //    {
                //        session.SetAdminRole("ALL");
                //    }

                //}

                if (!string.IsNullOrEmpty(session.GetAdminUserId()) && !string.IsNullOrEmpty(session.GetAdminRole()))
                {
                    if (session.GetAdminRole().ToUpper().Contains("ALL") || session.GetAdminRole().ToUpper().Contains("ADMIN"))
                    {
                        SystemActionAdmin = new SystemActionAdmin();
                        SystemActionAdmin.Add = true;
                        SystemActionAdmin.ViewFull = true;
                        SystemActionAdmin.View = true;
                        SystemActionAdmin.Edit = true;
                        SystemActionAdmin.Delete = true;
                        SystemActionAdmin.Order = true;
                        SystemActionAdmin.Active = true;
                        SystemActionAdmin.Public = true;
                        SystemActionAdmin.Sitemap = true;
                        SystemActionAdmin.IsAdmin = true;
                    }
                    else if (Request.Path.ToString().ToLower().Contains(ctrCodeCheck.ToLower()))
                    {
                        SystemActionAdmin = new SystemActionAdmin();
                        try
                        {
                            UserDa userDa = new UserDa(WebConfig.ConnectionString);
                            var userAdminLogin = userDa.GetMembership(session.GetAdminUserId());
                            if (userAdminLogin != null)
                            {
                                List<ModuleAdmin> listModuleItem = new();
                                try
                                {
                                    listModuleItem = JsonConvert.DeserializeObject<List<ModuleAdmin>>(userAdminLogin.DataJson);
                                    ModuleAdmin moduleItem = listModuleItem.Find(c => c.Tag != null && (c.Tag.ToLower() == CtrCode || c.Tag.ToLower().StartsWith(CtrCode)));
                                    if (moduleItem != null)
                                    {
                                        List<ActiveRoleAdmin> listActive = JsonConvert.DeserializeObject<List<ActiveRoleAdmin>>(moduleItem.DataJson);
                                        foreach (var item in listActive)
                                        {
                                            SystemActionAdmin.ViewFull = true;
                                            if (item.NameActive == "Add")
                                                SystemActionAdmin.Add = true;
                                            if (item.NameActive == "View")
                                                SystemActionAdmin.View = true;
                                            if (item.NameActive == "Edit")
                                                SystemActionAdmin.Edit = true;
                                            if (item.NameActive == "Delete")
                                                SystemActionAdmin.Delete = true;
                                            if (item.NameActive == "Active")
                                                SystemActionAdmin.Active = true;
                                            if (item.NameActive == "Order")
                                                SystemActionAdmin.Order = true;
                                            if (item.NameActive == "Public")
                                                SystemActionAdmin.Public = true;
                                            if (item.NameActive == "Sitemap")
                                                SystemActionAdmin.Sitemap = true;
                                        }
                                    }
                                }
                                catch
                                {
                                    HttpContext.Session.Clear();
                                    Redirect("/" + WebConfig.AdminAlias + "/HomeAdmin/Index");
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("/" + WebConfig.AdminAlias + "?returnUrl=" + Request.Host + Request.Path);
                }
            }
        }
        public List<int> ModuleID
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Form["ModuleIds"]))
                {
                    List<int> ltsTemp = new();
                    if (Request.Form["ModuleIds"].ToString().Contains(','))
                    {
                        string[] lst = Request.Form["ModuleIds"].ToString().Trim().Split(',');
                        foreach (string item in lst)
                        {
                            ltsTemp.Add(Convert.ToInt32(item));
                        }
                        return ltsTemp;
                    }
                    if (int.TryParse(Request.Form["ModuleIds"].ToString(), out int idTemp))
                    {
                        ltsTemp.Add(idTemp);
                    }
                    return ltsTemp;
                }
                return new List<int>();
            }
        }
        public List<int> ArrID
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Form["itemID"]))
                {
                    List<int> ltsTemp = new();
                    if (Request.Form["ItemID"].ToString().Contains(','))
                    {
                        string[] lst = Request.Form["ItemID"].ToString().Trim().Split(',');
                        foreach (string item in lst)
                        {
                            ltsTemp.Add(Convert.ToInt32(item));
                        }
                        return ltsTemp;
                    }
                    if (int.TryParse(Request.Form["ItemID"].ToString(), out int idTemp))
                    {
                        ltsTemp.Add(idTemp);
                    }
                    return ltsTemp;
                }
                return new List<int>();
            }
        }
        public List<string> ArrIDString
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Form["itemID"]))
                {
                    List<string> ltsTemp = new();
                    if (Request.Form["ItemID"].ToString().Contains(','))
                    {
                        var lst = Request.Form["ItemID"].ToString().Trim().Split(',');
                        foreach (string item in lst)
                        {
                            ltsTemp.Add(item);
                        }
                        return ltsTemp;
                    }
                    if (!string.IsNullOrEmpty(Request.Form["ItemID"].ToString()))
                    {
                        string idTemp = Request.Form["ItemID"].ToString();
                        ltsTemp.Add(idTemp);
                    }
                    return ltsTemp;
                }
                return new List<string>();
            }
        }
        public List<string> ArrKey
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Form["itemID"]))
                {
                    List<string> ltsTemp = new();
                    if (Request.Form["ItemID"].ToString().Contains(','))
                    {
                        string[] lst = Request.Form["ItemID"].ToString().Trim().Split(',');
                        foreach (string item in lst)
                        {
                            ltsTemp.Add(item);
                        }
                    }
                    else
                    {
                        ltsTemp.Add(Request.Form["itemID"]);
                    }
                    return ltsTemp;
                }
                return new List<string>();
            }
        }
        public string Lang() => Request.Cookies["lanad"] != null ? Utility.RemoveHTMLTag(Request.Cookies["lanad"]) : StaticEnum.DefaultLanguage;
        //public string Lang() => StaticEnum.DefaultLanguage;
        #region xử lý file
        public string ReadFile(string fileName, string path)
        {
            try
            {
                return System.IO.File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            { }
            return string.Empty;
        }
        public static void CreateAppendFile(string fileName, string content, string path)
        {
            try
            {
                string name = WebConfig.PathServer + path + "/" + fileName;
                FileInfo info = new(name);
                if (info.Exists)
                {
                    using (StreamWriter writer = info.AppendText())
                    {
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
                else
                {
                    using (StreamWriter writer = info.CreateText())
                    {
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
        #endregion
        #region addlogs
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
        public void AddLogEdit<T>(string url, string action, string id, T contentEdit)
        {
            try
            {
                session = new SessionBase(HttpContext);
                string userEdit = session.GetAdminUserName();
                string contentView = JsonConvert.SerializeObject(contentEdit);
                string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-url:" + url + "Action:" + action + ",ID:" + id + ",UserName:" + userEdit + "," + contentView;
                CreateAppendFile("LogAdminEdit" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", content, "Logs");
            }
            catch
            {
            }
        }

        public void AddLogAdmin(string url, string content, string action)
        {
            try
            {
                session = new SessionBase(HttpContext);
                string classControl = url;
                try
                {
                    classControl = url.Split('/')[2];
                }
                catch { }
                LogAdmin logAdmin = new()
                {
                    Action = action,
                    Url = url,
                    UserID = Guid.Parse(session.GetAdminUserId()),
                    DateCreated = DateTime.Now,
                    Content = content,
                    UserLogin = session.GetAdminUserName(),
                    ClassControl = classControl
                };
                DapperDA dapperDa = new DapperDA(WebConfig.ConnectionString);
                dapperDa.Insert(logAdmin);
            }
            catch (Exception ex)
            {
                AddLogError(ex);
            }
        }
        #endregion
        #region phân trang
        public string GetPage(int page, int totalRecord, int rowPerPage)
        {
            string html;
            if (totalRecord > rowPerPage)
            {
                int currentPage = page > 1 ? page : 1;
                html = "  <div class=\"total\"><span>Hiển thị " + Utility.FormatNumber(totalRecord < rowPerPage ? totalRecord : (page == 0 ? rowPerPage : (page * rowPerPage > totalRecord ? totalRecord : page * rowPerPage))) + " trên " + Utility.FormatNumber(totalRecord) + "</span>  </div>" +
                    "<div class=\"pagi\"><ul>" +
                    "<li>Trang [Current]/[Total]</li>" +
                    "<li class='[disabledPrevious]'><a href='[LinkDoublePrevious]' class='pagingData'>&laquo;</a></li>" +
                    "<li class='[disabledPrevious]'><a href='[LinkPrevious]' class='pagingData'>&lsaquo;</a></li>[LinkNumber]" +
                    "<li class='[disabledNext]'><a href='[LinkNext]' class='pagingData'>&rsaquo;</a></li>" +
                    "<li class='[disabledNext]'><a href='[LinkDoubleNext]' class='pagingData'>&raquo;</a></li>" +
                    "</ul></div>";
                html = html.Replace("[Current]", currentPage.ToString());
                int total = currentPage * rowPerPage;
                bool disabledNext = total >= totalRecord;
                bool disablePrevious = total <= rowPerPage;
                if (disablePrevious && disabledNext)
                {
                    html = html.Replace("[displayView]", "displayView hidden");
                }
                if (disablePrevious)
                {
                    html = html.Replace("[disabledPrevious]", "disabled hidden");
                    html = html.Replace("[LinkPrevious]", "javascript:");
                }
                if (disabledNext)
                {
                    html = html.Replace("[disabledNext]", "disabled hidden");
                    html = html.Replace("[LinkNext]", "javascript:");
                }
                int mode = totalRecord % rowPerPage;
                int doubleNext;
                if (mode == 0)
                {
                    doubleNext = totalRecord / rowPerPage;
                }
                else
                {
                    doubleNext = (totalRecord / rowPerPage) + 1;
                }
                html = html.Replace("[LinkPrevious]", "#page=" + (currentPage - 1));
                html = html.Replace("[LinkNext]", "#page=" + (currentPage + 1));
                html = html.Replace("[LinkDoublePrevious]", "#page=1");
                html = html.Replace("[LinkDoubleNext]", "#page=" + doubleNext);
                string link = string.Empty;
                int begin = 1;
                int end = 9;
                if (doubleNext < end)
                {
                    end = doubleNext;
                }
                else
                {
                    if (currentPage >= end)
                    {
                        begin = currentPage - (int)Math.Round((double)end / 2, 0);
                        end = currentPage + (int)Math.Round((double)end / 2, 0);
                    }
                    if (end >= doubleNext)
                    {
                        begin = doubleNext - (9 - 1);
                        end = doubleNext;

                    }
                }
                for (int i = begin; i <= end; i++)
                {
                    link += " <li " + (i == currentPage ? "class=\"active\"" : "") + "><a class=\"pagingData\" href=\"#page=" + i + "\">" + i + "</a></li>";
                }
                html = html.Replace("[Total]", end.ToString());
                html = html.Replace("[LinkNumber]", link);
            }
            else
            {
                html = "<span>Hiển thị " + Utility.FormatNumber(totalRecord < 50 ? totalRecord : (page == 0 ? 50 : page * 50)) + " trên " + Utility.FormatNumber(totalRecord) + "</span>";
            }
            return html;

        }
        public string GetPageAjax(int page, int totalRecord, int rowPerPage)
        {
            string html;
            if (totalRecord > rowPerPage)
            {
                int currentPage = page > 1 ? page : 1;
                html = "  <div class=\"total\"><span>Hiển thị " + Utility.FormatNumber(totalRecord < rowPerPage ? totalRecord : (page == 0 ? rowPerPage : (page * rowPerPage > totalRecord ? totalRecord : page * rowPerPage))) + " trên " + Utility.FormatNumber(totalRecord) + "</span>  </div>" +
                    "<div class=\"pagi\"><ul>" +
                    "<li>Trang [Current]/[Total]</li>" +
                    "<li class='[disabledPrevious]'><a data-page='[LinkDoublePrevious]' class='pagingData'>&laquo;</a></li>" +
                    "<li class='[disabledPrevious]'><a data-page='[LinkPrevious]' class='pagingData'>&lsaquo;</a></li>[LinkNumber]" +
                    "<li class='[disabledNext]'><a data-page='[LinkNext]' class='pagingData'>&rsaquo;</a></li>" +
                    "<li class='[disabledNext]'><a data-page='[LinkDoubleNext]' class='pagingData'>&raquo;</a></li>" +
                    "</ul></div>";
                html = html.Replace("[Current]", currentPage.ToString());
                int total = currentPage * rowPerPage;
                bool disabledNext = total >= totalRecord;
                bool disablePrevious = total <= rowPerPage;
                if (disablePrevious && disabledNext)
                {
                    html = html.Replace("[displayView]", "displayView hidden");
                }
                if (disablePrevious)
                {
                    html = html.Replace("[disabledPrevious]", "disabled hidden");
                    html = html.Replace("[LinkPrevious]", "javascript:");
                }
                if (disabledNext)
                {
                    html = html.Replace("[disabledNext]", "disabled hidden");
                    html = html.Replace("[LinkNext]", "javascript:");
                }
                int mode = totalRecord % rowPerPage;
                int doubleNext;
                if (mode == 0)
                {
                    doubleNext = totalRecord / rowPerPage;
                }
                else
                {
                    doubleNext = (totalRecord / rowPerPage) + 1;
                }
                html = html.Replace("[LinkPrevious]", (currentPage - 1).ToString());
                html = html.Replace("[LinkNext]", (currentPage + 1).ToString());
                html = html.Replace("[LinkDoublePrevious]", "1");
                html = html.Replace("[LinkDoubleNext]", doubleNext.ToString());
                string link = string.Empty;
                int begin = 1;
                int end = 9;
                if (doubleNext < end)
                {
                    end = doubleNext;
                }
                else
                {
                    if (currentPage >= end)
                    {
                        begin = currentPage - (int)Math.Round((double)end / 2, 0);
                        end = currentPage + (int)Math.Round((double)end / 2, 0);
                    }
                    if (end >= doubleNext)
                    {
                        begin = doubleNext - (9 - 1);
                        end = doubleNext;

                    }
                }
                for (int i = begin; i <= end; i++)
                {
                    link += " <li " + (i == currentPage ? "class=\"active\"" : "") + "><a class=\"pagingData\" data-page=\"" + i + "\">" + i + "</a></li>";
                }
                html = html.Replace("[Total]", end.ToString());
                html = html.Replace("[LinkNumber]", link);
            }
            else
            {
                html = "<span>Hiển thị " + Utility.FormatNumber(totalRecord < 50 ? totalRecord : (page == 0 ? 50 : page * 50)) + " trên " + Utility.FormatNumber(totalRecord) + "</span>";
            }
            return html;

        }
        public string GetTotal(int page, int total) => "<span>Hiển thị " + Utility.FormatNumber(total < 50 ? total : (page == 0 ? 50 : page * 50)) + " trên " + Utility.FormatNumber(total) + "</span>";
        #endregion
        #region UpdateListModule
        public List<T> UpdateModelLst<T>(T obj, List<T> objList, int length = 0, int i = 0) where T : new()
        {
            try
            {
                obj = new T();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    try
                    {
                        Type type = item.PropertyType;
                        type = Nullable.GetUnderlyingType(type) ?? type;
                        if (type != typeof(string) && type != typeof(int) && type != typeof(bool) && type != typeof(DateTime) && type != typeof(double) && type != typeof(decimal) && type != typeof(Nullable))
                            continue;
                        var val = Request.Form[item.Name];
                        if (!string.IsNullOrEmpty(val))
                        {
                            var valReplace = string.Empty;
                            int a = 1;
                            foreach (var it in val)
                            {
                                if (!string.IsNullOrEmpty(it))
                                {
                                    var rs = it.Replace(',', '|');
                                    if (a == 1)
                                        valReplace += rs;
                                    else
                                        valReplace += "," + rs;
                                }
                                else
                                {
                                    if (a == 1)
                                        valReplace += " ";
                                    else
                                        valReplace += ", ";
                                }
                                a++;
                            }
                            List<string> vals = Utility.StringToListString(valReplace);
                            length = vals.Count;
                            if (type == typeof(double))
                            {
                                item.SetValue(obj, ConvertUtil.ToDouble(vals[i]));
                            }
                            else if (type == typeof(int))
                            {
                                item.SetValue(obj, ConvertUtil.ToInt32(vals[i]));
                            }
                            else if (type == typeof(bool))
                            {
                                item.SetValue(obj, ConvertUtil.ToBool(vals[i]));
                            }
                            else if (type == typeof(DateTime))
                            {
                                item.SetValue(obj, ConvertUtil.ToDateTime(vals[i]));
                            }
                            else if (type == typeof(decimal))
                            {
                                item.SetValue(obj, ConvertUtil.ToDecimal(vals[i]));
                            }
                            else
                            {
                                item.SetValue(obj, vals[i].Replace("|", ","));
                            }

                        }
                    }
                    catch
                    {
                    }
                }

                if (i < length)
                {
                    objList.Add(obj);
                    i++;
                    return UpdateModelLst(obj, objList, length, i);
                }
            }
            catch
            {

            }
            return objList;

        }
        #endregion
        public ActionViewModel UpdateModelAction()
        {
            ActionViewModel action = new ActionViewModel();
            TryUpdateModelAsync(action);
            try
            {
                if (string.IsNullOrEmpty(action.Do) && Request != null && Request.Form != null)
                {
                    action.Do = Request.Form["do"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.Do) && Request.Query != null)
                {
                    action.Do = Request.Query["do"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.ItemId) && Request != null && Request.Form != null)
                {
                    action.ItemId = Request.Form["ItemId"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.Keys) && Request != null && Request.Form != null)
                {
                    action.Keys = Request.Form["Keys"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.Do) && Request.Query != null)
                {
                    action.ParentId = Request.Query["parentId"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.Do) && Request.Query != null)
                {
                    action.ModuleId = Request.Query["ModuleId"];
                }
            }
            catch { }
            try
            {
                if (string.IsNullOrEmpty(action.Do) && Request.Query != null)
                {
                    action.Status = Request.Query["Status"];
                }
            }
            catch { }
            return action;
        }
        #region sitemap        
        public async Task UpdateSitemapProduct(Product obj, Product oldObj)
        {
            string newUrl;
            string oldUrl;
            try
            {
                List<SitemapJson> listSitemap = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                SitemapJson sitemapparent = listSitemap.FirstOrDefault(x => x.Code == "sitemap-product");
                if (sitemapparent == null)
                {
                    sitemapparent = new SitemapJson
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/sitemap-product.xml",
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = 0,
                        Code = "sitemap-product"
                    };
                    listSitemap.Add(sitemapparent);
                }
                if (obj != null && oldObj == null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    if (listSitemap.Any(x => x.Url == newUrl))
                    {
                        listSitemap.RemoveAll(x => x.Url == newUrl);
                        listSitemap.Add(new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii,
                            ModuleIds = obj.ModuleIds
                        });
                    }
                }
                if (obj != null && oldObj != null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    var sitemap = listSitemap.FirstOrDefault(x => x.Url == oldUrl);
                    if (sitemap != null)
                    {
                        listSitemap.Remove(sitemap);
                        sitemap.ParentID = sitemapparent.ID;
                        sitemap.Url = newUrl;
                        sitemap.Code = obj.NameAscii;
                        sitemap.LastModified = DateTime.Now;
                        sitemap.ModuleIds = obj.ModuleIds;
                    }
                    else
                    {
                        sitemap = new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii,
                            ModuleIds = obj.ModuleIds
                        };
                    }
                    listSitemap.Add(sitemap);
                }
                if (obj == null && oldObj != null)
                {
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    listSitemap.RemoveAll(x => x.Url == oldUrl);
                }
                await Common.CreateFileJsonAsync(0, listSitemap, "Sitemap", "DataJson");
            }
            catch (Exception e)
            {
                Common.CreateAppendFile("LogClientEdit_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", e.Message, "Logs"); ;
                throw;
            }
        }
        public async Task UpdateSitemapNews(WebsiteContent obj, WebsiteContent oldObj)
        {
            string newUrl;
            string oldUrl;
            string oldCode;
            try
            {
                List<SitemapJson> listSitemap = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                SitemapJson sitemapparent = listSitemap.FirstOrDefault(x => x.Code == "sitemap-news");
                if (sitemapparent == null)
                {
                    sitemapparent = new SitemapJson
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/sitemap-news.xml",
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = 0,
                        Code = "sitemap-news"
                    };
                    listSitemap.Add(sitemapparent);
                }
                if (obj != null && oldObj == null)
                {
                    newUrl = WebConfig.Website + "/" + obj.ModuleNameAscii + "/" + obj.NameAscii;
                    if (listSitemap.Any(x => x.Url == newUrl))
                    {
                        listSitemap.RemoveAll(x => x.Url == newUrl);
                        listSitemap.Add(new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii,
                            ModuleIds = obj.ModuleIds
                        });
                    }
                }
                if (obj != null && oldObj != null)
                {
                    newUrl = WebConfig.Website + "/" + obj.ModuleNameAscii + "/" + obj.NameAscii;
                    oldUrl = WebConfig.Website + "/" + oldObj.ModuleNameAscii + "/" + oldObj.NameAscii;
                    SitemapJson sitemap = listSitemap.FirstOrDefault(x => x.Url == oldUrl);
                    if (sitemap != null)
                    {
                        listSitemap.Remove(sitemap);
                        sitemap.ParentID = sitemapparent.ID;
                        sitemap.Url = newUrl;
                        sitemap.LastModified = DateTime.Now;
                        sitemap.Code = obj.NameAscii;
                        sitemap.ModuleIds = obj.ModuleIds;
                    }
                    else
                    {
                        sitemap = new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii,
                            ModuleIds = obj.ModuleIds
                        };
                    }
                    listSitemap.Add(sitemap);
                }
                if (obj == null && oldObj != null)
                {
                    oldUrl = WebConfig.Website + "/" + oldObj.ModuleNameAscii + "/" + oldObj.NameAscii;
                    listSitemap.RemoveAll(x => x.Url == oldUrl);
                }
                if (oldObj != null && oldObj.ModifiedDate.HasValue)
                {
                    oldCode = "sitemap-news-" + oldObj.ModifiedDate.Value.Year + "-" + oldObj.ModifiedDate.Value.Month;
                    SitemapJson sitemapold = listSitemap.FirstOrDefault(x => x.Code == oldCode);
                    if (sitemapold != null && listSitemap.Count(x => x.ParentID == sitemapold.ID) == 0)
                    {
                        listSitemap.Remove(sitemapold);
                    }
                }
                await Common.CreateFileJsonAsync(0, listSitemap, "Sitemap", "DataJson");
            }
            catch (Exception e)
            {
                AddLogAdmin("", e.Message, "Actions-Ẩn");
            }
        }
        public async Task UpdateSitemapCategory(WebsiteModule obj, WebsiteModule oldObj)
        {
            string newUrl;
            string oldUrl;
            try
            {
                List<SitemapJson> listSitemap = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                SitemapJson sitemapparent = listSitemap.FirstOrDefault(x => x.Code == "sitemap-category");
                if (sitemapparent == null)
                {
                    sitemapparent = new SitemapJson
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/sitemap-category.xml",
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = 0,
                        Code = "sitemap-category"
                    };
                    listSitemap.Add(sitemapparent);
                }
                if (obj != null && oldObj != null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    SitemapJson sitemap = listSitemap.FirstOrDefault(x => x.Url == oldUrl);
                    if (sitemap != null)
                    {
                        listSitemap.Remove(sitemap);
                        sitemap.ParentID = sitemapparent.ID;
                        sitemap.Url = newUrl;
                        sitemap.LastModified = DateTime.Now;
                        sitemap.Code = obj.NameAscii;
                    }
                    else
                    {
                        sitemap = new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii
                        };
                    }
                    listSitemap.Add(sitemap);
                }
                if (obj != null && oldObj == null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    if (listSitemap.Any(x => x.Url == newUrl))
                    {
                        listSitemap.RemoveAll(x => x.Url == newUrl);
                        listSitemap.Add(new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapparent.ID,
                            Code = obj.NameAscii
                        });
                    }
                }
                if (obj == null && oldObj != null)
                {
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    listSitemap.RemoveAll(x => x.Url == oldUrl);
                }
                await Common.CreateFileJsonAsync(0, listSitemap, "Sitemap", "DataJson");
            }
            catch (Exception e)
            {
                Console.Write(value: e.Message);
                throw;
            }
        }

        public async Task UpdateSitemapBrand(WebsiteModule obj, WebsiteModule oldObj)
        {
            string newUrl;
            string oldUrl;
            try
            {
                List<SitemapJson> listSitemap = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                SitemapJson sitemapbrand = listSitemap.FirstOrDefault(x => x.Code == "sitemap-brand");
                if (sitemapbrand == null)
                {
                    sitemapbrand = new SitemapJson
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/sitemap-brand.xml",
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = 0,
                        Code = "sitemap-brand"
                    };
                    listSitemap.Add(sitemapbrand);
                }
                if (obj != null && oldObj != null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    SitemapJson sitemap = listSitemap.FirstOrDefault(x => x.Url == oldUrl);
                    if (sitemap != null)
                    {
                        listSitemap.Remove(sitemap);
                        sitemap.ParentID = sitemapbrand.ID;
                        sitemap.Url = newUrl;
                        sitemap.LastModified = DateTime.Now;
                        sitemap.Code = obj.NameAscii;
                    }
                    else
                    {
                        sitemap = new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapbrand.ID,
                            Code = obj.NameAscii
                        };
                    }
                    listSitemap.Add(sitemap);
                }
                if (obj != null && oldObj == null)
                {
                    newUrl = WebConfig.Website + "/" + obj.NameAscii;
                    if (listSitemap.Any(x => x.Url == newUrl))
                    {
                        listSitemap.RemoveAll(x => x.Url == newUrl);
                        listSitemap.Add(new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = newUrl,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = sitemapbrand.ID,
                            Code = obj.NameAscii
                        });
                    }
                }
                if (obj == null && oldObj != null)
                {
                    oldUrl = WebConfig.Website + "/" + oldObj.NameAscii;
                    listSitemap.RemoveAll(x => x.Url == oldUrl);
                }
                await Common.CreateFileJsonAsync(0, listSitemap, "Sitemap", "DataJson");
            }
            catch
            {
                throw;
            }
        }
        #endregion
        public async Task UpdateTotalOrderProduct(int id)
        {
            try
            {
                using (SqlConnection connect = _dapperDA.GetOpenConnection())
                {
                    var result = await connect.ExecuteAsync("update Product set TotalOrder = (select SUM(od.Quantity) from OrderDetail od inner join [Order] o on o.ID = od.OrderID where od.ProductID = @id and o.Status = 5 and o.IsPayment = 1) where ID in (@id)", new { id });
                    connect.Close();
                }
                //DapperDA _dapperDa = new DapperDA(WebConfig.ConnectionString);
                //string sql = string.Format("update Product set TotalOrder = (" +
                //    "select SUM(od.Quantity) from OrderDetail od inner join [Order] o on o.ID = od.OrderID " +
                //    "where od.ProductID = {0} and o.Status = 5 and o.IsPayment = 1) " +
                //    "where ID in({0})", id);
                //await _dapperDa.ExecuteSqlAsync(sql);
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> CountProductByModule(WebsiteModule obj)
        {
            try
            {
                List<WebsiteModuleAdmin> child = _websiteModuleDa.GetListChidrent(obj.ID);
                int products = _productDa.CountProductByModuleIds(string.Join(",", child.Select(x => x.ID.ToString())), Lang());
                obj.TotalProducts = products;
                int contents = _websiteContentDa.CountContentyModuleIds(string.Join(",", child.Select(x => x.ID.ToString())), Lang());
                obj.TotalContents = contents;
                await _websiteModuleDa.UpdateAsync(obj);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public const string Title = "title";
        public const string Code = "code";
        public const string ArrayInt = "arrayint";
        public const string ArrayCode = "arraycode";
        public const string Link = "link";
    }
}
