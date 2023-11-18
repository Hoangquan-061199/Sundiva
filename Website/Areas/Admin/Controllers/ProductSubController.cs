using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using Website.Utils;

namespace Website.Areas.Admin.Controllers
{
    public class ProductSubController : BaseController
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

        public ProductSubController(IWebHostEnvironment env)
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
        public IActionResult Index(int? parentId)
        {
            if (!parentId.HasValue || parentId.Value == 0)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }

            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Product");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            ProductViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module,
                ParentID = parentId.Value
            };
            return View(model);
        }

        public IActionResult ListItems()
        {
            var parentId = Request.Query["parentId"];
            SearchModel seach = new();
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
                ListItem = _productDa.ListSearchByParentId(seach, seach.page, seach.pagesize > 0 ? seach.pagesize : 50, false, moduleIds, Convert.ToInt32(parentId)),
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                SearchModel = seach,
                Product = seach.productId.HasValue ? _productDa.GetId(seach.productId.Value) : new Product(),
                moduleParentID = ConvertUtil.ToInt32(moduleID)
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, seach.pagesize > 0 ? seach.pagesize : 50);
            model.Total = _productDa.CountProductByModuleIds(string.Empty, Lang());
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
            };
            if (action.Do == ActionType.Edit)
            {
                ADCOnline.Simple.Base.Product obj = _productDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (obj != null)
                {
                    module.SubItems = _productDa.GetSubItemByProductId(obj.ID);
                    module.Product = obj;
                    module.ListWebsiteModuleAdmin = _websiteModuleDa.GetListByArrIdNotShow(obj.ModuleIds);
                    if (module.ListWebsiteModuleAdmin != null && !string.IsNullOrEmpty(module.ListWebsiteModuleAdmin.FirstOrDefault().TypeView))
                    {
                        ViewBag.TypeView = module.ListWebsiteModuleAdmin.FirstOrDefault().TypeView;
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
                var parentId = Request.Query["parentId"];
                if (!string.IsNullOrEmpty(parentId))
                {
                    var itemParent = _productDa.GetId(Convert.ToInt32(parentId));
                    action.ModuleId = itemParent.ModuleIds;
                    ViewBag.ParentID = parentId;
                }
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
                    }
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            if (ViewBag.TypeView == StaticEnum.TypeProduct) return View("~/Areas/Admin/Views/ProductSub/AjaxFormType.cshtml", module);
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

                        

                        int result = _productDa.Insert(obj);
                        if (!string.IsNullOrEmpty(obj.AttributeProductIds))
                        {
                            List<int> lstAttrIds = ListHelper.GetValuesArray(obj.AttributeProductIds);
                            foreach (int item in lstAttrIds)
                            {
                                if (item > 0)
                                {
                                    var name = Request.Form["AttributeName_" + item];
                                    var colorcode = Request.Form["AttributeColor_" + item];
                                    var order = Request.Form["AttributeOrderDisplay_" + item];
                                    var picture = Request.Form["AttributeUrlPicture_" + item];
                                    Attribute_WebsiteContent option = new()
                                    {
                                        AttributeID = item,
                                        ContentID = obj.ID,
                                        UrlPicture = picture,
                                        Name = name,
                                        OrderDisplay = Convert.ToInt32(order),
                                        Color = colorcode,
                                        Price = 0
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
                        obj.ModifiedDate = DateTime.Now;
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

                        int result = _productDa.Update(obj);
                        _productDa.DeleteAttr(obj.ID);
                        if (!string.IsNullOrEmpty(obj.AttributeProductIds))
                        {
                            List<int> lstAttrIds = ListHelper.GetValuesArray(obj.AttributeProductIds);
                            foreach (int item in lstAttrIds.Where(x => x > 0))
                            {
                                if (item > 0)
                                {
                                    var name = Request.Form["AttributeName_" + item];
                                    var colorcode = Request.Form["AttributeColor_" + item];
                                    var order = Request.Form["AttributeOrderDisplay_" + item];
                                    var picture = Request.Form["AttributeUrlPicture_" + item];
                                    Attribute_WebsiteContent option = new()
                                    {
                                        AttributeID = item,
                                        ContentID = obj.ID,
                                        UrlPicture = picture,
                                        Name = name,
                                        OrderDisplay = Convert.ToInt32(order),
                                        Color = colorcode,
                                        Price = 0
                                    };
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
            }

            return Ok(msg);
        }
    }
}
