using System;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Utils;

namespace Website.Areas.Admin.Controllers
{
    public class RolesController : BaseController
    {
        private readonly RolesDa _rolesDa;
        private readonly ModuleAdminDa _moduleAdminDa;

        public RolesController()
        {
            _rolesDa = new RolesDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            if (!SystemActionAdmin.View && role != "Admin")
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Roles");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module
            };
            return View(model);
        }
        public ActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            RolesViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _rolesDa.ListSearch(seach, seach.page, 50, false)
            };
            return View(model);
        }

        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            RolesViewModel model = new()
            {
                Roles = new AspnetRoles(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                AspnetRoles roleItem = _rolesDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                if (roleItem != null)
                {
                    //List<int> list = Utility.StringToListInt(roleItem.ModuleIds);
                    model.Roles = roleItem;
                    //model.ListModuleItem = _moduleDa.GetAllListSimpleView(list);
                    //if (!string.IsNullOrEmpty(roleItem.RoleChild))
                    //{
                    //    var listRole = roleItem.RoleChild.Split(',').ToList();
                    //    model.GetListItems = _roleDa.GetListByCode(listRole);
                    //}
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
            AspnetRoles obj = new();
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
                        obj.RoleName = Utility.ValidString(obj.RoleName, "Code", true);
                        obj.Description = Utility.ValidString(obj.Description, "", true);
                        AspnetRoles checkCode = _rolesDa.GetRoleName(obj.RoleName);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        _rolesDa.Insert(obj);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Thêm mới thành công."
                        };
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
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _rolesDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                        AspnetRoles objOld = obj;
                        TryUpdateModelAsync(obj);
                        obj.RoleName = Utility.ValidString(obj.RoleName, "Code", true);
                        obj.Description = Utility.ValidString(obj.Description, "", true);
                        AspnetRoles checkCode = _rolesDa.GetRoleName(obj.RoleName);
                        if (checkCode != null && checkCode.RoleId != obj.RoleId)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        _rolesDa.Update(obj);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Cập nhật thành công."
                        };
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
