using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using ADCOnline.Simple.Base;
using Website.Models;
using ADCOnline.Simple.Admin;
using Microsoft.AspNetCore.Http;
using ADCOnline.Simple;

namespace Website.Areas.Admin.Controllers
{
    public class AttributesController : BaseController
    {
        private readonly AttributesDa _AttributesDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public AttributesController()
        {
            _AttributesDa = new AttributesDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            Module module = _moduleAdminDa.GetTag("Attributes");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            AttributesViewModel model = new()
            {
                SearchModel = search,
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _AttributesDa.GetAdminAll(true, Lang()),
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                ListAttributeType = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("AttributeType.json", "DataJson")),
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
            SearchModel seach = new ();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            if (string.IsNullOrEmpty(seach.keyword))
            {
                seach.parentId = seach.parentId > 0 ? seach.parentId : 0;
            }
            seach.lang = Lang();
            ViewBag.IsExport = false;
            if (seach.IsExcel == true || seach.IsPdf == true || seach.IsCsv == true)
            {
                ViewBag.IsExport = true;
            }
            AttributesViewModel model = new ()
            {
                Attributes = seach.parentId.HasValue ? _AttributesDa.GetId(seach.parentId.Value) : new Attributes(),
                ListAttrbuteCode = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("AttributeCode.json", "DataJson")),
                ListAttributeType = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("AttributeType.json", "DataJson")),
                ListItem = _AttributesDa.ListSearch(seach, seach.page, 50, ViewBag.IsExport),
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                AttributesItem = seach.contentId.HasValue ? _AttributesDa.GetId(seach.contentId.Value) : new Attributes()
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 50);
            return View(model);
        }
        public ActionResult QuickAdd(int parent, string moduleIds)
        {
            ActionViewModel action = UpdateModelAction();
            AttributesViewModel model = new ()
            {
                Attributes = new Attributes()
                {
                    ParentID = parent,
                    ListModuleIds = moduleIds
                },
                ListAttrbuteCode = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("AttributeCode.json", "DataJson")),
                ListAttributeType = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("AttributeType.json", "DataJson")),
                ListItem = _AttributesDa.GetAdminAll(false, Lang()),
                SystemActionAdmin = SystemActionAdmin,
                ListWebsiteModule = new List<WebsiteModule>()
            };
            //modules
            List<WebsiteModule> modules = _websiteModuleDa.GetByAttr("," + moduleIds + ",");
            if (modules != null && modules.Count > 0)
            {
                model.ListWebsiteModule = modules;
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            AttributesViewModel model = new()
            {
                Attributes = new Attributes(),
                ListAttrbuteCode = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("AttributeCode.json", "DataJson")),
                ListAttributeType = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("AttributeType.json", "DataJson")),
                ListItem = _AttributesDa.GetAdminAll(false, Lang()),
                SystemActionAdmin = SystemActionAdmin,
                ListWebsiteModule = new List<WebsiteModule>()
            };
            if (action.Do == ActionType.Edit)
            {
                Attributes attributes = _AttributesDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                model.Attributes = attributes;
                //modules
                List<WebsiteModule> modules = _websiteModuleDa.GetByAttr("," + attributes.ListModuleIds + ",");
                if (modules != null && modules.Count > 0)
                {
                    model.ListWebsiteModule = modules;
                }
            }
            else
            {
                model.Attributes.ParentID = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0;
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Actions()
        {
            JsonMessage msg = new JsonMessage
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            try
            {
                ActionViewModel action = UpdateModelAction();
                Attributes obj = new Attributes();
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
                            await TryUpdateModelAsync(obj);
                            #region Valid Input
                            obj.Name = Utility.RemoveHTML(obj.Name.Trim());
                            obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                            obj.ClassCss = Utility.ConvertRewrite(obj.ClassCss);
                            obj.Description = Utility.RemoveHTML(obj.Description);
                            obj.ListModuleIds = Utility.ValidString(obj.ListModuleIds, ArrayInt, true);
                            obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                            obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                            obj.UrlPicture2 = Utility.ValidString(obj.UrlPicture2, Link, true);
                            obj.Weight = ConvertUtil.ToDouble(obj.Weight);
                            obj.TypeShow = ConvertUtil.ToInt32(obj.TypeShow);
                            obj.Type = Utility.ValidString(obj.Type, Code, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            #endregion
                            obj.Lang = Lang();
                            if (obj.ParentID == null) obj.ParentID = 0;
                            obj.IsDeleted = false;
                            obj.Name = obj.Name.Trim();
                            obj.Description = obj.Description;
                            obj.CreatedDate = DateTime.Now;
                            obj.ParentID = ConvertUtil.ToInt32(obj.ParentID);
                            Attributes checkattr = _AttributesDa.GetByNameOption(obj.Name, obj.ParentID.Value);
                            if (checkattr != null)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Thuộc tính này đã tồn tại"
                                };
                                return Ok(msg);
                            }
                            var result = _AttributesDa.Insert(obj);
                            #region Update Attribute Modules
                            var lstModuleIds = Request.Form["ListModuleIds"];
                            if (!string.IsNullOrEmpty(lstModuleIds))
                            {
                                List<WebsiteModule> lstModules = _websiteModuleDa.GetByAttr(obj.ListModuleIds);
                                foreach (WebsiteModule item in lstModules)
                                {
                                    string attrNew = string.Empty;
                                    attrNew = string.IsNullOrEmpty(item.AttributeModuleIds) ? obj.ID.ToString() : item.AttributeModuleIds + "," + obj.ID;
                                    _websiteModuleDa.UpdateAttrModule(attrNew, item.ID.ToString());
                                }
                            }
                            #endregion
                            int resultU = _AttributesDa.Update(obj);
                            AddLogAdmin(Request.Path, "Thêm mới thuộc tính:" + obj.Name, "Actions-Add");
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
                            obj = _AttributesDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            AddLogEdit("Edit", obj.ID.ToString(), session.GetAdminUserName(), obj);
                            string modulesOlds = obj.ListModuleIds;
                            await TryUpdateModelAsync(obj);
                            #region Valid Input
                            obj.Name = Utility.RemoveHTML(obj.Name.Trim());
                            obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                            obj.ClassCss = Utility.ConvertRewrite(obj.ClassCss);
                            obj.Description = Utility.RemoveHTML(obj.Description);
                            obj.ListModuleIds = Utility.ValidString(obj.ListModuleIds, ArrayInt, true);
                            obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                            obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                            obj.UrlPicture2 = Utility.ValidString(obj.UrlPicture2, Link, true);
                            obj.Weight = ConvertUtil.ToDouble(obj.Weight);
                            obj.Type = Utility.ValidString(obj.Type, Code, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            #endregion
                            if (obj.ParentID == null) obj.ParentID = 0;
                            int result = _AttributesDa.Update(obj);
                            #region Update Attribute Modules
                            var lstModuleIds = Request.Form["ListModuleIds"];
                            if (!string.IsNullOrEmpty(modulesOlds))
                            {
                                List<WebsiteModule> lstModulesOld = _websiteModuleDa.GetByAttr(modulesOlds);
                                foreach (WebsiteModule item in lstModulesOld)
                                {
                                    item.AttributeModuleIds = ("," + item.AttributeModuleIds + ",").Replace("," + obj.ID + ",", ",").Trim(',');
                                    _websiteModuleDa.UpdateAttrModule(item.AttributeModuleIds, item.ID.ToString());
                                }
                            }
                            if (!string.IsNullOrEmpty(lstModuleIds))
                            {
                                List<WebsiteModule> lstModulesNew = _websiteModuleDa.GetByAttr(lstModuleIds);
                                if (lstModulesNew.Any())
                                {
                                    foreach (WebsiteModule item in lstModulesNew)
                                    {
                                        item.AttributeModuleIds = string.IsNullOrEmpty(item.AttributeModuleIds) ? obj.ID.ToString() : item.AttributeModuleIds + "," + obj.ID;
                                        _websiteModuleDa.UpdateAttrModule(item.AttributeModuleIds, item.ID.ToString());
                                    }
                                }
                            }
                            //string moduleIdsAll = modulesOlds + "," + lstModuleIds;
                            //if (!string.IsNullOrEmpty(moduleIdsAll))
                            //{
                            //    var lstModules = _websiteModuleDa.GetByAttr(moduleIdsAll);
                            //    foreach (var item in lstModules)
                            //    {
                            //        string attrNew = "";
                            //        if (string.IsNullOrEmpty(item.AttributeModuleIds))
                            //        {
                            //            attrNew = obj.ID + "";
                            //        }
                            //        else
                            //        {
                            //            var attrByModuleId = _AttributesDa.GetListAttrIdsByModuleId(item.ID.ToString());
                            //            attrNew = string.Join(",", attrByModuleId.Select(c => c.ID).ToList());
                            //        }
                            //      _websiteModuleDa.UpdateAttrModule(attrNew, item.ID.ToString());
                            //    }
                            //}
                            #endregion
                            AddLogAdmin(Request.Path, "Sửa thuộc tính:" + obj.Name, "Actions-Edit");
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
                        catch
                        {

                        }
                        break;
                    case ActionType.AddQuick:
                        try
                        {
                            //if (SystemActionAdmin.AddQuick != true)
                            //{
                            //    msg = new JsonMessage
                            //    {
                            //        errors = true,
                            //        message = "Bạn chưa được phân quyền cho chức năng này."
                            //    };
                            //    return Ok(msg);
                            //}
                            await TryUpdateModelAsync(obj);
                            #region Valid Input
                            obj.Name = Utility.RemoveHTML(obj.Name.Trim());
                            obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                            obj.ClassCss = Utility.ConvertRewrite(obj.ClassCss);
                            obj.Description = Utility.RemoveHTML(obj.Description);
                            obj.ListModuleIds = Utility.ValidString(obj.ListModuleIds, ArrayInt, true);
                            obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                            var image = Request.Form["UrlPictureAttr"];
                            obj.UrlPicture = Utility.ValidString(image, Link, true);
                            obj.UrlPicture2 = Utility.ValidString(obj.UrlPicture2, Link, true);
                            obj.Weight = ConvertUtil.ToDouble(obj.Weight);
                            obj.Type = Utility.ValidString(obj.Type, Code, true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            #endregion
                            obj.Lang = Lang();
                            if (obj.ParentID == null) obj.ParentID = 0;
                            obj.IsDeleted = false;
                            obj.Name = obj.Name.Trim();
                            obj.Description = obj.Description;
                            obj.CreatedDate = DateTime.Now;
                            obj.ParentID = ConvertUtil.ToInt32(obj.ParentID);
                            Attributes checkattr = _AttributesDa.GetByNameOption(obj.Name, obj.ParentID.Value);
                            if (checkattr != null)
                            {
                                //neu thuoc tinh co roi, nhung chua chon module cua san pham nay?
                                var lstModuleIdsT = Request.Form["ListModuleIds"];//list modules cua san pham dang sua
                                string checkattrModuleIds = checkattr.ListModuleIds;//list modules cua thuoc tinh da ton tai
                                //lay ra danh sach id modules cua san pham dang sua co ma modules cua thuoc tinh da ton tai kia chu co
                                string diff = String.Empty;
                                List<string> lstModuleIdsTcheck = ListHelper.GetValuesArrayTag(lstModuleIdsT);
                                foreach (string item in lstModuleIdsTcheck)
                                {
                                    if (!("," + checkattrModuleIds + ",").Contains("," + item + ","))
                                    {
                                        diff = diff + "," + item;
                                    }
                                }
                                if (string.IsNullOrEmpty(diff))
                                {
                                    msg = new JsonMessage
                                    {
                                        Errors = true,
                                        Message = "Thuộc tính này đã tồn tại"
                                    };
                                    return Ok(msg);
                                }
                                else
                                {
                                    Attributes attr = _AttributesDa.GetId(checkattr.ID);
                                    attr.ListModuleIds += diff;
                                    int result1 = _AttributesDa.Update(attr);
                                    if (!string.IsNullOrEmpty(diff))
                                    {
                                        List<WebsiteModule> lstModulesNew = _websiteModuleDa.GetByAttr(diff);
                                        if (lstModulesNew.Any())
                                        {
                                            foreach (WebsiteModule item in lstModulesNew)
                                            {
                                                item.AttributeModuleIds = string.IsNullOrEmpty(item.AttributeModuleIds) ? obj.ID.ToString() : item.AttributeModuleIds + "," + obj.ID;
                                                _websiteModuleDa.UpdateAttrModule(item.AttributeModuleIds, item.ID.ToString());
                                            }
                                        }
                                    }
                                    if (result1 > 0)
                                    {
                                        msg = new JsonMessage
                                        {
                                            Errors = false,
                                            Message = "Cập nhật thành công.",
                                            Id = obj.ID.ToString(),
                                            Name = obj.Name,
                                        };
                                        return Ok(msg);
                                    }
                                }
                            }
                            int result = _AttributesDa.Insert(obj);
                            #region Update Attribute Modules
                            var lstModuleIds = Request.Form["ListModuleIds"];
                            if (!string.IsNullOrEmpty(lstModuleIds))
                            {
                                List<WebsiteModule> lstModules = _websiteModuleDa.GetByAttr(obj.ListModuleIds);
                                foreach (WebsiteModule item in lstModules)
                                {
                                    string attrNew = string.IsNullOrEmpty(item.AttributeModuleIds) ? obj.ID.ToString() : item.AttributeModuleIds + "," + obj.ID;
                                    _websiteModuleDa.UpdateAttrModule(attrNew, item.ID.ToString());
                                }
                            }
                            #endregion
                            int resultU = _AttributesDa.Update(obj);
                            AddLogAdmin(Request.Path, "Thêm mới thuộc tính:" + obj.Name, "Actions-Add");
                            if (result > 0)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = false,
                                    Id = obj.ID.ToString(),
                                    Name = obj.Name,
                                    Message = "Thêm mới thành công."
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
                            obj = _AttributesDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            await TryUpdateModelAsync(obj);
                            obj.IsDeleted = true;
                            _AttributesDa.Update(obj);
                            AddLogAdmin(Request.Path, "Xóa thuộc tính:" + obj.Name, "Actions-Delete");
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Xóa thành công."
                            };
                            return Ok(msg);
                        }
                        catch
                        {

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
                            obj = _AttributesDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            obj.IsShow = true;
                            _AttributesDa.Update(obj);
                            AddLogAdmin(Request.Path, "Hiển thị thuộc tính:" + obj.Name, "Actions-Show");
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Bạn đã hiển thị thành công.",
                                Obj = obj
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
                            obj = _AttributesDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            obj.IsShow = false;
                            _AttributesDa.Update(obj);
                            AddLogAdmin(Request.Path, "Ẩn thuộc tính:" + obj.Name, "Actions-Show");
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Bạn đã ẩn thành công.",
                                Obj = obj
                            };
                            return Ok(msg);
                        }
                        catch
                        {

                        }
                        break;
                    case ActionType.ShowAll:
                        try
                        {
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                Attributes content = _AttributesDa.GetId(item);
                                content.IsShow = true;
                                _AttributesDa.Update(content);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Hiện thị thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.HiddenAll:
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
                            foreach (int item in ArrID)
                            {
                                Attributes content = _AttributesDa.GetId(item);
                                content.IsShow = false;
                                _AttributesDa.Update(content);
                            }
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Ẩn thành công."
                            };
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
                            if (SystemActionAdmin.Delete != true)
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
                                Attributes content = _AttributesDa.GetId(item);
                                content.IsDeleted = true;
                                _AttributesDa.Update(content);
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
                    case ActionType.OrderBy:
                        try
                        {
                            if (!SystemActionAdmin.Order)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            if (!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                            {
                                var orderValues = Request.Form["OrderByValues"];
                                List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                                foreach (OrderByItem item in listOrderByItem)
                                {
                                    try
                                    {
                                        Attributes content = _AttributesDa.GetId(item.ID);
                                        content.OrderDisplay = item.OrderDisplay;
                                        _AttributesDa.Update(content);
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogError(ex);
                                    }
                                }
                                msg = new JsonMessage
                                {
                                    Errors = false,
                                    Message = "Cập nhật thứ tự thành công."
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
            }
            catch (Exception ex)
            {
                AddLogError(ex);
            }
            return Ok(msg);
        }        
    }
}
