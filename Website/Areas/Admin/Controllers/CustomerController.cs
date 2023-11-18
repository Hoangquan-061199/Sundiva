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
using ADCOnline.Business.Implementation.ClientManager;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Syncfusion.XlsIO;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;
namespace Website.Areas.Admin.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly CustomerDa _customerDa;
        private readonly CartManager _cartManager;
        private readonly OrderDa _OrderDa;
        private readonly CustomerCategoryDa _customerCategoryDa;
        private readonly string _systemRootPath;
        private readonly MembershipDa _membershipDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        public CustomerController(IWebHostEnvironment env)
        {
            _customerDa = new CustomerDa(WebConfig.ConnectionString);
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _OrderDa = new OrderDa(WebConfig.ConnectionString);
            _customerCategoryDa = new CustomerCategoryDa(WebConfig.ConnectionString);
            _cartManager = new CartManager(WebConfig.ConnectionString);
            _systemRootPath = env.ContentRootPath;
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Customer");
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
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            ViewBag.IsExport = false;
            List<CustomerAdmin> list = _customerDa.ListSearch(seach, seach.page, 50, ViewBag.IsExport);
            CustomerViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                ListItem = list,
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                Customer = seach.contentId.HasValue ? _customerDa.GetId(seach.contentId.Value) : new Customer()
            };
            int total = list.Any() ? list.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 50);
            return View(model);
        }
        public ActionResult ImportExcel()
        {
            ActionViewModel action = UpdateModelAction();
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            ProductViewModel model = new()
            {
                SearchModel = search
            };
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership membership = _membershipDa.GetId(aGuid);
            List<ImportHistoryJson> history = !string.IsNullOrEmpty(membership.ImportHistoryJson) ? JsonConvert.DeserializeObject<List<ImportHistoryJson>>(membership.ImportHistoryJson) : new List<ImportHistoryJson>();
            model.ImportHistoryJsons = history.Where(x => x.ModuleID == Convert.ToInt32(action.ModuleId))?.ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ImportExcelAction()
        {
            SearchModel search = new();
            await TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership memberShip = _membershipDa.GetId(aGuid);
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                string processPath = String.Empty;
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
                            List<string> strSuccess = new();
                            List<string> strError = new();
                            StringBuilder strMail = new();
                            string basePath = WebConfig.PathServer + processPath;
                            var info = new FileInfo(basePath);
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
                                        CustomerAdmin customer = new();
                                        for (int col = 1; col <= ColCount; col++)
                                        {
                                            IRange str = worksheet[row, col];
                                            switch (col)
                                            {
                                                #region genaral
                                                case 1:
                                                    {
                                                        //ID
                                                        customer.ID = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                        break;
                                                    }
                                                case 2:
                                                    {

                                                        customer.UserName = str.Value.ToLower();
                                                        break;
                                                    }
                                                case 3:
                                                    {

                                                        customer.Email = str.Value.ToLower();
                                                        break;
                                                    }
                                                case 4:
                                                    {

                                                        customer.FullName = str.Value;
                                                        break;
                                                    }
                                                case 5:
                                                    {
                                                        if (str.Value == "Nam")
                                                        {
                                                            customer.Gender = 1;
                                                        }
                                                        else if (str.Value == "Nữ")
                                                        {
                                                            customer.Gender = 0;
                                                        }
                                                        else
                                                        {
                                                            customer.Gender = 2;
                                                        }
                                                        break;
                                                    }
                                                case 6:
                                                    {
                                                        if (!string.IsNullOrEmpty(str.Value))
                                                        {
                                                            customer.Birthday = DateTime.ParseExact(str.Value, "dd/MM/yyyy", null);
                                                        }
                                                        break;
                                                    }
                                                case 7:
                                                    {

                                                        customer.Mobile = str.Value;
                                                        break;
                                                    }
                                                case 8:
                                                    {

                                                        customer.Address = str.Value;
                                                        break;
                                                    }
                                                case 9:
                                                    {
                                                        customer.ZaloId = str.Value;
                                                        break;
                                                    }
                                                case 10:
                                                    {
                                                        customer.FaceBookId = str.Value; ;
                                                        break;
                                                    }
                                                case 11:
                                                    {
                                                        customer.GoogleId = str.Value; ;
                                                        break;
                                                    }
                                                case 12:
                                                    {
                                                        if (!string.IsNullOrEmpty(str.Value))
                                                        {
                                                            customer.CreatedOnUtc = DateTime.ParseExact(str.Value, "dd/MM/yyyy HH:mm tt", null);
                                                        }
                                                        break;
                                                    }
                                                case 13:
                                                    {
                                                        customer.IsActivated = str.Value == "1" ? true : false;
                                                        break;
                                                    }
                                                    #endregion
                                            }
                                        }
                                        #region Check customer
                                        Customer customeritem = _customerDa.GetId(customer.ID);
                                        if (customeritem != null)
                                        {
                                            customeritem.UserName = customer.UserName;
                                            customeritem.Email = customer.Email;
                                            customeritem.FullName = customer.FullName;
                                            customeritem.Gender = customer.Gender.Value;
                                            customeritem.Birthday = customer.Birthday;
                                            customeritem.Mobile = customer.Mobile;
                                            customeritem.Address = customer.Address;
                                            customeritem.ZaloId = customer.ZaloId;
                                            customeritem.FaceBookId = customer.FaceBookId;
                                            customeritem.GoogleId = customer.GoogleId;
                                            customeritem.CreatedOnUtc = customer.CreatedOnUtc;
                                            customeritem.IsActivated = customer.IsActivated;
                                            if (!string.IsNullOrEmpty(customeritem.Email) && _customerDa.CheckByEmailExisted(customeritem.Email, customeritem.ID) != null)
                                            {
                                                strError.Add(customeritem.Email);
                                                erUser++;
                                            }
                                            else if (!string.IsNullOrEmpty(customeritem.Mobile) && _customerDa.GetByPhoneExisted(customer.Mobile, customeritem.ID) != null)
                                            {
                                                strError.Add(customeritem.Mobile);
                                                erUser++;
                                            }
                                            else
                                            {
                                                _customerDa.Update(customeritem);
                                                success++;
                                                strSuccess.Add(customeritem.Email);
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(customer.Email) && _customerDa.CheckByNewEmail(customer.Email) != null)
                                            {
                                                strError.Add(customer.Email);
                                                erUser++;
                                            }
                                            else if (!string.IsNullOrEmpty(customer.Mobile) && _customerDa.CheckByNewEmail(customer.Mobile) != null)
                                            {
                                                strError.Add(customer.Mobile);
                                                erUser++;
                                            }
                                            else
                                            {
                                                customeritem = new Customer
                                                {
                                                    UserName = customer.Email,
                                                    Email = customer.Email,
                                                    FullName = customer.FullName,
                                                    Gender = customer.Gender.Value,
                                                    Birthday = customer.Birthday,
                                                    Mobile = customer.Mobile,
                                                    Address = customer.Address,
                                                    ZaloId = customer.ZaloId,
                                                    FaceBookId = customer.FaceBookId,
                                                    GoogleId = customer.GoogleId,
                                                    CreatedOnUtc = customer.CreatedOnUtc,
                                                    IsActivated = customer.IsActivated
                                                };
                                                _customerDa.Insert(customeritem);
                                                strSuccess.Add(customeritem.Email);
                                                success++;
                                            }

                                        }
                                        #endregion
                                    }
                                    List<ImportHistoryJson> history = new();
                                    if (memberShip != null)
                                    {
                                        if (!string.IsNullOrEmpty(memberShip.ImportHistoryJson))
                                        {
                                            history = JsonConvert.DeserializeObject<List<ImportHistoryJson>>(memberShip.ImportHistoryJson);
                                        }
                                        history.Add(new ImportHistoryJson
                                        {
                                            Code = Utility.RenDateFileName(),
                                            Filename = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Utility.RenDateFileName() + System.IO.Path.GetExtension(file.FileName),
                                            Url = processPath,
                                            CreatedDate = DateTime.Now,
                                            Extention = extention,

                                        });
                                    }
                                    memberShip.ImportHistoryJson = JsonConvert.SerializeObject(history);
                                    _membershipDa.Update(memberShip, aGuid);
                                    string Mess = string.Empty;
                                    if (success > 0)
                                    {
                                        Mess += success + " thành viên được update.\t" + string.Join(",", strSuccess);
                                    }
                                    if (erUser > 0)
                                    {
                                        Mess += erUser + " email hoặc số điện thoại đã tồn tại.\t" + string.Join(",", strError);
                                    }
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
                msg = new JsonMessage { Errors = true, Logs = e.Message, Message = "Import thất bại: " + e.Message };
                return Ok(msg);
            }
            return Ok(msg);
        }
        public ActionResult ProcessExportFile()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.lang = Lang();
            List<CustomerAdmin> lts = _customerDa.ListExport(seach);
            string fileName = string.Format("khach-hang-quang-hanh-{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(_systemRootPath + "/wwwroot/files/ExportImport", fileName);
            string folder = _systemRootPath + "/wwwroot/files/ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            ExportReportToExcel(filePath, lts);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }
        public IActionResult ListProductByTypeAndCustomerId(string type, string customerid)
        {
            int id = string.IsNullOrEmpty(customerid) ? 0 : Convert.ToInt32(customerid);
            Customer thisCus = _customerDa.GetId(id) ?? new Customer();
            string idProducts = string.Empty;
            string cartCookie = string.Empty;
            //type
            string text;
            if (type == "1")
            {
                text = "Danh sách sản phẩm yêu thích của " + thisCus.FullName;
                idProducts = thisCus.LikedProductIds;
            }
            else if (type == "2")
            {
                text = "Danh sách sản phẩm mua sau của " + thisCus.FullName;
                idProducts = thisCus.BuyLaterProductIds;
            }
            else
            {
                text = "Danh sách sản phẩm trong giỏ hàng chưa thanh toán của " + thisCus.FullName;
                cartCookie = thisCus.CartCookies;
            }

            CustomerViewModel model = new()
            {
                Customer = thisCus,
                Text = text,
                Type = type
            };
            if (type != "3")
            {
                model.ListProducts = _customerDa.GetListProductByArrId(idProducts);
            }
            else
            {
                model.ListCartItem = _cartManager.GetCartData(cartCookie, "");
                model.TotalPriceCart = _cartManager.GetSumPrice(cartCookie);
                model.TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie);
                model.DisCountModule = _cartManager.GetDiscountMoudle(cartCookie);
            }
            return View(model);
        }
        public virtual void ExportReportToExcel(string filePath, List<CustomerAdmin> report)
        {
            FileInfo newFile = new(filePath);
            int dem = 0;
            using (ExcelPackage xlPackage = new(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 
                // get handle to the existing worksheet
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Products");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                //Create Headers and format them
                string[] properties = new string[100];
                properties[0] = "ID";
                properties[1] = "Username";
                properties[2] = "Email";
                properties[3] = "Họ tên";
                properties[4] = "Giới tính";
                properties[5] = "Ngày sinh";
                properties[6] = "Số điện thoại";
                properties[7] = "Địa chỉ";
                properties[8] = "Id Zalo";
                properties[9] = "Id Facebook";
                properties[10] = "Id Google";
                properties[11] = "Ngày tạo";
                properties[12] = "Kích hoạt";
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                int row = 2;
                foreach (CustomerAdmin item in report)
                {
                    string order = string.Empty;
                    SearchModel seach = new();
                    TryUpdateModelAsync(seach);
                    seach.customerId = item.ID;
                    List<OrderAdmin> tempO = _OrderDa.ListSearch(seach, seach.page, 50, false);
                    order = string.Join(",", tempO.Select(c => c.OrderCode));
                    dem++;
                    int col = 1;
                    if (item.ID > 0)//ID
                        worksheet.Cells[row, col].Value = item.ID;
                    col++;
                    if (item.UserName != null)
                        worksheet.Cells[row, col].Value = item.UserName;
                    col++;
                    if (item.Email != null)
                        worksheet.Cells[row, col].Value = item.Email;
                    col++;
                    if (item.FullName != null)
                        worksheet.Cells[row, col].Value = item.FullName;
                    col++;
                    if (item.Gender != null)
                        worksheet.Cells[row, col].Value = item.Gender == 1 ? "Nam" : item.Gender == 0 ? "Nữ" : "Khác";
                    col++;
                    if (item.Birthday != null)
                        worksheet.Cells[row, col].Value = item.Birthday.Value.ToString("dd/MM/yyyy");
                    col++;
                    if (item.Mobile != null)
                        worksheet.Cells[row, col].Value = item.Mobile;
                    col++;
                    if (item.Address != null)
                        worksheet.Cells[row, col].Value = item.Address;
                    col++;
                    if (item.ZaloId != null)
                        worksheet.Cells[row, col].Value = item.ZaloId;
                    col++;
                    if (item.FaceBookId != null)
                        worksheet.Cells[row, col].Value = item.FaceBookId;
                    col++;
                    if (item.GoogleId != null)
                        worksheet.Cells[row, col].Value = item.GoogleId;
                    col++;
                    if (item.CreatedOnUtc.HasValue)
                        worksheet.Cells[row, col].Value = item.CreatedOnUtc.HasValue ? item.CreatedOnUtc.Value.ToString("dd/MM/yyyy HH:mm tt") : string.Empty;
                    col++;
                    if (item.IsActivated.HasValue)
                        worksheet.Cells[row, col].Value = item.IsActivated;
                    row++;
                }
                string nameexcel = "Danh sách khách hàng " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0} reports", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} reports", "");
                xlPackage.Workbook.Properties.Category = "Report";
                xlPackage.Workbook.Properties.Company = "Khách hàng";
                xlPackage.Save();
            }
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            string type = String.Join(",", StaticEnum.ModuleContent);
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            CustomerViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Customer = new Customer(),
                ListOrders = new List<OrderAdmin>(),
                ListOrderDetails = new List<OrderDetail>(),
                ListProductAdmins = new List<ProductAdmin>(),
                CustomerCategoryAdmins = _customerCategoryDa.GetListAll(),
                ListWebsiteModuleAdmin = new List<WebsiteModuleAdmin>()
            };
            if (action.Do == ActionType.Edit)
            {
                int id = ConvertUtil.ToInt32(action.ItemId);
                model.Customer = _customerDa.GetId(id);
                model.ListOrders = _customerDa.GetListOrderByCustomerId(id);
                model.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(model.Customer.RoleModuleIds);
                if (model.ListOrders.Any())
                {
                    string lstIdOrders = string.Join(",", model.ListOrders.Select(c => c.ID).ToList());
                    model.ListOrderDetails = _customerDa.GetListOrderDetailByListOrderIds(lstIdOrders);
                    if (model.ListOrderDetails.Any())
                    {
                        string lstIdPros = string.Join(",", model.ListOrderDetails.Select(c => c.ProductID).ToList());
                        model.ListProductAdmins = _customerDa.GetListProductByListIds(lstIdPros);
                    }
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
            Customer obj = new();
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
                        TryUpdateModelAsync(obj);
                        if (Utility.IsValidEmail(obj.Email) == false)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Email không đúng"
                            };
                            return Ok(msg);
                        }
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Title, true);
                        obj.ZaloId = Utility.ValidString(obj.ZaloId, Link, true);
                        obj.FaceBookId = Utility.ValidString(obj.FaceBookId, Link, true);
                        obj.GoogleId = Utility.ValidString(obj.GoogleId, Link, true);
                        obj.Address = Utility.ValidAddress(obj.Address);
                        obj.UserName = Utility.ValidString(obj.UserName, Code, true);
                        obj.PasswordSalt = Utility.CreateSaltKey(5);
                        var NewPassword = Request.Form["NewPassword"];
                        if (!string.IsNullOrEmpty(NewPassword))
                        {
                            obj.Password = Utility.CreatePasswordHash(Utility.RemoveHTMLTag(NewPassword), obj.PasswordSalt);
                        }
                        else
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Chưa nhập mật khẩu."
                            };
                            return Ok(msg);
                        }
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), obj);
                        //obj.UserName = "ADC" + Utility.GetRandom();
                        Customer checkCode = _customerDa.GetUserName(obj.UserName);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã khách hàng đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        obj.IsDeleted = false;
                        obj.CreatedOnUtc = DateTime.Now;
                        obj.IsLocked = false;
                        var cateIds = Request.Form["cateIds"];
                        obj.CategoryIds = Utility.ValidString(cateIds, ArrayInt, true);
                        int result = _customerDa.Insert(obj);
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
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _customerDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        TryUpdateModelAsync(obj);
                        if (Utility.IsValidEmail(obj.Email) == false)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Email không đúng"
                            };
                            return Ok(msg);
                        }
                        if (string.IsNullOrEmpty(obj.PasswordSalt))
                        {
                            obj.PasswordSalt = Utility.CreateSaltKey(5);
                        }
                        if (string.IsNullOrEmpty(obj.Password))
                        {
                            var NewPassword = Request.Form["NewPassword"];
                            if (!string.IsNullOrEmpty(NewPassword))
                            {
                                obj.Password = Utility.CreatePasswordHash(Utility.RemoveHTMLTag(NewPassword), obj.PasswordSalt);
                            }
                            else
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Chưa nhập mật khẩu."
                                };
                                return Ok(msg);
                            }
                        }
                        obj.Email = Utility.RemoveHTMLTag(obj.Email);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Title, true);
                        obj.ZaloId = Utility.ValidString(obj.ZaloId, Link, true);
                        obj.FaceBookId = Utility.ValidString(obj.FaceBookId, Link, true);
                        obj.GoogleId = Utility.ValidString(obj.GoogleId, Link, true);
                        obj.Address = Utility.ValidAddress(obj.GoogleId);
                        var cateIds = Request.Form["cateIds"];
                        obj.CategoryIds = Utility.ValidString(cateIds, ArrayInt, true);
                        Customer checkCode = _customerDa.GetUserName(obj.UserName);
                        if (checkCode != null && checkCode.ID != obj.ID)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Mã khách hàng đã tồn tại."
                            };
                            return Ok(msg);
                        }
                        var newpass = Request.Form["NewPassword"];
                        if (!string.IsNullOrEmpty(newpass))
                        {
                            var sha1PasswordHash = Utility.CreatePasswordHash(newpass.ToString(), obj.PasswordSalt);
                            obj.Password = sha1PasswordHash;
                        }
                        int result = _customerDa.Update(obj);
                        AddLogAdmin(Request.Path, "Cập nhật Quản lý khách hàng", "Actions-Edit");
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

                        obj = _customerDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        _customerDa.Update(obj);
                        AddLogEdit(Request.Path, "Delete", action.ItemId.ToString(), obj);
                        AddLogAdmin(Request.Path, "Xóa Quản lý khách hàng", "Actions-Delete");
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
