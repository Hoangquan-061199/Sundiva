using System;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using Microsoft.AspNetCore.Http;

namespace Website.Areas.Admin.Controllers
{
    public class UserAdministrationController : BaseController
    {
        private readonly MembershipDa _membershipDa;
        private readonly DepartmentDa _departmentDa;
        private readonly ActiveRoleDa _activeRoleDa;
        private readonly RolesDa _rolesDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        public UserAdministrationController()
        {
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _departmentDa = new DepartmentDa(WebConfig.ConnectionString);
            _activeRoleDa = new ActiveRoleDa(WebConfig.ConnectionString);
            _rolesDa = new RolesDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("WebAdminRole");
            if (!SystemActionAdmin.View && role != "Admin")
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("UserAdministration");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SystemActionAdmin = SystemActionAdmin,
                Module = module
            };
            return View(model);
        }

        public ActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            UserAdminViewModel model = new()
            {
                ListItem = _membershipDa.ListSearch(seach, seach.page, 50, false),
                ListDepartment = _departmentDa.GetAll(),
                SystemActionAdmin = SystemActionAdmin
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            UserAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                MembershipAdmin = new MembershipAdmin(),
                ListDepartment = _departmentDa.GetAll(),
                ListRolesAdmin = _rolesDa.GetAdminAll(),
            };
            if (action.Do == ActionType.Edit)
            {
                model.MembershipAdmin = _membershipDa.GetAdminId(ConvertUtil.ToGuid(action.ItemId));
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        #region Active role
        public ActionResult AjaxActiveForm()
        {
            ActionViewModel action = UpdateModelAction();
            MembershipAdmin memberShip = _membershipDa.GetAdminId(ConvertUtil.ToGuid(action.ItemId));
            List<ModuleAdmin> listModule = _moduleAdminDa.GetListAdminByArrId(memberShip.ModuleIds);
            UserAdminViewModel model = new()
            {
                MembershipAdmin = _membershipDa.GetAdminId(ConvertUtil.ToGuid(action.ItemId)),
                ListActiveRole = _activeRoleDa.GetAll(),
                SystemActionAdmin = SystemActionAdmin,
                ListModuleAdmin = listModule
            };
            ViewBag.Action = "Active";
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        #endregion
        #region Active Module website
        public ActionResult AddModule()
        {
            ActionViewModel action = UpdateModelAction();
            MembershipAdmin memberShip = _membershipDa.GetAdminId(Guid.Parse(action.ItemId));
            UserAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                MembershipAdmin = memberShip,
                ListWebsiteModuleAdmin = new List<WebsiteModuleAdmin>()
            };
            if (memberShip != null && !string.IsNullOrEmpty(memberShip.WebsiteModuleIds))
            {
                model.ListWebsiteModuleAdmin = _websiteModuleDa.GetListAdminByArrId(memberShip.WebsiteModuleIds);
            }
            ViewBag.Action = ActionType.Role;
            ViewBag.ActionText = "Thêm";
            return View(model);
        }


        #endregion

        #region add module

        public ActionResult AjaxModuleForm()
        {
            ActionViewModel action = UpdateModelAction();
            MembershipAdmin memberShip = _membershipDa.GetAdminId(Guid.Parse(action.ItemId));
            UserAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                MembershipAdmin = memberShip,
                ListModuleAdmin = new List<ModuleAdmin>()
            };
            if (memberShip != null && !string.IsNullOrEmpty(memberShip.ModuleIds))
            {
                model.ListModuleAdmin = _moduleAdminDa.GetListAdminByArrId(memberShip.ModuleIds);
            }
            ViewBag.Action = ActionType.Role;
            ViewBag.ActionText = "Thêm";
            return View(model);
        }
        #endregion

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
            List<WebsiteModule> websiteModuleses = new();
            AspnetMembership obj = new();
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
                        AspnetUsers user = new();
                        TryUpdateModelAsync(user);
                        user.UserName = Utility.ValidString(user.UserName, "Code", true);
                        MembershipAdmin checkCode = _membershipDa.GetAdminUserName(user.UserName);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        var passwordNew = Request.Form["PasswordNew"];
                        obj.PasswordSalt = Utility.CreateSaltKey(8);
                        obj.Password = Utility.CreatePasswordHash(passwordNew, obj.PasswordSalt);
                        obj.ApplicationId = ConvertUtil.ToGuid("C6E5894C-95E4-4B21-9B5D-E86F27D7C862");
                        obj.UserId = Guid.NewGuid();
                        obj.IsApproved = true;
                        obj.CreateDate = DateTime.Now;
                        obj.LastLoginDate = DateTime.Now;
                        obj.LastPasswordChangedDate = DateTime.Now;
                        obj.LastLockoutDate = DateTime.Now;
                        obj.FailedPasswordAttemptWindowStart = DateTime.Now;
                        obj.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                        obj.Password = Utility.RemoveHTMLTag(obj.Password);
                        obj.PasswordSalt = Utility.RemoveHTMLTag(obj.PasswordSalt);
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Comment = Utility.RemoveHTMLTag(obj.Comment);
                        //Them vao aspnetUser
                        user.ApplicationId = ConvertUtil.ToGuid("C6E5894C-95E4-4B21-9B5D-E86F27D7C862");
                        user.UserId = obj.UserId;
                        user.LoweredUserName = user.UserName;
                        user.LastActivityDate = DateTime.Now;
                        int result = _membershipDa.InsertUser(user);
                        if (result > 0)
                        {
                            result = _membershipDa.Insert(obj);
                            if (result == 0)
                            {
                                return Ok(msg);
                            }
                        }
                        else
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Lỗi trong quá trình thêm mới."
                            };
                            return Ok(msg);
                        }
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
                        if (!SystemActionAdmin.Edit)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _membershipDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                        AspnetMembership objOld = obj;
                        TryUpdateModelAsync(obj);
                        AspnetUsers user = _membershipDa.GetUserId(ConvertUtil.ToGuid(action.ItemId));
                        obj.Password = Utility.RemoveHTMLTag(obj.Password);
                        obj.PasswordSalt = Utility.RemoveHTMLTag(obj.PasswordSalt);
                        TryUpdateModelAsync(user);
                        user.UserName = Utility.ValidString(user.UserName, "Code", true);
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Comment = Utility.RemoveHTMLTag(obj.Comment);
                        MembershipAdmin checkCode = _membershipDa.GetAdminUserName(user.UserName);
                        if (checkCode != null && checkCode.UserId != obj.UserId)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Tên đăng nhập tồn tại."
                            };
                            return Ok(msg);
                        }
                        var passwordNew = Request.Form["PasswordNew"];
                        if (!string.IsNullOrEmpty(obj.Password) && !passwordNew.Equals("######"))
                        {
                            obj.PasswordSalt = Utility.CreateSaltKey(8);
                            obj.Password = Utility.CreatePasswordHash(passwordNew, obj.PasswordSalt);
                        }
                        _membershipDa.Update(obj, obj.UserId);
                        _membershipDa.UpdateUser(user, obj.UserId);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Cập nhật thành công."
                        };
                        return Ok(msg);
                    }
                    catch
                    {

                    }
                    break;
                case ActionType.Role:
                    obj = _membershipDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                    TryUpdateModelAsync(obj);
                    if(obj.RoleCode.ToUpper() == "ADMIN")
                    {
                        obj.ModuleIds = null;
                        obj.DataJson = null;
                        obj.WebsiteModule = null;
                        obj.WebsiteModuleIds = null;
                    }
                    if(obj.ModuleIds != null)
                    {
                        obj.ModuleIds = obj.ModuleIds.TrimEnd(',').TrimStart(',');
                        List<ModuleAdmin> listModuleAdmin = _moduleAdminDa.GetListAdminByArrId(obj.ModuleIds);
                        obj.DataJson = JsonConvert.SerializeObject(listModuleAdmin);
                    }
                    _membershipDa.Update(obj, obj.UserId);
                    msg = new JsonMessage
                    {
                        Errors = false,
                        Message = "Bạn đã phân quyền thành công."
                    };
                    break;
                case ActionType.Active:
                    obj = _membershipDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                    List<ModuleAdmin> listModule = _moduleAdminDa.GetAdminAll();
                    List<ActiveRole> listActive = _activeRoleDa.GetAll();
                    if(listModule != null)
                    {
                        ActiveRoleAdmin active = new();
                        foreach (ModuleAdmin item in listModule)
                        {
                            item.DataJson = null;
                            var roleActive = Request.Form["Module" + item.ID];
                            if (!string.IsNullOrEmpty(roleActive))
                            {
                                List<string> listRoleActive = Utility.StringToListString(roleActive);
                                List<ActiveRole> listChecked = listActive.Where(c => listRoleActive.Contains(c.NameActive)).ToList();
                                string output = JsonConvert.SerializeObject(listChecked);
                                item.DataJson = output;
                            }
                        }
                        listModule = listModule.Where(c => c.DataJson != null).ToList();
                        string output2 = JsonConvert.SerializeObject(listModule);
                        obj.DataJson = output2;
                        _membershipDa.Update(obj, obj.UserId);
                    }
                    msg = new JsonMessage
                    {
                        Errors = false,
                        Message = "Bạn đã phân quyền thành công."
                    };
                    break;
                case ActionType.Delete:
                    obj = _membershipDa.GetId(ConvertUtil.ToGuid(action.ItemId));
                    AspnetUsers userdel = _membershipDa.GetUserId(ConvertUtil.ToGuid(action.ItemId));
                    _membershipDa.Delete(obj);
                    _membershipDa.DeleteUser(userdel);
                    msg = new JsonMessage
                    {
                        Errors = false,
                        Message = "Bạn đã phân quyền thành công."
                    };
                    break;
            }
            return Ok(msg);
        }
    }
}
