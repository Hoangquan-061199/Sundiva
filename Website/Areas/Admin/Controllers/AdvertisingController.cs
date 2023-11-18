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
using System.Text;
using Newtonsoft.Json;
using ADCOnline.Simple;
using System.Linq;
using ADCOnline.DA.Dapper.SqlView;

namespace Website.Areas.Admin.Controllers
{
    public class AdvertisingController : BaseController
    {
        private readonly AdvertisingDa _advertisingDa;
        private readonly ModulePositionDa _modulePositionDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public AdvertisingController()
        {
            _advertisingDa = new AdvertisingDa(WebConfig.ConnectionString);
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            AdvertisingViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListModulePositionAdmin = _modulePositionDa.ListAdminAll(),
                ListModule = _moduleAdminDa.GetTabMenu(role, userId)
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
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            AdvertisingViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _advertisingDa.ListSearch(seach, seach.page, 20, false),
                SearchModel = seach,
                Advertising = seach.contentId.HasValue ? _advertisingDa.GetId(seach.contentId.Value) : new Advertising()
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 20);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            AdvertisingViewModel model = new()
            {
                Advertising = new Advertising(),
                ListModulePosition = new List<ModulePosition>(),
                ListWebsiteModuleAdmin = new List<WebsiteModuleAdmin>(),
                ListBaseItem = _advertisingDa.ListAll(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                Advertising moduleAvertising = _advertisingDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (moduleAvertising != null)
                {
                    model.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(moduleAvertising.ModuleIds);
                    model.Advertising = moduleAvertising;
                    model.ListModulePosition = _modulePositionDa.GetListPositionIds(moduleAvertising.PositionIds);
                    if (!string.IsNullOrEmpty(moduleAvertising.AlbumPictureJson))
                    {
                        model.ListAlbumGalleryAdmin = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(moduleAvertising.AlbumPictureJson);
                    }
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }

        [HttpPost]
        public ActionResult Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            List<int> idValues = new();
            Advertising obj = new();
            List<AlbumGalleryAdmin> albumGalleryItemList = new();
            AlbumGalleryAdmin albumGalleryItem = new();
            string album = string.Empty;
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (SystemActionAdmin.Add != true)
                        {
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Title = Utility.ValidString(obj.Title, Title, true);
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.PositionCode = Utility.ValidString(obj.PositionCode, ArrayCode, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Video = Utility.ValidString(obj.Video, Link, true);
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), obj);
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
                        obj.AlbumPictureJson = Utility.RemoveHTML(album);
                        #endregion
                        obj.IsDeleted = false;
                        obj.Lang = Lang();
                        var listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                        obj.PositionCode = string.Join(",", listposition.Select(x => x.Code));
                        _advertisingDa.Insert(obj);
                        AddLogAdmin(Request.Path, "Thêm mới quản lý ảnh:" + obj.Name, "Actions-Add");
                        msg.Errors = false;
                        msg.Message = "Thêm mới thành công.";
                        msg.Obj = obj;
                        return Ok(msg);

                    }
                    catch
                    {

                    }
                    break;
                case ActionType.Edit:
                    try
                    {
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        obj = _advertisingDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        string oldPosition = obj.PositionIds;
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Title = Utility.ValidString(obj.Title, Title, true);
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.PositionCode = Utility.ValidString(obj.PositionCode, ArrayCode, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Video = Utility.ValidString(obj.Video, Link, true);
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
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
                        obj.AlbumPictureJson = Utility.RemoveHTML(album);
                        #endregion
                        List<ModulePosition> listposition = _modulePositionDa.GetListPositionItemIds(obj.PositionIds);
                        obj.PositionCode = string.Join(",", listposition.Select(x => x.Code));
                        _advertisingDa.Update(obj);
                        AddLogAdmin(Request.Path, "Sửa quản lý ảnh:" + obj.Name, "Actions-Edit");
                        msg.Errors = false;
                        msg.Message = "Cập nhật thành công.";
                        msg.Obj = obj;
                        return Ok(msg);
                    }
                    catch { }
                    break;
                case ActionType.Delete:
                    try
                    {
                        if (SystemActionAdmin.Delete != true)
                        {
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        Advertising content = _advertisingDa.GetId(Convert.ToInt32(action.ItemId));
                        AddLogEdit(Request.Path, "Delete", content.ID.ToString(), content);
                        content.IsDeleted = true;
                        _advertisingDa.Update(content);
                        AddLogAdmin(Request.Path, "Xóa banner:" + obj.Name, "Actions-Delete");
                        msg.Errors = false;
                        msg.Message = "Xóa thành công.";
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
                        if (SystemActionAdmin.Active != true)
                        {
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        obj = _advertisingDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = !ConvertUtil.ToBool(obj.IsShow);
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị banner" : "Ẩn banner";
                        _advertisingDa.Update(obj);
                        AddLogAdmin(Request.Path, message + obj.Name, ConvertUtil.ToBool(obj.IsShow) ? "Actions-hiện" : "Actions-Ẩn");
                        msg.Errors = false;
                        msg.Message = message;
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
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        foreach (int itemid in ArrID)
                        {
                            Advertising content = _advertisingDa.GetId(itemid);
                            AddLogEdit(Request.Path, "ShowAll", content.ID.ToString(), content);
                            content.IsShow = true;
                            _advertisingDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Hiển thị quản lý ảnh:" + obj.Name, "Actions-ShowAll");
                        msg.Errors = false;
                        msg.Message = "Hiện thị thành công.";
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
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        foreach (int itemid in ArrID)
                        {
                            Advertising content = _advertisingDa.GetId(itemid);
                            AddLogEdit(Request.Path, "HiddenAll", content.ID.ToString(), content);
                            content.IsShow = false;
                            _advertisingDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Ẩn banner:" + string.Join(",", ArrID), "Actions-HiddenAll");
                        msg.Errors = false;
                        msg.Message = "Ẩn thành công.";
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
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        foreach (int itemid in ArrID)
                        {
                            Advertising content = _advertisingDa.GetId(itemid);
                            AddLogEdit(Request.Path, "DeleteAll", content.ID.ToString(), content);
                            content.IsDeleted = true;
                            _advertisingDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Xóa all banner:" + string.Join(",", ArrID), "Actions-DeleteAll");
                        msg.Errors = false;
                        msg.Message = "Xóa thành công.";
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
                            msg.Errors = true;
                            msg.Message = "Bạn chưa được phân quyền cho chức năng này.";
                            return Ok(msg);
                        }
                        if (!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                        {
                            var orderValues = Request.Form["OrderByValues"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            foreach (OrderByItem itemid in listOrderByItem)
                            {
                                try
                                {
                                    Advertising content = _advertisingDa.GetId(itemid.ID);
                                    AddLogEdit(Request.Path, "OrderBy", content.ID.ToString(), content);
                                    content.OrderDisplay = itemid.OrderDisplay;
                                    _advertisingDa.Update(content);
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                            AddLogAdmin(Request.Path, "Cập nhật thứ tự quản lý ảnh:" + string.Join(",", ArrID), "Actions-OrderBy");
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
            }
            return Ok(msg);
        }
        private string GetOriginPosition(int id)
        {
            ModulePosition pos = _modulePositionDa.GetId(id);
            if (pos.ParentId.HasValue && pos.ParentId.Value > 0)
            {
                return GetOriginPosition(pos.ParentId.Value);
            }
            return pos.Code;
        }
    }
}
