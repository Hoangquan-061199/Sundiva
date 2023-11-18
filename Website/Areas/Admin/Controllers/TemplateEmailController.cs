using System;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Simple.Admin;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using ADCOnline.Utils;
using Website.Utils;
using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using ADCOnline.Simple.Base;

namespace Website.Areas.Admin.Controllers
{
    public class TemplateEmailController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        public TemplateEmailController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("TemplateEmail");
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
            ViewBag.IsExport = false;
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            List<CommonJsonAdmin> listItem = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TemplateEmail.json", "DataJson"));
            ContentDataJsonViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                GetListItem = listItem,
                SearchModel = seach,
                CommonJsonAdmin = seach.contentId.HasValue ? listItem.FirstOrDefault(c => c.ID == seach.contentId) : new CommonJsonAdmin()
            };            
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            ContentDataJsonViewModel model = new()
            {
                CommonJsonAdmin = new CommonJsonAdmin(),
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                List<CommonJsonAdmin> list = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TemplateEmail.json", "DataJson"));
                model.CommonJsonAdmin = list.FirstOrDefault(c => c.ID == ConvertUtil.ToInt32(action.ItemId));
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
            CommonJsonAdmin obj = new();
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
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TemplateEmail.json","DataJson"));
                        if (listCommon == null)
                        {
                            listCommon = new List<CommonJsonAdmin>();
                        }
                        obj.ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                        obj.CreatedDate = DateTime.Now;
                        listCommon.Add(obj);
                        listCommon = listCommon.OrderBy(c => c.OrderDisplay).ThenBy(c => c.ID).ToList();
                        Common.CreateFileJson(0, listCommon, "TemplateEmail", "DataJson");
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), listCommon);
                        AddLogAdmin(Request.Path, "Thêm mới Dữ liệu Json", "Actions-Add");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Thêm mới thành công.",
                            Obj = new WebsiteContentAdmin
                            {
                                ID = obj.ID
                            }
                        };
                        return Ok(msg);
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
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TemplateEmail.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrID.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            TryUpdateModelAsync(obj);
                            obj.Name = Utility.ValidString(obj.Name, Title, true);
                            obj.code = Utility.ValidString(obj.code, Code, true);
                            listCommon.Add(obj);
                            listCommon = listCommon.OrderBy(c => c.OrderDisplay).ThenBy(c => c.ID).ToList();
                            Common.CreateFileJson(0, listCommon, "TemplateEmail", "DataJson");
                            AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Sửa template", "Actions-Edit");
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Cập nhật thành công.",
                                Obj = new WebsiteContentAdmin
                                {
                                    ID = obj.ID
                                }
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
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TemplateEmail.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrID.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            Common.CreateFileJson(0, listCommon, "TemplateEmail", "DataJson");
                            AddLogEdit(Request.Path, "Delete", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Xóa template", "Actions-Delete");
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Xóa thành công."
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
