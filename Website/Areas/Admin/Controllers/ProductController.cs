using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using Website.Utils;

namespace Website.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ProductDa _productDa;
        private readonly ModuleTypeDa _moduleTypeDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly AttributesDa _attributesDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly MembershipDa _membershipDa;
        private readonly DapperDA _dapperDa;
        private string _systemRootPath;
        private readonly ModulePositionDa _modulePositionDa;
        private readonly IWebHostEnvironment _env;
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly OtherContentDa _otherContentDa;

        public ProductController(IWebHostEnvironment env)
        {
            _env = env;
            _systemRootPath = env.ContentRootPath;
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _moduleTypeDa = new ModuleTypeDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _attributesDa = new AttributesDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _modulePositionDa = new ModulePositionDa(WebConfig.ConnectionString);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _otherContentDa = new OtherContentDa(WebConfig.ConnectionString);
        }

        public IActionResult Index()
        {
            ADCOnline.Simple.Base.Module module = _moduleAdminDa.GetTag("Product");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            string type = $"{StaticEnum.Product},{StaticEnum.Trademark},{StaticEnum.Sale}";
            ProductViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListWebsiteModuleAdmin = _websiteModuleDa.GetAdminAll(false, Lang(), "", type, moduleIds),
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module
            };
            return View(model);
        }

        public IActionResult ListItems()
        {
            ADCOnline.Simple.Admin.SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            seach.lang = Lang();
            if (seach.ModuleId > 0)
            {
                seach.ModuleIds = string.Join(",", _websiteModuleDa.GetListChidrent(seach.ModuleId.Value).Select(x => x.ID));
            }
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            var moduleID = _websiteModuleDa.GetListByNotListModuleType(StaticEnum.Product, "", Lang()).FirstOrDefault();
            ProductViewModel model = new()
            {
                ListItem = _productDa.ListSearch(seach, seach.page, seach.pagesize > 0 ? seach.pagesize : 50, false, moduleIds),
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SearchModel = seach,
                Product = seach.productId.HasValue ? _productDa.GetId(seach.productId.Value) : new ADCOnline.Simple.Base.Product(),
                moduleParentID = ConvertUtil.ToInt32(moduleID)
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 50);
            model.Total = _productDa.CountProductByModuleIds(string.Empty, Lang());
            return View(model);
        }

        public IActionResult ListItemsAjax(string Code, string ids, bool isSearch = false)
        {
            ADCOnline.Simple.Admin.SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.lang = Lang();
            seach.page = seach.page > 0 ? seach.page : 1;
            seach.pagesize = seach.pagesize > 0 ? seach.pagesize : 10;
            if (seach.ModuleId > 0)
            {
                seach.ModuleIds = string.Join(",", _websiteModuleDa.GetListChidrent(seach.ModuleId.Value).Select(x => x.ID));
            }
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            ProductViewModel model = new()
            {
                ListItem = _productDa.ListSearch(seach, seach.page, seach.pagesize, false, moduleIds),
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = seach,
                ValueSelected = ids
            };
            string type = $"{StaticEnum.Product},{StaticEnum.Trademark},{StaticEnum.Sale}";
            model.ListWebsiteModuleAdmin = _websiteModuleDa.GetAdminAll(false, Lang(), "", type, moduleIds);
            ViewBag.Code = Code;
            ViewBag.IsSearch = isSearch;
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPageAjax(seach.page, total, seach.pagesize);
            return View(model);
        }

        public ActionResult AjaxAmount(string ids)
        {
            ProductViewModel model = new()
            {
                ContentIds = ids,
                SystemActionAdmin = SystemActionAdmin
            };
            ViewBag.Action = ActionType.UpdateAmount;
            ViewBag.ActionText = ActionType.ActionText(ViewBag.Action);
            return View(model);
        }

        public ActionResult AjaxForm()
        {
            string type = $"{StaticEnum.Product},{StaticEnum.Trademark},{StaticEnum.Sale}";
            ActionViewModel action = UpdateModelAction();
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            var listTimeTours = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("TimeTour.json", "DataJson"));
            var listAddressStart = JsonConvert.DeserializeObject<List<CommonJsonAdmin>>(ReadFile("AddressStart.json", "DataJson"));
            ProductViewModel module = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Product = new ADCOnline.Simple.Base.Product
                {
                    IsShow = true
                },
                ListWebsiteModule = _websiteModuleDa.GetAdminAll(true, Lang(), "", type, moduleIds),
                ListWebsiteModuleAdmin = new List<WebsiteModuleAdmin>(),
                ProductAdmins = new List<ProductAdmin>(),
                ProductAdminGifts = new List<ProductAdmin>(),
                ProductAdminReplaces = new List<ProductAdmin>(),
                ListModuleTypeAdmin = _moduleTypeDa.ListAll(),
                ListContentItem = new List<WebsiteContentAdmin>(),
                ListContentDocumentItem = new List<WebsiteContentAdmin>(),
                SubItems = new List<ADCOnline.Simple.Base.SubItem>(),
                Lang = Lang(),
                AttributesAdmins = _attributesDa.GetAdminAll(true, Lang()),
                Attribute_WebsiteContents = new List<Attribute_WebsiteContent>(),
                ListFileDownloadAdmin = new List<ADCOnline.Simple.Admin.FileDownloadAdmin>(),
                ListContentSubAdmin = new List<SubContentItem>(),
                LitsItemTimeTours = (listTimeTours != null) ? listTimeTours : new List<CommonJsonAdmin>(),
                LitsItemAddressStart = (listAddressStart != null) ? listAddressStart : new List<CommonJsonAdmin>()
            };
            if (action.Do == ActionType.Edit)
            {
                ADCOnline.Simple.Base.Product obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (obj != null)
                {
                    module.SubItems = _productDa.GetSubItemByProductId(obj.ID);
                    module.Product = obj;
                    module.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrIdNotShow(obj.ModuleIds);
                    if (module.ListWebsiteModuleAdmin != null)
                    {
                        ViewBag.TypeView = module.ListWebsiteModuleAdmin.FirstOrDefault().TypeView;
                        ViewBag.TypeViewMenu = module.ListWebsiteModuleAdmin.FirstOrDefault().TypeViewMenu;
                    }
                    if (!string.IsNullOrEmpty(obj.ContentIds))
                    {
                        module.ListContentItem = _websiteContentDa.GetListByArrId(obj.ContentIds);
                    }
                    if (!string.IsNullOrEmpty(obj.DocumentIds))
                    {
                        module.ListContentDocumentItem = _websiteContentDa.GetListByArrId(obj.DocumentIds);
                    }
                    if (!string.IsNullOrEmpty(obj.AttachedProductIds))
                    {
                        module.ProductAdmins = _productDa.GetListByArrId(obj.AttachedProductIds);
                    }
                    if (!string.IsNullOrEmpty(obj.GiftIds))
                    {
                        module.ProductAdminGifts = _productDa.GetListByArrId(obj.GiftIds);
                    }
                    if (!string.IsNullOrEmpty(obj.ReplaceIds))
                    {
                        module.ProductAdminReplaces = _productDa.GetListByArrId(obj.ReplaceIds);
                    }
                    if (!string.IsNullOrEmpty(obj.AlbumPictureJson))
                    {
                        module.ListAlbumGalleryAdmin = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(obj.AlbumPictureJson);
                    }
                    if (!string.IsNullOrEmpty(obj.SubContent))
                    {
                        module.ListContentSubAdmin = JsonConvert.DeserializeObject<List<SubContentItem>>(obj.SubContent);
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(obj.LinkDownload))
                        {
                            module.ListFileDownloadAdmin = JsonConvert.DeserializeObject<List<ADCOnline.Simple.Admin.FileDownloadAdmin>>(obj.LinkDownload);
                        }
                    }
                    catch
                    {
                        module.ListFileDownloadAdmin = new List<ADCOnline.Simple.Admin.FileDownloadAdmin>();
                    }
                    module.AttributesAdmins = _attributesDa.GetAdminByModuleIds(true, obj.ModuleIds);
                    module.Attribute_WebsiteContents = _productDa.GetAttrByProductId(obj.ID) ?? new List<Attribute_WebsiteContent>();
                    module.ListContentItem = !string.IsNullOrEmpty(obj.RelatedIds) ? _websiteContentDa.GetListByArrId(obj.RelatedIds) : new List<WebsiteContentAdmin>();
                    module.ListContentDocumentItem = !string.IsNullOrEmpty(obj.DocumentIds) ? _websiteContentDa.GetListByArrId(obj.DocumentIds) : new List<WebsiteContentAdmin>();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(action.ModuleId))
                {
                    string moduleIdss = action.ModuleId;
                    module.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(action.ModuleId);
                    module.Product.ModuleIds = action.ModuleId;
                    module.Product.ModuleNameAscii = module.ListWebsiteModuleAdmin.FirstOrDefault().NameAscii;
                    module.AttributesAdmins = _attributesDa.GetAdminByModuleIds(true, moduleIdss);
                    if (module.ListWebsiteModuleAdmin != null)
                    {
                        ViewBag.TypeView = module.ListWebsiteModuleAdmin.FirstOrDefault().TypeView;
                        ViewBag.TypeViewMenu = module.ListWebsiteModuleAdmin.FirstOrDefault().TypeViewMenu;
                    }
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            if (ViewBag.TypeView == StaticEnum.TypeProductI)
                return View("~/Areas/Admin/Views/Product/AjaxFormTypeI.cshtml", module);
            if (ViewBag.TypeView == StaticEnum.TypeProductII)
                return View("~/Areas/Admin/Views/Product/AjaxFormTypeII.cshtml", module);
            return View(module);
        }

        public ActionResult ImportExcel()
        {
            if (!SystemActionAdmin.Add)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            ActionViewModel action = UpdateModelAction();
            ADCOnline.Simple.Admin.SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            ProductViewModel model = new()
            {
                SearchModel = search,
                SystemActionAdmin = SystemActionAdmin
            };
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership membership = _membershipDa.GetId(aGuid);
            if (!string.IsNullOrEmpty(action.ModuleId))
            {
                model.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(action.ModuleId);
                model.ModuleId = action.ModuleId;
                List<ImportHistoryJson> history = !string.IsNullOrEmpty(membership.ImportHistoryJson) ? JsonConvert.DeserializeObject<List<ImportHistoryJson>>(membership.ImportHistoryJson) : new List<ImportHistoryJson>();
                model.ImportHistoryJsons = history.Where(x => x.ModuleID == Convert.ToInt32(action.ModuleId))?.ToList();
            }
            if (string.IsNullOrEmpty(action.ModuleId))
            {
                return View("~/Areas/Admin/Views/Product/ImportExcelNew.cshtml", model);
            }
            return View(model);
        }

        #region import excel số lượng kho hàng

        public ActionResult ImportExcelWarehouse()
        {
            if (!SystemActionAdmin.Add)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            ActionViewModel action = UpdateModelAction();
            ADCOnline.Simple.Admin.SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            ProductViewModel model = new()
            {
                SearchModel = search,
                SystemActionAdmin = SystemActionAdmin
            };
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership membership = _membershipDa.GetId(aGuid);
            if (!string.IsNullOrEmpty(action.ModuleId))
            {
                model.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrId(action.ModuleId);
                model.ModuleId = action.ModuleId;
            }
            if (string.IsNullOrEmpty(action.ModuleId))
            {
                return View("~/Areas/Admin/Views/Product/ImportExcelWarehouse.cshtml", model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ImportExcelWarehouseAction()
        {
            ADCOnline.Simple.Admin.SearchModel search = new();
            await TryUpdateModelAsync(search);
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership memberShip = _membershipDa.GetId(aGuid);
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                List<ADCOnline.Simple.Base.Product> listProduct = _productDa.GetListAllProducts(Lang());
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
                            processPath = $"{Url.Content("files/")}{Path.GetFileNameWithoutExtension(file.FileName)}_{Utility.RenDateFileName()}{Path.GetExtension(file.FileName)}";
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
                                    int rowCount = worksheet.Rows.Length - 1;
                                    int ColCount = worksheet.Columns.Length;
                                    for (int row = 6; row <= rowCount; row++)
                                    {
                                        var code = worksheet[row, 3].Value;
                                        if (code != null && !string.IsNullOrEmpty(code.ToString()) && listProduct.Any(x => x.ProductCode.Equals(code)))
                                        {
                                            var ProductItem = listProduct.FirstOrDefault(x => x.ProductCode.Equals(code));
                                            var quantiy = worksheet[row, 9].Value;
                                            var unit = worksheet[row, 5].Value;
                                            ProductItem.Quantity = ConvertUtil.ToInt32(quantiy);

                                            int result = _productDa.Update(ProductItem);
                                            if (result > 0)
                                            {
                                                erUser++;
                                            }
                                        }
                                        else
                                        {
                                            var codeProduct = worksheet[row, 3].Value;
                                            var nameProduct = worksheet[row, 4].Value;
                                            var quantityProduct = worksheet[row, 9].Value;
                                            var unitProduct = worksheet[row, 5].Value;
                                            if (!string.IsNullOrEmpty(codeProduct))
                                            {
                                                ADCOnline.Simple.Base.Product productInsert = new()
                                                {
                                                    Name = Utility.ValidString(nameProduct.ToString(), Title, true),
                                                    ProductCode = Utility.ValidString(codeProduct.ToString(), Title, true),
                                                    Quantity = ConvertUtil.ToInt32(quantityProduct),
                                                    Lang = "vi",
                                                    IsApproved = true,
                                                    NameAscii = Utility.ConvertRewrite(nameProduct.ToString()),
                                                    IsShow = true,
                                                    IsDeleted = false,
                                                    IsSitemap = true,
                                                    IndexGoogle = "noodp,index,follow",
                                                    CreatedDate = DateTime.Now,
                                                    PublishDate = DateTime.Now,
                                                    ModifiedDate = DateTime.Now,
                                                    ModifiedName = memberShip.FullName,
                                                    CreatorName = memberShip.FullName,
                                                    IsShowPrice = true,
                                                    CreatorID = aGuid,
                                                    UrlPicture = "/html/style/images/img-logo.webp"
                                                };
                                                int result = _productDa.Insert(productInsert);
                                                if (result > 0)
                                                {
                                                    success++;
                                                }
                                            }
                                        }
                                    }
                                    AspnetMembership membership = _membershipDa.GetId(aGuid);
                                    List<ImportHistoryExcelWarehouseJson> history = new();
                                    if (membership != null)
                                    {
                                        history.Add(new ImportHistoryExcelWarehouseJson
                                        {
                                            Code = Utility.RenDateFileName(),
                                            Filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Utility.RenDateFileName()}{Path.GetExtension(file.FileName)}",
                                            Url = processPath,
                                            CreatedDate = DateTime.Now,
                                            Extention = extention
                                        });
                                    }
                                    _membershipDa.Update(membership, aGuid);
                                    string Mess = string.Format("{0} sản phẩm được thêm thành công.\t{1} sản phẩm được update.\t{2}", success, erUser, string.Join(",", strUser));
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
                msg = new JsonMessage
                {
                    Errors = true,
                    Logs = e.Message,
                    Message = "Import thất bại: " + e.Message
                };
                return Ok(msg);
            }
            return Ok(msg);
        }

        #endregion import excel số lượng kho hàng

        public ActionResult ProcessExportFile()
        {
            ADCOnline.Simple.Admin.SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            seach.lang = Lang();
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            WebsiteModuleAdmin moduleItem = seach.ModuleId.HasValue ? _websiteModuleDa.GetModuleId(seach.ModuleId.Value) : new WebsiteModuleAdmin();
            if (!seach.ModuleId.HasValue)
            {
                seach.page = 1;
                seach.pagesize = 1000;
            }
            if (moduleItem != null && !string.IsNullOrEmpty(moduleItem.AttributeModuleIds))
            {
                moduleItem.Attributes = _attributesDa.GetListAttrByModuleId(moduleItem.ID.ToString());
            }
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            List<ProductAdmin> ltsListProduct = _productDa.ListExport(seach, moduleIds);
            List<AttributeContent> lstAttributeCotent = _attributesDa.GetListAttributeContent(string.Join(",", ltsListProduct.Select(x => x.ID)));
            string fileName = string.Format("san-pham-{1}_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), Utility.ConvertRewrite(moduleItem != null ? moduleItem.Name : string.Empty));
            string filePath = Path.Combine(_systemRootPath + "/wwwroot/files/ExportImport", fileName);
            string folder = _systemRootPath + "/wwwroot/files/ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            ExportReportToExcel(filePath, moduleItem, ltsListProduct, lstAttributeCotent);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }

        public virtual void ExportReportToExcel(string filePath, WebsiteModuleAdmin module, List<ProductAdmin> report, List<AttributeContent> lstAttributeCotent)
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
                properties[1] = "Tên sản phẩm";
                properties[2] = "Mã sản phẩm";
                properties[3] = "Link sản phẩm";
                properties[4] = "Model";
                properties[5] = "Thương hiệu";
                properties[6] = "Ảnh đại diện";
                properties[7] = "Album ảnh";
                properties[8] = "Video";
                properties[9] = "Số lượng";
                properties[10] = "Khuyến mãi";
                properties[11] = "Mô tả ngắn";
                properties[12] = "Đặc điểm nổi bật";
                properties[13] = "Thông tin sản phẩm";
                properties[14] = "Thông số kỹ thuật";
                properties[15] = "Giá gốc";
                properties[16] = "Giá bán";
                properties[17] = "Hình thức giảm giá";
                properties[18] = "% Giảm giá";
                properties[19] = "Số tiền giảm";
                properties[20] = "Thứ tự";
                properties[21] = "Trạng thái";
                properties[22] = "Trang chủ";
                properties[23] = "Nổi bật";
                properties[24] = "Mới";
                properties[25] = "Bán chạy";
                properties[26] = "Giá sốc";
                properties[27] = "Hiển thị";
                properties[28] = "Hiện giá";
                properties[29] = "Đã bao gồm VAT";
                properties[30] = "Hiện sitemap";
                properties[31] = "Index google";
                properties[32] = "Canonical";
                properties[33] = "Seo Description";
                properties[34] = "Seo Title";
                properties[35] = "Seo Keyword";
                int num = 36;
                if (module != null && module.Attributes != null)
                {
                    foreach (AttributesAdmin item in module.Attributes.Where(x => x.ParentID == 0).OrderBy(x => x.OrderDisplay))
                    {
                        properties[num] = item.Name;
                        num++;
                    }
                }
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                int row = 2;
                foreach (ProductAdmin item in report)
                {
                    dem++;
                    int col = 1;
                    if (item.ID > 0)//ID
                        worksheet.Cells[row, col].Value = item.ID;
                    col++;
                    if (item._Name != null)//Tên sản phẩm
                        worksheet.Cells[row, col].Value = item._Name;
                    col++;
                    if (item.ProductCode != null)//Mã sản phẩm
                        worksheet.Cells[row, col].Value = item.ProductCode;
                    col++;
                    if (item.NameAscii != null)//Mã sản phẩm
                        worksheet.Cells[row, col].Value = WebConfig.Website + "/" + item._NameAscii;
                    col++;
                    if (item.Model != null)//Model
                        worksheet.Cells[row, col].Value = item.Model;
                    col++;
                    if (item.BrandId != null)//Thương hiệu
                        worksheet.Cells[row, col].Value = item.BrandId;
                    col++;
                    if (!string.IsNullOrEmpty(item.UrlPicture))//Ảnh
                        worksheet.Cells[row, col].Value = item.UrlPicture;
                    col++;
                    if (item.AlbumPictureJson != null) //Album ảnh
                    {
                        var ListGallery = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(item.AlbumPictureJson);
                        var Album = ListGallery.Where(x => x.AlbumType == 4).Select(x => new AlbumExcel
                        {
                            Name = x.AlbumTitle,
                            Url = x.AlbumUrl,
                            Sort = x.AlbumOrderDisplay
                        });
                        worksheet.Cells[row, col].Value = JsonConvert.SerializeObject(Album).TrimStart('[').TrimEnd(']');
                    }
                    col++;
                    if (item.Video != null)//Video
                        worksheet.Cells[row, col].Value = item.Video;
                    col++;
                    if (item.Amount != null)//Số lượng
                        worksheet.Cells[row, col].Value = item.Amount;
                    col++;
                    if (!string.IsNullOrEmpty(item.ShortDescription))//Mô tả ngắn
                        worksheet.Cells[row, col].Value = HttpUtility.HtmlDecode(item.ShortDescription);
                    col++;
                    if (item.Description != null)//Đặc điểm nổi bật
                        worksheet.Cells[row, col].Value = HttpUtility.HtmlDecode(item.Description);
                    col++;
                    if (item.Content != null)//Thông tin sản phẩm
                        worksheet.Cells[row, col].Value = HttpUtility.HtmlDecode(item.Content);
                    col++;
                    if (item.Information != null)//Thông số kỹ thuật
                        worksheet.Cells[row, col].Value = HttpUtility.HtmlDecode(item.Information);
                    col++;
                    if (item.PriceOld.HasValue)//Giá gốc
                        worksheet.Cells[row, col].Value = item.PriceOld;
                    col++;
                    if (item.Price.HasValue)//Giá bán
                        worksheet.Cells[row, col].Value = item.Price;
                    col++;
                    if (item.TypeSale.HasValue)//hình thức giảm giá
                        worksheet.Cells[row, col].Value = item.TypeSale;
                    col++;
                    if (item.TypeSaleValue.HasValue)//% giảm giá
                        worksheet.Cells[row, col].Value = item.TypeSaleValue;
                    col++;
                    if (item.DiscountAmount.HasValue)//Số tiền giảm
                        worksheet.Cells[row, col].Value = item.DiscountAmount;
                    col++;
                    if (item.OrderDisplay.HasValue)//Thứ tự
                        worksheet.Cells[row, col].Value = item.OrderDisplay;
                    col++;
                    worksheet.Cells[row, col].Value = item.Status;//trạng thái
                    col++;
                    worksheet.Cells[row, col].Value = ("," + item.ViewHome + ",").Contains(",1,") ? 1 : 0;//Trang chủ
                    col++;
                    worksheet.Cells[row, col].Value = ("," + item.ViewHome + ",").Contains(",3,") ? 1 : 0;//Nổi bật
                    col++;
                    worksheet.Cells[row, col].Value = ("," + item.ViewHome + ",").Contains(",0,") ? 1 : 0;//Mới
                    col++;
                    worksheet.Cells[row, col].Value = ("," + item.ViewHome + ",").Contains(",2,") ? 1 : 0;//Bán chạy
                    col++;
                    worksheet.Cells[row, col].Value = ("," + item.ViewHome + ",").Contains(",5,") ? 1 : 0;//Giá sốc
                    col++;
                    worksheet.Cells[row, col].Value = item.IsShow == true ? 1 : 0;//Hiển thị
                    col++;
                    worksheet.Cells[row, col].Value = item.IsShowPrice == true ? 1 : 0;//Hiển giá
                    col++;
                    worksheet.Cells[row, col].Value = item.IsVAT == true ? 1 : 0;//Đã bao gồm VAT
                    col++;
                    worksheet.Cells[row, col].Value = item.IsSitemap == true ? 1 : 0;//Hiện sitemap
                    col++;
                    worksheet.Cells[row, col].Value = item.IndexGoogle == "noodp,index,follow" ? 1 : 0;//index google
                    col++;
                    worksheet.Cells[row, col].Value = item.Canonical;//Canonical
                    col++;
                    worksheet.Cells[row, col].Value = item.SeoDescription;//Seo Description
                    col++;
                    worksheet.Cells[row, col].Value = item.SEOTitle;//Seo Title
                    col++;
                    worksheet.Cells[row, col].Value = item.SeoKeyword;//Seo Keyword
                    col++;
                    if (module != null && module.Attributes != null)
                    {
                        foreach (AttributesAdmin attr in module.Attributes.Where(x => x.ParentID == 0).OrderBy(x => x.OrderDisplay))
                        {
                            List<AttributeContent> selected = lstAttributeCotent.Where(x => x.ParentID == attr.ID && x.ContentID == item.ID)?.ToList();
                            if (selected.Any() && selected.Count > 0)
                            {
                                Dictionary<string, decimal> dict = selected.Select(x => new KeyValuePair<string, decimal>(x.Name, x.Price.Value)).ToDictionary(x => x.Key, x => x.Value);
                                worksheet.Cells[row, col].Value = JsonConvert.SerializeObject(dict);
                            }
                            else
                            {
                                worksheet.Cells[row, col].Value = "";
                            }
                            col++;
                        }
                    }
                    row++;
                }
                string nameexcel = "Danh sách sản phẩm " + module.Name + " " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0} reports", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} reports", "");
                xlPackage.Workbook.Properties.Category = "Report";
                xlPackage.Workbook.Properties.Company = "Sản phẩm";
                xlPackage.Save();
            }
        }

        [HttpPost]
        public async Task<ActionResult> ImportExcelAction()
        {
            ADCOnline.Simple.Admin.SearchModel search = new();
            await TryUpdateModelAsync(search);
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership memberShip = _membershipDa.GetId(aGuid);
            ResizeImage resizeImage = new(_env);
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                if (search.ModuleId > 0)
                {
                    string processPath = string.Empty;
                    var file = Request.Form.Files["File"];
                    string extention = string.Empty;
                    if (!string.IsNullOrEmpty(file.ToString()))
                    {
                        WebsiteModule moduleItem = _websiteModuleDa.GetId(Convert.ToInt32(search.ModuleId));
                        if (file != null)
                        {
                            extention = Path.GetExtension(file.FileName);
                            if (extention == ".xlsx" || extention == ".xls")
                            {
                                processPath = Url.Content("files/") + Path.GetFileNameWithoutExtension(file.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(file.FileName);
                                string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                                FileStream stream = new(path: filePath, FileMode.Create);
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
                                            ProductAdmin product = new()
                                            {
                                                CreatorID = aGuid,
                                                Lang = "vi",
                                                IsApproved = true,
                                                IsDeleted = false,
                                                CreatedDate = DateTime.Now,
                                                PublishDate = DateTime.Now,
                                                ModifiedDate = DateTime.Now,
                                                ModifiedName = Utility.RemoveHTMLTag(memberShip.FullName),
                                                CreatorName = Utility.RemoveHTMLTag(memberShip.FullName),
                                                ModuleIds = moduleItem.ID.ToString(),
                                                ModuleNameAscii = Utility.ValidString(moduleItem.NameAscii, Link, true),
                                                AttributeContents = new List<AttributeContent>(),
                                                AlbumExcels = new List<AlbumExcel>()
                                            };
                                            for (int col = 1; col <= ColCount; col++)
                                            {
                                                IRange str = worksheet[row, col];
                                                switch (col)
                                                {
                                                    #region genaral

                                                    case 1:
                                                        {
                                                            //ID
                                                            product.ID = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 2:
                                                        {
                                                            //Tên
                                                            product._Name = Utility.ValidString(str.Value, Title, true);
                                                            break;
                                                        }
                                                    case 3:
                                                        {
                                                            //Mã sản phẩm
                                                            product.ProductCode = Utility.ValidString(str.Value, Link, true);
                                                            product._NameAscii = Utility.ConvertRewrite(product.Name);
                                                            break;
                                                        }
                                                    case 4:
                                                        {
                                                            //Model
                                                            product.Model = Utility.ValidString(str.Value, Title, true);
                                                            break;
                                                        }
                                                    case 5:
                                                        {
                                                            //Thương hiệu
                                                            if (!string.IsNullOrEmpty(str.Value))
                                                            {
                                                                product.BrandId = ConvertUtil.ToInt32(str.Value);
                                                            }
                                                            break;
                                                        }
                                                    case 6:
                                                        {
                                                            //Ảnh đại diện
                                                            product.UrlPicture = str.Value;
                                                            break;
                                                        }

                                                    case 7:
                                                        {
                                                            //Alnum
                                                            if (!string.IsNullOrEmpty(str.Value))
                                                            {
                                                                List<AlbumExcel> AlbumExcels = JsonConvert.DeserializeObject<List<AlbumExcel>>("[" + str.Value + "]");
                                                                product.AlbumExcels = AlbumExcels;
                                                            }
                                                            break;
                                                        }
                                                    case 8:
                                                        {
                                                            //Video
                                                            product.Video = str.Value;
                                                            break;
                                                        }
                                                    case 9:
                                                        {
                                                            //Số lượng
                                                            product.Amount = ConvertUtil.ToInt32(str.Value);
                                                            break;
                                                        }
                                                    case 11:
                                                        {
                                                            //Mô tả ngắn
                                                            product.ShortDescription = str.Value;
                                                            break;
                                                        }
                                                    case 12:
                                                        {
                                                            //Đặc điểm nổi bật
                                                            product.Description = str.Value;
                                                            break;
                                                        }
                                                    case 13:
                                                        {
                                                            //Thông tin sản phẩm
                                                            product.Content = str.Value;
                                                            break;
                                                        }
                                                    case 14:
                                                        {
                                                            //Thông số kỹ thuật
                                                            product.Information = str.Value;
                                                            break;
                                                        }
                                                    case 15:
                                                        {
                                                            //Giá gốc
                                                            product.PriceOld = !string.IsNullOrEmpty(str.Value) ? Convert.ToDecimal(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 16:
                                                        {
                                                            //Giá bán
                                                            product.Price = !string.IsNullOrEmpty(str.Value) ? Convert.ToDecimal(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 17: //Hình thức giảm giá
                                                        {
                                                            product.TypeSale = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 18: // % giảm
                                                        {
                                                            product.TypeSaleValue = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 19: // Số tiền giảm
                                                        {
                                                            product.DiscountAmount = !string.IsNullOrEmpty(str.Value) ? Convert.ToDecimal(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 20: //Thứ tự
                                                        {
                                                            product.OrderDisplay = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 21: //Trạng thái
                                                        {
                                                            product.Status = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                            break;
                                                        }
                                                    case 22:
                                                        {
                                                            //trang chủ
                                                            if (str.Value == "1")
                                                            {
                                                                product.ViewHome = string.IsNullOrEmpty(product.ViewHome) ? "1" : product.ViewHome + ",1";
                                                            }
                                                            break;
                                                        }
                                                    case 23:
                                                        {
                                                            //nổi bật
                                                            if (str.Value == "1")
                                                            {
                                                                product.ViewHome = string.IsNullOrEmpty(product.ViewHome) ? "3" : product.ViewHome + ",3";
                                                            }
                                                            break;
                                                        }
                                                    case 24:
                                                        {
                                                            //Mới
                                                            if (str.Value == "1")
                                                            {
                                                                product.ViewHome = string.IsNullOrEmpty(product.ViewHome) ? "0" : product.ViewHome + ",0";
                                                            }
                                                            break;
                                                        }
                                                    case 25:
                                                        {
                                                            //bán chạy
                                                            if (str.Value == "1")
                                                            {
                                                                product.ViewHome = string.IsNullOrEmpty(product.ViewHome) ? "2" : product.ViewHome + ",2";
                                                            }
                                                            break;
                                                        }
                                                    case 26:
                                                        {
                                                            //giá sốc
                                                            if (str.Value == "1")
                                                            {
                                                                product.ViewHome = string.IsNullOrEmpty(product.ViewHome) ? "5" : product.ViewHome + ",5";
                                                            }
                                                            break;
                                                        }
                                                    case 27:
                                                        {
                                                            //Ẩn hiện
                                                            product.IsShow = str.Value == "1";
                                                            break;
                                                        }
                                                    case 28:
                                                        {
                                                            //Hiện giá
                                                            product.IsShowPrice = str.Value == "1";
                                                            break;
                                                        }
                                                    case 29:
                                                        {
                                                            //Thuế VAT
                                                            product.IsVAT = str.Value == "1";
                                                            break;
                                                        }
                                                    case 30:
                                                        {
                                                            //Sitemap
                                                            product.IsSitemap = str.Value == "1";
                                                            break;
                                                        }
                                                    case 31:
                                                        {
                                                            //index google
                                                            product.IndexGoogle = str.Value == "1" ? "noodp,index,follow" : "noodp,noindex,nofollow";
                                                            break;
                                                        }
                                                    case 32:
                                                        {
                                                            //Canonical
                                                            product.Canonical = str.Value;
                                                            break;
                                                        }
                                                    case 33:
                                                        {
                                                            //Seo Description
                                                            product.SeoDescription = str.Value;
                                                            break;
                                                        }
                                                    case 34:
                                                        {
                                                            //SEO Title
                                                            product.SEOTitle = str.Value;
                                                            break;
                                                        }
                                                    case 35:
                                                        {
                                                            //Seo Keyword
                                                            product.SeoKeyword = str.Value;
                                                            break;
                                                        }

                                                    #endregion genaral

                                                    default:
                                                        {
                                                            if (!string.IsNullOrEmpty(worksheet[1, col].Value) && !string.IsNullOrEmpty(str.Value))
                                                            {
                                                                Attributes attr = _attributesDa.GetByName(worksheet[1, col].Value);
                                                                int parentID = 0;

                                                                #region Attribute

                                                                if (attr == null)
                                                                {
                                                                    Attributes attrItem = new()
                                                                    {
                                                                        Name = Utility.ValidString(worksheet[1, col].Value, Title, true),
                                                                        ParentID = 0,
                                                                        NameAscii = Utility.ConvertRewrite(worksheet[1, col].Value),
                                                                        IsShow = true,
                                                                        Lang = "vi",
                                                                        IsDeleted = false,
                                                                        IsAllowsFillter = true,
                                                                        CreatedDate = DateTime.Now,
                                                                        ListModuleIds = moduleItem.ID.ToString()
                                                                    };
                                                                    int idattr = _attributesDa.Insert(attrItem);
                                                                    attrItem.ID = idattr;
                                                                    parentID = attrItem.ID;
                                                                    AttributeContent attrcontent = new()
                                                                    {
                                                                        AttributeID = attrItem.ID,
                                                                        Price = 0
                                                                    };
                                                                    product.AttributeContents.Add(attrcontent);
                                                                    //attr in Module
                                                                    if (moduleItem.AttributeModuleIds == null)
                                                                    {
                                                                        moduleItem.AttributeModuleIds = idattr.ToString();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!("," + moduleItem.AttributeModuleIds + ",").Contains("," + idattr + ","))
                                                                        {
                                                                            moduleItem.AttributeModuleIds = moduleItem.AttributeModuleIds + "," + idattr;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    attr.IsShow = true;
                                                                    _attributesDa.Update(attr);
                                                                    parentID = attr.ID;
                                                                    AttributeContent attrcontent = new()
                                                                    {
                                                                        AttributeID = attr.ID,
                                                                        Price = 0
                                                                    };
                                                                    product.AttributeContents.Add(attrcontent);
                                                                    //Module in Attr
                                                                    if (!("," + attr.ListModuleIds + ",").Contains("," + moduleItem.ID + ","))
                                                                    {
                                                                        if (string.IsNullOrEmpty(attr.ListModuleIds))
                                                                        {
                                                                            attr.ListModuleIds = moduleItem.ID.ToString();
                                                                        }
                                                                        else
                                                                        {
                                                                            attr.ListModuleIds = attr.ListModuleIds + "," + moduleItem.ID;
                                                                        }
                                                                        _attributesDa.Update(attr);
                                                                    }
                                                                    //Attr in Module
                                                                    if (moduleItem.AttributeModuleIds == null)
                                                                    {
                                                                        moduleItem.AttributeModuleIds = attr.ID.ToString();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!("," + moduleItem.AttributeModuleIds + ",").Contains("," + attr.ID + ","))
                                                                        {
                                                                            moduleItem.AttributeModuleIds = moduleItem.AttributeModuleIds + "," + attr.ID;
                                                                        }
                                                                    }
                                                                }

                                                                #endregion Attribute

                                                                #region Attribute option

                                                                Dictionary<string, decimal> attroptions = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(str.Value);
                                                                foreach (KeyValuePair<string, decimal> item in attroptions)
                                                                {
                                                                    Attributes attroption = _attributesDa.GetByNameOption(Utility.RemoveHTMLTag(item.Key), parentID);
                                                                    if (attroption == null)
                                                                    {
                                                                        Attributes optionItem = new()
                                                                        {
                                                                            Name = Utility.ValidString(item.Key, Title, true),
                                                                            ParentID = parentID,
                                                                            IsShow = true,
                                                                            Lang = "vi",
                                                                            IsDeleted = false,
                                                                            CreatedDate = DateTime.Now,
                                                                            ListModuleIds = moduleItem.ID.ToString()
                                                                        };
                                                                        int idattr = _attributesDa.Insert(optionItem);
                                                                        optionItem.ID = idattr;
                                                                        AttributeContent attrcontent = new()
                                                                        {
                                                                            AttributeID = optionItem.ID,
                                                                            Price = ConvertUtil.ToDecimal(item.Value)
                                                                        };
                                                                        product.AttributeContents.Add(attrcontent);
                                                                        //attr in Module
                                                                        if (moduleItem.AttributeModuleIds == null)
                                                                        {
                                                                            moduleItem.AttributeModuleIds = idattr.ToString();
                                                                        }
                                                                        else if (!("," + moduleItem.AttributeModuleIds + ",").Contains("," + idattr + ","))
                                                                        {
                                                                            moduleItem.AttributeModuleIds = moduleItem.AttributeModuleIds + "," + idattr;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        attroption.IsShow = true;
                                                                        _attributesDa.Update(attroption);
                                                                        AttributeContent attrcontent = new()
                                                                        {
                                                                            AttributeID = attroption.ID,
                                                                            Price = ConvertUtil.ToDecimal(item.Value)
                                                                        };
                                                                        product.AttributeContents.Add(attrcontent);
                                                                        //Module in Attr
                                                                        if (!("," + attroption.ListModuleIds + ",").Contains("," + moduleItem.ID + ","))
                                                                        {
                                                                            attroption.ListModuleIds = string.IsNullOrEmpty(attroption.ListModuleIds) ? moduleItem.ID.ToString() : attroption.ListModuleIds + "," + moduleItem.ID;
                                                                            _attributesDa.Update(attroption);
                                                                        }
                                                                        //Attr in Module
                                                                        if (moduleItem.AttributeModuleIds == null)
                                                                        {
                                                                            moduleItem.AttributeModuleIds = attroption.ID.ToString();
                                                                        }
                                                                        else if (!("," + moduleItem.AttributeModuleIds + ",").Contains("," + attroption.ID + ","))
                                                                        {
                                                                            moduleItem.AttributeModuleIds = moduleItem.AttributeModuleIds + "," + attroption.ID;
                                                                        }
                                                                    }
                                                                }

                                                                #endregion Attribute option
                                                            }
                                                            break;
                                                        }
                                                }
                                            }
                                            _websiteModuleDa.Update(moduleItem);

                                            #region Check product

                                            ADCOnline.Simple.Base.Product productItem = _productDa.GetId(product.ID);
                                            if (productItem == null && !string.IsNullOrEmpty(product.ProductCode))
                                            {
                                                productItem = _productDa.GetByProductCode(product.ProductCode);
                                            }
                                            if (productItem != null)
                                            {
                                                productItem.Name = Utility.ValidString(product._Name, Title, true);
                                                productItem.NameAscii = Utility.ValidString(product._NameAscii, Link, true);
                                                productItem.CreatorID = product.CreatorID;
                                                productItem.ProductCode = Utility.ValidString(product.ProductCode, Link, true);
                                                productItem.Model = Utility.ValidString(product.Model, Title, true);
                                                productItem.BrandId = product.BrandId;
                                                productItem.UrlPicture = product.UrlPicture;
                                                Thread resizeOriginal = new(() =>
                                                {
                                                    if (!productItem.UrlPicture.Contains("http://") && !productItem.UrlPicture.Contains("https://") && !string.IsNullOrEmpty(productItem.UrlPicture))
                                                    {
                                                        try
                                                        {
                                                            int last = productItem.UrlPicture.LastIndexOf(@"/") + 1;
                                                            string path = productItem.UrlPicture[..last];
                                                            string name = productItem.UrlPicture[last..];
                                                            resizeImage.ConvertReSize(path, name);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            AddLogError(e);
                                                        }
                                                    }
                                                });
                                                resizeOriginal.IsBackground = true;
                                                resizeOriginal.Start();
                                                List<AlbumGalleryAdmin> listAlbum = new();
                                                if (!string.IsNullOrEmpty(productItem.AlbumPictureJson))
                                                {
                                                    List<AlbumGalleryAdmin> listGallery = JsonConvert.DeserializeObject<List<AlbumGalleryAdmin>>(productItem.AlbumPictureJson);
                                                    listAlbum = listGallery.Where(x => x.AlbumType != 4)?.ToList();
                                                }
                                                if (product.AlbumExcels.Any())
                                                {
                                                    product.AlbumExcels.ForEach(x =>
                                                    {
                                                        listAlbum.Add(new AlbumGalleryAdmin
                                                        {
                                                            AlbumTitle = Utility.ValidString(x.Name, Title, true),
                                                            AlbumOrderDisplay = x.Sort ?? 0,
                                                            AlbumType = 4,
                                                            AlbumUrl = Utility.ValidString(x.Url, Link, true)
                                                        });
                                                        Thread resizethumb = new(() =>
                                                        {
                                                            if (!x.Url.Contains("http://") && !x.Url.Contains("https://") && !string.IsNullOrEmpty(x.Url))
                                                            {
                                                                try
                                                                {
                                                                    int last = x.Url.LastIndexOf(@"/") + 1;
                                                                    string path = x.Url[..last];
                                                                    string name = x.Url[last..];
                                                                    resizeImage.ConvertReSize(path, name);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    AddLogError(e);
                                                                }
                                                            }
                                                        })
                                                        {
                                                            IsBackground = true
                                                        };
                                                        resizethumb.Start();
                                                    });
                                                }
                                                productItem.AlbumPictureJson = listAlbum.Any() ? JsonConvert.SerializeObject(listAlbum) : null;
                                                productItem.Video = Utility.ValidString(product.Video, Link, true);
                                                productItem.Amount = product.Amount;
                                                productItem.ShortDescription = product.ShortDescription;
                                                productItem.Description = product.Description;
                                                productItem.Content = product.Content;
                                                productItem.Information = product.Information;
                                                productItem.Price = product.Price;
                                                productItem.PriceOld = product.PriceOld;
                                                productItem.TypeSale = product.TypeSale;
                                                productItem.TypeSaleValue = product.TypeSaleValue;
                                                productItem.DiscountAmount = product.DiscountAmount;
                                                productItem.OrderDisplay = product.OrderDisplay;
                                                productItem.Status = product.Status;
                                                productItem.ViewHome = Utility.ValidString(product.ViewHome, ArrayInt, true);
                                                productItem.IsShow = product.IsShow;
                                                productItem.IsShowPrice = product.IsShowPrice;
                                                productItem.IsVAT = product.IsVAT;
                                                productItem.IsSitemap = product.IsSitemap;
                                                productItem.IndexGoogle = Utility.RemoveHTMLTag(product.IndexGoogle);
                                                productItem.Canonical = Utility.ValidString(product.Canonical, Link, true);
                                                productItem.SeoDescription = product.SeoDescription;
                                                productItem.SEOTitle = product.SEOTitle;
                                                productItem.SeoKeyword = product.SeoKeyword;
                                                productItem.ModifiedDate = product.ModifiedDate;
                                                productItem.ModifiedName = product.ModifiedName;
                                                if (string.IsNullOrEmpty(productItem.ModuleIds))
                                                {
                                                    productItem.ModuleIds = moduleItem.ID.ToString();
                                                }
                                                else if (!("," + productItem.ModuleIds + ",").Contains("," + moduleItem.ID + ","))
                                                {
                                                    productItem.ModuleIds = productItem.ModuleIds + "," + moduleItem.ID.ToString();
                                                }
                                                if (string.IsNullOrEmpty(productItem.ModuleNameAscii))
                                                {
                                                    productItem.ModuleNameAscii = moduleItem.NameAscii;
                                                }
                                                _attributesDa.DeleteAttributeContent(productItem.ID);
                                                if (product.AttributeContents.Any())
                                                {
                                                    productItem.AttributeProductIds = string.Join(",", product.AttributeContents.Select(x => x.AttributeID));
                                                    foreach (AttributeContent val in product.AttributeContents)
                                                    {
                                                        Attribute_WebsiteContent optioncontent = new()
                                                        {
                                                            AttributeID = val.AttributeID,
                                                            ContentID = productItem.ID,
                                                            Price = val.Price
                                                        };
                                                        _productDa.InsertAttr(optioncontent);
                                                    }
                                                }
                                                _productDa.Update(productItem);
                                                erUser++;
                                            }
                                            else
                                            {
                                                ADCOnline.Simple.Base.Product productInsert = new()
                                                {
                                                    Name = Utility.ValidString(product._Name, Title, true),
                                                    NameAscii = Utility.ValidString(product._NameAscii, Link, true),
                                                    ProductCode = Utility.ValidString(product.ProductCode, Link, true),
                                                    Model = Utility.ValidString(product.Model, Title, true),
                                                    BrandId = product.BrandId,
                                                    UrlPicture = Utility.ValidString(product.UrlPicture, Link, true),
                                                    Video = Utility.ValidString(product.Video, Link, true),
                                                    Amount = product.Amount,
                                                    ShortDescription = product.ShortDescription,
                                                    Description = product.Description,
                                                    Content = product.Content,
                                                    Information = product.Information,
                                                    Price = product.Price,
                                                    PriceOld = product.PriceOld,
                                                    TypeSale = product.TypeSale,
                                                    TypeSaleValue = product.TypeSaleValue,
                                                    DiscountAmount = product.DiscountAmount,
                                                    OrderDisplay = product.OrderDisplay,
                                                    Status = product.Status,
                                                    ViewHome = Utility.ValidString(product.ViewHome, ArrayInt, true),
                                                    IsShow = product.IsShow,
                                                    IsShowPrice = product.IsShowPrice,
                                                    SeoDescription = product.SeoDescription,
                                                    SeoKeyword = product.SeoKeyword,
                                                    SEOTitle = product.SEOTitle,
                                                    IsSitemap = product.IsSitemap,
                                                    IndexGoogle = Utility.RemoveHTMLTag(product.IndexGoogle),
                                                    Canonical = Utility.ValidString(product.Canonical, Link, true),
                                                    IsVAT = product.IsVAT,
                                                    TotalViews = 0,
                                                    CreatorID = product.CreatorID,
                                                    ModuleIds = Utility.ValidString(product.ModuleIds, ArrayInt, true),
                                                    IsDeleted = product.IsDeleted,
                                                    CreatedDate = product.CreatedDate,
                                                    CreatorName = product.CreatorName,
                                                    ModuleNameAscii = Utility.ValidString(product.ModuleNameAscii, Link, true),
                                                    Lang = Lang(),
                                                    IsApproved = true,
                                                    AttributeProductIds = product.AttributeContents != null ? string.Join(",", product.AttributeContents.Select(x => x.AttributeID)) : null,
                                                    PublishDate = product.PublishDate,
                                                    ModifiedDate = product.ModifiedDate
                                                };
                                                Thread resizeOriginal = new(() =>
                                                {
                                                    if (!product.UrlPicture.Contains("http://") && !product.UrlPicture.Contains("https://") && !string.IsNullOrEmpty(product.UrlPicture))
                                                    {
                                                        try
                                                        {
                                                            int last = product.UrlPicture.LastIndexOf(@"/") + 1;
                                                            string path = product.UrlPicture[..last];
                                                            string name = product.UrlPicture[last..];
                                                            resizeImage.ConvertReSize(path, name);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            AddLogError(e);
                                                        }
                                                    }
                                                })
                                                {
                                                    IsBackground = true
                                                };
                                                resizeOriginal.Start();
                                                List<AlbumGalleryAdmin> listAlbum = new();
                                                if (product.AlbumExcels.Any())
                                                {
                                                    product.AlbumExcels.ForEach(x =>
                                                    {
                                                        listAlbum.Add(new AlbumGalleryAdmin
                                                        {
                                                            AlbumTitle = Utility.ValidString(x.Name, Title, true),
                                                            AlbumOrderDisplay = x.Sort ?? 0,
                                                            AlbumType = 4,
                                                            AlbumUrl = Utility.ValidString(x.Url, Link, true)
                                                        });
                                                        Thread resizethumb = new(() =>
                                                        {
                                                            if (!x.Url.Contains("http://") && !x.Url.Contains("https://") && !string.IsNullOrEmpty(x.Url))
                                                            {
                                                                try
                                                                {
                                                                    int last = x.Url.LastIndexOf(@"/") + 1;
                                                                    string path = x.Url[..last];
                                                                    string name = x.Url[last..];
                                                                    resizeImage.ConvertReSize(path, name);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    AddLogError(e);
                                                                }
                                                            }
                                                        })
                                                        {
                                                            IsBackground = true
                                                        };
                                                        resizethumb.Start();
                                                    });
                                                }
                                                productInsert.AlbumPictureJson = listAlbum.Any() ? JsonConvert.SerializeObject(listAlbum) : null;
                                                int result = _productDa.Insert(productInsert);
                                                productInsert.ID = result;
                                                if (product.AttributeContents.Any())
                                                {
                                                    foreach (AttributeContent val in product.AttributeContents)
                                                    {
                                                        Attribute_WebsiteContent optioncontent = new()
                                                        {
                                                            AttributeID = val.AttributeID,
                                                            ContentID = productInsert.ID,
                                                            Price = val.Price
                                                        };
                                                        _productDa.InsertAttr(optioncontent);
                                                    }
                                                }
                                                success++;
                                            }

                                            #endregion Check product
                                        }
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
                                                Filename = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(file.FileName),
                                                Url = processPath,
                                                CreatedDate = DateTime.Now,
                                                Extention = extention,
                                                ModuleID = moduleItem.ID
                                            });
                                        }
                                        membership.ImportHistoryJson = JsonConvert.SerializeObject(history);
                                        _membershipDa.Update(membership, aGuid);
                                        string Mess = string.Format("{0} sản phẩm được thêm thành công.\t{1} sản phẩm được update.\t{2}", success, erUser, string.Join(",", strUser));
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
                else
                {
                    msg = new JsonMessage { Errors = true, Message = "Chưa chọn danh mục" };
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

        [HttpPost]
        public async Task<ActionResult> ImportExcelNewAction()
        {
            ADCOnline.Simple.Admin.SearchModel search = new();
            await TryUpdateModelAsync(search);
            Guid aGuid = new(HttpContext.Session.GetString("WebAdminUserID"));
            AspnetMembership memberShip = _membershipDa.GetId(aGuid);
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
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
                            processPath = $"{Url.Content("files/")}{Path.GetFileNameWithoutExtension(file.FileName)}_{Utility.RenDateFileName()}{Path.GetExtension(file.FileName)}";
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
                                    for (int row = 3; row <= rowCount; row++)
                                    {
                                        ProductAdmin product = new()
                                        {
                                            CreatorID = aGuid,
                                            Lang = Lang(),
                                            IsApproved = true,
                                            Status = 1,
                                            IsShowPrice = true,
                                            IsDeleted = false,
                                            CreatedDate = DateTime.Now,
                                            PublishDate = DateTime.Now,
                                            ModifiedDate = DateTime.Now,
                                            ModifiedName = memberShip.FullName,
                                            CreatorName = memberShip.FullName,
                                            TotalOrder = 0,
                                            TotalRate = 0,
                                            TotalStar = 0,
                                            TotalLike = 0,
                                            IndexGoogle = "noodp,index,follow",
                                            IsSitemap = true
                                        };
                                        for (int col = 1; col <= ColCount; col++)
                                        {
                                            var str = worksheet[row, col];
                                            switch (col)
                                            {
                                                #region genaral

                                                case 1:
                                                    {
                                                        //ID
                                                        product.ID = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                        product.ProductCode = product.ID.ToString();
                                                        break;
                                                    }
                                                case 2:
                                                    {
                                                        //Model
                                                        product.Model = str.Value;
                                                        break;
                                                    }
                                                case 3:
                                                case 4:
                                                case 5:
                                                case 6:
                                                case 11:
                                                    {
                                                        break;
                                                    }
                                                case 7:
                                                    {
                                                        //Tên
                                                        product._Name = str.Value;
                                                        break;
                                                    }
                                                case 8:
                                                    {
                                                        //số lượng
                                                        product.Amount = ConvertUtil.ToInt32(str.Value);
                                                        break;
                                                    }
                                                case 9:
                                                    {
                                                        //Giá bán
                                                        product.Price = !string.IsNullOrEmpty(str.Value) ? Convert.ToDecimal(str.Value) : 0;
                                                        break;
                                                    }
                                                case 10:
                                                    {
                                                        //Giá gốc
                                                        product.PriceOld = !string.IsNullOrEmpty(str.Value) ? Convert.ToDecimal(str.Value) : 0;
                                                        break;
                                                    }
                                                case 12:
                                                    {
                                                        //Thuế VAT
                                                        product.IsVAT = str.Value == "0";
                                                        break;
                                                    }
                                                case 13:
                                                    {
                                                        //Ẩn hiện
                                                        product.IsShow = str.Value == "yes";
                                                        break;
                                                    }
                                                case 14:
                                                    {
                                                        //Thứ tự
                                                        product.OrderDisplay = !string.IsNullOrEmpty(str.Value) ? Convert.ToInt32(str.Value) : 0;
                                                        break;
                                                    }
                                                case 15:
                                                    {
                                                        //Tình trạng
                                                        if (!string.IsNullOrEmpty(str.Value) && str.Value.ToLower().Contains("mới"))
                                                        {
                                                            product.ViewHome = "0";
                                                        }
                                                        break;
                                                    }
                                                case 16:
                                                    {
                                                        //Ảnh
                                                        product.UrlPicture = str.Value;
                                                        break;
                                                    }

                                                    #endregion genaral
                                            }
                                        }

                                        #region Check product

                                        ADCOnline.Simple.Base.Product productItem = _productDa.GetId(product.ID);
                                        if (productItem == null)
                                        {
                                            ADCOnline.Simple.Base.Product productInsert = new()
                                            {
                                                ID = product.ID,
                                                Name = Utility.ValidString(product._Name, Title, true),
                                                NameAscii = Utility.ValidString(product._NameAscii, Link, true),
                                                ProductCode = Utility.ValidString(product.ProductCode, Link, true),
                                                Model = Utility.ValidString(product.Model, Title, true),
                                                UrlPicture = Utility.ValidString(product.UrlPicture, Link, true),
                                                Amount = product.Amount,
                                                Price = product.Price,
                                                PriceOld = product.PriceOld,
                                                OrderDisplay = product.OrderDisplay,
                                                Status = product.Status,
                                                ViewHome = Utility.ValidString(product.ViewHome, ArrayInt, true),
                                                IsShow = product.IsShow,
                                                IsShowPrice = product.IsShowPrice,
                                                IsSitemap = product.IsSitemap,
                                                IndexGoogle = Utility.RemoveHTMLTag(product.IndexGoogle),
                                                IsVAT = product.IsVAT,
                                                TotalViews = 0,
                                                TotalOrder = product.TotalOrder,
                                                TotalLike = product.TotalLike,
                                                TotalRate = product.TotalRate,
                                                TotalStar = Convert.ToInt32(product.TotalStar),
                                                CreatorID = product.CreatorID,
                                                IsDeleted = product.IsDeleted,
                                                CreatedDate = product.CreatedDate,
                                                CreatorName = product.CreatorName,
                                                Lang = "vi",
                                                IsApproved = true,
                                                PublishDate = product.PublishDate,
                                                ModifiedDate = product.ModifiedDate
                                            };
                                            int result = _productDa.InsertNoId(productInsert);
                                            if (result > 0)
                                            {
                                                success++;
                                            }
                                        }

                                        #endregion Check product
                                    }
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
                                            Extention = extention
                                        });
                                    }
                                    membership.ImportHistoryJson = JsonConvert.SerializeObject(history);
                                    _membershipDa.Update(membership, aGuid);
                                    string Mess = string.Format("{0} sản phẩm được thêm thành công.\t{1} sản phẩm được update.\t{2}", success, erUser, string.Join(",", strUser));
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

        public ActionResult AttrAjax()
        {
            ProductViewModel module = new()
            {
                Product = new ADCOnline.Simple.Base.Product
                {
                    IsShow = true
                },
                SystemActionAdmin = SystemActionAdmin,
                AttributesAdmins = _attributesDa.GetAdminAll(true, Lang()),
            };
            return View(module);
        }

        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            List<AlbumGalleryAdmin> albumGalleryItemList = new();
            AlbumGalleryAdmin albumGalleryItem = new();
            List<ColorTableAdmin> colorTableItemList = new();
            ColorTableAdmin colorTableItem = new();
            List<ADCOnline.Simple.Item.FileDownloadAdmin> fileDownloadAdminList = new();
            ADCOnline.Simple.Item.FileDownloadAdmin fileDownloadAdmin = new();
            string album = string.Empty;
            string color = string.Empty;
            string fileDownload = string.Empty;
            ADCOnline.Simple.Base.Product obj = new();
            MembershipAdmin membership = _membershipDa.GetAdminId(ConvertUtil.ToGuid(session.GetAdminUserId()));
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

                        #region Valid input

                        //title
                        if (string.IsNullOrEmpty(obj.Name))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                            return Ok(msg);
                        }
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.SEOTitle = Utility.ValidString(obj.SEOTitle, Title, true);
                        obj.SeoKeyword = Utility.ValidString(obj.SeoKeyword, Title, true);
                        obj.SeoDescription = Utility.ValidString(obj.SeoDescription, Title, true);
                        //code
                        obj.ProductCode = Utility.ValidString(obj.ProductCode, Title, true);
                        obj.Quantity = obj.Quantity;
                        //url
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        obj.ModuleNameAscii = Utility.RemoveHTML(obj.ModuleNameAscii);
                        //link
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Video = obj.Video;
                        obj.LinkFile = Utility.ValidString(obj.LinkFile, Link, true);
                        obj.LinkDownload = Utility.RemoveHTML(obj.LinkDownload);
                        //arrayid
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                        obj.RelatedIds = Utility.ValidString(obj.RelatedIds, ArrayInt, true);
                        obj.DocumentIds = Utility.ValidString(obj.DocumentIds, ArrayInt, true);
                        obj.ContentIds = Utility.ValidString(obj.ContentIds, ArrayInt, true);
                        obj.GiftIds = Utility.ValidString(obj.GiftIds, ArrayInt, true);
                        obj.ReplaceIds = Utility.ValidString(obj.ReplaceIds, ArrayInt, true);
                        //arraycode
                        //decimal
                        obj.PriceOld = obj.PriceOld.HasValue && obj.PriceOld.Value > 0 ? obj.PriceOld : 0;
                        obj.Price = obj.Price.HasValue && obj.Price.Value > 0 ? obj.Price : 0;
                        obj.TypeSaleValue = obj.TypeSaleValue.HasValue && obj.TypeSaleValue.Value > 0 ? obj.TypeSaleValue : 0;
                        if (obj.TypeSaleValue > 100)
                        {
                            obj.TypeSaleValue = 100;
                        }

                        #endregion Valid input

                        ADCOnline.Simple.Base.Product checkCode = _productDa.GetNameAscii(obj.NameAscii);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại." };
                            return Ok(msg);
                        }
                        var codeNameAscii = Request.Form["CodeNameAscii"];
                        obj.NameAscii += codeNameAscii;
                        var attr = Request.Form["AttributeProductIds"];
                        var viewHome = Request.Form["ViewHome"];
                        obj.ViewHome = Utility.ValidString(viewHome, ArrayInt, true);
                        obj.AttributeProductIds = string.IsNullOrEmpty(attr) ? null : ("," + attr + ",");
                        obj.AttributeProductIds = Utility.ValidString(obj.AttributeProductIds, ArrayInt, true);
                        obj.IsDeleted = false;
                        obj.IsApproved = true;
                        obj.CreatedDate = DateTime.Now;
                        obj.Lang = Lang();
                        obj.ProductGroupCode = obj.ProductGroupCode;
                        obj.CreatorID = ConvertUtil.ToGuid(session.GetAdminUserId());
                        obj.CreatorName = Utility.RemoveHTML(membership.FullName);
                        obj.Description = obj.Description;
                        obj.SEOTitle ??= obj.Name;
                        obj.SeoDescription ??= obj.Name;
                        obj.SeoKeyword ??= obj.Name;
                        if (obj.TypeSale == 2)
                        {
                            obj.TypeSaleValue = Math.Round(obj.TypeSaleValue.Value, 0);
                        }
                        if (obj.Status is 0 or 2)
                        {
                            obj.Amount = 0;
                        }
                        if (!string.IsNullOrEmpty(obj.ModuleIds))
                        {
                            List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(obj.ModuleIds);
                            if (string.IsNullOrEmpty(obj.ModuleNameAscii))
                            {
                                if (lstModule.Any(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale) && !string.IsNullOrEmpty(x.NameAscii) && string.IsNullOrEmpty(x.LinkUrl)))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale));
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && !string.IsNullOrEmpty(x.NameAscii) && string.IsNullOrEmpty(x.LinkUrl)))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => (x.ModuleTypeCode == StaticEnum.Trademark));
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else
                                {
                                    obj.ModuleNameAscii = null;
                                }
                            }
                            if (!obj.BrandId.HasValue || obj.BrandId.Value == 0)
                            {
                                if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0))
                                {
                                    obj.BrandId = lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0).ID;
                                }
                                else
                                {
                                    if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0))
                                    {
                                        WebsiteModule brand = _websiteModuleDa.GetId(lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0).ParentID.Value);
                                        obj.BrandId = brand?.ID;
                                    }
                                }
                            }
                        }
                        var AllFile = Request.Form["FileUrl"];
                        var AllFileName = Request.Form["FileUrl"];
                        obj.LinkDownload = AllFile;

                        #region loadfile

                        fileDownloadAdminList = UpdateModelLst(fileDownloadAdmin, fileDownloadAdminList);
                        if (fileDownloadAdminList.Any())
                        {
                            fileDownloadAdminList = fileDownloadAdminList.OrderBy(c => c.ID).ToList();
                            fileDownload = JsonConvert.SerializeObject(fileDownloadAdminList);
                        }
                        else
                        {
                            fileDownload = null;
                        }
                        obj.LinkDownload = Utility.RemoveHTMLTag(fileDownload);

                        #endregion loadfile

                        #region loadAlbumanh

                        albumGalleryItemList = UpdateModelLst(albumGalleryItem, albumGalleryItemList);
                        if (albumGalleryItemList != null && albumGalleryItemList.Count > 0)
                        {
                            albumGalleryItemList = albumGalleryItemList.OrderBy(c => c.AlbumOrderDisplay).ToList();
                            album = JsonConvert.SerializeObject(albumGalleryItemList);
                        }
                        else
                        {
                            album = null;
                        }
                        obj.AlbumPictureJson = Utility.RemoveHTMLTag(album);

                        #endregion loadAlbumanh

                        #region load subcontent

                        string idsNewSubContent = Request.Form["NewSubContent"];
                        List<SubContentItem> SubContentList = new();
                        string SubContentItem = string.Empty;
                        if (!string.IsNullOrEmpty(idsNewSubContent))
                        {
                            List<int> IdsNew = ListHelper.GetValuesArray(idsNewSubContent);
                            foreach (int item in IdsNew)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                Random random = new Random();
                                var id = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + random.Next(100000, 999999);
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = id,
                                    Name = Utility.RemoveHTMLTag(name),
                                    Content = Utility.RemoveHTMLTag(content),
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }

                        if (SubContentItem.Any())
                        {
                            SubContentList = SubContentList.OrderBy(x => x.OrderDisplay).ToList();
                            SubContentItem = JsonConvert.SerializeObject(SubContentList);
                        }
                        else
                        {
                            SubContentItem = null;
                        }
                        obj.SubContent = Utility.RemoveHTMLTag(SubContentItem);

                        #endregion load subcontent

                        int result = _productDa.Insert(obj);
                        if (!string.IsNullOrEmpty(obj.AttributeProductIds))
                        {
                            List<int> lstAttrIds = ListHelper.GetValuesArray(obj.AttributeProductIds);
                            foreach (int item in lstAttrIds)
                            {
                                if (item > 0)
                                {
                                    var price = Request.Form["AttributePrice_" + item];
                                    var picture = Request.Form["AttributeUrlPicture_" + item];
                                    Attribute_WebsiteContent option = new()
                                    {
                                        AttributeID = item,
                                        ContentID = result,
                                        UrlPicture = Utility.ValidString(picture, Link, true),
                                        Price = !string.IsNullOrEmpty(price) ? Convert.ToDecimal(price) : 0
                                    };
                                    _productDa.InsertAttr(option);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, null);
                        }
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                        msg = new JsonMessage { Errors = true, Message = ex.Message, Obj = obj };
                        return Ok(msg);
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
                        obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ADCOnline.Simple.Base.Product oldObj = obj;
                        await TryUpdateModelAsync(obj);

                        #region Valid input

                        //title
                        if (string.IsNullOrEmpty(obj.Name))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa nhập tiêu đề" };
                            return Ok(msg);
                        }
                        obj.Name = Utility.ValidString(obj.Name, Title, true);
                        obj.SEOTitle = Utility.ValidString(obj.SEOTitle, Title, true);
                        obj.SeoKeyword = Utility.ValidString(obj.SeoKeyword, Title, true);
                        obj.SeoDescription = Utility.ValidString(obj.SeoDescription, Title, true);
                        obj.ProductGroupCode = obj.ProductGroupCode;
                        //code
                        obj.ProductCode = Utility.ValidString(obj.ProductCode, Title, true);
                        obj.Quantity = obj.Quantity;
                        obj.TimeStart = obj.TimeStart ?? DateTime.Now;
                        //url
                        obj.NameAscii = Utility.ConvertRewrite(obj.NameAscii);
                        obj.ModuleNameAscii = Utility.RemoveHTML(obj.ModuleNameAscii);
                        //link
                        obj.LinkUrl = Utility.ValidString(obj.LinkUrl, Link, true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Video = obj.Video;
                        obj.LinkFile = Utility.ValidString(obj.LinkFile, Link, true);
                        obj.LinkDownload = Utility.RemoveHTML(obj.LinkDownload);
                        //arrayid
                        obj.ModuleIds = Utility.ValidString(obj.ModuleIds, ArrayInt, true);
                        obj.ContentIds = Utility.ValidString(obj.ContentIds, ArrayInt, true);
                        obj.DocumentIds = Utility.ValidString(obj.DocumentIds, ArrayInt, true);
                        obj.RelatedIds = Utility.ValidString(obj.RelatedIds, ArrayInt, true);
                        obj.ReplaceIds = Utility.ValidString(obj.ReplaceIds, ArrayInt, true);
                        obj.GiftIds = Utility.ValidString(obj.GiftIds, ArrayInt, true);
                        //arraycode
                        //decimal
                        obj.PriceOld = ConvertUtil.ToDecimal(obj.PriceOld);
                        obj.Price = ConvertUtil.ToDecimal(obj.Price);

                        #endregion Valid input

                        if (!string.IsNullOrEmpty(obj.NameAscii))
                        {
                            ADCOnline.Simple.Base.Product checkCode = _productDa.GetNameAscii(obj.NameAscii);
                            if (checkCode != null && checkCode.ID != obj.ID)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Link đã tồn tại." };
                                return Ok(msg);
                            }
                        }
                        var codeNameAscii = Request.Form["CodeNameAscii"];
                        obj.NameAscii += codeNameAscii;
                        var attr = Request.Form["AttributeProductIds"];
                        var viewHome = Request.Form["ViewHome"];
                        obj.ViewHome = viewHome;
                        obj.ModifiedDate = obj.ModifiedDate ?? DateTime.Now;
                        obj.CreatedDate = obj.CreatedDate ?? DateTime.Now;
                        obj.AttributeProductIds = string.IsNullOrEmpty(attr) ? null : ("," + attr + ",");
                        obj.AttributeProductIds = Utility.ValidString(obj.AttributeProductIds, ArrayInt, true);
                        obj.ModifiedName = Utility.RemoveHTML(membership.FullName);
                        obj.Description = obj.Description;
                        obj.SEOTitle ??= obj.Name;
                        obj.SeoDescription ??= Utility.RemoveHTMLTag(obj.Name);
                        obj.SeoKeyword ??= obj.Name;
                        if (obj.TypeSale == 2)
                        {
                            obj.TypeSaleValue = Math.Round(obj.TypeSaleValue.Value, 0);
                        }
                        if (obj.Status is 0 or 2)
                        {
                            obj.Amount = 0;
                        }
                        if (!string.IsNullOrEmpty(obj.ModuleIds))
                        {
                            List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(obj.ModuleIds);
                            if (string.IsNullOrEmpty(obj.ModuleNameAscii))
                            {
                                if (lstModule.Any(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale) && !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale) && !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null);
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null))
                                {
                                    WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && !string.IsNullOrEmpty(x.NameAscii) && x.NameAscii != null && string.IsNullOrEmpty(x.LinkUrl) && x.LinkUrl == null);
                                    obj.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                }
                                else
                                {
                                    obj.ModuleNameAscii = null;
                                }
                            }
                            if (!obj.BrandId.HasValue || obj.BrandId.Value == 0)
                            {
                                if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0))
                                {
                                    obj.BrandId = lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0).ID;
                                }
                                else
                                {
                                    if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0))
                                    {
                                        WebsiteModule brand = _websiteModuleDa.GetId(lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0).ParentID.Value);
                                        obj.BrandId = brand?.ID;
                                    }
                                }
                            }
                        }
                        var AllFile = Request.Form["FileUrl"];
                        obj.LinkDownload = Utility.RemoveHTMLTag(AllFile);

                        #region loadfile

                        fileDownloadAdminList = UpdateModelLst(fileDownloadAdmin, fileDownloadAdminList);
                        if (fileDownloadAdminList.Any())
                        {
                            fileDownloadAdminList = fileDownloadAdminList.OrderBy(c => c.ID).ToList();
                            fileDownload = JsonConvert.SerializeObject(fileDownloadAdminList);
                        }
                        else
                        {
                            fileDownload = null;
                        }
                        obj.LinkDownload = Utility.RemoveHTMLTag(fileDownload);

                        #endregion loadfile

                        #region loadAlbumanh

                        var st = Request.Form["AlbumIsShow"];
                        albumGalleryItemList = UpdateModelLst(albumGalleryItem, albumGalleryItemList);
                        if (albumGalleryItemList != null && albumGalleryItemList.Count > 0)
                        {
                            albumGalleryItemList = albumGalleryItemList.OrderBy(c => c.AlbumOrderDisplay).ToList();
                            album = JsonConvert.SerializeObject(albumGalleryItemList);
                        }
                        else
                        {
                            album = null;
                        }
                        obj.AlbumPictureJson = Utility.RemoveHTMLTag(album);
                        var UrlPicture = Request.Form["UrlPicture"];
                        obj.UrlPicture = Utility.ValidString(UrlPicture, Link, true);

                        #endregion loadAlbumanh

                        #region load subcontent

                        string idsNewSubContent = Request.Form["NewSubContent"];
                        string idsOldSubContent = Request.Form["OldSubContent"];
                        List<SubContentItem> SubContentList = new();
                        string SubContentItem = string.Empty;
                        if (!string.IsNullOrEmpty(idsNewSubContent))
                        {
                            List<int> IdsNew = ListHelper.GetValuesArray(idsNewSubContent);
                            foreach (int item in IdsNew)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                Random random = new Random();
                                var id = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + random.Next(100000, 999999);
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = id,
                                    Name = Utility.RemoveHTMLTag(name),
                                    Content = Utility.RemoveHTMLTag(content),
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }
                        if (!string.IsNullOrEmpty(idsOldSubContent))
                        {
                            List<int> IdsOld = ListHelper.GetValuesArray(idsOldSubContent);
                            foreach (int item in IdsOld)
                            {
                                var name = Request.Form["Name_" + item];
                                var content = Request.Form["Content_" + item];
                                var order = Request.Form["OrderDisplay_" + item];
                                var img = Request.Form["UrlPicture_" + item];
                                SubContentItem newContentSub = new SubContentItem
                                {
                                    ID = item,
                                    Name = Utility.RemoveHTMLTag(name),
                                    Content = Utility.RemoveHTMLTag(content),
                                    OrderDisplay = Convert.ToInt32(order),
                                    UrlPicture = !string.IsNullOrEmpty(img) ? Utility.RemoveHTMLTag(img) : null,
                                    IsShow = Request.Form["IsShow_" + item].Count == 1 ? true : false
                                };
                                SubContentList.Add(newContentSub);
                            }
                        }

                        if (SubContentList.Any())
                        {
                            SubContentList = SubContentList.OrderBy(x => x.OrderDisplay).ToList();
                            SubContentItem = JsonConvert.SerializeObject(SubContentList);
                        }
                        else
                        {
                            SubContentItem = null;
                        }
                        obj.SubContent = Utility.RemoveHTMLTag(SubContentItem);

                        #endregion load subcontent

                        int result = _productDa.Update(obj);
                        _productDa.DeleteAttr(obj.ID);
                        if (!string.IsNullOrEmpty(obj.AttributeProductIds))
                        {
                            List<int> lstAttrIds = ListHelper.GetValuesArray(obj.AttributeProductIds);
                            foreach (int item in lstAttrIds.Where(x => x > 0))
                            {
                                if (item > 0)
                                {
                                    var price = Request.Form["AttributePrice_" + item];
                                    var picture = Request.Form["AttributeUrlPicture_" + item];
                                    Attribute_WebsiteContent option = new() { AttributeID = item, ContentID = obj.ID, UrlPicture = picture, Price = !string.IsNullOrEmpty(price) ? Convert.ToDecimal(price) : 0 };
                                    _productDa.InsertAttr(option);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapProduct(null, oldObj);
                        }
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Obj = obj };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
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
                        try
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(Convert.ToInt32(action.ItemId));
                            content.IsDeleted = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl))
                            {
                                await UpdateSitemapProduct(null, content);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.Display:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ADCOnline.Simple.Base.Product oldObj = obj;
                        obj.IsShow = true;
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _productDa.Update(obj);
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapProduct(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Hiển thị quản lý nội dung:" + obj.Name, "Actions-Display");
                        msg = new JsonMessage { Errors = false, Message = "Hiện thị thành công." };
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
                        obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ADCOnline.Simple.Base.Product oldObj = obj;
                        obj.IsShow = obj.IsShow == true ? false : true;
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị thành công" : "Ẩn thành công";
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _productDa.Update(obj);
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapProduct(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + obj.Name, "Actions-Ẩn");
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
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsShow = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
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
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsShow = false;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
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
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsDeleted = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsVat:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ADCOnline.Simple.Base.Product oldObj = obj;
                        obj.IsVAT = obj.IsVAT != true;
                        string message = "Cập nhật thành công";
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _productDa.Update(obj);
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapProduct(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsVatAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsVAT = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Chọn giá đã bao gồm VAT:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotIsVatALL:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsVAT = false;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Bỏ chọn giá đã bao gồm VAT:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsSitemap:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        ADCOnline.Simple.Base.Product oldObj = obj;
                        obj.IsSitemap = obj.IsSitemap != true;
                        string message = "Cập nhật thành công";
                        obj.ModifiedDate = DateTime.Now;
                        obj.ModifiedName = membership.FullName;
                        int result = _productDa.Update(obj);
                        if (!string.IsNullOrEmpty(obj.NameAscii) && string.IsNullOrEmpty(obj.LinkUrl) && obj.IsShow == true && obj.IsSitemap == true)
                        {
                            await UpdateSitemapProduct(obj, oldObj);
                        }
                        else
                        {
                            await UpdateSitemapProduct(null, oldObj);
                        }
                        AddLogAdmin(Request.Path, "Cập nhật thành công sản phẩm:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsSitemapAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsSitemap = true;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Thêm sitemap:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotIsSitemapAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.IsSitemap = false;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        AddLogAdmin(Request.Path, "Bỏ sitemap:" + obj.Name, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công" };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.BestSell:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "2";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",2,"))
                            {
                                content.ViewHome += ",2";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotBestSell:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",2,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",2,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.ChangeStatus:
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
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.Status = Convert.ToInt32(action.Status);
                            if (content.Status is 0 or 2)
                            {
                                content.Amount = 0;
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.ChangeStatusAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.Status = Convert.ToInt32(action.Status);
                            if (content.Status is 0 or 2)
                            {
                                content.Amount = 0;
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "cập nhật thành công."
                        };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsNew:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "0";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",0,"))
                            {
                                content.ViewHome += ",0";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotNew:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",0,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",0,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsHot:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "3";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",3,"))
                            {
                                content.ViewHome += ",3";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotHot:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",3,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",3,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsHome:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "1";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",1,"))
                            {
                                content.ViewHome += ",1";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotHome:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",1,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",1,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsShock:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "5";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",5,"))
                            {
                                content.ViewHome += ",5";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotShock:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",5,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",5,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.IsSelling:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (string.IsNullOrEmpty(content.ViewHome))
                            {
                                content.ViewHome = "2";
                            }
                            else if (!("," + content.ViewHome + ",").Contains(",2,"))
                            {
                                content.ViewHome += ",2";
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.NotIsSelling:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            if (("," + content.ViewHome + ",").Contains(",2,"))
                            {
                                content.ViewHome = ("," + content.ViewHome + ",").Replace(",2,", ",").Trim(',');
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
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
                                    ADCOnline.Simple.Base.Product content = _productDa.GetId(item.ID);
                                    content.OrderDisplay = item.OrderDisplay;
                                    content.ModifiedDate = DateTime.Now;
                                    content.ModifiedName = membership.FullName;
                                    _productDa.Update(content);
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

                case ActionType.ChangePriceOld:
                    try
                    {
                        if (!SystemActionAdmin.Edit)
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
                                    item.Price = item.Price.Replace("đ", "").Replace(",", "").Replace(",", "").Replace(",", "");
                                    if (!Utility.CheckDecimal(item.Price))
                                    {
                                        msg = new JsonMessage { Errors = true, Message = "Giá trị không đúng" };
                                        return Ok(msg);
                                    }
                                    ADCOnline.Simple.Base.Product content = _productDa.GetId(item.ID);
                                    ADCOnline.Simple.Base.Product oldObj = content;
                                    content.Price = Convert.ToDecimal(item.Price);
                                    content.ModifiedDate = DateTime.Now;
                                    content.ModifiedName = membership.FullName;
                                    _productDa.Update(content);
                                    if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                    {
                                        await UpdateSitemapProduct(content, oldObj);
                                    }
                                    else
                                    {
                                        await UpdateSitemapProduct(null, oldObj);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Request.Form["OrderByValuesOld"]))
                        {
                            var orderValues = Request.Form["OrderByValuesOld"];
                            List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                            foreach (OrderByItem item in listOrderByItem)
                            {
                                try
                                {
                                    ADCOnline.Simple.Base.Product content = _productDa.GetId(item.ID);
                                    ADCOnline.Simple.Base.Product oldObj = content;
                                    if (string.IsNullOrEmpty(item.Price))
                                    {
                                        content.PriceOld = null;
                                    }
                                    else
                                    {
                                        item.Price = item.Price.Replace("đ", "").Replace(",", "").Replace(",", "").Replace(",", "");
                                        if (!Utility.CheckDecimal(item.Price))
                                        {
                                            msg = new JsonMessage { Errors = true, Message = "Giá trị không đúng" };
                                            return Ok(msg);
                                        }
                                        content.PriceOld = Convert.ToDecimal(item.Price);
                                    }
                                    if (content.TypeSaleValue.HasValue && content.TypeSale == 1)
                                    {
                                        decimal amount = Convert.ToDecimal(content.PriceOld.Value * Convert.ToDecimal(content.TypeSaleValue.Value) / 100);
                                        content.Price = content.PriceOld - amount;
                                        content.DiscountAmount = amount;
                                    }
                                    if (content.DiscountAmount.HasValue && content.DiscountAmount.Value > 0 && content.TypeSale == 2)
                                    {
                                        content.Price = content.PriceOld - content.DiscountAmount.Value;
                                    }
                                    content.ModifiedDate = DateTime.Now;
                                    content.ModifiedName = membership.FullName;
                                    _productDa.Update(content);
                                    if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                    {
                                        await UpdateSitemapProduct(content, oldObj);
                                    }
                                    else
                                    {
                                        await UpdateSitemapProduct(null, oldObj);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                        }
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

                case ActionType.ChangeSale:
                    try
                    {
                        if (!SystemActionAdmin.Edit)
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
                                    ADCOnline.Simple.Base.Product content = _productDa.GetId(item.ID);
                                    ADCOnline.Simple.Base.Product oldObj = content;
                                    if (string.IsNullOrEmpty(item.Price) || item.Price == "%")
                                    {
                                        content.TypeSaleValue = 0;
                                    }
                                    else
                                    {
                                        item.Price = item.Price.Replace("%", "");
                                        if (!Utility.CheckDecimal(item.Price))
                                        {
                                            return Ok(new JsonMessage { Errors = true, Message = "Giá trị không đúng" });
                                        }
                                        content.TypeSaleValue = Convert.ToDouble(item.Price);
                                    }
                                    decimal amount = Convert.ToDecimal(content.PriceOld.Value * Convert.ToDecimal(content.TypeSaleValue.Value) / 100);
                                    content.Price = content.PriceOld - amount;
                                    content.DiscountAmount = amount;
                                    content.ModifiedDate = DateTime.Now;
                                    content.ModifiedName = membership.FullName;
                                    _productDa.Update(content);
                                    if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                                    {
                                        await UpdateSitemapProduct(content, oldObj);
                                    }
                                    else
                                    {
                                        await UpdateSitemapProduct(null, oldObj);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AddLogError(ex);
                                }
                            }
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật giảm giá thành công." };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.UpdateSale:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        int saleValue = !string.IsNullOrEmpty(Request.Form["SaleValue"]) ? Convert.ToInt32(Request.Form["SaleValue"]) : 0;
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.TypeSaleValue = saleValue;
                            content.Price = content.PriceOld - content.PriceOld * Convert.ToDecimal(content.TypeSaleValue) / 100;
                            content.DiscountAmount = content.PriceOld * Convert.ToDecimal(content.TypeSaleValue) / 100;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.UpdateAmount:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        int Amount = ConvertUtil.ToInt32(Request.Form["Amount"]);
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.Amount = Amount;
                            content.Status = 1;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.ChangeViewHome:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        string ViewHome = !string.IsNullOrEmpty(Request.Form["ViewHome"]) ? Request.Form["ViewHome"].ToString() : string.Empty;
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            ADCOnline.Simple.Base.Product oldObj = content;
                            content.ViewHome = ViewHome;
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                            if (!string.IsNullOrEmpty(content.NameAscii) && string.IsNullOrEmpty(content.LinkUrl) && content.IsShow == true && content.IsSitemap == true)
                            {
                                await UpdateSitemapProduct(content, oldObj);
                            }
                            else
                            {
                                await UpdateSitemapProduct(null, oldObj);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.AddModule:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        if (ModuleID.Count == 0)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Chưa chọn module" };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            foreach (int module in ModuleID)
                            {
                                if (string.IsNullOrEmpty(content.ModuleIds))
                                {
                                    content.ModuleIds = module.ToString();
                                }
                                else
                                {
                                    if (!("," + content.ModuleIds + ",").Contains("," + module + ","))
                                    {
                                        content.ModuleIds = content.ModuleIds + "," + module;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(content.ModuleIds))
                            {
                                List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(content.ModuleIds);
                                if (string.IsNullOrEmpty(obj.ModuleNameAscii))
                                {
                                    var moduleNameAscii = Request.Form["ModuleNameAsciiNew"];
                                    obj.ModuleNameAscii = moduleNameAscii;
                                    if (lstModule.Any(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale) && !string.IsNullOrEmpty(x.NameAscii) && string.IsNullOrEmpty(x.LinkUrl)))
                                    {
                                        WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => (x.ModuleTypeCode == StaticEnum.Product || x.ModuleTypeCode == StaticEnum.Sale));
                                        content.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                    }
                                    else if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && !string.IsNullOrEmpty(x.NameAscii) && string.IsNullOrEmpty(x.LinkUrl)))
                                    {
                                        WebsiteModuleAdmin module = lstModule.FirstOrDefault(x => (x.ModuleTypeCode == StaticEnum.Trademark));
                                        content.ModuleNameAscii = Utility.Link(module.NameAscii, string.Empty, module.LinkUrl).Trim('/');
                                    }
                                    else
                                    {
                                        content.ModuleNameAscii = null;
                                    }
                                }
                                if (!obj.BrandId.HasValue || obj.BrandId.Value == 0)
                                {
                                    if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0))
                                    {
                                        content.BrandId = lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0).ID;
                                    }
                                    else if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0))
                                    {
                                        WebsiteModule brand = _websiteModuleDa.GetId(lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0).ParentID.Value);
                                        content.BrandId = brand?.ID;
                                    }
                                }
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;

                case ActionType.RemoveModule:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        if (ModuleID.Count == 0)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Chưa chọn module" };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            ADCOnline.Simple.Base.Product content = _productDa.GetId(item);
                            foreach (int module in ModuleID)
                            {
                                if (("," + content.ModuleIds + ",").Contains("," + module + ","))
                                {
                                    content.ModuleIds = ("," + content.ModuleIds + ",").Replace("," + module + ",", ",").Trim(',');
                                }
                            }
                            if (!string.IsNullOrEmpty(content.ModuleIds))
                            {
                                List<WebsiteModuleAdmin> lstModule = _websiteModuleDa.GetListByArrId(content.ModuleIds);
                                if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0))
                                {
                                    content.BrandId = lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID == 0).ID;
                                }
                                else if (lstModule.Any(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0))
                                {
                                    WebsiteModule brand = _websiteModuleDa.GetId(lstModule.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.Trademark && x.ParentID != 0).ParentID.Value);
                                    content.BrandId = brand?.ID;
                                }
                                else
                                {
                                    content.BrandId = null;
                                }
                            }
                            else
                            {
                                content.BrandId = null;
                            }
                            content.ModifiedDate = DateTime.Now;
                            content.ModifiedName = membership.FullName;
                            _productDa.Update(content);
                        }
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
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

        [HttpPost]
        public async Task<ActionResult> ResetOrder()
        {
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                if (SystemActionAdmin.Order != true)
                {
                    msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                    return Ok(msg);
                }
                string role = HttpContext.Session.GetString("WebAdminRole");
                string userId = HttpContext.Session.GetString("WebAdminUserID");
                string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
                ADCOnline.Simple.Admin.SearchModel seach = new();
                await TryUpdateModelAsync(seach);
                seach.lang = Lang();
                List<ProductAdmin> products = _productDa.ListProductSort(seach, moduleIds);
                if (products.Any())
                {
                    List<ProductAdmin> ListItems = products.ToList();
                    int i = 1;
                    ListItems.ForEach(x =>
                    {
                        _productDa.UpdateSort(i, x.ID);
                        i++;
                    });
                }
                msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                return Ok(msg);
            }
            catch
            {
            }
            return Ok(msg);
        }

        [HttpGet]
        public ActionResult AjaxSale(string ids)
        {
            ProductViewModel model = new()
            {
                ContentIds = ids,
                SystemActionAdmin = SystemActionAdmin
            };
            ViewBag.Action = ActionType.UpdateSale;
            ViewBag.ActionText = ActionType.ActionText(ViewBag.Action);
            return View(model);
        }

        [HttpGet]
        public ActionResult AjaxChangeModule(string ids)
        {
            ActionViewModel action = UpdateModelAction();
            ProductViewModel model = new()
            {
                ContentIds = ids,
                SystemActionAdmin = SystemActionAdmin
            };
            ViewBag.Action = action.Do;
            ViewBag.ActionText = ActionType.ActionText(ViewBag.Action);
            return View(model);
        }

        public ActionResult AjaxSelectProduct()
        {
            ActionViewModel action = UpdateModelAction();
            ProductViewModel model = new();
            ViewBag.Action = action.Do;
            ViewBag.ActionText = ActionType.ActionText(ViewBag.Action);
            return View(model);
        }
    }
}