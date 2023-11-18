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
    public class ModuleTypeController : BaseController
    {
        private readonly ModuleTypeDa _ModuleTypeDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public ModuleTypeController()
        {
            _ModuleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("ModuleType");
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
            ModuleTypeViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _ModuleTypeDa.ListSearch(seach, seach.page, 100, false),
                SearchModel = seach,
                ObjBase = seach.contentId.HasValue ? _ModuleTypeDa.GetId(seach.contentId.Value) : new ModuleType()
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            ModuleTypeViewModel module = new()
            {
                ObjBase = new ModuleType(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                ModuleType obj = _ModuleTypeDa.GetId(ConvertUtil.ToInt32(action.ItemId));
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
            JsonMessage msg = new JsonMessage
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            ModuleType obj = new();
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
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        ModuleType checkCode = _ModuleTypeDa.GetCode(obj.Name);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Tên mã đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        obj.IsDeleted = false;
                        obj.OrderDisplay = 0;
                        obj.IsShow = true;
                        int result = _ModuleTypeDa.Insert(obj);
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
                        obj = _ModuleTypeDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ModuleType objOld = obj;
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        if (objOld.Code!= obj.Code)
                        {
                            ModuleType checkCode = _ModuleTypeDa.GetCode(obj.Name);
                            if(checkCode != null && checkCode.ID != obj.ID)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Tên phòng ban đã tồn tại."
                                };
                                return Ok(msg);
                            }
                        }
                        int result = _ModuleTypeDa.Update(obj);
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
                        obj = _ModuleTypeDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        int result = _ModuleTypeDa.Update(obj);
                        if(result > 0)
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
                        if(SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true,Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach(int item in ArrID)
                        {
                            ModuleType content = _ModuleTypeDa.GetId(item);
                            content.IsShow = true;
                            _ModuleTypeDa.Update(content);
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
                        foreach (int item in ArrID)
                        {
                            ModuleType content = _ModuleTypeDa.GetId(item);
                            content.IsShow = false;
                            _ModuleTypeDa.Update(content);
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
                        foreach (int item in ArrID)
                        {
                            ModuleType content = _ModuleTypeDa.GetId(item);
                            content.IsDeleted = true;
                            _ModuleTypeDa.Update(content);
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
                        obj = _ModuleTypeDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = obj.IsShow == true ? false : true;
                        _ModuleTypeDa.Update(obj);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Bạn đã hiển thị thành công."
                        };
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
                        obj = _ModuleTypeDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = obj.IsShow == true ? false : true;
                        var message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị thành công" : "Ẩn thành công";
                        int result = _ModuleTypeDa.Update(obj);
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch
                    {

                    }
                    break;
            }
            return Ok(msg);
        }
    }
}
