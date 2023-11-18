using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using Website.Utils;

namespace Website.Areas.Admin.Controllers
{
    public class LanguageController : BaseController
    {
        private readonly LanguageDa _languageDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly MembershipDa _membershipDa;
        private readonly SystemTagDa _systemTagDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public LanguageController()
        {
            _languageDa = new LanguageDa(WebConfig.ConnectionString);
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
            Module module = _moduleAdminDa.GetTag("Language");
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
            SearchModel search = new()
            {
                lang = Lang()
            };
            TryUpdateModelAsync(search);
            
            LanguageViewModel model = new()
            {
                ListItem = _languageDa.ListSearch(search),
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = search,
                ObjBase = search.contentId.HasValue ? _languageDa.GetLanguageByID(search.contentId.Value) : new Language(),
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            LanguageViewModel module = new()
            {
                ObjBase = new Language(),
                SystemActionAdmin = SystemActionAdmin,
            };
            if (action.Do == ActionType.Edit)
            {
                Language obj = _languageDa.GetLanguageByID(ConvertUtil.ToInt32(action.ItemId));
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
        public ActionResult Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            var obj = new Language();
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (SystemActionAdmin.Add != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        TryUpdateModelAsync(obj);
                        #region Valid
                        obj.Name = Utility.RemoveHTML(obj.Name);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.OrderDisplay = obj.OrderDisplay;
                        #endregion
                        obj.IsShow = true;
                        obj.IsDeleted = false;
                        var result = _languageDa.Insert(obj);
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
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _languageDa.GetLanguageByID(ConvertUtil.ToInt32(action.ItemId));
                        TryUpdateModelAsync(obj);
                        #region Valid
                        obj.Name = Utility.RemoveHTML(obj.Name);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.OrderDisplay = obj.OrderDisplay;
                        #endregion
                        var result = _languageDa.Update(obj);
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
                    if (SystemActionAdmin.Delete != true)
                    {
                        msg = new JsonMessage
                        {
                            Errors = true,
                            Message = "Bạn chưa được phân quyền cho chức năng này."
                        };
                        return Ok(msg);
                    }
                    try
                    {
                        obj = _languageDa.GetLanguageByID(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        var result = _languageDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Xóa thành công."
                            };
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
                        foreach (var item in ArrID)
                        {
                            var content = _languageDa.GetLanguageByID(item);
                            content.IsShow = true;
                            _languageDa.Update(content);
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
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        foreach (var item in ArrID)
                        {
                            var content = _languageDa.GetLanguageByID(item);
                            content.IsShow = false;
                            _languageDa.Update(content);
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Ẩn thành công."
                        };
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
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            Language content = _languageDa.GetLanguageByID(item);
                            content.IsDeleted = true;
                            _languageDa.Update(content);
                        }
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
                case ActionType.Show:
                    if (SystemActionAdmin.Active != true)
                    {
                        msg = new JsonMessage
                        {
                            Errors = true,
                            Message = "Bạn chưa được phân quyền cho chức năng này."
                        };
                        return Ok(msg);
                    }
                    try
                    {
                        obj = _languageDa.GetLanguageByID(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = obj.IsShow == true ? false : true;
                        _languageDa.Update(obj);
                        msg.Errors = false;
                        msg.Message = (obj.IsShow == true) ? "Hiện thị thành công." : "Ẩn thành công.";
                        return Ok(msg);

                    }
                    catch
                    {

                    }
                    break;
                case ActionType.Hidden:
                    if (SystemActionAdmin.Active != true)
                    {
                        msg = new JsonMessage
                        {
                            Errors = true,
                            Message = "Bạn chưa được phân quyền cho chức năng này."
                        };
                        return Ok(msg);
                    }
                    try
                    {
                        obj = _languageDa.GetLanguageByID(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = !ConvertUtil.ToBool(obj.IsShow);
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị thành công" : "Ẩn thành công";
                        int result = _languageDa.Update(obj);
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch
                    {

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
                            StringValues orderValues = Request.Form["OrderByValues"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            foreach (OrderByItem item in listOrderByItem)
                            {
                                try
                                {
                                    Language content = _languageDa.GetLanguageByID(item.ID);
                                    content.OrderDisplay = item.OrderDisplay;
                                    _languageDa.Update(content);
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                            AddLogAdmin(Request.Path, $"Cập nhật thứ tự thành công:{string.Join(",", ArrID)}", "Actions-OrderBy");
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
    }
}
