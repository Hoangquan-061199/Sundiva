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

namespace Website.Areas.Admin.Controllers
{
    public class OtherContentController : BaseController
    {
        private readonly OtherContentDa _otherContentDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public OtherContentController()
        {
            _otherContentDa = new OtherContentDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("OtherContent");
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
            seach.lang = Lang();
            OtherContentViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _otherContentDa.ListSearch(seach, seach.page, 100, false),
                SearchModel = seach,
                OtherContent = seach.contentId.HasValue ? _otherContentDa.GetId(seach.contentId.Value) : new OtherContent()
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            OtherContentViewModel module = new()
            {
                OtherContent = new OtherContent(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                OtherContent obj = _otherContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (obj != null)
                {
                    module.OtherContent = obj;
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
            JsonMessage msg = new JsonMessage
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            OtherContent obj = new();
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
                        #region Valid Input
                        obj.Name = Utility.ValidString(obj.Name, "", true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        #endregion
                        OtherContent oldObj = obj;
                        obj.IsDeleted = false;
                        obj.Lang = Lang();
                        OtherContent checkCode = _otherContentDa.GetName(obj.Name);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Nội dung khác đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        int result = _otherContentDa.Insert(obj);
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
                        obj = _otherContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        OtherContent objOld = obj;
                        TryUpdateModelAsync(obj);
                        #region Valid Input
                        obj.Name = Utility.ValidString(obj.Name, "", true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        #endregion
                        if (objOld.Code != obj.Code)
                        {
                            OtherContent checkCode = _otherContentDa.GetName(obj.Name);
                            if(checkCode != null && checkCode.ID != obj.ID)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Nội dung khác đã tồn tại."
                                };
                                return Ok(msg);
                            }
                        }
                        int result = _otherContentDa.Update(obj);
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
                        if (SystemActionAdmin.Delete != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        OtherContent content = _otherContentDa.GetId(Convert.ToInt32(action.ItemId));
                        content.IsDeleted = true;
                        _otherContentDa.Update(content);                        
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
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _otherContentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = !ConvertUtil.ToBool(obj.IsShow);
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị thành công" : "Ẩn thành công";
                        int result = _otherContentDa.Update(obj);
                        AddLogAdmin(Request.Path, "Ẩn nội dung khác:" + obj.Name, "Actions-Ẩn");
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
                        if(SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true,Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach(int item in ArrID)
                        {
                            OtherContent content = _otherContentDa.GetId(item);
                            content.IsShow = true;
                            _otherContentDa.Update(content);
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
                        if(SystemActionAdmin.Active != true)
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
                            OtherContent content = _otherContentDa.GetId(item);
                            content.IsShow = false;
                            _otherContentDa.Update(content);
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
                        if(SystemActionAdmin.Delete != true)
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
                            OtherContent content = _otherContentDa.GetId(item);
                            content.IsDeleted = true;
                            _otherContentDa.Update(content);
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
            }
            return Ok(msg);
        }
    }
}
