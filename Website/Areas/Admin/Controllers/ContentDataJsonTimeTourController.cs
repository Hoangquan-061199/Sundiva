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
    public class ContentDataJsonTimeTourController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        public ContentDataJsonTimeTourController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("ContentDataJsonTimeTour");
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

        /// <summary>
        /// Load danh sách bản ghi dưới dạng bảng
        /// </summary>
        /// <returns></returns>
        public IActionResult ListItems()
        {
            ViewBag.Type = "TimeTour";
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            List<CommonJsonAdmin> listItem = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
            ContentDataJsonViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                GetListItem = listItem,
                SearchModel = seach,
                Lang = Lang(),
                CommonJsonAdmin = seach.contentId.HasValue ? listItem.FirstOrDefault(c => c.ID == seach.contentId) : new CommonJsonAdmin()
            };
            return View("~/Areas/Admin/Views/ContentDataJsonTimeTour/ListItems.cshtml", model);
        }

        /// <summary>
        /// Form dùng cho thêm mới, sửa. Load bằng Ajax dialog
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxForm()
        {
            string type = "TimeTour";
            ActionViewModel action = UpdateModelAction();
            ContentDataJsonViewModel model = new()
            {
                CommonJsonAdmin = new CommonJsonAdmin(),
                SystemActionAdmin = SystemActionAdmin,
                TypeJson = type
            };
            if (action.Do == ActionType.Edit)
            {
                List<CommonJsonAdmin> list = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
                model.CommonJsonAdmin = list.FirstOrDefault(c => c.ID == ConvertUtil.ToInt32(action.ItemId));
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            if (!string.IsNullOrEmpty(type))
            {
                return View("~/Areas/Admin/Views/ContentDataJsonTimeTour/AjaxForm.cshtml", model);
            }
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
                        obj.NameEn = Utility.ValidString(obj.NameEn, Title, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.UrlImg = Utility.ValidString(obj.UrlImg, Link, true);
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
                        if (listCommon == null)
                        {
                            listCommon = new List<CommonJsonAdmin>();
                        }
                        obj.ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss"));
                        obj.CreatedDate = DateTime.Now;
                        listCommon.Add(obj);
                        listCommon = listCommon.OrderBy(c => c.OrderDisplay).ThenBy(c => c.ID).ToList();
                        Common.CreateFileJson(0, listCommon, "TimeTour", "DataJson");
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), listCommon);
                        AddLogAdmin(Request.Path, "Thêm mới", "Actions-Add");
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
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrID.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            TryUpdateModelAsync(obj);
                            obj.Name = Utility.ValidString(obj.Name, Title, true);
                            obj.NameEn = Utility.ValidString(obj.NameEn, Title, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            obj.UrlImg = Utility.ValidString(obj.UrlImg, Link, true);
                            obj.Content = obj.Content;
                            listCommon.Add(obj);
                            listCommon = listCommon.OrderBy(c => c.OrderDisplay).ThenBy(c => c.ID).ToList();
                            Common.CreateFileJson(0, listCommon, "TimeTour", "DataJson");
                            AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Sửa", "Actions-Edit");
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
                        List<CommonJsonAdmin> listCommon = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrID.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            Common.CreateFileJson(0, listCommon, "TimeTour", "DataJson");
                            AddLogEdit(Request.Path, "Delete", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Xóa", "Actions-Delete");
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
