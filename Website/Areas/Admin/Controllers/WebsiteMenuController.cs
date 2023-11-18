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
using System.Linq;
using System.Threading.Tasks;
using ADCOnline.Simple;

namespace Website.Areas.Admin.Controllers
{
    public class WebsiteMenuController : BaseController
    {
        private readonly WebsiteMenuDa _WebsiteMenuDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public WebsiteMenuController()
        {
            _WebsiteMenuDa = new WebsiteMenuDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("WebsiteMenu");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module
            };
            return View(model);
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            List<WebsiteMenuAdmin> ltsSourceModule = _WebsiteMenuDa.GetAdminAll(false, Lang());
            WebsiteMenuViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = ltsSourceModule,
                SearchModel = seach,
                ObjBase = seach.contentId.HasValue ? _WebsiteMenuDa.GetId(seach.contentId.Value) : new WebsiteMenu()
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            WebsiteMenuViewModel module = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ObjBase = new WebsiteMenu
                {
                    ParentID = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0
                },
                ListItem = _WebsiteMenuDa.GetAdminAll(false, Lang()),
                ListModule = _websiteModuleDa.GetAdminAll(false, Lang(), "", "",moduleIds)
            };
            if (action.Do == ActionType.Edit)
            {
                WebsiteMenu obj = _WebsiteMenuDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                module.ObjBase = obj;
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(module);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            WebsiteMenu obj = new();
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
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Title = Utility.ValidString(obj.Name, Title, true);
                        obj.Link = Utility.ValidString(obj.Link, Link, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.UrlVideo = Utility.ValidString(obj.UrlVideo, Link, true);
                        obj.IsDeleted = false;
                        obj.Lang = Lang();
                        obj.IsShow = true;
                        int result = _WebsiteMenuDa.Insert(obj);
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
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _WebsiteMenuDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        WebsiteMenu objOld = obj;
                        await TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Title = Utility.ValidString(obj.Name, Title, true);
                        obj.Link = Utility.ValidString(obj.Link, Link, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.UrlVideo = Utility.ValidString(obj.UrlVideo, Link, true);
                        int result = _WebsiteMenuDa.Update(obj);
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
                case ActionType.Show:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _WebsiteMenuDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = true;
                        string message = "Hiển thị thành công";
                        int result = _WebsiteMenuDa.Update(obj);
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Hidden:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _WebsiteMenuDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = false;
                        string message = "Ẩn thành công";
                        int result = _WebsiteMenuDa.Update(obj);
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    try
                    {
                        if (SystemActionAdmin.Delete != true)
                        {
                            msg = new JsonMessage{ Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _WebsiteMenuDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        int result = _WebsiteMenuDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                            return Ok(msg);
                        }
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
                            WebsiteMenu content = _WebsiteMenuDa.GetId(item);
                            content.IsShow = true;
                            _WebsiteMenuDa.Update(content);
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
                        foreach (int item in ArrID)
                        {
                            WebsiteMenu content = _WebsiteMenuDa.GetId(item);
                            content.IsShow = false;
                            _WebsiteMenuDa.Update(content);
                        }
                        msg = new JsonMessage{ Errors = false, Message = "Ẩn thành công." };
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
                        foreach (int item in ArrID)
                        {
                            WebsiteMenu content = _WebsiteMenuDa.GetId(item);
                            content.IsDeleted = true;
                            _WebsiteMenuDa.Update(content);
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
                                    WebsiteMenu content = _WebsiteMenuDa.GetId(it.ID);
                                    content.OrderDisplay = it.OrderDisplay;
                                    _WebsiteMenuDa.Update(content);                                   
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
            }
            return Ok(msg);
        }
    }
}
