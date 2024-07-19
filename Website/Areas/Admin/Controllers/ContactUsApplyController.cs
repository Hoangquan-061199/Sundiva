using System;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Website.Areas.Admin.Controllers
{
    public class ContactUsApplyController : BaseController
    {
        private readonly ContactUsDa _contactUsDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        private string _systemRootPath;
        public ContactUsApplyController(IWebHostEnvironment env)
        {
            _systemRootPath = env.ContentRootPath;
            _contactUsDa = new ContactUsDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("ContactUsApply");
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
            seach.type = "Apply";
            ViewBag.IsExport = false;
            if (seach.IsExcel == true || seach.IsPdf == true || seach.IsCsv == true)
            {
                ViewBag.IsExport = true;
            }
            var list = _contactUsDa.ListSearch(seach, seach.page, 20, ViewBag.IsExport);
            var model = new ContactUsViewModel
            {
                SearchModel = seach,
                ListItem = list,
                SystemActionAdmin = SystemActionAdmin,
                ContactUs = seach.contentId.HasValue ? _contactUsDa.GetId(seach.contentId.Value) : new ContactUs()
            };
            if (!ViewBag.IsExport)
            {
                int total = list != null && list.Count > 0 ? list[0].TotalRecord : 0;
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
            List<ContactUsAdmin> ltsList = _contactUsDa.ListApplySearch(seach, seach.page, 50, true);
            string fileName = string.Format("don-ung-tuyen_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(_systemRootPath + "/wwwroot/files/ExportImport", fileName);
            string folder = _systemRootPath + "/wwwroot/files/ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            ExportReportToExcel(filePath, ltsList);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }
        public virtual void ExportReportToExcel(string filePath, List<ContactUsAdmin> report)
        {
            FileInfo newFile = new(filePath);
            int dem = 0;
            using (ExcelPackage xlPackage = new(newFile))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("DonUngTuyen");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                string[] properties = new string[100];
                properties[0] = "STT";
                properties[1] = "Họ tên";
                properties[2] = "Tên công việc";
                properties[3] = "Điện thoại";
                properties[4] = "Email";
                properties[5] = "Nội dung";
                properties[6] = "Hồ sơ";
                properties[7] = "Trang thái";
                properties[8] = "Ngày ứng tuyển";
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                int row = 2;
                int stt = 1;
                foreach (ContactUsAdmin item in report)
                {
                    dem++;
                    int col = 1;
                    if (item.ID > 0)//ID
                        worksheet.Cells[row, col].Value = stt;
                    col++;
                    if (item.FullName != null)//Họ và Tên
                        worksheet.Cells[row, col].Value = item.FullName;
                    col++;
                    if (item.ProductName != null)//Tên công việc
                        worksheet.Cells[row, col].Value = item.ProductName;
                    col++;
                    if (item.Phone != null)//Số điện thoại
                        worksheet.Cells[row, col].Value = item.Phone;
                    col++;
                    if (item.Email != null)//Email
                        worksheet.Cells[row, col].Value = item.Email;
                    col++;
                    if (item.Content != null)//Số điện thoại
                        worksheet.Cells[row, col].Value = item.Content;
                    col++;
                    if (item.Division != null)//Hồ sơ
                        worksheet.Cells[row, col].Value = WebConfig.Website + item.Division;
                    col++;
                    if (item.Phone != null)//Trang thái
                        if (item.Status == 1)
                        {
                            worksheet.Cells[row, col].Value = "Chưa đọc";
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = "Đã đọc";
                        }
                    col++;
                    if (item.CreatedDate.HasValue)//Ngày ứng tuyển
                        worksheet.Cells[row, col].Value = item.CreatedDate.Value.ToString("dd/MM/yyyy");
                    col++;
                    row++;
                    stt++;
                }
                string nameexcel = "Danh sách đơn ứng tuyển " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0} reports", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} reports", "");
                xlPackage.Workbook.Properties.Category = "Report";
                xlPackage.Workbook.Properties.Company = "Don Ung Tuyen";
                xlPackage.Save();
            }
        }
    }
}
