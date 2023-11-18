using System.Collections.Generic;
using System.Linq;
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

namespace Website.Areas.Admin.Controllers
{
    public class WebsiteModuleProductController : BaseController
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly ActiveRoleDa _activeRoleDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly ModulePositionDa _modulePositionDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly ProductDa _productDa;
        private readonly AttributesDa _attributesDa;
        private readonly AdvertisingDa _advertisingDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public WebsiteModuleProductController()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _activeRoleDa = new ActiveRoleDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _attributesDa = new AttributesDa(WebConfig.ConnectionString);
            _advertisingDa = new AdvertisingDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("WebsiteModuleProduct");
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
            SearchModel seach = new()
            {
                lang = Lang()
            };
            TryUpdateModelAsync(seach);
            string type = $"{StaticEnum.Product}";
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            seach.ModuleIds = moduleIds;
            List<WebsiteModuleAdmin> ltsSourceModule = _websiteModuleDa.GetAdminAllFilter(seach, Lang(), string.Empty, type);   
            WebsiteModuleViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                ListItem = ltsSourceModule,
                WebsiteModule = seach.ModuleId.HasValue ? _websiteModuleDa.GetId(seach.ModuleId.Value) : new WebsiteModule()
            };
            return View(model);
        }       
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            string type = $"{StaticEnum.Product},{StaticEnum.Sale},{StaticEnum.Manufacturer},{StaticEnum.Trademark}";
            ActionViewModel action = UpdateModelAction();
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            WebsiteModuleViewModel model = new WebsiteModuleViewModel
            {
                WebsiteModule = new WebsiteModule
                {
                    ParentID = !string.IsNullOrEmpty(action.ItemId) ? ConvertUtil.ToInt32(action.ItemId) : 0
                },
                ListModuleType = _moduleTypeDa.GetListCode(type, ""),
                ListItem = _websiteModuleDa.GetAdminAll(false, Lang(), string.Empty, type,moduleIds),
                ListModulePosition = new List<ModulePosition>(),
                SystemActionAdmin = SystemActionAdmin,
                ListSelected = new List<WebsiteContentAdmin>(),
                ListNote = new List<WebsiteContentAdmin>(),
                AttributesAdmins = _attributesDa.GetAdminAll(true, Lang()),
                AdvertisingAdmins = _advertisingDa.ListAdvertisingEmpty()
            };
            if (action.Do == ActionType.Edit)
            {
                WebsiteModule websiteModule = _websiteModuleDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                List<Advertising> lstselect = _advertisingDa.ListAdvertisingByModuleIds(websiteModule.ID.ToString());
                model.AdvertisingIds = lstselect != null ? string.Join(",", lstselect.Select(x => x.ID)) : string.Empty;
                model.WebsiteModule = websiteModule;
                model.ListModulePosition = _modulePositionDa.GetListPositionCode(websiteModule.PositionCode);
                if (!string.IsNullOrEmpty(websiteModule.RelateIds))
                {
                    model.ListSelected = _websiteContentDa.GetListByArrId(websiteModule.RelateIds);
                }
                if (!string.IsNullOrEmpty(websiteModule.NoteIds))
                {
                    model.ListNote = _websiteContentDa.GetListByArrId(websiteModule.NoteIds);
                }
                if (!string.IsNullOrEmpty(websiteModule.AlbumPictureJson))
                {
                    model.ListAlbumGalleryAdmin = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(websiteModule.AlbumPictureJson);
                }
                if (!string.IsNullOrEmpty(websiteModule.AlbumImageJson))
                {
                    model.ListAlbumImageAdmin = JsonConvert.DeserializeObject<List<AlbumImageAdmin>>(websiteModule.AlbumImageJson);
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            ViewBag.TypeModule = "Product";
            return View("~/Areas/Admin/Views/WebsiteModule/AjaxForm.cshtml", model);
        }
    }
}
