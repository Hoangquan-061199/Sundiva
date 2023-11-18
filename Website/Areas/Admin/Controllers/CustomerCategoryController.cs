using System;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Website.Areas.Admin.Controllers
{
    public class CustomerCategoryController : BaseController
    {
        private readonly CustomerCategoryDa _CustomerCategoryDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public CustomerCategoryController()
        {
            _CustomerCategoryDa = new CustomerCategoryDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("CustomerCategory");
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
            List<CustomerCategoryAdmin> list = _CustomerCategoryDa.ListSearch(seach, seach.page, 50, ViewBag.IsExport);
            CustomerCategoryViewModel model = new()
            {
                ListItem = list,
                SystemActionAdmin = SystemActionAdmin
            };
            int total = list.Any() ? list.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 50);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            CustomerCategoryViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                CustomerCategory = new CustomerCategory()
            };
            if (action.Do == ActionType.Edit)
            {
                model.CustomerCategory = _CustomerCategoryDa.GetId(ConvertUtil.ToInt32(action.ItemId));
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        [HttpPost]
        public ActionResult Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            CustomerCategory obj = new();
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
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.IsDeleted = false;
                        int result = _CustomerCategoryDa.Insert(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Thêm mới thành công."
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
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _CustomerCategoryDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        TryUpdateModelAsync(obj);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        int result = _CustomerCategoryDa.Update(obj);
                        AddLogAdmin(Request.Path, "Cập nhật loại khách hàng", "Actions-Edit");
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    if (!SystemActionAdmin.Delete)
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
                        obj = _CustomerCategoryDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        _CustomerCategoryDa.Update(obj);
                        AddLogEdit(Request.Path, "Delete", action.ItemId.ToString(), obj);
                        AddLogAdmin(Request.Path, "Xóa loại tài khoản", "Actions-Delete");
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
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
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            obj = _CustomerCategoryDa.GetId(item);
                            obj.IsDeleted = true;
                            _CustomerCategoryDa.Update(obj);
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
            }
            return Ok(msg);
        }
    }
}