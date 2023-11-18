using System;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Website.Areas.Admin.Controllers
{
    public class LogAdminController : BaseController
    {
        private readonly LogAdminDa _LogAdminDa;
        public LogAdminController()
        {
            _LogAdminDa = new LogAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            List<LogAdminAdmin> list = _LogAdminDa.ListSearch(seach, seach.page, 50, ViewBag.IsExport);
            LogAdminViewModel model = new()
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
            LogAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                LogAdmin = new LogAdmin(),
            };
            if (action.Do == ActionType.Edit)
            {
                model.LogAdmin = _LogAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
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
            LogAdmin obj = new();
            switch (action.Do)
            {                                
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
                        _LogAdminDa.Delete(obj, " ID =" + action.ItemId);
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
                    if(SystemActionAdmin.Delete != true)
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
                        foreach (int item in ArrID)
                        {
                            _LogAdminDa.Delete(obj," ID =" + item);
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Xóa thành công."
                        };
                        return Ok(msg);
                    }
                    catch(Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
            }
            return Ok(msg);
        }
    }
}

