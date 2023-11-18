using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using ADCOnline.Simple.Base;
using Website.Models;
using ADCOnline.Simple.Admin;
using Microsoft.AspNetCore.Http;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.Simple;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class WebsiteModuleController : BaseController
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ActiveRoleDa _activeRoleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly ModulePositionDa _modulePositionDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly ProductDa _productDa;
        private readonly AttributesDa _attributesDa;
        private readonly AdvertisingDa _advertisingDa;
        public WebsiteModuleController()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _activeRoleDa = new ActiveRoleDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _attributesDa = new AttributesDa(WebConfig.ConnectionString);
            _advertisingDa = new AdvertisingDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            ModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin
            };
            return View(model);
        }

        /// <summary>
        /// Load danh sách bản ghi dưới dạng bảng
        /// </summary>
        /// <returns></returns>
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIdss = _websiteModuleDa.GetModuleIds(role, userId);
            List<WebsiteModuleAdmin> ltsSourceModule = _websiteModuleDa.GetAdminAll(false, Lang(), StaticEnum.Trademark, "", moduleIdss);
            ltsSourceModule.ForEach(x =>
            {
                x.Attributes = _attributesDa.GetListByArrId(x.AttributeModuleIds);
                List<WebsiteModuleAdmin> moduleIds = _websiteModuleDa.GetListChidrent(x.ID);
                if (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Trademark || x.ModuleTypeCode == StaticEnum.Sale)
                {
                    x.TotalProduct = _productDa.CountProductByModuleIds(string.Join(",", moduleIds.Select(m => m.ID.ToString())), Lang());
                }
                else
                {
                    x.TotalContent = _websiteContentDa.CountContentyModuleIds(string.Join(",", moduleIds.Select(m => m.ID.ToString())), Lang());
                }
            });
            WebsiteModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = ltsSourceModule,
                WebsiteModule = seach.ModuleId.HasValue ? _websiteModuleDa.GetId(seach.ModuleId.Value) : new WebsiteModule()
            };
            return View(model);
        }

        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            ActionViewModel action = UpdateModelAction();
            WebsiteModuleViewModel model = new()
            {
                WebsiteModule = new WebsiteModule
                {
                    ParentID = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0
                },
                ListModuleType = _moduleTypeDa.ListAllIsHow(),
                ListItem = _websiteModuleDa.GetAdminAll(false, Lang(), StaticEnum.Trademark, "", moduleIds),
                ListModulePosition = new List<ModulePosition>(),
                SystemActionAdmin = SystemActionAdmin,
                ListSelected = new List<WebsiteContentAdmin>(),
                ListNote = new List<WebsiteContentAdmin>(),
                AttributesAdmins = _attributesDa.GetAdminAll(true, Lang()),
                AdvertisingAdmins = _advertisingDa.ListAdvertisingEmpty()
            };
            if (action.Do == ActionType.Edit)
            {
                WebsiteModule websiteModule = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                List<Advertising> lstselect = _advertisingDa.ListAdvertisingByModuleIds(websiteModule.ID.ToString());
                model.AdvertisingIds = lstselect != null ? string.Join(",", lstselect.Select(x => x.ID)) : string.Empty;
                model.WebsiteModule = websiteModule;
                model.ListModulePosition = _modulePositionDa.GetListPositionIds(websiteModule.PositionIds);
                if (!string.IsNullOrEmpty(websiteModule.RelateIds))
                {
                    model.ListSelected = _websiteContentDa.GetListByArrId(websiteModule.RelateIds);
                }
                if (!string.IsNullOrEmpty(websiteModule.NoteIds))
                {
                    model.ListNote = _websiteContentDa.GetListByArrId(websiteModule.NoteIds);
                }
                if (!string.IsNullOrEmpty(websiteModule.AlbumPictureJson))
                {
                    model.ListAlbumGalleryAdmin = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(websiteModule.AlbumPictureJson);
                }
                if (!string.IsNullOrEmpty(websiteModule.AlbumImageJson))
                {
                    model.ListAlbumImageAdmin = JsonConvert.DeserializeObject<List<AlbumImageAdmin>>(websiteModule.AlbumImageJson);
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                ActionViewModel action = UpdateModelAction();
                WebsiteModule obj = new();
                List<AlbumGalleryAdmin> albumGalleryItemList = new();
                AlbumGalleryAdmin albumGalleryItem = new();
                string album = string.Empty;
                List<AlbumImageAdmin> imageGalleryItemList = new();
                AlbumImageAdmin imageGalleryItem = new();
                string image = string.Empty;
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
                            if (string.IsNullOrEmpty(obj.Name))
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                                return Ok(msg);
                            }
                            #region Valid
                            obj.Name = obj.Name;
                            obj.TypeViewMenu = obj.TypeViewMenu;
                            obj.Title = Utility.ValidString(obj.Title, Title, true);
                            obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                            obj.ModuleTypeCode = Utility.ValidString(obj.ModuleTypeCode, Code, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            obj.LayoutCode = Utility.ValidString(obj.LayoutCode, Code, true);
                            obj.TypeView = Utility.ValidString(obj.TypeView, Code, true);
                            obj.PositionCode = Utility.ValidString(obj.PositionCode, ArrayCode, true);
                            obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                            obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                            obj.Video = Utility.ValidString(obj.Video, Link, true);
                            obj.Icon = Utility.ValidString(obj.Icon, Link, true);
                            obj.BackgroundColor = Utility.ValidString(obj.BackgroundColor, Link, true);
                            obj.BackgroundHeader = Utility.ValidString(obj.BackgroundHeader, Link, true);
                            obj.BackgroundIndex = Utility.ValidString(obj.BackgroundIndex, Link, true);
                            obj.BackgroundFooter = Utility.ValidString(obj.BackgroundFooter, Link, true);
                            obj.Canonical = Utility.ValidString(obj.Canonical, Link, true);
                            obj.Background = Utility.RemoveHTML(obj.Background);
                            obj.AlbumPictureJson = Utility.RemoveHTML(obj.AlbumPictureJson);
                            obj.AlbumImageJson = Utility.RemoveHTML(obj.AlbumImageJson);
                            obj.SEOTitle = Utility.ValidString(obj.SEOTitle, Title, true);
                            obj.SeoKeyword = Utility.ValidString(obj.SeoKeyword, Title, true);
                            obj.SeoDescription = Utility.ValidString(obj.SeoDescription, Title, true);
                            obj.AttributeModuleIds = Utility.ValidString(obj.SeoDescription, ArrayInt, true);
                            obj.AttributeModuleIdsCal = Utility.ValidString(obj.SeoDescription, ArrayInt, true);
                            obj.HotlineProduct = Utility.RemoveHTML(obj.HotlineProduct);
                            obj.Rel = Utility.ValidString(obj.Rel, Code, true);
                            obj.Faqs = Utility.ValidString(obj.Faqs, ArrayInt, true);
                            obj.IndexGoogle = Utility.RemoveHTMLTag(obj.IndexGoogle);
                            #endregion
                            var newAttr = Request.Form["AttributeProductIds"];
                            obj.AttributeModuleIds = Utility.ValidString(newAttr, ArrayInt, true);
                            //to cal
                            var newAttrCal = Request.Form["AttributeModuleIdsCal"];
                            obj.AttributeModuleIdsCal = Utility.ValidString(newAttrCal, ArrayInt, true);
                            WebsiteModule checkCodeUrl = _websiteModuleDa.GetByNameAscii(obj.NameAscii);
                            if (checkCodeUrl != null)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại. Tên moudle là:" + checkCodeUrl.Name };
                                return Ok(msg);
                            }
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
                            #region load image
                            imageGalleryItemList = UpdateModelLst(imageGalleryItem, imageGalleryItemList);
                            if (imageGalleryItemList != null && imageGalleryItemList.Count > 0)
                            {
                                imageGalleryItemList = imageGalleryItemList.OrderBy(c => c.ImageOrder).ToList();
                                image = JsonConvert.SerializeObject(imageGalleryItemList);
                            }
                            else
                            {
                                image = null;
                            }
                            obj.AlbumImageJson = Utility.RemoveHTMLTag(image);
                            #endregion
                            obj.Lang = Lang();
                            obj.IsDeleted = false;
                            obj.Content ??= string.Empty;
                            obj.SeoDescription = obj.SeoDescription != null ? Utility.TrimLength(obj.Description, 200) : string.Empty;
                            obj.SeoKeyword ??= obj.Name;
                            obj.SEOTitle ??= obj.Name;
                            obj.CreatedDate = DateTime.Now;
                            obj.ParentID = ConvertUtil.ToInt32(obj.ParentID);
                            #region Nội dung liên quan (Khi cần thiết)
                            var RelateId = Request.Form["RelatedIds"];
                            obj.RelateIds = !string.IsNullOrEmpty(RelateId) ? Utility.ValidString(RelateId, ArrayInt, true) : null;
                            var NoteId = Request.Form["NotedIds"];
                            obj.NoteIds = !string.IsNullOrEmpty(NoteId) ? Utility.ValidString(NoteId, ArrayInt, true) : null;
                            var FagIds = Request.Form["FagIds"];
                            obj.Faqs = !string.IsNullOrEmpty(FagIds) ? Utility.ValidString(FagIds, ArrayInt, true) : null;
                            var layout = Request.Form["LayoutCode"];
                            obj.LayoutCode = !string.IsNullOrEmpty(layout) ? Utility.ValidString(layout, Code, true) : null;
                            #endregion
                            var listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                            obj.PositionCode = listposition.Any() ? string.Join(",", listposition.Select(x => x.Code)) : null;
                            int result = _websiteModuleDa.Insert(obj);
                            #region Update Attribute
                            if (!string.IsNullOrEmpty(newAttr))
                            {
                                List<AttributesAdmin> lstAttrNew = _attributesDa.GetListByAttrByMoudleId(newAttr);
                                if (lstAttrNew.Any())
                                {
                                    foreach (AttributesAdmin item in lstAttrNew)
                                    {
                                        if (string.IsNullOrEmpty(item.ListModuleIds))
                                        {
                                            item.ListModuleIds = obj.ID.ToString();
                                        }
                                        else
                                        {
                                            item.ListModuleIds = item.ListModuleIds + "," + obj.ID;
                                        }
                                        _attributesDa.UpdateModuleIds(item.ListModuleIds, item.ID.ToString());
                                    }
                                    obj.TotalAttributes = lstAttrNew.Count;
                                }
                                else
                                {
                                    obj.TotalAttributes = 0;
                                }
                            }
                            #endregion
                            AddLogAdmin(Request.Path, "Thêm mới quản lý Module:" + obj.Name, "Actions-Add");
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(obj, null);
                            }
                            if (result > 0)
                            {
                                msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
                                return Ok(msg);
                            }
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Edit:
                        try
                        {
                            if (SystemActionAdmin.Edit != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }

                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            AddLogEdit("Edit", obj.ID.ToString(), session.GetAdminUserName(), obj);
                            string oldNameAscii = obj.NameAscii;
                            string oldAttr = obj.AttributeModuleIds;
                            string oldUrl = (string.IsNullOrEmpty(obj.LinkUrl) && !string.IsNullOrEmpty(obj.NameAscii)) ? WebConfig.Website + "/" + obj.NameAscii : string.Empty;
                            await TryUpdateModelAsync(obj);
                            if (string.IsNullOrEmpty(obj.Name))
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                                return Ok(msg);
                            }
                            #region Valid
                            obj.Name = obj.Name;
                            obj.Title = Utility.ValidString(obj.Title, Title, true);
                            obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                            obj.ModuleTypeCode = Utility.ValidString(obj.ModuleTypeCode, Code, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            obj.LayoutCode = Utility.ValidString(obj.LayoutCode, Code, true);
                            obj.TypeView = Utility.ValidString(obj.TypeView, Code, true);
                            obj.PositionCode = Utility.ValidString(obj.PositionCode, ArrayCode, true);
                            obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                            obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                            obj.Video = Utility.ValidString(obj.Video, Link, true);
                            obj.Icon = Utility.ValidString(obj.Icon, Link, true);
                            obj.BackgroundColor = Utility.ValidString(obj.BackgroundColor, Link, true);
                            obj.BackgroundHeader = Utility.ValidString(obj.BackgroundHeader, Link, true);
                            obj.BackgroundIndex = Utility.ValidString(obj.BackgroundIndex, Link, true);
                            obj.BackgroundFooter = Utility.ValidString(obj.BackgroundFooter, Link, true);
                            obj.Canonical = Utility.ValidString(obj.Canonical, Link, true);
                            obj.Background = Utility.RemoveHTML(obj.Background);
                            obj.AlbumPictureJson = Utility.RemoveHTML(obj.AlbumPictureJson);
                            obj.AlbumImageJson = Utility.RemoveHTML(obj.AlbumImageJson);
                            obj.SEOTitle = Utility.ValidString(obj.SEOTitle, Title, true);
                            obj.SeoKeyword = Utility.ValidString(obj.SeoKeyword, Title, true);
                            obj.SeoDescription = Utility.ValidString(obj.SeoDescription, Title, true);
                            obj.AttributeModuleIds = Utility.ValidString(obj.SeoDescription, ArrayInt, true);
                            obj.AttributeModuleIdsCal = Utility.ValidString(obj.SeoDescription, ArrayInt, true);
                            obj.HotlineProduct = Utility.RemoveHTML(obj.HotlineProduct);
                            obj.Rel = Utility.ValidString(obj.Rel, Code, true);
                            obj.Faqs = Utility.ValidString(obj.Faqs, ArrayInt, true);
                            obj.IndexGoogle = Utility.RemoveHTMLTag(obj.IndexGoogle);
                            #endregion
                            #region Update Attribute
                            //to cal
                            var newAttr = Request.Form["AttributeProductIds"];
                            obj.AttributeModuleIds = Utility.ValidString(newAttr, ArrayInt, true);
                            //to cal
                            var newAttrCal = Request.Form["AttributeModuleIdsCal"];
                            obj.AttributeModuleIdsCal = Utility.ValidString(newAttrCal, ArrayInt, true);
                            if (!string.IsNullOrEmpty(oldAttr))
                            {
                                List<AttributesAdmin> lstAttrOld = _attributesDa.GetListByAttrByMoudleId(oldAttr);
                                foreach (AttributesAdmin item in lstAttrOld)
                                {
                                    item.ListModuleIds = ("," + item.ListModuleIds + ",").Replace("," + obj.ID + ",", ",").Trim(',');
                                    _attributesDa.UpdateModuleIds(item.ListModuleIds, item.ID.ToString());
                                }
                            }
                            if (!string.IsNullOrEmpty(newAttr))
                            {
                                List<AttributesAdmin> lstAttrNew = _attributesDa.GetListByAttrByMoudleId(newAttr);
                                if (lstAttrNew.Any())
                                {
                                    foreach (AttributesAdmin item in lstAttrNew)
                                    {
                                        if (string.IsNullOrEmpty(item.ListModuleIds))
                                        {
                                            item.ListModuleIds = obj.ID.ToString();
                                        }
                                        else
                                        {
                                            item.ListModuleIds = item.ListModuleIds + "," + obj.ID;
                                        }
                                        _attributesDa.UpdateModuleIds(item.ListModuleIds, item.ID.ToString());
                                    }
                                    obj.TotalAttributes = lstAttrNew.Count;
                                }
                                else
                                {
                                    obj.TotalAttributes = 0;
                                }
                            }
                            #endregion                            
                            #region Nội dung liên quan (Khi cần thiết)
                            var RelateId = Request.Form["RelatedIds"];
                            obj.RelateIds = !string.IsNullOrEmpty(RelateId) ? Utility.ValidString(RelateId, ArrayInt, true) : null;
                            var NoteId = Request.Form["NotedIds"];
                            obj.NoteIds = !string.IsNullOrEmpty(NoteId) ? Utility.ValidString(NoteId, ArrayInt, true) : null;
                            var FagIds = Request.Form["FagIds"];
                            obj.Faqs = !string.IsNullOrEmpty(FagIds) ? Utility.ValidString(FagIds, ArrayInt, true) : null;
                            var layout = Request.Form["LayoutCode"];
                            obj.LayoutCode = !string.IsNullOrEmpty(layout) ? Utility.ValidString(layout, Code, true) : null;
                            #endregion
                            obj.ModifiedDate = DateTime.Now;
                            if (obj.NameAscii != oldNameAscii)
                            {
                                WebsiteModule checkCodeUrl = _websiteModuleDa.GetByNameAscii(obj.NameAscii);
                                if (checkCodeUrl != null && checkCodeUrl.ID != obj.ID)
                                {
                                    msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại. Tên moudle là:" + checkCodeUrl.Name };
                                    return Ok(msg);
                                }
                                #region cập nhật nếu thay đổi nameAscii
                                if (obj.ModuleTypeCode == StaticEnum.Product)
                                {
                                    _websiteModuleDa.UpdateProduct(obj.NameAscii, oldNameAscii);
                                }
                                else
                                {
                                    _websiteModuleDa.UpdateWebsiteContent(obj.NameAscii, oldNameAscii);
                                }
                                #endregion
                            }
                            var avatar = Request.Form["UrlPicture"];
                            if (string.IsNullOrEmpty(avatar))
                            {
                                obj.UrlPicture = null;
                            }
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
                            #region load image
                            imageGalleryItemList = UpdateModelLst(imageGalleryItem, imageGalleryItemList);
                            if (imageGalleryItemList != null && imageGalleryItemList.Count > 0)
                            {
                                imageGalleryItemList = imageGalleryItemList.OrderBy(c => c.ImageOrder).ToList();
                                image = JsonConvert.SerializeObject(imageGalleryItemList);
                            }
                            else
                            {
                                image = null;
                            }
                            obj.AlbumImageJson = Utility.RemoveHTMLTag(image);
                            #endregion
                            var listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                            obj.PositionCode = listposition.Any() ? string.Join(",", listposition.Select(x => x.Code)) : null;
                            int result = _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Sửa quản lý Module:" + obj.Name, "Actions-Edit");
                            #region Update Sitemap
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(obj, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapCategory(null, oldObj);
                            }
                            #endregion
                            if (result > 0)
                            {
                                msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Obj = obj };
                                return Ok(msg);
                            }
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Delete:
                        try
                        {
                            if (SystemActionAdmin.Delete != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            await TryUpdateModelAsync(obj);
                            obj.IsDeleted = true;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Xóa quản lý Module:" + obj.Name, "Actions-Delete");
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(null, obj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Show:
                        try
                        {
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            obj.IsShow = obj.IsShow == true ? false : true;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Hiển thị danh mục:" + obj.Name, "Actions-Show");
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(obj, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapCategory(null, oldObj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Bạn đã hiển thị thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Hidden:
                        try
                        {
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            obj.IsShow = false;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Ẩn danh mục:" + action.ItemId, "Actions-Hidden");
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(obj, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapCategory(null, oldObj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Bạn đã ẩn thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.ShowAll:
                        try
                        {
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                WebsiteModule oldObj = content;
                                content.IsShow = true;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                {
                                    await UpdateSitemapCategory(content, oldObj);
                                }
                                else
                                {
                                    await UpdateSitemapCategory(null, oldObj);
                                }
                            }
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
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                WebsiteModule oldObj = content;
                                content.IsShow = false;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                {
                                    await UpdateSitemapCategory(content, oldObj);
                                }
                                else
                                {
                                    await UpdateSitemapCategory(null, oldObj);
                                }
                            }
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
                            if (SystemActionAdmin.Delete != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                WebsiteModule oldObj = content;
                                content.IsDeleted = true;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                {
                                    await UpdateSitemapCategory(content, oldObj);
                                }
                                else
                                {
                                    await UpdateSitemapCategory(null, oldObj);
                                }
                            }
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
                                foreach (OrderByItem it in listOrderByItem)
                                {
                                    try
                                    {
                                        WebsiteModule content = _websiteModuleDa.GetId(it.ID);
                                        content.OrderDisplay = it.OrderDisplay;
                                        _websiteModuleDa.Update(content);
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogError(ex);
                                    }
                                }
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
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            obj.IsSitemap = obj.IsSitemap == true ? false : true;
                            string message = "Cập nhật thành công";
                            obj.ModifiedDate = DateTime.Now;
                            int result = _websiteModuleDa.Update(obj);
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                            {
                                await UpdateSitemapCategory(obj, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapCategory(null, oldObj);
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
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(item);
                                WebsiteModule oldObj = content;
                                content.IsSitemap = true;
                                content.ModifiedDate = DateTime.Now;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                {
                                    await UpdateSitemapCategory(content, oldObj);
                                }
                                else
                                {
                                    await UpdateSitemapCategory(null, oldObj);
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
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(item);
                                WebsiteModule oldObj = obj;
                                content.IsSitemap = false;
                                content.ModifiedDate = DateTime.Now;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                {
                                    await UpdateSitemapCategory(content, oldObj);
                                }
                                else
                                {
                                    await UpdateSitemapCategory(null, oldObj);
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
                }
            }
            catch (Exception ex)
            {
                AddLogError(ex);
            }
            return Ok(msg);
        }
        public async Task<ActionResult> UpdateTotalProducts(int id)
        {
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                WebsiteModule obj = _websiteModuleDa.GetId(id);
                if (obj != null)
                {
                    await CountProductByModule(obj);
                    msg.Errors = false;
                    msg.Message = "Cập nhật thành công";
                }
            }
            catch (Exception e)
            {
                msg.Message = e.Message;
            }
            return Ok(msg);
        }

        public ActionResult UpdateTotalAttributes(int id)
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            try
            {
                WebsiteModule obj = _websiteModuleDa.GetId(id);
                if (obj != null && !string.IsNullOrEmpty(obj.AttributeModuleIds))
                {
                    List<AttributesAdmin> lstAttrNew = _attributesDa.GetListByAttrByMoudleId(obj.AttributeModuleIds);
                    if (lstAttrNew.Any())
                    {
                        obj.TotalAttributes = lstAttrNew.Count;
                    }
                    _websiteModuleDa.Update(obj);
                    msg.Errors = false;
                    msg.Message = "Cập nhật thành công";
                }
            }
            catch (Exception e)
            {
                msg.Message = e.Message;
            }
            return Ok(msg);
        }
        //load dữ liệu
        public ActionResult AjaxTreeSelect(int typeId = 1, string ValuesSelected = null, string module = null)
        {
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = new();
            ltsSourcewebsiteModule = _websiteModuleDa.GetListByModuleLang(module, Lang());
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewCheckBox(ltsSourcewebsiteModule.Where(x => x.ModuleTypeCode != StaticEnum.Trademark).ToList(), null, true, ltsValues, ref stbHtml);
            StringBuilder stbHtmlTrade = new();
            _websiteModuleDa.BuildTreeViewCheckBoxNotLang(ltsSourcewebsiteModule.Where(x => x.ModuleTypeCode == StaticEnum.Trademark).ToList(), null, true, ltsValues, ref stbHtmlTrade);
            WebsiteModuleViewModel model = new()
            {
                StringBuilder = stbHtml.ToString(),
                StringTradeBuilder = stbHtmlTrade.ToString(),
                Type = typeId
            };
            return View(model);
        }
        public ActionResult AjaxGridSelect(int typeId = 1, string ValuesSelected = null, string module = null, string selected = null, string unselected = null, bool isSearch = false)
        {
            SearchModel search = new();
            TryUpdateModelAsync(search);
            List<string> codes = ListHelper.GetValuesArrayString(module);
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            search.ModuleIds = moduleIds;
            search.selected = selected;
            search.unselected = unselected;
            WebsiteModuleViewModel model = new()
            {
                ListItem = _websiteModuleDa.GetGridByModuleLang(search, module, Lang()),
                SystemActionAdmin = SystemActionAdmin,
                ValuesSelected = ValuesSelected,
                Type = typeId,
                Code = codes.FirstOrDefault(),
                Types = module,
                SearchModel = search
            };
            ViewBag.IsSearch = isSearch;
            return View(model);
        }
        public ActionResult AjaxTreeSelectNotLang(int typeId = 1, string ValuesSelected = null, string module = null)
        {
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = new();
            ltsSourcewebsiteModule = _websiteModuleDa.GetListByModule();
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewCheckBoxNotLang(ltsSourcewebsiteModule.Where(x => x.ModuleTypeCode != StaticEnum.Trademark).ToList(), null, true, ltsValues, ref stbHtml, null);
            StringBuilder stbHtmlTrade = new();
            _websiteModuleDa.BuildTreeViewCheckBoxNotLang(ltsSourcewebsiteModule.Where(x => x.ModuleTypeCode == StaticEnum.Trademark).ToList(), null, true, ltsValues, ref stbHtmlTrade, 1);
            WebsiteModuleViewModel model = new()
            {
                StringBuilder = stbHtml.ToString(),
                SystemActionAdmin = SystemActionAdmin,
                StringTradeBuilder = stbHtmlTrade.ToString(),
                Type = typeId
            };
            return View(model);
        }
        public ActionResult AjaxTreeSelectProductNotLang(int typeId = 1, string ValuesSelected = null, string module = null)
        {
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByModuleProduct();
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewCheckBoxNotLang(ltsSourcewebsiteModule, null, true, ltsValues, ref stbHtml);
            WebsiteModuleViewModel model = new()
            {
                StringBuilder = stbHtml.ToString(),
                SystemActionAdmin = SystemActionAdmin,
                Type = typeId
            };
            return View(model);
        }
        public ActionResult AjaxTreeSelectModuleContent(string ValuesSelected)
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            string type = $"{StaticEnum.Product},{StaticEnum.Trademark},{StaticEnum.Sale},{StaticEnum.Cart}";
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByNotListModuleType(type, moduleIds, Lang());
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewNotCheckBox(ltsSourcewebsiteModule, 0, true, ltsValues, ref stbHtml);
            WebsiteModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                StringBuilder = stbHtml.ToString()
            };
            return View(model);
        }

        public ActionResult AjaxTreeSelectModuleProduct(string ValuesSelected)
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            string type = $"{StaticEnum.Product}";
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByListModuleType(type, moduleIds, Lang());
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewNotCheckBox(ltsSourcewebsiteModule, 0, true, ltsValues, ref stbHtml);
            WebsiteModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                StringBuilder = stbHtml.ToString()
            };
            return View(model);
        }
    }
}
