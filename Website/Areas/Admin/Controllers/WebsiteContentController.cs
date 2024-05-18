using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Base;
using System.Collections.Generic;
using Website.Models;
using ADCOnline.Utils;
using System;
using ADCOnline.Simple.Admin;
using Website.Areas.Admin.ViewModels;
using ADCOnline.Simple;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;
using System.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Website.Areas.Admin.Controllers
{
    public class WebsiteContentController : BaseController
    {
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly ProductDa _productDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly MembershipDa _membershipDa;
        private readonly SystemTagDa _systemTagDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public WebsiteContentController()
        {
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _systemTagDa = new SystemTagDa(WebConfig.ConnectionString);
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("WebsiteContent");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            string type = $"{StaticEnum.Product},{StaticEnum.Trademark},{StaticEnum.Sale}";
            WebsiteContentViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListWebsiteModuleAdmin = _websiteModuleDa.GetAdminAll(false, Lang(), type, "", moduleIds),
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module
            };
            return View(model);
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new()
            {
                lang = Lang()
            };
            TryUpdateModelAsync(seach);
            session = new SessionBase(HttpContext);
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            if (seach.ModuleId != null && seach.ModuleId > 0)
            {
                WebsiteModule moduleItem = _websiteModuleDa.GetId(seach.ModuleId.Value);
                if (moduleItem != null)
                {
                    seach.type = moduleItem.ModuleTypeCode;
                    if (moduleItem.ModuleTypeCode == StaticEnum.Document)
                        seach.sort = 2;
                }
                seach.ModuleIds = string.Join(",", _websiteModuleDa.GetListChidrent(seach.ModuleId.Value).Select(x => x.ID));
            }
            else
            {
                SystemActionAdmin.View = false;
            }
            if (role == "Admin")
            {
                SystemActionAdmin.View = true;
            }
            MembershipAdmin membership = _membershipDa.GetAdminId(ConvertUtil.ToGuid(userId));
            if (membership != null)
            {
                seach.company = membership.Company;
            }
            List<WebsiteContentAdmin> list = _websiteContentDa.ListSearch(seach, seach.page, seach.pagesize > 0 ? seach.pagesize : 50, false, moduleIds, session.GetAdminUserName());
            WebsiteContentViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SystemActionAdmin = SystemActionAdmin,
                ListItem = list,
                SearchModel = seach,
                WebsiteContent = seach.contentId.HasValue ? _websiteContentDa.GetId(seach.contentId.Value) : new WebsiteContent(),
                UserId = userId,
                Role = role,
            };
            int total = list.Any() ? list.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 50);
            return View(model);
        }
        public IActionResult ListItemsAjax(string Code, string ids, bool isSearch = false)
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.lang = Lang();
            seach.page = seach.page > 0 ? seach.page : 1;
            seach.pagesize = seach.pagesize > 0 ? seach.pagesize : 10;
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            if (seach.ModuleId != null && seach.ModuleId > 0)
            {
                WebsiteModule moduleItem = _websiteModuleDa.GetId(seach.ModuleId.Value);
                if (moduleItem != null)
                {
                    seach.type = moduleItem.ModuleTypeCode;
                    if (moduleItem.ModuleTypeCode == StaticEnum.News)
                        seach.sort = 2;
                }
                seach.ModuleIds = string.Join(",", _websiteModuleDa.GetListChidrent(seach.ModuleId.Value).Select(x => x.ID));
            }
            WebsiteContentViewModel model = new()
            {
                ListItem = _websiteContentDa.ListSearch(seach, seach.page, seach.pagesize, false, moduleIds, session.GetAdminUserName()),
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                ValueSelected = Utility.ValidString(ids, ArrayInt, true)
            };
            string type = String.Join(",", StaticEnum.ModuleContent);
            model.ListWebsiteModuleAdmin = _websiteModuleDa.GetAdminAll(false, Lang(), "", type, moduleIds);
            ViewBag.Code = Utility.ValidString(Code, Code, true);
            ViewBag.IsSearch = isSearch;
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPageAjax(seach.page, total, seach.pagesize);
            return View(model);
        }
        public IActionResult ListItemsDocumentAjax(string Code, string ids, bool isSearch = false)
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.lang = Lang();
            seach.page = seach.page > 0 ? seach.page : 1;
            seach.pagesize = seach.pagesize > 0 ? seach.pagesize : 10;
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            if (seach.ModuleId != null && seach.ModuleId > 0)
            {
                WebsiteModule moduleItem = _websiteModuleDa.GetId(seach.ModuleId.Value);
                if (moduleItem != null)
                {
                    seach.type = moduleItem.ModuleTypeCode;
                    if (moduleItem.ModuleTypeCode == StaticEnum.Document)
                        seach.sort = 2;
                }
                seach.ModuleIds = string.Join(",", _websiteModuleDa.GetListChidrent(seach.ModuleId.Value).Select(x => x.ID));
            }
            WebsiteContentViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                ValueSelected = Utility.ValidString(ids, ArrayInt, true)
            };
            string type = String.Join(",", StaticEnum.Document);
            model.ListWebsiteModuleAdmin = _websiteModuleDa.GetAdminAll(false, Lang(), "", type, moduleIds);
            foreach (var item in model.ListWebsiteModuleAdmin)
            {
                seach.ModuleId = item.ID;
                model.ListItem = _websiteContentDa.ListSearch(seach, seach.page, seach.pagesize, false, moduleIds, session.GetAdminUserName());
            }
            ViewBag.Code = Utility.ValidString(Code, Code, true);
            ViewBag.IsSearch = isSearch;
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPageAjax(seach.page, total, seach.pagesize);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var moduleId = Request.Query["ModuleId"];
            string type = string.Empty;
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            List<string> lstTypeInChild = new() { StaticEnum.Services };
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            WebsiteContentViewModel module = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                WebsiteContent = new WebsiteContent(),
                ListWebsiteModuleAdmin = new List<WebsiteModuleAdmin>(),
                ListModuleTypeAdmin = _moduleTypeDa.ListAll(),
                ListWebsiteModule = _websiteModuleDa.GetAdminAll(false, Lang(), "", "", moduleIds),
                ListProduct = new List<ProductAdmin>(),
                SystemTags = new List<SystemTag>(),
                ListFileDownloadAdmin = new List<FileDownloadAdmin>(),
                ListProductRelated = new List<ProductAdmin>(),
                ListSpecialistItem = _websiteContentDa.GetListByModuleTypeCode(StaticEnum.Specialist),
                ListAddressItem = _websiteContentDa.GetListByModuleTypeCode(StaticEnum.Contact),
                ListContentSubAdmin = new List<SubContentItem>(),
            };
            if (!string.IsNullOrEmpty(moduleId))
            {
                WebsiteModuleAdmin moduleItem = _websiteModuleDa.GetModuleAdminId(Convert.ToInt32(moduleId));
                if (moduleItem != null)
                {
                    ViewBag.TypeView = moduleItem.TypeView;
                    type = moduleItem.ModuleTypeCode;
                    if (action.Do == ActionType.Add || action.Do == null)
                    {
                        module.WebsiteContent.ModuleIds = moduleItem.ID.ToString();
                        module.ListWebsiteModuleAdmin.Add(moduleItem);
                        module.WebsiteContent.ModuleNameAscii = moduleItem.NameAscii;
                    }
                    if (!string.IsNullOrEmpty(type) && lstTypeInChild.Contains(type))
                    {
                        SearchModel seach = new();
                        seach.lang = Lang();
                        seach.ModuleId = moduleItem.ID;
                        module.ListRelatedProduct = _productDa.ListSearch(seach, 1, 50, false, moduleIds);
                    }
                }
            }
            if (action.Do == ActionType.Edit)
            {
                WebsiteContent obj = _websiteContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (role != "Admin" && obj.CreatorID.ToString() != userId)
                {
                    module.SystemActionAdmin.Add = false;
                    module.SystemActionAdmin.Edit = false;
                    return View(module);
                }
                if (obj != null)
                {
                    WebsiteModule moduleItem = _websiteModuleDa.GetByNameAscii(obj.ModuleNameAscii);
                    if (moduleItem == null)
                    {
                        moduleItem = _websiteModuleDa.GetListAdminByArrIdNotShow(obj.ModuleIds)?.FirstOrDefault();
                    }
                    if (moduleItem != null)
                    {
                        type = moduleItem.ModuleTypeCode;
                        ViewBag.TypeView = moduleItem.TypeView;
                        if (!string.IsNullOrEmpty(type) && lstTypeInChild.Contains(type))
                        {
                            SearchModel seach = new();
                            seach.lang = Lang();
                            seach.ModuleId = moduleItem.ID;
                            module.ListRelatedProduct = _productDa.ListSearch(seach, 1, 50, false, moduleIds);
                        }
                    }
                    module.WebsiteContent = obj;
                    module.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(obj.ModuleIds);
                    module.ListProduct = !string.IsNullOrEmpty(obj.RelatedIds) ? _productDa.GetListByArrId(obj.RelatedIds) : new List<ProductAdmin>();
                    if (!string.IsNullOrEmpty(obj.AlbumPictureJson))
                    {
                        module.ListAlbumGalleryAdmin = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(obj.AlbumPictureJson);
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(obj.LinkDownload))
                        {
                            module.ListFileDownloadAdmin = JsonConvert.DeserializeObject<List<FileDownloadAdmin>>(obj.LinkDownload);
                        }
                    }
                    catch
                    {
                        module.ListFileDownloadAdmin = new List<FileDownloadAdmin>();
                    }
                    if (!string.IsNullOrEmpty(obj.Tags))
                    {
                        module.SystemTags = _systemTagDa.GetListByArrId(obj.Tags);
                    }
                    if (!string.IsNullOrEmpty(obj.SubContent))
                    {
                        module.ListContentSubAdmin = JsonConvert.DeserializeObject<List<SubContentItem>>(obj.SubContent);
                    }
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            module.ModuleTypeCode = type;
            List<string> lstNoUrl = new() { StaticEnum.SimpleModule };
            if (!string.IsNullOrEmpty(type) && lstNoUrl.Contains(type))
            {
                return View("AjaxFormSimple", module);
            }
            List<string> lstType = new() {
                StaticEnum.Catalogue, StaticEnum.ContactUs, StaticEnum.Doctorteam, StaticEnum.Specialist,StaticEnum.Video, StaticEnum.Contact, StaticEnum.Project, StaticEnum.DistributionSystem, StaticEnum.QA,StaticEnum.Gallery,
                StaticEnum.Document, StaticEnum.Recuitment, StaticEnum.Company, StaticEnum.Introduce, StaticEnum.Application, StaticEnum.Partner, StaticEnum.BusinessAreas
            };
            if (!string.IsNullOrEmpty(type) && lstType.Contains(type))
            {
                if (type == StaticEnum.Introduce)
                {
                    return View("AjaxFormIntroduce", module);
                }
                if (type == StaticEnum.Recuitment)
                {
                    module.Position = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("Position.json", "DataJson"));
                    module.Area = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("Area.json", "DataJson"));
                }
                if (type == StaticEnum.Contact || type == StaticEnum.ContactUs)
                {
                    module.Position = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("DistrictJson.json", "DataJson"));
                    module.Area = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("CityJson.json", "DataJson"));
                    return View("AjaxFormContact", module);
                }
                if (type == StaticEnum.Document)
                {
                    //Cơ quan ban hành
                    module.Area = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("Organization.json", "DataJson"));
                    if (module.Area == null)
                        module.Area = new List<CommonJsonAdmin>();
                    //Loại văn bản
                    module.Position = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TypeDocument.json", "DataJson"));
                    if (module.Position == null)
                        module.Position = new List<CommonJsonAdmin>();
                }
                if (type == StaticEnum.BusinessAreas)
                {
                    return View("AjaxFormProject", module);
                }
                return View("AjaxForm" + type, module);
            }
            return View(module);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            List<AlbumGalleryAdmin> albumGalleryItemList = new();
            AlbumGalleryAdmin albumGalleryItem = new();
            List<FileDownloadAdmin> fileDownloadAdminList = new();
            FileDownloadAdmin fileDownloadAdmin = new();
            string album = string.Empty;
            string fileDownload = string.Empty;
            WebsiteContent obj = new();
            MembershipAdmin membership = _membershipDa.GetAdminId(ConvertUtil.ToGuid(session.GetAdminUserId()));
            List<string> listCode = new() { StaticEnum.News };
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (SystemActionAdmin.Add != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        #region Valid Input
                        if (string.IsNullOrEmpty(obj.Name))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                            return Ok(msg);
                        }
                        obj.Name = Utility.RemoveHTML(obj.Name);
                        obj.Title = Utility.RemoveHTML(obj.Title);
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.ModuleNameAscii = Utility.RemoveHTML(obj.ModuleNameAscii);
                        obj.Video = obj.Video;
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Salary = Utility.ValidString(obj.Salary, Title, true);
                        //arrayint
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                        obj.LinkDownload = Utility.RemoveHTML(obj.LinkDownload);
                        obj.RelatedIds = Utility.ValidString(obj.RelatedIds, ArrayInt, true);
                        obj.AdvIds = Utility.ValidString(obj.AdvIds, ArrayInt, true);
                        obj.Tags = Utility.ValidString(obj.Tags, ArrayInt, true);
                        obj.LinkMap = Utility.RemoveHTML(obj.LinkMap);
                        obj.LinkDownload = Utility.RemoveHTML(obj.LinkDownload);
                        obj.IndexGoogle = Utility.RemoveHTML(obj.IndexGoogle);
                        obj.Canonical = Utility.ValidString(obj.Canonical, Link, true);
                        obj.Position = Utility.ValidString(obj.Position, Code, true);
                        obj.Area = Utility.ValidString(obj.Area, Code, true);
                        obj.Phone = Utility.ValidString(obj.Phone, Title, true);
                        obj.Hotline = Utility.ValidString(obj.Hotline, Title, true);
                        obj.Fax = Utility.ValidString(obj.Fax, Title, true);
                        obj.Email = Utility.ValidString(obj.Email, Title, true);
                        obj.Website = Utility.ValidString(obj.Website, Title, true);
                        obj.Address = Utility.ValidString(obj.Address, Title, true);
                        obj.SpecialistID = obj.SpecialistID;
                        obj.IMap = obj.IMap;
                        obj.AddressID = obj.AddressID;
                        obj.Status = 1;
                        #endregion
                        Utility.AddAltImage(obj.Content, obj.Name);
                        if (!obj.CreatedDate.HasValue)
                        {
                            obj.CreatedDate = DateTime.Now;
                        }
                        obj.TotalViews = 1;
                        obj.IsDeleted = false;
                        obj.IsApproved = true;
                        obj.Lang = Lang();
                        obj.CreatorID = ConvertUtil.ToGuid(session.GetAdminUserId());
                        obj.CreatorName = Utility.RemoveHTML(membership.FullName);
                        obj.SEOTitle ??= obj.Name;
                        obj.SeoDescription ??= Utility.RemoveHTMLTag(obj.Description) ?? Utility.TrimLength(Utility.RemoveHTMLTag(obj.Content), 200);
                        obj.SeoKeyword ??= obj.Name;
                        WebsiteContentDataJson dataJson = new();
                        await TryUpdateModelAsync(dataJson);
                        obj.DataJson = JsonConvert.SerializeObject(dataJson);
                        var codeNameAscii = Request.Form["CodeNameAscii"];
                        WebsiteContent checkCode = _websiteContentDa.GetByNameAscii(obj.NameAscii + codeNameAscii);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại. Tên link là:" + checkCode.Name };
                            return Ok(msg);
                        }
                        var AllFile = Request.Form["FileUrl"];
                        var AllFileName = Request.Form["FileUrl"];
                        obj.LinkDownload = AllFile;
                        #region ModuleIds
                        if (!string.IsNullOrEmpty(obj.ModuleIds))
                        {
                            List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(obj.ModuleIds);
                            if (string.IsNullOrEmpty(obj.ModuleNameAscii))
                            {
                                if (lstModule.Any(x => !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null);
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else
                                {
                                    obj.ModuleNameAscii = null;
                                }
                            }
                        }
                        #endregion
                        #region Avatar
                        string processPath = string.Empty;
                        var Avatar = Request.Form.Files["File"];
                        string extention = string.Empty;
                        //if (Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                        //{
                        //    extention = Path.GetExtension(Avatar.FileName);
                        //    if (extention.ToLower() == ".pdf" || extention.ToLower() == ".doc" || extention.ToLower() == ".docx" || extention.ToLower() == ".xls" || extention.ToLower() == ".xlsx")
                        //    {
                        //        processPath = Url.Content("upload/files/") + Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(Avatar.FileName);
                        //        string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                        //        FileStream stream = new(filePath, FileMode.Create);
                        //        Avatar.CopyTo(stream);
                        //        stream.Close();
                        //        obj.LinkFile = "/" + processPath;
                        //    }
                        //    else
                        //    {
                        //        msg = new JsonMessage { Errors = true, Message = "File không đúng định dạng cho phép! (pdf,doc,docx,xls,xlsx)" };
                        //        return Ok(msg);
                        //    }
                        //}
                        //else
                        //{
                        //    obj.LinkFile = null;
                        //}
                        obj.LinkFile = Utility.ValidString(obj.LinkFile, Link, true);
                        #endregion
                        #region loadAlbumanh
                        albumGalleryItemList = UpdateModelLst(albumGalleryItem, albumGalleryItemList);
                        if (albumGalleryItemList != null && albumGalleryItemList.Count > 0)
                        {
                            albumGalleryItemList = albumGalleryItemList.OrderBy(c => c.AlbumOrderDisplay).ToList();
                            album = JsonConvert.SerializeObject(albumGalleryItemList);
                        }
                        else
                        {
                            album = null;
                        }
                        obj.AlbumPictureJson = Utility.RemoveHTMLTag(album);
                        #endregion
                        #region loadfile
                        fileDownloadAdminList = UpdateModelLst(fileDownloadAdmin, fileDownloadAdminList);
                        if (fileDownloadAdminList.Any())
                        {
                            fileDownloadAdminList = fileDownloadAdminList.OrderBy(c => c.ID).ToList();
                            fileDownload = JsonConvert.SerializeObject(fileDownloadAdminList);
                        }
                        else
                        {
                            fileDownload = null;
                        }
                        obj.LinkDownload = Utility.RemoveHTMLTag(fileDownload);
                        #endregion
                        #region Tags
                        var tags = Request.Form["TagSaves"].ToString();
                        if (!string.IsNullOrEmpty(tags))
                        {
                            string tagsIdsSave = string.Empty;
                            List<string> itemTags = new();
                            if (!tags.Contains(','))
                                itemTags.Add(tags);
                            else
                                itemTags = tags.Split(',').ToList();
                            foreach (string item in itemTags)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    string itemNameAscii = "tag-" + Utility.ConvertRewrite(item.Trim());
                                    SystemTag tag = _systemTagDa.GetByNameAscii(itemNameAscii);
                                    if (tag == null)
                                    {
                                        string name = Utility.ValidString(item, Title, true);
                                        SystemTag systemTag = new()
                                        {
                                            Name = Utility.ValidString(name, Title, true),
                                            NameAscii = itemNameAscii,
                                            IsShow = true,
                                            CreatedDate = DateTime.Now,
                                            IsDeleted = false,
                                            SeoDescription = Utility.ValidString(name, Title, true),
                                            SeoKeyword = Utility.ValidString(name, Title, true),
                                            SEOTitle = Utility.ValidString(name, Title, true)
                                        };
                                        int tagR = _systemTagDa.Insert(systemTag);
                                        tagsIdsSave = tagsIdsSave + "," + systemTag.ID;
                                    }
                                    else
                                    {
                                        tagsIdsSave = tagsIdsSave + "," + tag.ID;
                                    }
                                }

                            }
                            obj.Tags = Utility.ValidString(tagsIdsSave, ArrayInt, true);
                        }

                        #endregion

                        #region load subcontent

                        string idsNewSubContent = Request.Form["NewSubContent"];
                        List<SubContentItem> SubContentList = new();
                        string SubContentItem = string.Empty;
                        if (!string.IsNullOrEmpty(idsNewSubContent))
                        {
                            List<int> IdsNew = ListHelper.GetValuesArray(idsNewSubContent);
                            foreach (int item in IdsNew)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                Random random = new Random();
                                var id = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + random.Next(100000, 999999);
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = id,
                                    Name = name,
                                    Content = content,
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }

                        if (SubContentItem.Any())
                        {
                            SubContentList = SubContentList.OrderBy(x => x.OrderDisplay).ToList();
                            SubContentItem = JsonConvert.SerializeObject(SubContentList);
                        }
                        else
                        {
                            SubContentItem = null;
                        }
                        obj.SubContent = SubContentItem;

                        #endregion load subcontent
                        obj.ViewHome = Utility.ValidString(Request.Form["ViewHome"], ArrayInt, true);
                        int result = _websiteContentDa.Insert(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, null);
                        }
                        AddLogAdmin(Request.Path, "Thêm mới quản lý bài viết:" + obj.Name, "Actions-Add");
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Edit:
                    try
                    {
                        obj = _websiteContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        if (SystemActionAdmin.Edit != true && role != "Admin" && obj.CreatorID.ToString() != userId)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        var oldObj = obj;
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        if (obj == null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Dữ liệu không tồn tại." };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        if (string.IsNullOrEmpty(obj.Name))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                            return Ok(msg);
                        }
                        #region Valid Input
                        obj.Name = Utility.RemoveHTML(obj.Name);
                        obj.Title = Utility.RemoveHTML(obj.Title);
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.ModuleNameAscii = Utility.RemoveHTML(obj.ModuleNameAscii);
                        obj.Video = obj.Video;
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Salary = Utility.ValidString(obj.Salary, Title, true);
                        //arrayint
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                        obj.RelatedIds = Utility.ValidString(obj.RelatedIds, ArrayInt, true);
                        obj.AdvIds = Utility.ValidString(obj.AdvIds, ArrayInt, true);
                        obj.Tags = Utility.ValidString(obj.Tags, ArrayInt, true);
                        obj.LinkMap = Utility.RemoveHTML(obj.LinkMap);
                        obj.LinkDownload = Utility.RemoveHTML(obj.LinkDownload);
                        obj.IndexGoogle = Utility.RemoveHTML(obj.IndexGoogle);
                        obj.Canonical = Utility.ValidString(obj.Canonical, Link, true);
                        obj.Position = Utility.ValidString(obj.Position, Code, true);
                        obj.Area = Utility.ValidString(obj.Area, Code, true);
                        obj.Phone = Utility.ValidString(obj.Phone, Title, true);
                        obj.Hotline = Utility.ValidString(obj.Hotline, Title, true);
                        obj.Fax = Utility.ValidString(obj.Fax, Title, true);
                        obj.Email = Utility.ValidString(obj.Email, Title, true);
                        obj.Website = Utility.ValidString(obj.Website, Title, true);
                        obj.Address = Utility.ValidString(obj.Address, Title, true);
                        obj.LinkFile = Utility.ValidString(obj.LinkFile, Link, true);
                        #endregion
                        if (!obj.CreatedDate.HasValue)
                        {
                            obj.CreatedDate = DateTime.Now;
                        }
                        obj.IMap = obj.IMap;
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = Utility.RemoveHTML(membership.FullName);
                        obj.IsApproved = true;
                        obj.IsDeleted = false;
                        obj.Status = 1;
                        var SpecialistID = Request.Form["SpecialistID"];
                        obj.SpecialistID = Utility.ValidString(SpecialistID, ArrayInt, true);
                        var AddressID = Request.Form["AddressID"];
                        obj.AddressID = Utility.ValidString(AddressID, ArrayInt, true);
                        var codeNameAscii = Request.Form["CodeNameAscii"];
                        WebsiteContent checkCode = _websiteContentDa.GetByNameAscii(obj.NameAscii + codeNameAscii);
                        if (checkCode != null && checkCode.ID != obj.ID)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại. Tên link là:" + checkCode.Name };
                            return Ok(msg);
                        }
                        var avatar = Request.Form["UrlPicture"];
                        if (string.IsNullOrEmpty(avatar))
                        {
                            obj.UrlPicture = null;
                        }
                        var AllFile = Request.Form["FileUrl"];
                        obj.LinkDownload = Utility.RemoveHTMLTag(AllFile);
                        #region ModuleIds
                        if (!string.IsNullOrEmpty(obj.ModuleIds))
                        {
                            List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(obj.ModuleIds);
                            if (string.IsNullOrEmpty(obj.ModuleNameAscii))
                            {
                                if (lstModule.Any(x => !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null);
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else
                                {
                                    obj.ModuleNameAscii = null;
                                }
                            }
                        }
                        #endregion
                        #region Avatar
                        //string processPath = string.Empty;
                        //var Avatar = Request.Form.Files["File"];
                        //string extention = string.Empty;
                        //var isFile = Request.Form["IsFile"];
                        //if (isFile == "false" && Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                        //{
                        //    extention = Path.GetExtension(Avatar.FileName);
                        //    if (extention.ToLower() == ".pdf" || extention.ToLower() == ".doc" || extention.ToLower() == ".docx" || extention.ToLower() == ".xls" || extention.ToLower() == ".xlsx")
                        //    {
                        //        processPath = Url.Content("upload/files/") + Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(Avatar.FileName);
                        //        string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                        //        FileStream stream = new(filePath, FileMode.Create);
                        //        Avatar.CopyTo(stream);
                        //        stream.Close();
                        //        obj.LinkFile = "/" + processPath;
                        //    }
                        //    else
                        //    {
                        //        msg = new JsonMessage { Errors = true, Message = "File không đúng định dạng cho phép! (pdf,doc,docx,xls,xlsx)" };
                        //        return Ok(msg);
                        //    }
                        //}
                        //else
                        //{
                        //    obj.LinkFile = null;
                        //}
                        #endregion
                        #region loadAlbumanh
                        albumGalleryItemList = UpdateModelLst(albumGalleryItem, albumGalleryItemList);
                        if (albumGalleryItemList != null && albumGalleryItemList.Count > 0)
                        {
                            albumGalleryItemList = albumGalleryItemList.OrderBy(c => c.AlbumOrderDisplay).ToList();
                            album = JsonConvert.SerializeObject(albumGalleryItemList);
                        }
                        else
                        {
                            album = null;
                        }
                        obj.AlbumPictureJson = Utility.RemoveHTMLTag(album);
                        #endregion
                        #region loadfile
                        fileDownloadAdminList = UpdateModelLst(fileDownloadAdmin, fileDownloadAdminList);
                        if (fileDownloadAdminList.Any())
                        {
                            fileDownloadAdminList = fileDownloadAdminList.OrderBy(c => c.ID).ToList();
                            fileDownload = JsonConvert.SerializeObject(fileDownloadAdminList);
                        }
                        else
                        {
                            fileDownload = null;
                        }
                        obj.LinkDownload = Utility.RemoveHTMLTag(fileDownload);
                        #endregion
                        #region Tags
                        var tags = Request.Form["TagSaves"].ToString();
                        if (!string.IsNullOrEmpty(tags))
                        {
                            string tagsIdsSave = string.Empty;
                            List<string> itemTags = new();
                            if (!tags.Contains(','))
                                itemTags.Add(tags);
                            else
                                itemTags = tags.Split(',').ToList();
                            foreach (string item in itemTags)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    string itemNameAscii = "tag-" + Utility.ConvertRewrite(item.Trim());
                                    SystemTag tag = _systemTagDa.GetByNameAscii(itemNameAscii);
                                    if (tag == null)
                                    {
                                        string name = Utility.ValidString(item, Title, true);
                                        SystemTag systemTag = new()
                                        {
                                            Name = name,
                                            NameAscii = itemNameAscii,
                                            IsShow = true,
                                            CreatedDate = DateTime.Now,
                                            IsDeleted = false,
                                            SeoDescription = name,
                                            SeoKeyword = name,
                                            SEOTitle = name
                                        };
                                        int tagR = _systemTagDa.Insert(systemTag);
                                        tagsIdsSave = tagsIdsSave + "," + systemTag.ID;
                                    }
                                    else
                                    {
                                        tagsIdsSave = tagsIdsSave + "," + tag.ID;
                                    }
                                }

                            }
                            obj.Tags = Utility.ValidString(tagsIdsSave, ArrayInt, true);
                        }
                        #endregion
                        #region load subcontent

                        string idsNewSubContent = Request.Form["NewSubContent"];
                        string idsOldSubContent = Request.Form["OldSubContent"];
                        List<SubContentItem> SubContentList = new();
                        string SubContentItem = string.Empty;
                        if (!string.IsNullOrEmpty(idsNewSubContent))
                        {
                            List<int> IdsNew = ListHelper.GetValuesArray(idsNewSubContent);
                            foreach (int item in IdsNew)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                Random random = new Random();
                                var id = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + random.Next(100000, 999999);
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = id,
                                    Name = name,
                                    Content = content,
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }
                        if (!string.IsNullOrEmpty(idsOldSubContent))
                        {
                            List<int> IdsOld = ListHelper.GetValuesArray(idsOldSubContent);
                            foreach (int item in IdsOld)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = item,
                                    Name = name,
                                    Content = content,
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }

                        if (SubContentList.Any())
                        {
                            SubContentList = SubContentList.OrderBy(x => x.OrderDisplay).ToList();
                            SubContentItem = JsonConvert.SerializeObject(SubContentList);
                        }
                        else
                        {
                            SubContentItem = null;
                        }
                        obj.SubContent = SubContentItem;

                        #endregion load subcontent
                        obj.SEOTitle ??= obj.Name;
                        obj.SeoDescription ??= Utility.RemoveHTMLTag(obj.Description) ?? Utility.TrimLength(Utility.RemoveHTMLTag(obj.Content), 200);
                        obj.SeoKeyword ??= obj.Name;
                        obj.ViewHome = Utility.ValidString(Request.Form["ViewHome"], ArrayInt, true);
                        WebsiteContentDataJson dataJson = new();
                        if (!string.IsNullOrEmpty(obj.DataJson))
                        {
                            dataJson = JsonConvert.DeserializeObject<WebsiteContentDataJson>(obj.DataJson);
                        }
                        await TryUpdateModelAsync(dataJson);
                        obj.DataJson = JsonConvert.SerializeObject(dataJson);
                        int result = _websiteContentDa.Update(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapNews(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Sửa quản lý nội dung:" + obj.Name, "Actions-Edit");
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Obj = obj };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Display:
                    try
                    {
                        obj = _websiteContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        if (SystemActionAdmin.Active != true && role != "Admin" && obj.CreatorID.ToString() != userId)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        WebsiteContent oldObj = obj;
                        obj.IsShow = true;
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _websiteContentDa.Update(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapNews(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Hiển thị quản lý nội dung:" + obj.Name, "Actions-Display");
                        msg = new JsonMessage { Errors = false, Message = "Hiện thị thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Hidden:
                    try
                    {
                        obj = _websiteContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        if (SystemActionAdmin.Active != true && role != "Admin" && obj.CreatorID.ToString() != userId)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        WebsiteContent oldObj = obj;
                        obj.IsShow = obj.IsShow == true ? false : true;
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị thành công" : "Ẩn thành công";
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _websiteContentDa.Update(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapNews(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    try
                    {
                        obj = _websiteContentDa.GetId(Convert.ToInt32(action.ItemId));
                        if (SystemActionAdmin.Delete != true && role != "Admin" && obj.CreatorID.ToString() != userId)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        WebsiteContent oldObj = obj;
                        obj.IsDeleted = true;
                        _websiteContentDa.Update(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapNews(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Xóa quản lý nội dung:" + action.ItemId, "Actions-Xóa");
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.ShowAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && content.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            content.IsShow = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsDeleted == false && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Hiện thị quản lý nội dung:" + string.Join(",", ArrID), "Actions-ShowAll");
                        msg = new JsonMessage { Errors = false, Message = "Hiện thị thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.HiddenAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && content.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            content.IsShow = false;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsDeleted == false && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + string.Join(",", ArrID), "Actions-HiddenAll");
                        msg = new JsonMessage { Errors = false, Message = "Ẩn thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.DeleteAll:
                    try
                    {
                        if (!SystemActionAdmin.Delete)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && content.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            content.IsDeleted = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsDeleted == false && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Xóa quản lý nội dung:" + string.Join(",", ArrID), "Actions-DeleteAll");
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.OrderBy:
                    try
                    {
                        if (!SystemActionAdmin.Order)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        if (!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                        {
                            var orderValues = Request.Form["OrderByValues"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            foreach (OrderByItem item in listOrderByItem)
                            {
                                try
                                {
                                    WebsiteContent content = _websiteContentDa.GetId(item.ID);
                                    if (role != "Admin" && content.CreatorID.ToString() != userId)
                                    {
                                        msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                        return Ok(msg);
                                    }
                                    content.OrderDisplay = item.OrderDisplay;
                                    content.ModifiedDate = DateTime.Now;
                                    content.ModifiedName = membership.FullName;
                                    _websiteContentDa.Update(content);
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                            AddLogAdmin(Request.Path, "Cập nhật thứ tự bài viết:" + string.Join(",", ArrID), "Actions-OrderBy");
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật thứ tự thành công." };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.IsSitemap:
                    try
                    {
                        obj = _websiteContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        if (!SystemActionAdmin.Active && role != "Admin" && obj.CreatorID.ToString() != userId)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        WebsiteContent oldObj = obj;
                        obj.IsSitemap = obj.IsSitemap != true;
                        string message = "Cập nhật thành công";
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _websiteContentDa.Update(obj);
                        WebsiteModule moduleItem = !string.IsNullOrEmpty(obj.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(obj.ModuleNameAscii) : new WebsiteModule();
                        if (!string.IsNullOrEmpty(obj.NameAscii) && !string.IsNullOrEmpty(obj.ModuleNameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsDeleted == false && obj.IsShow == true && obj.IsSitemap == true && moduleItem != null && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                        {
                            await UpdateSitemapNews(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapNews(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.IsSitemapAll:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            content.IsSitemap = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsDeleted == false && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Thêm sitemap:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotIsSitemapAll:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            content.IsSitemap = false;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsDeleted == false && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Bỏ sitemap:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.IsHot:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "3";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",3,"))
                            {
                                content.ViewHome += ",3";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotHot:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",3,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",3,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.IsHome:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "1";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",1,"))
                            {
                                content.ViewHome += ",1";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotHome:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",1,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",1,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.IsNew:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "4";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",4,"))
                            {
                                content.ViewHome += ",4";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotNew:
                    try
                    {
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            WebsiteContent content = _websiteContentDa.GetId(item);
                            if (role != "Admin" && obj.CreatorID.ToString() != userId)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            WebsiteContent oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",4,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",4,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _websiteContentDa.Update(content);
                            WebsiteModule moduleItem = !string.IsNullOrEmpty(content.ModuleNameAscii) ? _websiteModuleDa.GetByNameAsciiMain(content.ModuleNameAscii) : new WebsiteModule();
                            if (!string.IsNullOrEmpty(content.NameAscii) && !string.IsNullOrEmpty(content.ModuleNameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true && moduleItem != null && listCode.Contains(moduleItem.ModuleTypeCode))
                            {
                                await UpdateSitemapNews(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapNews(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
            }
            return Ok(msg);
        }
    }
}
