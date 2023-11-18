using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Base;
using System.Collections.Generic;
using Website.Models;
using ADCOnline.Utils;
using System;
using ADCOnline.Simple.Admin;
using Website.Areas.Admin.ViewModels;
using System.Text;
using ADCOnline.Simple;
using Newtonsoft.Json;
using System.Linq;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.DA.Dapper;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class ModulePositionController : BaseController
    {
        private readonly ModulePositionDa _modulePositionDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly DapperDA _dapperDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public ModulePositionController()
        {
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            Module module = _moduleAdminDa.GetTag("ModulePosition");
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
            ModulePositionViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _modulePositionDa.ListSearch(seach, seach.page, 50, false),
                SearchModel = seach,
                ModulePosition = seach.contentId.HasValue ? _modulePositionDa.GetId(seach.contentId.Value) : new ModulePosition()
            };
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            ModulePositionViewModel module = new()
            {
                ModulePosition = new ModulePosition
                {
                    ParentId = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0
                },
                SystemActionAdmin = SystemActionAdmin,
                ListBaseItem = _modulePositionDa.ListAll(),
                ListModuleType = new List<ModuleType>(),
                ListWebsiteModule = new List<WebsiteModule>()
            };
            if (action.Do == ActionType.Edit)
            {
                ModulePosition modulePosition = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (modulePosition != null)
                {
                    module.ModulePosition = modulePosition;
                    List<WebsiteModule> lstModule = _websiteModuleDa.GetPositionIds(modulePosition.ID.ToString());
                    if (lstModule != null && lstModule.Count > 0)
                    {
                        module.ListWebsiteModule = lstModule;
                    }
                    if (!string.IsNullOrEmpty(modulePosition.ModuleTypeCode))
                    {
                        List<ModuleType> listModuleType = _moduleTypeDa.GetCode(Utility.StringToListString(modulePosition.ModuleTypeCode));
                        if (listModuleType != null && listModuleType.Count > 0)
                        {
                            module.ListModuleType = listModuleType;
                        }
                    }
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(module);
        }

        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                ActionViewModel action = UpdateModelAction();
                ModulePosition obj = new();
                switch (action.Do)
                {
                    case ActionType.Add:
                        try
                        {
                            if (SystemActionAdmin.Add != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            await TryUpdateModelAsync(obj);
                            #region Valid
                            obj.Name = Utility.ValidString(obj.Name, "", true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            obj.LinkBanner = Utility.ValidString(obj.LinkBanner, Link, true);
                            obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                            obj.ModuleTypeCode = Utility.ValidString(obj.ModuleTypeCode, Code, true);
                            obj.TypeView = Utility.ValidString(obj.TypeView, Code, true);
                            obj.UrlPicture = Utility.RemoveHTML(obj.UrlPicture);
                            obj.UrlPictureMobile = Utility.RemoveHTML(obj.UrlPictureMobile);
                            obj.Video = Utility.RemoveHTML(obj.Video);
                            obj.Icon = Utility.ValidString(obj.Icon, Link, true);
                            #endregion
                            var wh = Request.Form["Where"];
                            switch (wh)
                            {
                                case "4":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,4,%'";
                                        break;
                                    }
                                case "1":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,1,%'";
                                        break;
                                    }
                                case "3":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,3,%'";
                                        break;
                                    }
                                case "5":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,5,%'";
                                        break;
                                    }
                                default:
                                    {
                                        obj.SqlContent = string.Empty;
                                        break;
                                    }
                            }
                            var so = Request.Form["Sort"];
                            switch (so)
                            {
                                case "1":
                                    {
                                        obj.SqlContentOrderBy = " Order By OrderDisplay Asc";
                                        break;
                                    }
                                case "2":
                                    {
                                        obj.SqlContentOrderBy = " Order By OrderDisplay Desc";
                                        break;
                                    }
                                default:
                                    {
                                        obj.SqlContentOrderBy = string.Empty;
                                        break;
                                    }
                            }
                            obj.SqlModule = Utility.RemoveHTML(obj.SqlModule);
                            _modulePositionDa.UpdateWebsiteModuleIds(obj.ID.ToString(), obj.ModuleIds);
                            _modulePositionDa.UpdateWebsiteModule(obj.Code, obj.ModuleIds);
                            obj.IsDeleted = false;
                            _modulePositionDa.Insert(obj);                            
                            msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
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
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            ModulePosition objOld = obj;
                            await TryUpdateModelAsync(obj);
                            #region Valid
                            obj.Name = Utility.ValidString(obj.Name, "", true);
                            obj.Code = Utility.ValidString(obj.Code, Code, true);
                            obj.LinkBanner = Utility.ValidString(obj.LinkBanner, Link, true);
                            obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                            obj.ModuleTypeCode = Utility.ValidString(obj.ModuleTypeCode, Code, true);
                            obj.TypeView = Utility.ValidString(obj.TypeView, Code, true);
                            obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                            obj.UrlPictureMobile = Utility.ValidString(obj.UrlPictureMobile, Link, true);
                            obj.Video = Utility.RemoveHTML(obj.Video);
                            #endregion
                            var wh = Request.Form["Where"];
                            switch (wh)
                            {
                                case "4":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,4,%'";
                                        break;
                                    }
                                case "1":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,1,%'";
                                        break;
                                    }
                                case "3":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,3,%'";
                                        break;
                                    }
                                case "5":
                                    {
                                        obj.SqlContent = " And ',' + ViewHome + ',' like N'%,5,%'";
                                        break;
                                    }
                                default:
                                    {
                                        obj.SqlContent = string.Empty;
                                        break;
                                    }
                            }
                            var so = Request.Form["Sort"];
                            switch (so)
                            {
                                case "1":
                                    {
                                        obj.SqlContentOrderBy = " Order By OrderDisplay Asc";
                                        break;
                                    }
                                case "2":
                                    {
                                        obj.SqlContentOrderBy = " Order By OrderDisplay Desc";
                                        break;
                                    }
                                default:
                                    {
                                        obj.SqlContentOrderBy = string.Empty;
                                        break;
                                    }
                            }
                            obj.SqlModule = Utility.RemoveHTML(obj.SqlModule);
                            var UrlPicture = Request.Form["UrlPicture"];
                            var Icon = Request.Form["Icon"];
                            obj.UrlPicture = Utility.ValidString(UrlPicture, Link, true);
                            obj.Icon = Utility.ValidString(Icon, Link, true); ;
                            //update position website module
                            _modulePositionDa.UpdateWebsiteModuleIds(obj.ID.ToString(), obj.ModuleIds);
                            _modulePositionDa.UpdateWebsiteModule(obj.Code, obj.ModuleIds);
                            obj.IsDeleted = false;
                            int result = _modulePositionDa.Update(obj);                            
                            if (result > 0)
                            {
                                msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Obj = obj };
                            }
                            return Ok(msg);
                        }
                        catch(Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.Delete:
                        try
                        {
                            if (SystemActionAdmin.Delete != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }                            
                            ModulePosition objItem = _modulePositionDa.GetId(Convert.ToInt32(action.ItemId));
                            string code = objItem.Code;                            
                            _modulePositionDa.Delete(obj, " ID =" + action.ItemId);
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.Hidden:
                        try
                        {
                            if (SystemActionAdmin.Active != true)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            obj = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            obj.IsShow = obj.IsShow == true ? false : true;
                            var message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị vị trí" : "Ẩn vị trí";
                            var result = _modulePositionDa.Update(obj);                            
                            AddLogAdmin(Request.Path, message + obj.Name, ConvertUtil.ToBool(obj.IsShow) ? "Actions-hiện" : "Actions-Ẩn");
                            msg = new JsonMessage { Errors = false, Message = message };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
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
                                ModulePosition content = _modulePositionDa.GetId(item);
                                content.IsShow = true;
                                _modulePositionDa.Update(content);                                
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
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                ModulePosition content = _modulePositionDa.GetId(item);
                                content.IsShow = false;
                                _modulePositionDa.Update(content);                                
                            }
                            msg = new JsonMessage { Errors = false, Message = "Ẩn thành công." };
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
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                ModulePosition content = _modulePositionDa.GetId(item);
                                content.IsDeleted = true;
                                _modulePositionDa.Update(content);                                
                            }
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
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
                                        ModulePosition content = _modulePositionDa.GetId(item.ID);
                                        content.OrderDisplay = item.OrderDisplay;
                                        _modulePositionDa.Update(content);                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogError(ex);
                                    }
                                }
                                msg = new JsonMessage { Errors = false, Message = "Cập nhật thứ tự thành công." };
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
        public ActionResult AjaxTreeSelect(int? typeId)
        {
            List<ModulePositionAdmin> ltsSourcewebsiteModule = _modulePositionDa.ListAdminIsShow();
            List<string> ltsValues = Utility.StringToListString(Request.Query["ValuesSelected"]);
            StringBuilder stbHtml = new();
            _modulePositionDa.BuildTreeViewCheckBox(ltsSourcewebsiteModule, 0, true, ltsValues, ref stbHtml);
            ModulePositionViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                StringBuilder = stbHtml.ToString()
            };
            return View(model);
        }
    }
}
