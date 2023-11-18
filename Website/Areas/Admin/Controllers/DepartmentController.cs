using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Base;
using Website.Models;
using ADCOnline.Utils;
using System;
using ADCOnline.Simple.Admin;
using Website.Areas.Admin.ViewModels;

namespace Website.Areas.Admin.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly DepartmentDa _departmentDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public DepartmentController()
        {
            _departmentDa = new DepartmentDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Department");
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
            DepartmentViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _departmentDa.ListSearch(seach, seach.page, 100, false),
                SearchModel = seach,
                Department = seach.contentId.HasValue ? _departmentDa.GetId(seach.contentId.Value) : new Department()
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            DepartmentViewModel module = new()
            {
                Department = new Department(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                Department obj = _departmentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (obj != null)
                {
                    module.Department = obj;
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
            Department obj = new();
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
                        obj.Name = Utility.ValidString(obj.Name, "", true);
                        Department checkCode = _departmentDa.GetName(obj.Name);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Tên phòng ban đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        int result = _departmentDa.Insert(obj);
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
                        obj = _departmentDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        Department objOld = obj;
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, "", true);
                        if (objOld.Name != obj.Name)
                        {
                            Department checkCode = _departmentDa.GetName(obj.Name);
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
                        int result = _departmentDa.Update(obj);
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
                        _departmentDa.Delete(obj, " ID =" + action.ItemId);
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
                case ActionType.DeleteAll:
                    if(SystemActionAdmin.Delete != true)
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
                        foreach(int item in ArrID)
                        {
                            _departmentDa.Delete(obj," ID =" + item);
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
