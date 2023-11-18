using System;
using System.Text;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using ADCOnline.Simple.Base;
using Website.Models;
using Microsoft.AspNetCore.Http;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;

namespace Website.Areas.Admin.Controllers
{
    public class ModuleController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly ActiveRoleDa _activeRoleDa;
        public ModuleController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _activeRoleDa = new ActiveRoleDa(WebConfig.ConnectionString);
        }

        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Module");
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
        public ActionResult AjaxTreeSelect(int type = 1, string ValuesSelected = null)
        {
            List<ModuleAdmin> ltsSourceModule = _moduleAdminDa.GetAdminAll();
            List<int> ltsValues = Utility.StringToListInt(ValuesSelected);
            StringBuilder stbHtml = new();
            bool multi = false;
            if (type == 1)
            {
                _moduleAdminDa.BuildTreeViewCheckBox(ltsSourceModule, 1, true, ltsValues, ref stbHtml);
                multi = Convert.ToBoolean(Request.Form["SelectMutil"]);
            }
            if (type == 2)
            {
                _moduleAdminDa.BuildTreeViewCheckBox(ltsSourceModule, 1, true, ltsValues, ref stbHtml);
            }
            ModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                StringBuilder = stbHtml.ToString(),
                SelectMutil = multi,
                Type = type
            };
            return View(model);
        }

        /// <summary>
        /// Load danh sách bản ghi dưới dạng bảng
        /// </summary>
        /// <returns></returns>
        public IActionResult ListItems()
        {
            List<ModuleAdmin> ltsSourceModule = _moduleAdminDa.GetAdminAll();
            StringBuilder stbHtml = new();
            _moduleAdminDa.BuildTreeView(ltsSourceModule, 1, false, ref stbHtml, true, true, true, true, true);
            ModuleViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                StringBuilder = stbHtml.ToString()
            };
            return View(model);
        }

        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            ModuleViewModel model = new()
            {
                Module = new Module(),
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _moduleAdminDa.GetAdminAll(true)
            };
            if (action.Do == ActionType.Edit)
                model.Module = _moduleAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
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
            Module obj = new();
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
                        obj.NameModule = Utility.ValidString(obj.NameModule, Title, true);
                        obj.Tag = Utility.ValidString(obj.Tag, Code, true);
                        obj.Redirect = Utility.ValidString(obj.Redirect, Link, true);
                        obj.Content = Utility.RemoveHTMLTag(obj.Content);
                        obj.DataJson = Utility.RemoveHTMLTag(obj.DataJson);
                        if (!string.IsNullOrEmpty(obj.Tag))
                        {
                            Module checkCode = _moduleAdminDa.GetTag(obj.Tag);
                            if (checkCode != null)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Link chuyên mục đã tồn tại. Tên thư mục là:" + checkCode.NameModule
                                };
                                return Ok(msg);
                            }
                        }
                        List<ActiveRole> listActive = _activeRoleDa.GetAll();
                        string output = JsonConvert.SerializeObject(listActive);
                        obj.DataJson = output;
                        int result = _moduleAdminDa.Insert(obj);
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
                        obj = _moduleAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        Module objOld = obj;
                        TryUpdateModelAsync(obj);
                        obj.NameModule = Utility.ValidString(obj.NameModule, Title, true);
                        obj.Tag = Utility.ValidString(obj.Tag, Code, true);
                        obj.Redirect = Utility.ValidString(obj.Redirect, Link, true);
                        obj.Content = Utility.RemoveHTMLTag(obj.Content);
                        obj.DataJson = Utility.RemoveHTMLTag(obj.DataJson);
                        if (!string.IsNullOrEmpty(obj.Tag) && obj.Tag != objOld.Tag)
                        {
                            Module checkCode = _moduleAdminDa.GetTag(obj.Tag);
                            if (checkCode != null && checkCode.ID != obj.ID)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Link chuyên mục đã tồn tại. Tên thư mục là:" + checkCode.NameModule
                                };
                                return Ok(msg);
                            }
                        }
                        int result = _moduleAdminDa.Update(obj);
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
                    catch
                    {

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
                        _moduleAdminDa.Delete(obj, "ID = " + action.ItemId);
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Bạn đã xóa thành công."
                        };
                        return Ok(msg);
                    }
                    catch
                    {

                    }
                    break;
                case ActionType.Display:
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
                        obj = _moduleAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = true;
                        int result = _moduleAdminDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Bạn đã hiển thị thành công."
                            };
                            return Ok(msg);
                        }
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
                        obj = _moduleAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = false;
                        int result = _moduleAdminDa.Update(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Bạn đã ẩn thành công."
                            };
                            return Ok(msg);
                        }
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
