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
using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Http;
using ADCOnline.Business.Implementation.AdminManager;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using ADCOnline.Simple.Base;

namespace Website.Areas.Admin.Controllers
{
    public class RedirectController : BaseController
    {
        private readonly MembershipDa _membershipDa;
        private string _systemRootPath;
        private readonly ModuleAdminDa _moduleAdminDa;
        public RedirectController(IWebHostEnvironment env)
        {
            _systemRootPath = env.ContentRootPath;
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Redirect");
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
            ViewBag.IsExport = false;
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            seach.page = seach.page > 0 ? seach.page : 1;
            seach.pagesize = seach.pagesize > 0 ? seach.pagesize : 50;
            List<RedirectJson> listItems = new();
            List<RedirectJson> list = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
            if (list != null)
            {
                if (!string.IsNullOrEmpty(seach.keyword))
                {
                    list = list.Where(x => x.OldUrl.Contains(seach.keyword) || x.NewUrl.Contains(seach.keyword)).ToList();
                }
                if (!string.IsNullOrEmpty(seach.service))
                {
                    list = list.Where(x => x.TypeRedirect == seach.service).ToList();
                }
                listItems = list;
            }
            int total = listItems.Any() ? listItems.Count : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 20);
            RedirectJsonViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                GetListItem = listItems.Skip((seach.page - 1) * seach.pagesize).Take(seach.pagesize)?.ToList(),
                SearchModel = seach,
                RedirectJsonAdmin = !string.IsNullOrEmpty(seach.ItemID) ? list.FirstOrDefault(c => c.ID == seach.ItemID) : new RedirectJson()
            };

            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            RedirectJsonViewModel model = new() { RedirectJsonAdmin = new RedirectJson(), SystemActionAdmin = SystemActionAdmin };
            if (action.Do == ActionType.Edit)
            {
                List<RedirectJson> list = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                model.RedirectJsonAdmin = list.FirstOrDefault(c => c.ID == action.ItemId);
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        public ActionResult ImportExcel()
        {
            if (!SystemActionAdmin.Add)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            RedirectJsonViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = search
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ImportExcelAction()
        {
            SearchModel search = new();
            await TryUpdateModelAsync(search);
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership memberShip = _membershipDa.GetId(aGuid);
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                List<RedirectJson> list = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                if (list == null)
                {
                    list = new List<RedirectJson>();
                }
                string processPath = string.Empty;
                var file = Request.Form.Files["File"];
                string extention = string.Empty;
                if (!string.IsNullOrEmpty(file.ToString()))
                {
                    if (file != null)
                    {
                        extention = Path.GetExtension(file.FileName);
                        if (extention == ".xlsx" || extention == ".xls")
                        {
                            processPath = Url.Content("files/") + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + "_" + Utility.RenDateFileName() + System.IO.Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                            FileStream stream = new(filePath, FileMode.Create);
                            file.CopyTo(stream);
                            stream.Close();
                            int success = 0;
                            int erUser = 0;
                            List<string> strUser = new();
                            List<string> strCode = new();
                            List<string> strEmail = new();
                            StringBuilder strMail = new();
                            string basePath = WebConfig.PathServer + processPath;
                            FileInfo info = new(basePath);
                            if (info.Exists)
                            {
                                ExcelEngine excelEngine = new();
                                IApplication application = excelEngine.Excel;
                                application.DefaultVersion = ExcelVersion.Excel2013;
                                using (FileStream sampleFile = new(basePath, FileMode.Open))
                                {
                                    IWorkbook workbook = application.Workbooks.Open(sampleFile);
                                    IWorksheet worksheet = workbook.Worksheets[0];
                                    int rowCount = worksheet.Rows.Length;
                                    int ColCount = worksheet.Columns.Length;
                                    for (int row = 2; row <= rowCount; row++)
                                    {
                                        RedirectJson url = new();
                                        for (int col = 1; col <= ColCount; col++)
                                        {
                                            IRange str = worksheet[row, col];
                                            switch (col)
                                            {
                                                #region genaral
                                                case 1:
                                                    {
                                                        //đường dẫn cũ
                                                        url.OldUrl = Utility.ValidString(str.Value, Link, true);
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        //đường dẫn mới
                                                        url.NewUrl = Utility.ValidString(str.Value, Link, true);
                                                        break;
                                                    }
                                                case 3:
                                                    {
                                                        //Loại
                                                        url.TypeRedirect = Utility.ValidString(str.Value, Code, true);
                                                        break;
                                                    }
                                                    #endregion
                                            }
                                        }
                                        #region Check product
                                        if (list != null && list.Any(x => x.OldUrl == url.OldUrl))
                                        {
                                            list.RemoveAll(x => x.OldUrl == url.OldUrl);
                                        }
                                        url.ID = RecreateIDExisted(Utility.DateTimeToJson(DateTime.Now) + Utility.RandomNumber(1, 100).ToString(), list);
                                        list.Add(url);
                                        success++;
                                        #endregion
                                    }
                                    Common.CreateFileJson(0, list, "Redirect", "DataJson");
                                    AspnetMembership membership = _membershipDa.GetId(aGuid);
                                    List<ImportHistoryJson> history = new();
                                    if (membership != null)
                                    {
                                        if (!string.IsNullOrEmpty(membership.ImportHistoryJson))
                                        {
                                            history = JsonConvert.DeserializeObject<List<ImportHistoryJson>>(membership.ImportHistoryJson);
                                        }
                                        history.Add(new ImportHistoryJson
                                        {
                                            Code = Utility.RenDateFileName(),
                                            Filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Utility.RenDateFileName()}{Path.GetExtension(file.FileName)}",
                                            Url = processPath,
                                            CreatedDate = DateTime.Now,
                                            Extention = extention,
                                        });
                                    }
                                    membership.ImportHistoryJson = JsonConvert.SerializeObject(history);
                                    _membershipDa.Update(membership, aGuid);
                                    string Mess = string.Format("{0} đường dẫn điều hướng được thêm thành công.\t{1} đường dẫn điều hướng được update.\t{2}", success, erUser, string.Join(",", strUser));
                                    sampleFile.Close();
                                    msg = new JsonMessage { Errors = false, Message = Mess };
                                    return Ok(msg);
                                }
                            }
                            else
                            {
                                msg = new JsonMessage { Errors = true, Message = "File không tồn tại." };
                                return Ok(msg);
                            }
                        }
                        else
                        {
                            msg = new JsonMessage { Errors = true, Message = "File không đúng định dạng cho phép! (xlsx,xls)" };
                            return Ok(msg);
                        }
                    }
                }
                else
                {
                    msg = new JsonMessage { Errors = true, Message = "Chưa chọn file." };
                    return Ok(msg);
                }
            }
            catch (Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = "Import thất bại: " + e.Message };
                return Ok(msg);
            }
            return Ok(msg);
        }
        public ActionResult ProcessExportFile()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            List<RedirectJson> list = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
            string fileName = string.Format("redirect-url_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(_systemRootPath + "/wwwroot/files/ExportImport", fileName);
            string folder = _systemRootPath + "/wwwroot/files/ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            ExportReportToExcel(filePath, list);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }
        public virtual void ExportReportToExcel(string filePath, List<RedirectJson> report)
        {
            FileInfo newFile = new(filePath);
            int dem = 0;
            using (ExcelPackage xlPackage = new(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 
                // get handle to the existing worksheet
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("RedirectUrl");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                string[] properties = new string[3];
                properties[0] = "Đường dẫn cũ";
                properties[1] = "Đường dẫn mới";
                properties[2] = "Loại";
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                int row = 2;
                foreach (RedirectJson item in report)
                {
                    dem++;
                    int col = 1;
                    if (item.OldUrl != null)//Đường dẫn cũ
                        worksheet.Cells[row, col].Value = item.OldUrl;
                    col++;
                    if (item.NewUrl != null)//Đường dẫn mới
                        worksheet.Cells[row, col].Value = item.NewUrl;
                    col++;
                    if (item.TypeRedirect != null)//Loại
                        worksheet.Cells[row, col].Value = item.TypeRedirect;
                    col++;
                    row++;
                }
                string nameexcel = "Danh sách điều hướng đường dẫn " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0} reports", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} reports", "");
                xlPackage.Workbook.Properties.Category = "Report";
                xlPackage.Workbook.Properties.Company = "Điều hướng 301";
                xlPackage.Save();
            }
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
            RedirectJson obj = new();
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
                        await TryUpdateModelAsync(obj);
                        obj.OldUrl = Utility.ValidString(obj.OldUrl, Link, true);
                        obj.NewUrl = Utility.ValidString(obj.NewUrl, Link, true);
                        obj.TypeRedirect = Utility.ValidString(obj.TypeRedirect, Code, true);
                        List<RedirectJson> listCommon = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                        if (listCommon == null)
                        {
                            listCommon = new List<RedirectJson>();
                        }
                        obj.ID = Utility.DateTimeToJson(DateTime.Now) + Utility.RandomNumber(1, 100).ToString();
                        listCommon.Add(obj);
                        Common.CreateFileJson(0, listCommon, "Redirect", "DataJson");
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), listCommon);
                        AddLogAdmin(Request.Path, "Thêm mới điều hướng", "Actions-Add");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Thêm mới thành công.",
                            Obj = obj
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
                        List<RedirectJson> listCommon = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrIDString.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            await TryUpdateModelAsync(obj);
                            obj.OldUrl = Utility.ValidString(obj.OldUrl, Link, true);
                            obj.NewUrl = Utility.ValidString(obj.NewUrl, Link, true);
                            obj.TypeRedirect = Utility.ValidString(obj.TypeRedirect, Code, true);
                            listCommon.Add(obj);
                            Common.CreateFileJson(0, listCommon, "Redirect", "DataJson");
                            AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Sửa điều hướng", "Actions-Edit");
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
                        List<RedirectJson> listCommon = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrIDString.FirstOrDefault());
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            Common.CreateFileJson(0, listCommon, "Redirect", "DataJson");
                            AddLogEdit(Request.Path, "Delete", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Xóa điều hướng", "Actions-Delete");
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
                        List<RedirectJson> listCommon = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                        foreach (string item in ArrIDString)
                        {
                            obj = listCommon.FirstOrDefault(c => c.ID == item);
                            listCommon.Remove(obj);
                        }
                        Common.CreateFileJson(0, listCommon, "Redirect", "DataJson");
                        AddLogAdmin(Request.Path, "Xóa điều hướng:" + string.Join(",", ArrID), "Actions-DeleteAll");
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
        public ActionResult DeleteAll()
        {
            if (!SystemActionAdmin.Delete)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                List<RedirectJson> listCommon = new();
                Common.CreateFileJson(0, listCommon, "Redirect", "DataJson");
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "Xóa thành công."
                };
                return Ok(msg);
            }
            catch
            {
                return Ok(msg);
            }
        }
        public string RecreateIDExisted(string ID, List<RedirectJson> list)
        {
            if (!string.IsNullOrEmpty(ID) && list != null && list.Any(x => x.ID == ID))
            {
                string newID = Utility.DateTimeToJson(DateTime.Now) + Utility.RandomNumber(1, 10000).ToString();
                return RecreateIDExisted(newID, list);
            }
            else
            {
                return ID;
            }
        }
    }
}
