using System;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using ADCOnline.Utils;
using Website.Utils;
using ADCOnline.Simple;
using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using ADCOnline.Simple.Base;

namespace Website.Areas.Admin.Controllers
{
    public class ResourceController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        public ResourceController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Resource");
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
            var type = Request.Query["TypeJson"];
            if (string.IsNullOrEmpty(type))
            {
                type = "vi";
            }
            ViewBag.Type = type;
            SearchModel seach = new();
            ViewBag.IsExport = false;
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            if (seach.IsExcel == true || seach.IsPdf == true || seach.IsCsv == true)
            {
                ViewBag.IsExport = true;
            }
            Dictionary<string, string> listItem = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + type + ".json", "DataJson/Resource"));
            if (!string.IsNullOrEmpty(seach.keyword))
            {
                listItem = listItem.Where(x => x.Key.ToLower().Contains(seach.keyword.ToLower()) || x.Value.ToLower().Contains(seach.keyword.ToLower())).ToDictionary(i => i.Key, i => i.Value);
            }
            int total = listItem != null ? listItem.Count : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 20);
            ResourceViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                GetListItem = listItem != null ? listItem.OrderBy(x => x.Key).Skip((seach.page - 1) * 20).Take(20).ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, string>(),
                Lang = Lang(),
                Keys = seach.ItemID
            };
            if (!string.IsNullOrEmpty(type))
            {
                return View("~/Areas/Admin/Views/Resource/ListResource.cshtml", model);
            }
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            var type = Request.Query["TypeJson"].ToString();
            ActionViewModel action = UpdateModelAction();
            ResourceViewModel model = new()
            {
                TypeJson = type,
                SystemActionAdmin = SystemActionAdmin
            };
            if (action.Do == ActionType.Edit)
            {
                Dictionary<string, string> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + type + ".json", "DataJson/Resource"));
                if (list.ContainsKey(action.ItemId))
                {
                    model.Keys = action.ItemId;
                    model.Values = list[action.ItemId];
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
                        var value = Request.Form["Values"];
                        Dictionary<string, string> listCommon = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + Request.Form["TypeJson"] + ".json", "DataJson/Resource"));
                        if (listCommon == null)
                        {
                            listCommon = new Dictionary<string, string>
                            {
                                { Utility.ValidString(action.Keys, Code, true), Utility.ValidString(value , Title, true) }
                            };
                        }
                        else
                        {
                            if (listCommon.ContainsKey(action.Keys))
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Mã này đã tồn tại."
                                };
                            }
                            else
                            {
                                listCommon.Add(Utility.ValidString(action.Keys, Code, true), Utility.ValidString(value, Title, true));
                            }
                        }
                        Common.CreateFileJson(0, listCommon, "Resources_" + Request.Form["TypeJson"], "DataJson/Resource");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Thêm mới thành công.",
                            Obj = new ObjJson
                            {
                                ID = action.Keys
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
                        var value = Request.Form["values"];
                        Dictionary<string, string> listCommon = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + Request.Form["TypeJson"] + ".json", "DataJson/Resource"));
                        if (listCommon == null)
                        {
                            listCommon = new Dictionary<string, string>
                            {
                                { Utility.ValidString(action.Keys, Code, true), Utility.ValidString(value , Title, true) }
                            };
                        }
                        else
                        {
                            if (listCommon.ContainsKey(action.Keys))
                            {
                                listCommon[action.Keys] = Utility.ValidString(value, Title, true);
                            }
                            else
                            {
                                listCommon.Add(Utility.ValidString(action.Keys, Code, true), Utility.ValidString(value, Title, true));
                            }
                        }
                        Common.CreateFileJson(0, listCommon, "Resources_" + Request.Form["TypeJson"], "DataJson/Resource");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Cập nhật thành công.",
                            Obj = new ObjJson
                            {
                                ID = action.Keys
                            }
                        };
                        return Ok(msg);
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
                        Dictionary<string, string> listCommon = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + Request.Form["TypeJson"] + ".json", "DataJson/Resource"));
                        if (listCommon != null)
                        {
                            listCommon.Remove(action.ItemId);
                            Common.CreateFileJson(0, listCommon, "Resources_" + Request.Form["TypeJson"], "DataJson/Resource");
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
                        var type = Request.Form["TypeJson"];
                        Dictionary<string, string> listCommon = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + type + ".json", "DataJson/Resource"));
                        if (listCommon != null)
                        {
                            foreach (string item in ArrKey)
                            {
                                listCommon.Remove(item);
                            }
                            Common.CreateFileJson(0, listCommon, "Resources_" + type, "DataJson/Resource");
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
                case ActionType.ChangeSource:
                    try
                    {
                        if (!SystemActionAdmin.Edit)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        if (!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                        {
                            var orderValues = Request.Form["OrderByValues"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            string type = listOrderByItem.Any() ? listOrderByItem.FirstOrDefault().Type : string.Empty;
                            Dictionary<string, string> listCommon = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + type + ".json", "DataJson/Resource"));
                            foreach (OrderByItem item in listOrderByItem)
                            {
                                try
                                {
                                    if (listCommon.ContainsKey(item.Key))
                                    {
                                        listCommon[item.Key] = Utility.ValidString(item.Price, Title, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                            Common.CreateFileJson(0, listCommon, "Resources_" + type, "DataJson/Resource");
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
            }
            return Ok(msg);
        }
    }
}