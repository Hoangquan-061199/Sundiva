using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.Simple;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Models;
using Website.Utils;
using Website.ViewModels;

namespace Website.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DapperDA _dapperDa;
        private readonly ContactUsManager _contactUsManager;
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        private readonly WebsiteContentManager _webContentManager;
        private readonly WebsiteModuleManager _websiteModuleManager;
        private readonly ProductManager _productManager;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly GoogleCapthaService _captchaService;

        public HomeController(IDistributedCache distributedCache, GoogleCapthaService service)
        {
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _contactUsManager = new ContactUsManager(WebConfig.ConnectionString);
            _webContentManager = new WebsiteContentManager(WebConfig.ConnectionString);
            _websiteModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _captchaService = service;
        }

        public IActionResult Index(string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                List<string> listculture = new() { Resources.VI, Resources.EN, Resources.JP };
                if (listculture.Contains(culture.Trim()))
                {
                    SetCookies("lang", culture.Trim(), 10);
                    Response.Redirect("/");
                }
            }
            string view = StaticEnum.LayoutHome;
            List<ModulePositionItem> listPosition = new();
            IEnumerable<WebsiteContentItem> listContent = new List<WebsiteContentItem>();
            IEnumerable<ProductItem> listProduct = new List<ProductItem>();
            SearchModel search = new()
            {
                lang = Lang()
            };
            #region
            try
            {
                listPosition = cacheUtils.GetListPositionViewIndex(view);
                StringBuilder sqlWhere = new();
                StringBuilder sqlContent = new();
                List<string> listModuleByContent = new();
                if (listPosition.Any())
                {
                    listPosition.ForEach(x =>
                    {
                        x.AdvertisingItems = cacheUtils.GetListAdvertisingItemInPositionIds(x.ID.ToString(), Lang(), x.NumberCount ?? 0);
                        x.WebsiteModulesItems = cacheUtils.GetListModuleInPositionIds(x.ID.ToString(), Lang(), x.NumberCount ?? 0);
                        if (x.TypeView == StaticEnum.Content)
                        {
                            sqlWhere.Clear();
                            sqlContent.Clear();
                            listModuleByContent = x.WebsiteModulesItems.Select(c => c.ID.ToString()).ToList();
                            if (listModuleByContent.Count > 0)
                            {
                                sqlWhere.Append(SqlUtility.WhereOrLikeList(listModuleByContent, "ModuleIds"));
                            }
                            else
                            {
                                sqlWhere.Append(SqlUtility.AND("ModuleIds", 0, 1));
                            }
                            sqlWhere.Append(x.SqlContent);
                            sqlContent.Append(string.Format(SqlContent.SqlView, x.NumberContent ?? 20, "," + x.Code + ",", Lang(), sqlWhere, !string.IsNullOrEmpty(x.SqlContentOrderBy) ? x.SqlContentOrderBy : " Order By CreatedDate Desc"));
                            string sql1 = string.Format(SqlCommon.SqlHome, sqlContent);
                            listContent = _dapperDa.Select<WebsiteContentItem>(sql1);
                            x.WebsiteContentItems = listContent?.ToList();
                        }
                        if (x.TypeView == StaticEnum.Product)
                        {
                            sqlWhere.Clear();
                            sqlContent.Clear();
                            listModuleByContent = x.WebsiteModulesItems.Select(c => c.ID.ToString()).ToList();
                            if (listModuleByContent.Count > 0)
                            {
                                sqlWhere.Append(SqlUtility.WhereOrLikeList(listModuleByContent, "ModuleIds"));
                            }
                            else
                            {
                                sqlWhere.Append(SqlUtility.AND("ModuleIds", 0, 1));
                            }
                            sqlWhere.Append(x.SqlContent);
                            sqlContent.Append(string.Format(SqlProduct.SqlView, x.NumberContent ?? 20, "," + x.Code + ",", Lang(), sqlWhere, " Order By OrderDisplay Asc"));
                            string sql1 = string.Format(SqlCommon.SqlHome, sqlContent);
                            listProduct = _dapperDa.Select<ProductItem>(sql1);
                            x.ProductItems = listProduct?.ToList();
                            List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
                            List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
                            x.ProductItems.ForEach(x =>
                            {
                                if (!string.IsNullOrEmpty(x.Times))
                                    x.TimesValue = listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(x.Times)).Name;
                                if(!string.IsNullOrEmpty(x.AddressId))
                                    x.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(x.AddressId)).Name;
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            #endregion
            IndexViewModel model = new()
            {
                ListPositionItem = listPosition,
                //ListContentItem = listContent?.ToList(),
                SystemConfigItem = cacheUtils.SystemConfigItem(search.lang)
            };
            //if (Utility.IsMobile(HttpContext.Request.Headers["User-Agent"]))
            //{
            //    //return View(@"~/Views/Mobile/Mobile.cshtml", model);
            //}
            return View(model);
        }

        #region Đăng ký nhận bản tin

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ReceiverEmail()
        {
            JsonMessage msg = new();
            try
            {
                var email = Request.Form["Email"];
                List<ReceiverEmailItem> listReceiverEmail = new();
                listReceiverEmail = JsonConvert.DeserializeObject<List<ReceiverEmailItem>>(Common.ReadFile(StaticEnum.ReceiverEmail + ".json", "DataJson"));
                if (listReceiverEmail == null)
                {
                    listReceiverEmail = new List<ReceiverEmailItem>();
                }
                ReceiverEmailItem receiver = new()
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")),
                    Email = email,
                    CreatedDate = DateTime.Now
                };
                ReceiverEmailItem checkEmail = listReceiverEmail.FirstOrDefault(c => c.Email == Email);
                if (checkEmail == null)
                {
                    listReceiverEmail.Add(receiver);
                    Common.CreateFileJson(0, listReceiverEmail, StaticEnum.ReceiverEmail, "DataJson");
                    msg = new JsonMessage
                    {
                        Errors = false,
                        Message = ResourceData.Resource("GuiThanhCong", Lang())
                    };
                }
                else
                {
                    msg = new JsonMessage
                    {
                        Errors = true,
                        Message = ResourceData.Resource("BanDaDangKySoDienThoai", Lang()) + checkEmail.Email
                    };
                }
            }
            catch (Exception e)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = ResourceData.Resource("GuiThatBai", Lang()),
                    Logs = e.Message
                };
            }
            return Ok(msg);
        }

        #endregion

        #region liên hệ đặt hàng
        [HttpPost]
        [Route("/lien-he-dat-hang")]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendContactOrder()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Number) || string.IsNullOrEmpty(model.Email))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Content) || Utility.CheckBlackList(model.Email) || Utility.CheckBlackList(model.Number))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateContactOrder") ? list.FirstOrDefault(x => x.Code == "TemplateContactOrder") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Number = Utility.RemoveHTMLTag(model.Number);
                model.Content = Utility.RemoveHTMLTag(model.Content);
                #endregion
                var product = _productManager.GetById(model.ProductID);

                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[SoLuong]", model.Number);
                str = str.Replace("[NoiDung]", model.Content);
                str = str.Replace("[SanPham]", WebConfig.Website + Utility.Link(product.NameAscii, string.Empty, product.LinkUrl));
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs contact = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Email = model.Email,
                        Number = model.Number,
                        Content = model.Content,
                        ProductID = product.ID,
                        ProductLink = Utility.Link(product.NameAscii, string.Empty, product.LinkUrl),
                        ProductName = product.Name,
                        Code = "ContactOrder"
                    };
                    int result = _contactUsManager.Insert(contact);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        #endregion

        #region Liên hệ

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendContact()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateContact") ? list.FirstOrDefault(x => x.Code == "TemplateContact") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Content = Utility.RemoveHTMLTag(model.Content);
                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[NoiDung]", model.Content);
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs contact = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Email = model.Email,
                        Content = model.Content,
                        Code = "Contact"
                    };
                    int result = _contactUsManager.Insert(contact);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> CarRental()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email) || !model.StartDate.HasValue || !model.EndDate.HasValue || string.IsNullOrEmpty(model.Destination) || string.IsNullOrEmpty(model.PointGo))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateCarRental") ? list.FirstOrDefault(x => x.Code == "TemplateCarRental") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                string servicejson = await ReadFileAsync("ServiceCarJson.json", "DataJson");
                IEnumerable<CommonJsonItem> listService = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(servicejson);
                if (listService != null)
                {
                    foreach (var item in listService.Where(x => x.ID == Convert.ToInt32(model.Service)))
                    {
                        model.ServicesName = item.Name;
                    }
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Content = Utility.RemoveHTMLTag(model.Content);
                model.StartDate = model.StartDate;
                model.EndDate = model.EndDate;
                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[StartDate]", model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                str = str.Replace("[EndDate]", model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                str = str.Replace("[DiemDen]", model.Destination);
                str = str.Replace("[DiemDi]", model.PointGo);
                str = str.Replace("[DichVu]", model.ServicesName);
                str = str.Replace("[NoiDung]", model.Content);
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs contact = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Email = model.Email,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Destination = model.Destination,
                        PointGo = model.PointGo,
                        Service = model.ServicesName,
                        Content = model.Content,
                        Code = "CarRental"
                    };
                    int result = _contactUsManager.Insert(contact);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> BookHotel()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateBookHotel") ? list.FirstOrDefault(x => x.Code == "TemplateBookHotel") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Address = Utility.RemoveHTMLTag(model.Address);
                model.Content = Utility.RemoveHTMLTag(model.Content);
                model.ProductID = model.ProductID;
                var product = _productManager.GetId(model.ProductID);
                model.ProductName = product.Name;
                model.ProductLink = Utility.Link(product._NameAscii, string.Empty, product.LinkUrl);
                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[DiaChi]", model.Address);
                str = str.Replace("[NoiDung]", model.Content);
                str = str.Replace("[KhachSan]", Utility.ReplaceHttpToHttps(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + model.ProductLink, WebConfig.EnableHttps));
                str = str.Replace("[TenKhachSan]", model.ProductName);

                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs contact = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Address = model.Address,
                        Email = model.Email,
                        Content = model.Content,
                        ProductName = model.ProductName,
                        ProductLink = model.ProductLink,
                        Code = "BookHotel"
                    };
                    int result = _contactUsManager.Insert(contact);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendOrder()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Number) || string.IsNullOrEmpty(model.Address))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateOrder") ? list.FirstOrDefault(x => x.Code == "TemplateOrder") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Address = Utility.RemoveHTMLTag(model.Address);
                model.Number = model.Number;
                model.Content = Utility.RemoveHTMLTag(model.Content);
                model.ProductID = model.ProductID;
                var product = _productManager.GetId(model.ProductID);
                model.ProductName = product.Name;
                model.ProductLink = Utility.Link(product._NameAscii, string.Empty, product.LinkUrl);
                var priceres = Request.Form["Price"];
                var typeAttr = Request.Form["TypeAttr"];
                model.Price = Utility.GetFormatPriceType(Convert.ToDecimal(priceres), 1,ResourceData.Resource("LienHe", ViewBag.Lang), true);
                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[DiaChi]", model.Address);
                str = str.Replace("[Number]", model.Number.ToString());
                str = str.Replace("[NoiDung]", model.Content);
                str = str.Replace("[SanphamLink]", Utility.ReplaceHttpToHttps(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + model.ProductLink, WebConfig.EnableHttps));
                str = str.Replace("[Gia]", model.Price);
                str = str.Replace("[Loai]", typeAttr);
                str = str.Replace("[TenSanPham]", model.ProductName);
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs contact = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Email = model.Email,
                        Address = model.Address,
                        Number = model.Number,
                        ProductName = model.ProductName,
                        ProductLink = model.ProductLink,
                        Content = model.Content,
                        Price = model.Price,
                        Title = typeAttr,
                        Code = "OrderProduct"
                    };
                    int result = _contactUsManager.Insert(contact);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendTuVan()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Address))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                var captchaResult = await _captchaService.VerifyToken(model.Token);
                if (!captchaResult)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateTuVan") ? list.FirstOrDefault(x => x.Code == "TemplateTuVan") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Address = Utility.RemoveHTMLTag(model.Address);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Content = Utility.RemoveHTMLTag(model.Content);

                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[Address]", model.Address);
                str = str.Replace("[NoiDung]", model.Content);
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs tuvan = new()
                    {
                        FullName = model.FullName,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        Content = model.Content,
                        Code = "TuVan"
                    };
                    int result = _contactUsManager.Insert(tuvan);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("MessageRegister", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendBaoGia()
        {
            JsonMessage msg = new();
            try
            {
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Email) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                CommonJsonItem mainTemplate = new();
                if (list == null)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                mainTemplate = list.Any(x => x.Code == "TemplateBaoGia") ? list.FirstOrDefault(x => x.Code == "TemplateBaoGia") : new CommonJsonItem();
                if (string.IsNullOrEmpty(mainTemplate.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    return Ok(msg);
                }
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Content = Utility.RemoveHTMLTag(model.Content);

                #endregion
                str = str.Replace("[Main]", mainTemplate.Content);
                str = str.Replace("[HoTen]", model.FullName);
                str = str.Replace("[SoDienThoai]", model.Phone);
                str = str.Replace("[Email]", model.Email);
                str = str.Replace("[NoiDung]", model.Content);
                if (model.TypeCode == StaticEnum.Product)
                {
                    var product = _productManager.GetById(model.ProductID);
                    str = str.Replace("[SanPham]", product.Name);
                    str = str.Replace("[LinkSanPham]", HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Utility.Link(product.NameAscii, string.Empty, product.LinkUrl));
                }
                else
                {
                    var content = _webContentManager.GetContentById(model.ContentID);
                    str = str.Replace("[SanPham]", content.Name);
                    str = str.Replace("[LinkSanPham]", HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + Utility.Link(content.NameAscii, string.Empty, content.LinkUrl));
                }
                #region Send mail
                try
                {
                    SendEmailModels objEmail = new()
                    {
                        Subject = mainTemplate.Name,
                        DisName = systemConfig.Name,
                        EmailBody = str,
                        EmailSend = systemConfig.EmailCms,
                        Password = systemConfig.PassEmailCms,
                        Port = Convert.ToInt32(systemConfig.Port),
                        Servername = systemConfig.Severname,
                        EnableSSL = systemConfig.SSLEmail ?? false,
                        To = systemConfig.Email
                    };
                    string sent = SendEmailModels.SendmailKitTest(objEmail);
                    if (!string.IsNullOrEmpty(sent))
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = sent };
                        return Ok(msg);
                    }
                    ContactUs baogia = new()
                    {
                        FullName = model.FullName,
                        Phone = model.Phone,
                        Email = model.Email,
                        Content = model.Content,
                        TypeCode = model.TypeCode,
                        Code = "BaoGia"
                    };
                    if (model.TypeCode == StaticEnum.Product)
                    {
                        baogia.ProductID = model.ProductID;
                    }
                    else
                    {
                        baogia.ContentID = model.ContentID;
                    }
                    int result = _contactUsManager.Insert(baogia);
                    if (result > 0)
                    {
                        msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                    }
                }
                catch (Exception e)
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                }
                #endregion
            }
            catch (Exception e)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = ResourceData.Resource("GuiThatBai", Lang()),
                    Logs = e.Message
                };
            }
            return Ok(msg);
        }

        #endregion
        #region Apply

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendApply()
        {
            JsonMessage msg = new();
            try
            {
                string cv = string.Empty;
                string fullcv = string.Empty;
                SendContactModels model = new();
                await TryUpdateModelAsync(model);
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Email))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                #region Captcha
                //var captchaResult = await _captchaService.VerifyToken(model.Token);
                //if (!captchaResult)
                //{
                //    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                //    return Ok(msg);
                //}
                if (string.IsNullOrEmpty(model.Token))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "202" };
                    return Ok(msg);
                }
                #endregion
                #region check blacklist
                if (Utility.CheckBlackList(model.FullName) || Utility.CheckBlackList(model.Email) || Utility.CheckBlackList(model.Content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "120" };
                    return Ok(msg);
                }
                #endregion
                #region Valid Input
                model.FullName = Utility.ValidString(model.FullName, string.Empty, true);
                model.Phone = Utility.RemoveHTMLTag(model.Phone);
                model.Email = Utility.RemoveHTMLTag(model.Email);
                model.Content = Utility.RemoveHTMLTag(model.Content);
                #endregion
                var content = _webContentManager.GetContentById(model.ContentID);
                if (content != null)
                {
                    #region CV
                    var FileCV = Request.Form.Files["CVFile"];
                    if (FileCV != null && !string.IsNullOrEmpty(FileCV.ToString()))
                    {
                        string processPath = "";
                        string extention = Path.GetExtension(FileCV.FileName);
                        if (FileCV.Length >= 2097152)
                        {
                            msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("Hotrodinhdangdocdocxpdfvakhongqua2MB", Lang()), Logs = "line 428" };
                        }
                        if (extention.ToLower() == ".pdf" || extention.ToLower() == ".doc" || extention.ToLower() == ".docx")
                        {
                            processPath = Url.Content("apply/") + Path.GetFileNameWithoutExtension(FileCV.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(FileCV.FileName);
                            string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                            FileStream stream = new(filePath, FileMode.Create);
                            FileCV.CopyTo(stream);
                            stream.Close();
                            cv = "/" + processPath;
                            fullcv = filePath;
                        }
                        else
                        {
                            return Ok(new JsonMessage { Errors = true, Message = ResourceData.Resource("LoiFile", Lang()) });
                        }
                    }
                    else
                    {
                        return Ok(new JsonMessage { Errors = true, Message = ResourceData.Resource("BanChuaNhapCV", Lang()), Logs = "line 442" });
                    }
                    #endregion
                    SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
                    List<CommonJsonItem> list = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TemplateEmail.json", "DataJson"));
                    if (list == null)
                    {
                        return Ok(new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "line 413" });
                    }
                    #region Send mail
                    try
                    {
                        List<string> file = new();
                        file.Add(fullcv);
                        //send mail to customer
                        CommonJsonItem mainTemplate = list.Any(x => x.Code == "Apply") ? list.FirstOrDefault(x => x.Code == "Apply") : new CommonJsonItem();
                        if (string.IsNullOrEmpty(mainTemplate.Content))
                        {
                            msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "line 419" };
                            return Ok(msg);
                        }
                        string str = Common.ReadFile("LayoutEmail.htm", "html/Configuaration");
                        str = str.Replace("[Main]", mainTemplate.Content);
                        str = str.Replace("[TenJob]", content.Name);
                        str = str.Replace("[HoTen]", model.FullName);
                        str = str.Replace("[NoiDung]", model.Content);
                        str = str.Replace("[Email]", !string.IsNullOrEmpty(model.Email) ? model.Email : "N/A");
                        str = str.Replace("[SoDienThoai]", !string.IsNullOrEmpty(model.Phone) ? model.Phone : "N/A");
                        str = str.Replace("[CV]", !string.IsNullOrEmpty(cv) ? (WebConfig.Website + cv) : "N/A");
                        SendEmailModels objEmailCus = new()
                        {
                            Subject = mainTemplate.Name + " - " + content.Name,
                            DisName = systemConfig.Name,
                            EmailBody = str,
                            EmailSend = systemConfig.EmailCms,
                            Password = systemConfig.PassEmailCms,
                            Port = Convert.ToInt32(systemConfig.Port),
                            Servername = systemConfig.Severname,
                            EnableSSL = systemConfig.SSLEmail ?? false,
                            To = model.Email,
                            Files = file
                        };
                        System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmailCus);
                        //send mail to admin
                        CommonJsonItem mainTemplateAdmin = list.Any(x => x.Code == "ApplyAdmin") ? list.FirstOrDefault(x => x.Code == "ApplyAdmin") : new CommonJsonItem();
                        if (string.IsNullOrEmpty(mainTemplateAdmin.Content))
                        {
                            msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "line 671" };
                            return Ok(msg);
                        }
                        string strAdmin = Common.ReadFile("LayoutEmail.htm", "html/Configuaration");
                        strAdmin = strAdmin.Replace("[Main]", mainTemplateAdmin.Content);
                        strAdmin = strAdmin.Replace("[TenJob]", content.Name);
                        strAdmin = strAdmin.Replace("[HoTen]", model.FullName);
                        strAdmin = strAdmin.Replace("[NoiDung]", model.Content);
                        strAdmin = strAdmin.Replace("[Email]", !string.IsNullOrEmpty(model.Email) ? model.Email : "N/A");
                        strAdmin = strAdmin.Replace("[SoDienThoai]", !string.IsNullOrEmpty(model.Phone) ? model.Phone : "N/A");
                        strAdmin = strAdmin.Replace("[CV]", !string.IsNullOrEmpty(cv) ? (WebConfig.Website + cv) : "N/A");
                        SendEmailModels objEmail = new()
                        {
                            Subject = mainTemplateAdmin.Name + " - " + content.Name,
                            DisName = systemConfig.Name,
                            EmailBody = strAdmin,
                            EmailSend = systemConfig.EmailCms,
                            Password = systemConfig.PassEmailCms,
                            Port = Convert.ToInt32(systemConfig.Port),
                            Servername = systemConfig.Severname,
                            EnableSSL = systemConfig.SSLEmail ?? false,
                            To = systemConfig.Email,
                            Files = file
                        };
                        string s = SendEmailModels.SendmailKitTest(objEmail);
                        if (!string.IsNullOrEmpty(s))
                        {
                            msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "line 478 - " + s };
                            return Ok(msg);
                        }
                        System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmail);
                        ContactUs contact = new()
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            Phone = model.Phone,
                            Content = model.Content,
                            ProductID = model.ContentID,
                            ProductName = content.Name,
                            ProductLink = Utility.Link(content.ModuleNameAscii, content._NameAscii, content.LinkUrl),
                            //ProductCode = content.Description,
                            Division = cv,
                            Code = "Apply"
                        };
                        int result = _contactUsManager.Insert(contact);
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = ResourceData.Resource("GuiThanhCong", Lang()) };
                        }
                        else
                        {
                            msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()) };
                        }
                    }
                    catch (Exception e)
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
                    }
                    #endregion
                }
                else
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = "523" + model.ProductID };
                }
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("GuiThatBai", Lang()), Logs = e.Message };
            }
            return Ok(msg);
        }

        #endregion
        #region Json Data

        [HttpPost]
        public ActionResult GetCity()
        {
            try
            {
                string Str = Common.ReadFile("city.json", "DataJson");
                Dictionary<string, CityJson> city = JsonConvert.DeserializeObject<Dictionary<string, CityJson>>(Str);
                List<CityJson> cities = new();
                foreach (KeyValuePair<string, CityJson> item in city)
                {
                    cities.Add(item.Value);
                }
                return Ok(cities);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult GetDistrict()
        {
            try
            {
                string Str = Common.ReadFile("district.json", "DataJson");
                Dictionary<string, DistrictJson> district = JsonConvert.DeserializeObject<Dictionary<string, DistrictJson>>(Str);
                List<DistrictJson> districties = new();
                foreach (KeyValuePair<string, DistrictJson> item in district)
                {
                    districties.Add(item.Value);
                }
                return Ok(districties);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult AutoComplete(string term)
        {
            try
            {
                List<SearchJsonModel> list = new();
                string keyword = term ?? string.Empty;
                #region Product
                StringBuilder sqlWhere = new();
                StringBuilder sqlWhereKey = new();
                StringBuilder keyView = new();
                StringBuilder totalNameAscii = new();
                List<string> key = new();
                string sqlOrder = " ORDER BY c.Name Asc, c.Price Desc";
                if (!string.IsNullOrEmpty(keyword))
                {
                    key = keyword.Trim().ToLower().Split(' ').ToList();
                    sqlOrder = string.Format(" ORDER BY case when Lower(Name) LIKE N'%{0}%' then 99999999 when NameAscii LIKE N'%{1}%'" +
                  " then 99999998 end DESC, Name Asc, Price DESC ", keyword, Utility.ConvertRewrite(keyword));
                }
                if (key.Count > 0)
                {
                    List<string> listColum = new()
                    {
                        nameof(ProductItem.Name),
                        nameof(ProductItem.ProductCode)
                    };
                    sqlWhere.Append(SqlUtility.WherAndNLikeListSearch(key, listColum));
                    foreach (var item in key)
                    {
                        keyView.Append(string.Format(SqlConst.TotalNameAscii, Utility.ConvertRewrite(item)));
                    }
                    sqlWhereKey.Append(SqlUtility.WherAndNLikeListSearch(key, new List<string> { nameof(WebsiteModulesItem.Name) }));
                }
                sqlWhere.Append(" AND CreatedDate <= GETDATE()");
                sqlWhere.Append(" AND NameAscii is not null and LinkUrl is null");
                sqlWhereKey.Append(" AND NameAscii is not null and LinkUrl is null");
                sqlWhere.Append(" AND Lang = '" + Lang() + "'");
                sqlWhereKey.Append(" AND Lang = '" + Lang() + "'");
                StringBuilder sqlModule = new StringBuilder();
                sqlModule.Append("Select * from(");
                sqlModule.Append(string.Format("select * from WebsiteModule where IsDeleted = 0 and IsShow = 1{0}", sqlWhereKey));
                sqlModule.Append(") c Order by c.Name Asc");
                var moduleList = _dapperDa.SelectPage<WebsiteModulesItem>(sqlModule.ToString(), 1, 10).ToList();
                if (moduleList.Any())
                {
                    list.Add(new SearchJsonModel
                    {
                        Html = "<span class=\"title-auto\">Có phải bạn muốn tìm</span>"
                    });
                    list.AddRange(moduleList.Select(item => new SearchJsonModel
                    {
                        Html = "<a class=\"title-keyword\" href=\"" + Utility.Link(item.NameAscii, string.Empty, item.LinkUrl) + "\" title=\"" + item.Name + "\">" + item.Name + "</a>"
                    }).Select(model => (model)).ToList());
                }
                StringBuilder sqlContent = new();
                sqlContent.Append("Select * from(");
                sqlContent.Append(string.Format(SqlContent.SqlByProduct, totalNameAscii, sqlWhere));
                sqlContent.Append(") c");
                sqlContent.Append(sqlOrder);
                var contentList = _dapperDa.SelectPage<ProductItem>(sqlContent.ToString(), 1, 10).ToList();
                #endregion
                if (contentList.Any())
                {
                    list.Add(new SearchJsonModel
                    {
                        Html = "<span class=\"title-auto\">Sản phẩm gợi ý</span>"
                    });
                    list.AddRange(contentList.OrderByDescending(x => x.ModuleTypeCode).Select(item => new SearchJsonModel
                    {
                        Html = "<a href=\"" + Utility.Link(string.Empty, item._NameAscii, item.LinkUrl) + "\" title=\"" + item.Name + "\" class=\"tag-item\">" + (!string.IsNullOrEmpty(item.UrlPicture) ? "<div class='_img' style='float:left; display: block;'><img style=\"width:80px\" src=" + item.UrlPicture + " /></div>" : string.Empty) +
                      "<div class='info' style='float:left;display:block;padding-left:10px;'>" +
                      "<span style='display:block;margin-bottom:0px; line-height:1.5;font-size:13px;'>" + item.Name + "</span>" + (item.Price.HasValue && item.Price != 0 ? "<span style='font-size: 12px;color:#c10017;margin-right: 10px;'>" + Utility.GetFormatPriceType(item.Price, 1, "Liên hệ") + "</span>" : string.Empty) +
                      (item.PriceOld.HasValue && item.PriceOld > item.Price ? "<span style='font-size: 11px;color: #777;text-decoration: line-through;'>" + Utility.GetFormatPriceType(item.PriceOld, 1, "") + "</span>" : string.Empty) +
                      "</div></a>"
                    }).Select(model => (model)).ToList());
                    return Ok(list);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
        }

        #endregion
    }
}