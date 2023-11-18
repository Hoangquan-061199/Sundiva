using System;
using System.Collections.Generic;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using Website.Utils;
using System.Linq;

namespace Website.Areas.Admin.Controllers
{
    public class MemberAdminController : BaseController
    {
        private readonly MemberDa _MemberDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public MemberAdminController()
        {
            _MemberDa = new MemberDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("MemberAdmin");
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
            List<MemberAdmin> list = _MemberDa.ListSearch(seach, seach.page, 50, ViewBag.IsExport);
            MemberViewModel model = new()
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
            MemberViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Member = new Member(),
            };
            if (action.Do == ActionType.Edit)
            {
                model.Member = _MemberDa.GetId(ConvertUtil.ToInt32(action.ItemId));
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
            Member obj = new();
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
                        if (Utility.IsValidEmail(obj.Email) == false)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Email không đúng"
                            };
                            return Ok(msg);
                        }
                        obj.UserName = Utility.ValidString(obj.UserName,Code, true);
                        obj.FullName = Utility.ValidString(obj.FullName, "", true);
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Title, true);
                        obj.FaceBookId = Utility.ValidString(obj.FaceBookId, Link, true);
                        obj.GoogleId = Utility.ValidString(obj.GoogleId, Link, true);
                        obj.Address = Utility.ValidAddress(obj.Address);
                        Member checkCode = _MemberDa.GetUserName(obj.UserName);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã khách hàng đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        obj.IsDeleted = false;
                        obj.CreatedDate = DateTime.Now;
                        int result = _MemberDa.Insert(obj);
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
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _MemberDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        AddLogEdit("Edit", obj.ID.ToString(), session.GetAdminUserId(), obj);
                        TryUpdateModelAsync(obj);
                        if (Utility.IsValidEmail(obj.Email) == false)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Email không đúng"
                            };
                            return Ok(msg);
                        }
                        obj.UserName = Utility.ValidString(obj.UserName, Code, true);
                        obj.FullName = Utility.ValidString(obj.FullName, "", true);
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Title, true);
                        obj.FaceBookId = Utility.ValidString(obj.FaceBookId, Link, true);
                        obj.GoogleId = Utility.ValidString(obj.GoogleId, Link, true);
                        obj.Address = Utility.ValidAddress(obj.Address);
                        Member checkCode = _MemberDa.GetUserName(obj.UserName);
                        if (checkCode != null && checkCode.ID != obj.ID)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã khách hàng đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        int result = _MemberDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Cập nhật thành công."
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
                        obj = _MemberDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        _MemberDa.Update(obj);
                        AddLogEdit("Delete", obj.ID.ToString(), session.GetAdminUserId(), obj);
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
                        AddLogEdit("Delete", obj.ID.ToString(), session.GetAdminUserId(), obj);
                        foreach (int item in ArrID)
                        {
                            obj = _MemberDa.GetId(item);
                            obj.IsDeleted = true;
                            _MemberDa.Update(obj);
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
                case ActionType.Active:
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
                        obj = _MemberDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.Status = 1;
                        _MemberDa.Update(obj);
                        AddLogEdit("Active", obj.ID.ToString(), session.GetAdminUserId(), obj);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Thêm mới thành công."
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
