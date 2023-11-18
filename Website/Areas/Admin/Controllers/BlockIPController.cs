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

namespace Website.Areas.Admin.Controllers
{
    public class BlockIPController : BaseController
    {
        private readonly ModuleAdminDa _moduleAdminDa;
        public BlockIPController()
        {
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("BlockIP");
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
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            List<string> addressIp = JsonConvert.DeserializeObject<List<string>>(ReadFile("BlockIP.json", "DataJson"));
            BlockIPViewModel model = new ()
            {
                SystemActionAdmin = SystemActionAdmin,
                AddressIp = addressIp
            };
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
            List<string> addressIps = new();
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
                        string addressip = Request.Form["AddressIps"];
                        if (!string.IsNullOrEmpty(addressip))
                        {
                            addressip = Utility.RemoveHTMLTag(addressip);
                            addressIps = ListHelper.GetValuesArrayTag(addressip);
                        }
                        Common.CreateFileJson(0, addressIps, "BlockIP", "DataJson");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Cập nhật thành công."
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
