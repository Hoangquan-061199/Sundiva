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
using Newtonsoft.Json;
using ADCOnline.Simple;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class SystemTagController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly SystemTagDa _systemTagDa;
        public SystemTagController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _systemTagDa = new SystemTagDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("SystemTag");
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
            SystemTagViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _systemTagDa.ListSearch(seach, seach.page, 20, false),
                SearchModel = seach,
                ObjBase = seach.contentId.HasValue ? _systemTagDa.GetId(seach.contentId.Value) : new SystemTag()
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;            
            ViewBag.GridHtml = GetPage(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 20);
            return View(model);
        }
        public IActionResult ListItemsAjax(string Code, string ids, bool isSearch = false)
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            SystemTagViewModel model = new()
            {
                ListItem = _systemTagDa.ListSearch(seach, seach.page, 20, false),
                SearchModel = seach,
                ObjBase = seach.contentId.HasValue ? _systemTagDa.GetId(seach.contentId.Value) : new SystemTag(),
                ValueSelected = ids
            };
            ViewBag.Code = Code;
            ViewBag.IsSearch = isSearch;
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPageAjax(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 20);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            SystemTagViewModel module = new()
            {
                ObjBase = new SystemTag
                {
                    IsShow = true,
                    IsHome=false
                },
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                SystemTag obj = _systemTagDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (obj != null)
                {
                    module.ObjBase = obj;
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(module);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            SystemTag obj = new();
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (!SystemActionAdmin.Add)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.ContentIDs = Utility.ValidString(obj.ContentIDs, ArrayInt, true);
                        obj.SeoDescription = !string.IsNullOrEmpty(obj.SeoDescription) ? Utility.ValidString(obj.SeoDescription, Title, true) : obj.Name;
                        obj.SeoKeyword = !string.IsNullOrEmpty(obj.SeoKeyword) ? Utility.ValidString(obj.SeoKeyword, Title, true) : obj.Name;
                        obj.SEOTitle = !string.IsNullOrEmpty(obj.SEOTitle) ? Utility.ValidString(obj.SEOTitle, Title, true) : obj.Name;
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        var order = Request.Form["OrderDisplay"];
                        obj.OrderBy = Convert.ToInt32(order);
                        obj.IsDeleted = false;
                        obj.CreatedDate = DateTime.Now;
                        SystemTag checkCode = _systemTagDa.GetName(obj.Name);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Từ khóa đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        int result = _systemTagDa.Insert(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Thêm mới thành công.",
                                Obj = obj
                            };
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
                        if (!SystemActionAdmin.Edit)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _systemTagDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        SystemTag objOld = obj;
                        await TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.ContentIDs = Utility.ValidString(obj.ContentIDs, ArrayInt, true);
                        obj.SeoDescription = !string.IsNullOrEmpty(obj.SeoDescription) ? Utility.ValidString(obj.SeoDescription, Title, true) : obj.Name;
                        obj.SeoKeyword = !string.IsNullOrEmpty(obj.SeoKeyword) ? Utility.ValidString(obj.SeoKeyword, Title, true) : obj.Name;
                        obj.SEOTitle = !string.IsNullOrEmpty(obj.SEOTitle) ? Utility.ValidString(obj.SEOTitle, Title, true) : obj.Name;
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        var order = Request.Form["OrderDisplay"];
                        obj.OrderBy = Convert.ToInt32(order);
                        SystemTag checkCode = _systemTagDa.GetName(obj.Name);
                        if (checkCode != null && checkCode.ID != obj.ID)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Từ khóa đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        int result = _systemTagDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Cập nhật thành công.",
                                 Obj = obj
                            };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    try
                    {
                        if (!SystemActionAdmin.Delete)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        SystemTag content = _systemTagDa.GetId(Convert.ToInt32(action.ItemId));
                        content.IsDeleted = true;
                        _systemTagDa.Update(content);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Xóa thành công."
                        };
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
                        if (!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _systemTagDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = !ConvertUtil.ToBool(obj.IsShow);
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị từ khóa" : "Ẩn từ khóa";
                        int result = _systemTagDa.Update(obj);
                        AddLogAdmin(Request.Path, message + obj.Name, ConvertUtil.ToBool(obj.IsShow) ? "Actions-hiện" : "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
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
                        if(!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage { Errors = true,Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach(int item in ArrID)
                        {
                            SystemTag content = _systemTagDa.GetId(item);
                            content.IsShow = true;
                            _systemTagDa.Update(content);
                        }
                        msg = new JsonMessage { Errors = false,Message = "Hiện thị thành công." };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.HiddenAll:
                    try
                    {
                        if(!SystemActionAdmin.Active)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            SystemTag content = _systemTagDa.GetId(item);
                            content.IsShow = false;
                            _systemTagDa.Update(content);
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Ẩn thành công."
                        };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.DeleteAll:
                    try
                    {
                        if(!SystemActionAdmin.Delete)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        foreach(int item in ArrID)
                        {
                            SystemTag content = _systemTagDa.GetId(item);
                            content.IsDeleted = true;
                            _systemTagDa.Update(content);
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Xóa thành công."
                        };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.OrderBy:
                    try
                    {
                        if(!SystemActionAdmin.Order)
                        {
                            msg = new JsonMessage { Errors = true,Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        if(!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                        {
                            var orderValues = Request.Form["OrderByValues"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            foreach (OrderByItem item in listOrderByItem)
                            {
                                try
                                {
                                    SystemTag content = _systemTagDa.GetId(item.ID);
                                    content.OrderBy = item.OrderDisplay;
                                    _systemTagDa.Update(content);
                                }
                                catch(Exception ex)
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
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
            }
            return Ok(msg);
        }
    }
}
