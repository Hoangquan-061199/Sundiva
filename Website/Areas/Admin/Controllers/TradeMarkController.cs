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
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class TradeMarkController : BaseController
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ActiveRoleDa _activeRoleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly ModulePositionDa _modulePositionDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly AttributesDa _attributesDa;
        private readonly ProductDa _productDa;
        private readonly AdvertisingDa _advertisingDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public TradeMarkController()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _activeRoleDa = new ActiveRoleDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _attributesDa = new AttributesDa(WebConfig.ConnectionString);
            _advertisingDa = new AdvertisingDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            Module module = _moduleAdminDa.GetTag("TradeMark");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                SearchModel = seach,
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module
            };
            return View(model);
        }

        /// <summary>
        /// Load danh sách bản ghi dưới dạng bảng
        /// </summary>
        /// <returns></returns>
        public IActionResult ListItems()
        {
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            search.lang = Lang();
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            search.ModuleIds = Utility.ValidString(moduleIds, ArrayInt, true);
            List<WebsiteModuleAdmin> ltsSourceModule = _websiteModuleDa.GetAdminAllFilter(search, Lang(), "", StaticEnum.Trademark);           
            WebsiteModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = ltsSourceModule,
                SearchModel = search,
                WebsiteModule = search.ModuleId.HasValue ? _websiteModuleDa.GetId(search.ModuleId.Value) : new WebsiteModule()
            };
            return View(model);
        }

        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = Utility.RemoveHTMLTag(tab);
            ActionViewModel action = UpdateModelAction();
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            WebsiteModuleViewModel model = new()
            {
                WebsiteModule = new WebsiteModule
                {
                    ParentID = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0
                },
                ListModuleType = _moduleTypeDa.GetListCode(StaticEnum.Trademark, ""),
                ListItem = _websiteModuleDa.GetAdminAll(false, Lang(), "", StaticEnum.Trademark, moduleIds),
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
                            if (!SystemActionAdmin.Add)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            await TryUpdateModelAsync(obj);
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
                            obj.Content ??= "";
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
                            var layout = Request.Form["LayoutCode"];
                            obj.LayoutCode = !string.IsNullOrEmpty(layout) ? Utility.ValidString(layout, Code, true) : null;
                            #endregion
                            var listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                            obj.PositionCode = string.Join(",", listposition.Select(x => x.Code));
                            int result = _websiteModuleDa.Insert(obj);
                            #region Update Attribute
                            if (!string.IsNullOrEmpty(newAttr))
                            {
                                List<AttributesAdmin> lstAttrNew = _attributesDa.GetListByAttrByMoudleId(newAttr);
                                if (lstAttrNew.Any())
                                {
                                    foreach (AttributesAdmin item in lstAttrNew)
                                    {
                                        item.ListModuleIds = string.IsNullOrEmpty(item.ListModuleIds) ? obj.ID.ToString() : item.ListModuleIds + "," + obj.ID;
                                        _attributesDa.UpdateModuleIds(item.ListModuleIds, item.ID.ToString());
                                    }
                                }
                            }
                            #endregion
                            AddLogAdmin(Request.Path, "Thêm mới quản lý thương hiệu:" + obj.Name, "Actions-Add");                           
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsSitemap == true && obj.IsShow == true && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(obj, null);
                            }
                            if (result > 0)
                            {
                                msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
                                return Ok(msg);
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                            msg.Message = ex.Message;
                        }
                        break;
                    case ActionType.Edit:
                        try
                        {
                            if (!SystemActionAdmin.Edit)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            string oldPosition = obj.PositionIds;
                            AddLogEdit("Edit", obj.ID.ToString(), session.GetAdminUserName(), obj);
                            string oldNameAscii = obj.NameAscii;
                            string oldAttr = obj.AttributeModuleIds;
                            string oldUrl = (string.IsNullOrEmpty(obj.LinkUrl) && !string.IsNullOrEmpty(obj.NameAscii)) ? WebConfig.Website + "/" + obj.NameAscii : string.Empty;
                            await TryUpdateModelAsync(obj);
                            #region Update Attribute
                            //to cal
                            var newAttrCal = Request.Form["AttributeModuleIdsCal"];
                            obj.AttributeModuleIdsCal = !string.IsNullOrEmpty(newAttrCal) ? Utility.ValidString(newAttrCal, ArrayInt, true) : null;
                            var newAttr = Request.Form["AttributeProductIds"];
                            obj.AttributeModuleIds = !string.IsNullOrEmpty(newAttr) ? Utility.ValidString(newAttr, ArrayInt, true) : null;
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
                                        item.ListModuleIds = string.IsNullOrEmpty(item.ListModuleIds) ? obj.ID.ToString() : item.ListModuleIds + "," + obj.ID;
                                        _attributesDa.UpdateModuleIds(item.ListModuleIds, item.ID.ToString());
                                    }
                                }
                            }
                            #endregion
                            #region banner chung
                            List<Advertising> lstOld = _advertisingDa.ListAdvertisingByModuleIds(obj.ID.ToString());
                            if (lstOld != null && lstOld.Count > 0)
                            {
                                lstOld.ForEach(x =>
                                {
                                    x.ModuleIds = ("," + x.ModuleIds + ",").Replace("," + obj.ID + ",", ",").Trim(',');
                                    _advertisingDa.Update(x);
                                });
                            }
                            var AdvertisingIds = Request.Form["PositionIds"];
                            if (!string.IsNullOrEmpty(AdvertisingIds))
                            {
                                List<Advertising> lstNew = _advertisingDa.ListAdvertisingByIds(AdvertisingIds);
                                if (lstNew != null && lstNew.Count > 0)
                                {
                                    lstNew.ForEach(x =>
                                    {
                                        x.ModuleIds = string.IsNullOrEmpty(x.ModuleIds) ? obj.ID.ToString() : (x.ModuleIds + "," + obj.ID.ToString());
                                        _advertisingDa.Update(x);
                                    });
                                }
                            }
                            #endregion
                            #region Nội dung liên quan (Khi cần thiết)
                            var RelateId = Request.Form["RelatedIds"];
                            obj.RelateIds = !string.IsNullOrEmpty(RelateId) ? Utility.ValidString(RelateId, ArrayInt, true) : null;
                            var NoteId = Request.Form["NotedIds"];
                            obj.NoteIds = !string.IsNullOrEmpty(NoteId) ? Utility.ValidString(NoteId, ArrayInt, true) : null;
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
                            List<ModulePosition> listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                            obj.PositionCode = string.Join(",", listposition.Select(x => x.Code));
                            var result = _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Sửa thương hiệu:" + obj.Name, "Actions-Edit");                            
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(obj.IsShow == true && obj.IsSitemap == true ? obj : null, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapBrand(null, oldObj);
                            }
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
                            if (!SystemActionAdmin.Delete)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            await TryUpdateModelAsync(obj);
                            obj.IsDeleted = true;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Xóa thương hiệu:" + obj.Name, "Actions-Delete");                            
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(null, obj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Show:
                        try
                        {
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            WebsiteModule oldObj = obj;
                            obj.IsShow = true;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Hiển thị thương hiệu:" + obj.Name, "Actions-Show");                            
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(obj, oldObj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Bạn đã hiển thị thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.Hidden:
                        try
                        {
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            obj.IsShow = obj.IsShow != true;
                            _websiteModuleDa.Update(obj);
                            AddLogAdmin(Request.Path, "Ẩn thương hiệu:" + action.ItemId, "Actions-Hidden");                            
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(obj.IsShow == true && obj.IsSitemap == true ? obj : null, obj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Bạn đã ẩn thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex) { AddLogError(ex); }
                        break;
                    case ActionType.ShowAll:
                        try
                        {
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                WebsiteModule oldContent = content;
                                content.IsShow = true;
                                _websiteModuleDa.Update(content);                                
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.ParentID > 0)
                                {
                                    await UpdateSitemapBrand(obj.IsSitemap == true ? content : null, oldContent);
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
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                content.IsShow = false;
                                _websiteModuleDa.Update(content);                                
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.ParentID > 0)
                                {
                                    await UpdateSitemapBrand(null, content);
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
                            if (!SystemActionAdmin.Delete)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int id in ArrID)
                            {
                                WebsiteModule content = _websiteModuleDa.GetId(id);
                                content.IsDeleted = true;
                                _websiteModuleDa.Update(content);                                
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.ParentID > 0)
                                {
                                    await UpdateSitemapBrand(null, content);
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
                                        await _websiteModuleDa.UpdateAsync(content);                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogError(ex);
                                    }
                                }
                                msg = new JsonMessage
                                {
                                    Errors = false,
                                    Message = "Cập nhật thứ tự thành công."
                                };
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
                            obj.IsSitemap = obj.IsSitemap != true;
                            string message = "Cập nhật thành công";
                            obj.ModifiedDate = DateTime.Now;
                            int result = _websiteModuleDa.Update(obj);
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.ParentID > 0)
                            {
                                await UpdateSitemapBrand(obj.IsSitemap == true && obj.IsShow == true ? obj : null, obj);
                            }
                            AddLogAdmin(Request.Path, "Cập nhật thương hiệu:" + obj.Name, "Actions-Sitemap");
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
                                WebsiteModule content = _websiteModuleDa.GetId(item);
                                content.IsSitemap = true;
                                content.ModifiedDate = DateTime.Now;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.ParentID > 0)
                                {
                                    await UpdateSitemapBrand(content.IsShow == true ? content : null, content);
                                }
                            }
                            AddLogAdmin(Request.Path, "Thêm sitemap:" + obj.Name, "Actions-Sitemap");
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
                                content.IsSitemap = false;
                                content.ModifiedDate = DateTime.Now;
                                _websiteModuleDa.Update(content);
                                if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.ParentID > 0)
                                {
                                    await UpdateSitemapBrand(null, obj);
                                }
                            }
                            AddLogAdmin(Request.Path, "Bỏ sitemap:" + obj.Name, "Actions-Sitemap");
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
        //load dữ liệu
        public ActionResult AjaxTreeSelect(int typeId = 1, string ValuesSelected = null, string module = null)
        {
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByModuleLang(module, Lang());
            StringBuilder stbHtml = new();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            _websiteModuleDa.BuildTreeViewCheckBox(ltsSourcewebsiteModule, null, true, ltsValues, ref stbHtml);
            WebsiteModuleViewModel model = new()
            {
                StringBuilder = stbHtml.ToString(),
                SystemActionAdmin = SystemActionAdmin,
                Type = typeId
            };
            return View(model);
        }
        public ActionResult AjaxTreeSelectNotLang(int typeId = 1, string ValuesSelected = null, string module = null)
        {
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByModule();
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
            List<WebsiteModuleAdmin> ltsSourcewebsiteModule = _websiteModuleDa.GetListByNotListModuleType(StaticEnum.Product, moduleIds, Lang());
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
