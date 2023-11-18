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
using Newtonsoft.Json;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class SystemConfigController : BaseController
    {
        private readonly SystemConfigDa _systemConfigDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public SystemConfigController()
        {
            _systemConfigDa = new SystemConfigDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("SystemConfig");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            HomeAdminViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SystemActionAdmin = SystemActionAdmin,
                Module = module
            };
            return View(model);
        }        
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            SystemConfigViewModel module = new() { SystemConfig = new SystemConfig(), ConfigPopup = new Dictionary<string, string>(), SystemActionAdmin = SystemActionAdmin };
            if (action.Do == ActionType.Edit)
            {
                SystemConfig obj = _systemConfigDa.GetByLang(Lang());
                if (obj != null)
                {
                    module.SystemConfig = obj;
                    if (!string.IsNullOrEmpty(obj.ConfigPopupJson))
                    {
                        module.ConfigPopup = JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.ConfigPopupJson);
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
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            SystemConfig obj = new();
            ConfigPopupJson config = new();
            switch (action.Do)
            {               
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
                        obj = _systemConfigDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        SystemConfig objOld = obj;
                        await TryUpdateModelAsync(obj);
                        #region Valid Input
                        obj.Address1 = Utility.ValidAddress(obj.Address1);
                        obj.Address2 = Utility.ValidAddress(obj.Address2);
                        obj.Address3 = Utility.ValidAddress(obj.Address3);
                        obj.Address4 = Utility.ValidAddress(obj.Address4);
                        obj.BusinessLicence = Utility.ValidString(obj.BusinessLicence, Title, true);
                        obj.Copyright = Utility.RemoveHTMLTag(obj.Copyright);
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Email1 = Utility.RemoveHTMLTag(obj.Email1);
                        obj.Email2 = Utility.RemoveHTMLTag(obj.Email2);
                        obj.Email3 = Utility.RemoveHTMLTag(obj.Email3);
                        obj.Email4 = Utility.RemoveHTMLTag(obj.Email4);
                        obj.EmailAdmin = Utility.RemoveHTMLTag(obj.EmailAdmin);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.EmailCms = Utility.RemoveHTMLTag(obj.EmailCms);
                        obj.Facebook = Utility.ValidString(obj.Facebook, Link, true);
                        obj.Fax1 = Utility.ValidString(obj.Fax1, Title, true);
                        obj.Fax2 = Utility.ValidString(obj.Fax2, Title, true);
                        obj.Fax3 = Utility.ValidString(obj.Fax3, Title, true);
                        obj.Fax4 = Utility.ValidString(obj.Fax4, Title, true);
                        obj.Google = Utility.ValidString(obj.Google, Link, true);
                        obj.Hotline = Utility.ValidString(obj.Hotline, Title, true);
                        obj.Instagram = Utility.ValidString(obj.Instagram, Link, true);
                        obj.KeyApiMap = Utility.ValidString(obj.KeyApiMap, Title, true);
                        obj.Lang = Utility.ValidString(obj.Lang, Code, true);
                        obj.LinkFacebook = Utility.ValidString(obj.LinkFacebook, Link, true);
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.Name2 = Utility.ValidString(obj.Name2, Title, true);
                        obj.Name3 = Utility.ValidString(obj.Name3, Title, true);
                        obj.ogimage = Utility.ValidString(obj.ogimage, Link, true);
                        obj.ogimagealt = Utility.ValidString(obj.ogimagealt, Title, true);
                        obj.PassEmailCms = Utility.ValidString(obj.PassEmailCms, Code, true);
                        obj.Phone = Utility.ValidString(obj.Phone, Title, true);
                        obj.PhoneAdvice2 = Utility.ValidString(obj.PhoneAdvice2, Title, true);
                        obj.PhoneAdvice3 = Utility.ValidString(obj.PhoneAdvice3, Title, true);
                        obj.PhoneAdvice4 = Utility.ValidString(obj.PhoneAdvice4, Title, true);
                        obj.SEODescription = Utility.ValidString(obj.SEODescription, Title, true);
                        obj.SEOKeyword = Utility.ValidString(obj.SEOKeyword, Title, true);
                        obj.SEOTitle = Utility.ValidString(obj.SEOTitle, Title, true);
                        obj.Severname = Utility.ValidString(obj.Severname, Title, true);
                        obj.Twitter = Utility.ValidString(obj.Twitter, Link, true);
                        obj.Website = Utility.ValidString(obj.Website, Link, true);
                        obj.Youtube = Utility.ValidString(obj.Youtube, Link, true);
                        obj.Zalo = Utility.ValidString(obj.Zalo, Link, true);
                        #endregion
                        await TryUpdateModelAsync(config);
                        SystemConfig checkCode = _systemConfigDa.GetNameByLang(obj.Name,Lang());
                        if (checkCode != null && checkCode.ID != obj.ID)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Tên này đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        obj.ConfigPopupJson = JsonConvert.SerializeObject(config);
                        int result = _systemConfigDa.Update(obj);
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
