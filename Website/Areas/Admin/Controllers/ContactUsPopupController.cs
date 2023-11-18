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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Website.Areas.Admin.Controllers
{
    public class ContactUsPopupController : BaseController
    {
        private readonly ContactUsDa _contactUsDa;
        private string _systemRootPath;
        private readonly ModuleAdminDa _moduleAdminDa;
        public ContactUsPopupController(IWebHostEnvironment env)
        {
            _systemRootPath = env.ContentRootPath;
            _contactUsDa = new ContactUsDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("ContactUsPopup");
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
            ViewBag.IsExport = false;
            if (seach.IsExcel == true || seach.IsPdf == true || seach.IsCsv == true)
            {
                ViewBag.IsExport = true;
            }
            List<ContactUsAdmin> list = _contactUsDa.ListPopupSearch(seach, seach.page, 50, ViewBag.IsExport);
            ContactUsViewModel model = new()
            {
                ListItem = list,
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                ContactUs = seach.contentId.HasValue ? _contactUsDa.GetId(seach.contentId.Value) : new ContactUs()
            };
            if (!ViewBag.IsExport)
            {
                int total = list.Any() ? list.FirstOrDefault().TotalRecord : 0;                
                ViewBag.GridHtml = GetPage(seach.page, total, 20);
            }
            return View(model);
        }       
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            ContactUsViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ContactUs = new ContactUs(),
            };
            if (action.Do == ActionType.Edit)
            {
                model.ContactUs = _contactUsDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (model.ContactUs.Code == "Apply")
                {
                    ViewBag.Action = action.Do ?? ActionType.Add;
                    ViewBag.ActionText = ActionType.ActionText(action.Do);
                    return View("AjaxFormApply",model);
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        public ActionResult ProcessExportFile()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.lang = Lang();
            List<ContactUsAdmin> ltsList = _contactUsDa.ListPopupSearch(seach, seach.page, 50, true);
            string fileName = string.Format("lien-he-popup_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(_systemRootPath + "/wwwroot/files/ExportImport", fileName);
            string folder = _systemRootPath + "/wwwroot/files/ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            ExportReportToExcel(filePath, ltsList);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }
        public virtual void ExportReportToExcel(string filePath, List<ContactUsAdmin> report)
        {
            FileInfo newFile = new(filePath);
            int dem = 0;
            using (ExcelPackage xlPackage = new(newFile))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Products");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                string[] properties = new string[100];
                properties[0] = "ID";
                properties[1] = "Họ và tên";
                properties[2] = "Email";
                properties[3] = "Số điện thoại";
                properties[4] = "Địa chỉ";
                properties[5] = "Tỉnh/Thành";
                properties[5] = "Quận/Huyện";
                properties[6] = "Ngày gửi";
                properties[7] = "Nội dung";
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                int row = 2;
                foreach (ContactUsAdmin item in report)
                {
                    dem++;
                    int col = 1;
                    if (item.ID > 0)//ID
                        worksheet.Cells[row, col].Value = item.ID;
                    col++;
                    if (item.FullName != null)//Họ và Tên
                        worksheet.Cells[row, col].Value = item.FullName;
                    col++;
                    if (item.Email != null)//Email
                        worksheet.Cells[row, col].Value = item.Email;
                    col++;
                    if (item.Phone != null)//Số điện thoại
                        worksheet.Cells[row, col].Value = item.Phone;
                    col++;
                    if (item.Address != null)//Địa chỉ
                        worksheet.Cells[row, col].Value = item.Address;
                    col++;
                    if (item.City != null)//Tỉnh/Thành
                        worksheet.Cells[row, col].Value = item.City;
                    col++;
                    if (item.District != null)//Quận/Huyện
                        worksheet.Cells[row, col].Value = item.District;
                    col++;
                    if (item.CreatedDate != null)//Quận/Huyện
                        worksheet.Cells[row, col].Value = item.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm");
                    col++;
                    if (item.Content != null)//Nội dung yêu cầu
                        worksheet.Cells[row, col].Value = item.Content;
                    row++;
                }
                string nameexcel = "Danh sách thông tin liên hệ popup " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0} reports", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} reports", "");
                xlPackage.Workbook.Properties.Category = "Report";
                xlPackage.Workbook.Properties.Company = "Popup";
                xlPackage.Save();
            }
        }
        [HttpPost]
        public ActionResult Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            ContactUs obj = new();
            switch (action.Do)
            {                
                case ActionType.Edit:
                    try
                    {
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage{Errors = true,Message = "Bạn chưa được phân quyền cho chức năng này."};
                            return Ok(msg);
                        }
                        obj = _contactUsDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        TryUpdateModelAsync(obj);
                        int result = _contactUsDa.Update(obj);
                        AddLogAdmin(Request.Path, "Cập nhật liên hệ", "Actions-Edit");
                        if (result > 0)
                        {
                            msg = new JsonMessage{Errors = false,Message = "Cập nhật thành công.", Obj = obj};
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
                        _contactUsDa.Delete(obj, " ID =" + action.ItemId);
                        AddLogEdit(Request.Path, "Delete", action.ItemId.ToString(), obj);
                        AddLogAdmin(Request.Path, "Xóa liên hệ", "Actions-Delete");
                        msg = new JsonMessage{Errors = false,Message = "Xóa thành công."};
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
                            _contactUsDa.Delete(obj, " ID =" + item);
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
            }
            return Ok(msg);
        }
    }
}