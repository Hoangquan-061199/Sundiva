using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Website.Utils;
using Website.ViewModels;

namespace Website.Controllers
{
    public class ContentController : BaseController
    {
        private readonly ModulePositionManager _positionManager;
        private readonly WebsiteModuleManager _webModuleManager;
        private readonly WebsiteContentManager _webContentManager;
        private readonly ModulePositionManager _modulePositionManager;
        private readonly ProductManager _productManager;
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        private readonly DapperDA _dapperDa;
        private readonly CustomerManager customerManager;
        private readonly CommentManager _commentManager;
        private readonly CartManager _cartManager;
        private readonly CustomerManager _customerManager;
        private readonly OtherContentManager _otherContentManager;
        private readonly AgencyManager _agencyManager;
        private readonly SystemTagManager _systemTagManager;
        private readonly SubItemManager _subItemManager;

        public ContentController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
            _positionManager = new ModulePositionManager(WebConfig.ConnectionString);
            _agencyManager = new AgencyManager(WebConfig.ConnectionString);
            _webModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
            _modulePositionManager = new ModulePositionManager(WebConfig.ConnectionString);
            _webContentManager = new WebsiteContentManager(WebConfig.ConnectionString);
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _cartManager = new CartManager(WebConfig.ConnectionString);
            customerManager = new CustomerManager(WebConfig.ConnectionString);
            _commentManager = new CommentManager(WebConfig.ConnectionString);
            _otherContentManager = new OtherContentManager(WebConfig.ConnectionString);
            _customerManager = new CustomerManager(WebConfig.ConnectionString);
            _systemTagManager = new SystemTagManager(WebConfig.ConnectionString);
            _subItemManager = new SubItemManager(WebConfig.ConnectionString);
        }

        public async Task<IActionResult> Index(string nameAscii, string nameAsciiC, int? p)
        {
            string path = HttpUtility.UrlDecode(Request.Path).ToLower();
            if (!string.IsNullOrEmpty(path))
            {
                List<RedirectJson> listredirect = JsonConvert.DeserializeObject<List<RedirectJson>>(ReadFile("Redirect.json", "DataJson"));
                if (listredirect != null && listredirect.Any(x => HttpUtility.UrlDecode(x.OldUrl).ToLower() == path))
                {
                    RedirectJson newurl = listredirect.FirstOrDefault(x => HttpUtility.UrlDecode(x.OldUrl).ToLower() == path);
                    return newurl.TypeRedirect == "302" ? Redirect(newurl.NewUrl) : RedirectPermanent(newurl.NewUrl);
                }
            }
            if (!string.IsNullOrEmpty(nameAscii) && !string.IsNullOrEmpty(nameAsciiC) && nameAsciiC.Contains(".htm"))
            {
                string newLink = "/" + Utility.ValidString(nameAsciiC.Replace(".htm", ""), "301", true).ToLower();
                var page = Request.Query["page"];
                if (!string.IsNullOrEmpty(page) && Utility.IsNumber(page) && ConvertUtil.ToInt32(page.ToString()) > 1)
                {
                    newLink += "/page/" + page;
                }
                return RedirectPermanent(newLink);
            }
            if (nameAscii != null && nameAscii.Contains(".htm"))
            {
                return RedirectPermanent("/" + nameAscii.Replace(".htm", "").ToLower());
            }
            WebsiteModulesItem module = new();
            module = nameAscii == "all" && !string.IsNullOrEmpty(nameAsciiC)
                ? await cacheUtils.GetModuleByNameAscii(nameAsciiC)
                : await cacheUtils.GetModuleByNameAscii(nameAscii);
            string moduleType = string.Empty;
            try
            {
                if (module != null && (string.IsNullOrEmpty(nameAsciiC) || (!string.IsNullOrEmpty(nameAsciiC) && nameAscii == "all")))
                {
                    if (module.IsLogin == true && UserId == 0)
                    {
                        return Redirect("/dang-nhap");
                    }
                    if (Lang() != (module.Lang.Trim()))
                    {
                        SetCookies("lang", module.Lang.Trim(), 10);
                        Response.Redirect(HttpContext.Request.GetDisplayUrl());
                    }
                    SearchModel search = new();
                    await TryUpdateModelAsync(search);
                    search.page = p ?? 1;
                    search.lang = Lang();
                    search.sort = module.ModuleTypeCode == StaticEnum.News ? search.sort == 0 ? 1 : search.sort : search.sort > 0 ? search.sort : 0;
                    moduleType = module.ModuleTypeCode;
                    module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                    int total = 0;
                    switch (moduleType)
                    {
                        #region Giới thiệu
                        case StaticEnum.Introduce:
                            {
                                List<WebsiteModulesItem> Allmodule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang())
                                };
                                model.ListModuleItems = await cacheUtils.GetListModuleChildID(module.ID, Lang());
                                model.ListContentItemAsync = await _webContentManager.GetListContent(search, 0, module.ID, "0", "0");

                                search.sort = 5;
                                if (model.ListModuleItems != null)
                                {
                                    foreach (var item in model.ListModuleItems)
                                    {
                                        item.ListContentItem = await _webContentManager.GetListContent(search, 0, item.ID, "", "0");
                                    }
                                }
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/Content/Introduce.cshtml", model);
                            }
                        #endregion Giới thiệu
                        #region Sản phẩm
                        case StaticEnum.Product:
                            {
                                SetWebBack(nameAscii);
                                SystemConfigJson config = cacheUtils.SystemConfigItem(Lang());
                                //config.ConfigPopup = JsonConvert.DeserializeObject<Dictionary<string, string>>(config.ConfigPopupJson);
                                module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();

                                ModuleViewModels model = new()
                                {
                                    Schema = await cacheUtils.GetOtherItemByCode("SchemaCategoryProduct", Lang()),
                                    BreadcrumbList = await cacheUtils.GetListBreadcrumb(module.ID, Lang()),
                                    ModuleItem = module,
                                    SystemConfigItem = config,
                                    WebsiteModulesItem = _webModuleManager.GetByTypeCode(StaticEnum.Contact, Lang()),
                                    AdvertisingItems = cacheUtils.GetListAdvertisingItemByCode(StaticEnum.TourIndex, Lang())
                                };

                                var moduleNews = _webModuleManager.GetByTypeCode(StaticEnum.News, Lang());
                                var moduleNewschild = cacheUtils.GetListModuleChidrentNotAsync(moduleNews.ID);
                                model.ListContentItemHot = _webContentManager.GetListContentHot(3, moduleNews.ID, string.Join(",", moduleNewschild.Select(x => x.ID)));
                                int pageSize = 6;

                                #region Lọc (Ẩn)

                                //List<string> lstattr = new();
                                //for (int idx = 0; idx < Request.Query.Keys.Count; idx++)
                                //{
                                //    string key = Request.Query.Keys.ToList()[idx];
                                //    var checkattr = _productManager.CheckAttrByNameAscii(key);
                                //    if (key != "sort" && key != "seoUrl" && key != "page" && key != "gia" && key != "hsx" && key != "size" && key != "view" && Utility.IsArrIds(Request.Query[key]) && checkattr == true)
                                //        lstattr.Add(Request.Query[key]);
                                //}
                                //search.attr = string.Join(",", lstattr);
                                //search.ListAttr = lstattr;
                                ////search.view = string.IsNullOrEmpty(search.view) ? "grid" : search.view;
                                //model.AttributeItemsRight = _productManager.GetAllAttributeByListIdsAndAllParent(module.AttributeModuleIds, module.ID, string.Join(",", AllModule.Select(x => x.ID)));

                                #endregion Lọc (Ẩn)

                                List<string> lstattr = new();
                                for (int idx = 0; idx < Request.Query.Keys.Count; idx++)
                                {
                                    string key = Request.Query.Keys.ToList()[idx];
                                    //bool checkattr = _productManager.CheckAttrByNameAscii(key);
                                    if (key == "brandids")
                                    {
                                        lstattr.Add(Request.Query[key]);
                                    }
                                }

                                search.attr = string.Join(",", lstattr);
                                model.ListModuleItems = await cacheUtils.GetListModuleChildID(module.ID, Lang());
                                var childModule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                List<ProductItem> listProduct = _productManager.GetListProduct(new SearchModel { page = search.page, lang = Lang(), sort = 5 }, pageSize, module.ID, string.Join(",", childModule.Select(x => x.ID)), "0");
                                model.ListModulesItemTour = childModule;
                                var moduleproduct = _webModuleManager.GetByModuleTypeCode(StaticEnum.Product, Lang());
                                model.WebsiteModulesItems = moduleproduct != null ? moduleproduct : new();

                                #region check khuyen mai (Ẩn)

                                //IEnumerable<PromotionItem> allpromotion = await _productManager.GetAllPromotionAsync();
                                //listProduct.ToAsyncEnumerable().ForEach(x =>
                                //{
                                //    if ((!x.TypeSaleValue.HasValue || x.TypeSaleValue == 0) && (!x.DiscountAmount.HasValue || x.DiscountAmount == 0))
                                //    {
                                //        List<int> ListIds = ListHelper.GetValuesArray(x.ModuleIds);
                                //        if (allpromotion.Any(m => ("," + m.ProductIds + ",").Contains("," + x.ID + ",") || (m.ForAll == true && ListHelper.GetValuesArray(m.ModuleIds).Intersect(ListIds).Count() > 0)))
                                //        {
                                //            PromotionItem listSale = allpromotion.Where(n => ("," + n.ProductIds + ",").Contains("," + x.ID + ",") || (n.ForAll == true && ListHelper.GetValuesArray(n.ModuleIds).Intersect(ListIds).Count() > 0)).OrderBy(n => n.OrderDisplay).FirstOrDefault();
                                //            if (listSale.TypeSale == 2 && listSale.DiscountAmount > 0)
                                //            {
                                //                x.TypeSale = listSale.TypeSale;
                                //                x.TypeSaleValue = 0;
                                //                x.Price = x.PriceOld - listSale.DiscountAmount;
                                //            }
                                //            else if (listSale.TypeSale == 1 && listSale.SaleValue > 0)
                                //            {
                                //                x.TypeSale = listSale.TypeSale;
                                //                x.TypeSaleValue = listSale.SaleValue;
                                //                x.Price = x.PriceOld - x.PriceOld * (listSale.SaleValue.HasValue ? listSale.SaleValue.Value : 0) / 100;
                                //            }
                                //            x.UrlLogoSale = !string.IsNullOrEmpty(listSale.UrlPicture) ? listSale.UrlPicture : string.Empty;
                                //            x.HtmlSale = !string.IsNullOrEmpty(listSale.ShortDescription) ? listSale.ShortDescription : string.Empty;
                                //        }
                                //    }
                                //});

                                #endregion check khuyen mai (Ẩn)

                                model.ListProductItem = listProduct;

                                #region module thương hiệu

                                //IEnumerable<int> idsTrade = await _productManager.ListIdsTradeMark(searchproduct, module.ID, string.Join(",", AllModule.Select(x => x.ID)));
                                //if (idsTrade.Any() && idsTrade.Count() > 0)
                                //{
                                //    model.ListModuleItemParents = await cacheUtils.GetByAllTradeMark(string.Join(",", idsTrade), Lang(), StaticEnum.Trademark);
                                //}
                                //else
                                //{
                                //    model.ListModuleItemParents = new List<WebsiteModulesItem>();
                                //}

                                #endregion module thương hiệu

                                //rate category
                                //IEnumerable<int> idsProduct = await _productManager.GetListIdsProduct(searchproduct, module.ID, string.Join(",", AllModule.Select(x => x.ID)));
                                //model.RateItems = idsProduct != null && idsProduct.Count() > 0 ? await _commentManager.GetAllCommentCategory(string.Join(",", idsProduct)) : new List<CommentItem>();
                                //model.ListModuleItem = cacheUtils.GetByModuleTypeCode(moduleType, Lang());
                                model.ListProductModels = _productManager.GetListProduct(new SearchModel { page = search.page, lang = Lang(), sort = 5 }, 5, moduleproduct.FirstOrDefault().ID, string.Join(",", moduleproduct.Select(x => x.ID)), "0");
                                total = listProduct.Any() ? listProduct.FirstOrDefault().TotalRecord : 0;
                                model.Total = total;
                                model.PageSize = pageSize;
                                model.Page = p ?? 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());

                                //ViewBag.GridHtml = Common.GetAjaxPage(search.page, model.Total.Value, pageSize);
                                //if(model.ListProductItem.Count() == 1)
                                //{
                                //    return RedirectPermanent(model.ListProductItem.FirstOrDefault().NameAscii);
                                //}
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                if (module.ParentID == 0)
                                    return View(@"~/Views/Product/ListGroupProduct.cshtml", model);
                                return View(@"~/Views/Product/ListGridProduct.cshtml", model);
                            }

                        #endregion Sản phẩm

                        // ẩn
                        #region Thư viện
                        case StaticEnum.Gallery:
                            {
                                List<WebsiteModulesItem> Allmodule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang())
                                };
                                WebsiteModulesItem moduleParent = GetOrigin(module);
                                var childmodule = await cacheUtils.GetListModuleChildID(moduleParent.ID, Lang());
                                model.ModuleParentItem = moduleParent;
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                search.sort = 5;
                                if (model.ModuleItem != null)
                                {
                                    int pageSize = 6;
                                    model.ModuleItem.ListContentItem = await _webContentManager.GetListContent(search, pageSize, model.ModuleItem.ID, "", "0");
                                    model.Total = model.ModuleItem.ListContentItem.Any() ? model.ModuleItem.ListContentItem.FirstOrDefault().TotalRecord : 0;
                                    model.PageSize = pageSize;
                                    model.Page = search.page > 1 ? search.page : 1;
                                    model.SearchModel = search;
                                    ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                }
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/News/ListGallery.cshtml", model);
                            }
                        #endregion Thư viện

                        #region Đối tác
                        case StaticEnum.Partner:
                            {
                                List<WebsiteModulesItem> Allmodule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang())
                                };
                                WebsiteModulesItem moduleParent = GetOrigin(module);
                                var childmodule = await cacheUtils.GetListModuleChildID(moduleParent.ID, Lang());
                                model.ListModuleChildItem = await cacheUtils.GetListModuleChildID(module.ID, Lang());
                                model.ModuleParentItem = moduleParent;
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                search.sort = 4;
                                int pageSize = 0;
                                if (module.TypeView == StaticEnum.CongTrinhUngDung)
                                {
                                    pageSize = 6;
                                }
                                model.ListContentItemAsync = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", Allmodule.Select(x => x.ID)), "0");
                                model.Total = model.ListContentItemAsync.Any() ? model.ListContentItemAsync.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                if (module.TypeView == StaticEnum.CongTrinhUngDung)
                                {
                                    ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                }
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/Content/Partner.cshtml", model);
                            }
                        #endregion Đối tác
                        #region Lĩnh vực kinh doanh
                        case StaticEnum.BusinessAreas:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                };
                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                search.sort = 4;
                                int pageSize = 6;
                                model.ListContentItemAsync = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                model.Total = model.ListContentItemAsync.Any() ? model.ListContentItemAsync.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                if (module.ParentID == 0)
                                {
                                    return View(@"~/Views/News/ListGroupBusiness.cshtml", model);
                                }
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                return View(@"~/Views/News/ListBusiness.cshtml", model);
                            }
                        #endregion Lĩnh vực kinh doanh
                        #region Liên hệ

                        case StaticEnum.Contact:
                            {
                                IEnumerable<WebsiteModulesItem> AllModule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(search.lang),
                                    ListContentItemAsync = await _webContentManager.GetListContent(search, 0, module.ID, string.Join(",", AllModule.Select(x => x.ID)), "0"),
                                };
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/Content/Contact.cshtml", model);
                            }

                        #endregion Liên hệ
                        #region Tuyển dụng
                        case StaticEnum.Recuitment:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                };

                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                model.ModuleParentItem = childmodule.Any(x => x.ParentID == module.ID) ? module : cacheUtils.GetModuleById(module.ParentID.HasValue && module.ParentID.Value > 0 ? module.ParentID.Value : module.ID);
                                int pageSize = 4;
                                search.sort = 1;
                                search.start = (search.page - 1) * pageSize;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, "0", "0");
                                model.ListContentItemAsync = listContent;
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                if (model.ListContentItemAsync.Count() == 1)
                                    return RedirectPermanent(Utility.Link(model.ListContentItemAsync.FirstOrDefault().NameAscii, string.Empty, model.ListContentItemAsync.FirstOrDefault().LinkUrl));
                                if (module.TypeView == StaticEnum.Recuiment1)
                                    return View(@"~/Views/News/ListRecuitment1.cshtml", model);
                                IEnumerable<WebsiteContentItem> listContentNoUrl = await _webContentManager.GetListContent(search, 0, model.WebsiteModulesItems.FirstOrDefault(x => x.ModuleTypeCode == StaticEnum.SimpleModule).ID, "0", "0");
                                model.ListContentItem = listContentNoUrl != null ? listContentNoUrl : null;
                                return View(@"~/Views/News/ListRecuitment.cshtml", model);
                            }
                        #endregion Tuyển dụng
                        #region Tin tức
                        case StaticEnum.News:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                    ListSystemTags = _systemTagManager.GetListSystemTag()
                                };

                                WebsiteModulesItem moduleParent = GetOrigin(module);
                                model.ModuleParentItem = moduleParent;
                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                int pageSize = 6;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                model.ListContentItemAsync = listContent;

                                List<WebsiteContentItem> listContentHot = _webContentManager.GetListContentHot(pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)));
                                model.ListContentItemHot = listContentHot;
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/News/ListNews.cshtml", model);
                            }
                        #endregion
                        #region Báo giá
                        case StaticEnum.PriceQuote:
                        case StaticEnum.Album:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                };

                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = _webModuleManager.GetByModuleTypeCode(module.ModuleTypeCode, Lang());
                                var moduleProduct = _webModuleManager.GetByModuleTypeCode(StaticEnum.Product, Lang());
                                int pageSize = 6;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                model.ListContentItemAsync = listContent;

                                model.ListProductModels = _productManager.GetListProduct(new SearchModel { page = search.page, lang = Lang(), sort = 5 }, 5, moduleProduct.FirstOrDefault().ID, string.Join(",", moduleProduct.Select(x => x.ID)), "0");
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                if (listContent.Count() == 1)
                                    return RedirectPermanent(Utility.Link(listContent.FirstOrDefault().NameAscii, string.Empty, listContent.FirstOrDefault().LinkUrl));
                                if (module.ParentID == 0)
                                {
                                    if (model.WebsiteModulesItems.Count() == 1)
                                        return View(@"~/Views/News/ListPriceQuote.cshtml", model);
                                    return View(@"~/Views/News/ListGroupPriceQuote.cshtml", model);
                                }
                                return View(@"~/Views/News/ListPriceQuote.cshtml", model);
                            }
                        #endregion
                        #region Dự án
                        case StaticEnum.Project:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                };

                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                int pageSize = 5;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                model.ListContentItemAsync = listContent;

                                List<WebsiteContentItem> listContentHot = _webContentManager.GetListContentHighlights(10, module.ID, string.Join(",", childmodule.Select(x => x.ID)));
                                model.ListContentItemHot = listContentHot;
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/News/ListProject.cshtml", model);
                            }
                        #endregion
                        #region Hoạt động công ty

                        case StaticEnum.CompanyActivities:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                };
                                var ModuleTypes = cacheUtils.GetByModuleTypeCode(StaticEnum.CompanyActivities, Lang());
                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                model.ModuleParentItem = childmodule.Any(x => x.ParentID == module.ID) ? module : cacheUtils.GetModuleById(module.ParentID.HasValue && module.ParentID.Value > 0 ? module.ParentID.Value : module.ID);
                                int pageSize = 6;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/News/CompanyActivitiesList.cshtml", model);
                            }

                        #endregion Hoạt động công ty
                        #region Nội dung đơn

                        case StaticEnum.SimpleModule:
                            {
                                List<WebsiteModulesItem> Allmodule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                };
                                //Bài viết mới nhất
                                //var ModuleTypes = cacheUtils.GetByModuleTypeCode(StaticEnum.News, Lang());
                                //model.ListContentItem = await _webContentManager.GetListContent(new SearchModel { page = 1, sort = 0, lang = Lang() }, 6, ModuleTypes.FirstOrDefault().ID, string.Join(",", ModuleTypes.Select(x => x.ID)), "0");

                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/Content/SimpleModule.cshtml", model);
                            }

                        #endregion Nội dung đơn
                        #region Catalog

                        case StaticEnum.Catalogue:
                            {
                                List<WebsiteModulesItem> Allmodule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                };
                                int pageSize = 6;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", Allmodule.Select(x => x.ID)), "0");
                                model.ListContentItemAsync = listContent;
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/Content/Catalogue.cshtml", model);
                            }

                        #endregion Nội dung đơn
                        #region Chức năng đang cập nhật

                        case StaticEnum.Updating:
                            {
                                return View(@"~/Views/Content/Updating.cshtml", new ModuleViewModels
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                });
                            }

                        #endregion Chức năng đang cập nhật
                        #region Câu hỏi thường gặp

                        case StaticEnum.QA:
                            {
                                ModuleViewModels model = new()
                                {
                                    ModuleItem = module,
                                    SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                                    ModuleParentItem = new WebsiteModulesItem(),
                                };
                                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                model.WebsiteModulesItems = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : childmodule.Any(x => x.ParentID == module.ID) ? childmodule.Where(x => x.ParentID == module.ID)?.ToList() : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                model.ModuleParentItem = childmodule.Any(x => x.ParentID == module.ID) ? module : cacheUtils.GetModuleById(module.ParentID.HasValue && module.ParentID.Value > 0 ? module.ParentID.Value : module.ID);
                                int pageSize = 10;
                                search.sort = 4;
                                search.start = (search.page - 1) * pageSize;
                                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                model.ListContentItemAsync = listContent;
                                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                                model.PageSize = pageSize;
                                model.Page = search.page > 1 ? search.page : 1;
                                model.SearchModel = search;
                                ViewBag.GridHtml = Common.GetHtmlPageLink(search.page, model.Total.Value, pageSize, Utility.GetUrl(module.NameAscii), Lang());
                                await _webModuleManager.UpdateTotalViews(module.ID);
                                return View(@"~/Views/News/ListQA.cshtml", model);
                            }

                        #endregion Câu hỏi thường gặp
                        #region ẩn

                        #region Cart (Ẩn)

                        case StaticEnum.Cart:
                            {
                                ViewBag.Back = GetWebBack();
                                string cartCookie = Request.Cookies["shopping_cart"];
                                CartViewModel objCartViewModel = new()
                                {
                                    ListCartItem = _cartManager.GetCartData(cartCookie, ""),
                                    TotalPriceCart = _cartManager.GetSumPrice(cartCookie),
                                    TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
                                    TotalPriceCartAfterVoucher = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
                                    DisCountModule = _cartManager.GetDiscountMoudle(cartCookie),
                                    DisCountCombo = _cartManager.GetDiscountCombo(cartCookie),
                                    ModuleItem = module,
                                    CityItems = await _cartManager.GetAllCity("city.json"),
                                    CustomerItem = _customerManager.GetId(UserId.Value) ?? new CustomerItem()
                                };
                                return View(@"~/Views/Cart/Cart.cshtml", objCartViewModel);
                            }
                        case StaticEnum.Payment:
                            {
                                string UrlBackCookies = Request.Cookies["CartUrlBack"];
                                string cartCookie = Request.Cookies["shopping_cart"];
                                //area
                                //string vouchercode = Request.Query["vouchercode"];
                                CartViewModel objCartViewModel = new()
                                {
                                    ListCartItem = _cartManager.GetCartData(cartCookie, ""),
                                    TotalPriceCart = _cartManager.GetSumPrice(cartCookie),
                                    TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
                                    TotalPriceCartAfterVoucher = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
                                    DisCountModule = _cartManager.GetDiscountMoudle(cartCookie),
                                    DisCountCombo = _cartManager.GetDiscountCombo(cartCookie),
                                    IsApplyOtherCampaign = _cartManager.GetIsApplyOtherCampaign(cartCookie),
                                    DisCountVoucher = _cartManager.GetDiscountVoucher(cartCookie),
                                    //VoucherCode = vouchercode,
                                    UrlBack = UrlBackCookies != null && UrlBackCookies != "" ? System.Net.WebUtility.UrlDecode(UrlBackCookies) : string.Empty,
                                    //TotalVAT = _cartManager.GetTotalVAT(cartCookie),
                                    CityItems = await _cartManager.GetAllCity("city.json"),
                                    AreaAgencyItems = await _agencyManager.GetByParentArrId(",0,"),
                                    ModuleItem = module,
                                    CustomerItem = _customerManager.GetId(UserId.Value) ?? new CustomerItem(),
                                    OtherContentItems = await _otherContentManager.GetListByCodeLang(",NoteCod,CODPaymentInfor,NoteAtmBanking,NoteVisaMaster,NoteCK,CKPaymentInfor,NoteATM,", Lang())
                                };
                                return View(@"~/Views/Cart/CartPayment.cshtml", objCartViewModel);
                            }

                            #endregion Cart (Ẩn)

                            //#region Member (Ẩn)
                            //case StaticEnum.LogIn:
                            //    {
                            //        if (UserId != 0)
                            //        {
                            //            return Redirect("/");
                            //        }
                            //        return View(@"~/Views/Member/LogIn.cshtml", new ModuleViewModels
                            //        {
                            //            SystemConfig = cacheUtils.SystemConfigItem(search.lang),
                            //            ModuleItem = module
                            //        });
                            //    }
                            //case StaticEnum.Register:
                            //    {
                            //        if (UserId != 0)
                            //        {
                            //            return Redirect("/");
                            //        }
                            //        return View(@"~/Views/Member/Register.cshtml", new ModuleViewModels
                            //        {
                            //            SystemConfig = cacheUtils.SystemConfigItem(search.lang),
                            //            ModuleItem = module
                            //        });
                            //    }
                            //#endregion

                            #endregion ẩn
                    }
                }
                else
                {
                    WebsiteContentItem content = new();
                    SessionBase session = new(HttpContext);
                    _ = !string.IsNullOrEmpty(session.GetAdminUserId()) ? content = _webContentManager.GetByNameAscii(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii) : content = cacheUtils.GetContentByNameAscii(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii);
                    if (content != null && !string.IsNullOrEmpty(nameAscii) && !string.IsNullOrEmpty(nameAsciiC))
                    {
                        if (!string.IsNullOrEmpty(content.ModuleNameAscii))
                        {
                            module = await cacheUtils.GetModuleByNameAscii(content.ModuleNameAscii);
                        }
                        else
                        {
                            List<WebsiteModulesItem> modules = _webModuleManager.GetListByArrId(content.ModuleIds);
                            module = modules.Any() ? modules.FirstOrDefault() : new WebsiteModulesItem();
                        }
                        module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                        moduleType = module != null ? module.ModuleTypeCode : StaticEnum.News;
                        SearchModel search = new()
                        {
                            page = 0,
                            lang = Lang(),
                            sort = 1
                        };
                        switch (moduleType)
                        {
                            #region Tin tức chi tiết
                            case StaticEnum.News:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    var moduleNews = _webModuleManager.GetByTypeCode(StaticEnum.News, Lang());
                                    var curtaincollection = cacheUtils.GetListModuleInPositionCode(StaticEnum.CurtainCollection, Lang());
                                    ContentViewModels model = new()
                                    {
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()),//Bài viết khác
                                        ListContentItemNew = await _webContentManager.GetListContent(search, 5, moduleNews != null ? moduleNews.ID : 0, "", content.ID.ToString()), //Bài viết mới nhất 
                                        ListSystemTags = _systemTagManager.GetListSystemTag(),
                                        ListCurtainCollection = curtaincollection != null ? curtaincollection : new List<WebsiteModulesJson>()
                                    };

                                    WebsiteModulesItem moduleParent = GetOrigin(module);
                                    model.ListModuleItems = await cacheUtils.GetListModuleChidrentAsync(moduleParent.ID);

                                    IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);

                                    IEnumerable<WebsiteContentItem> listContentVew = await _webContentManager.GetListContentViewed(3, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                    model.ListContentItemView = listContentVew;
                                    await _webContentManager.UpdateTotalViews(content.ID);
                                    return View(@"~/Views/Detail/DetailNews.cshtml", model);
                                }

                            #endregion Tin tức chi tiết
                            #region Báo giá chi tiết
                            case StaticEnum.PriceQuote:
                            case StaticEnum.Album:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    var moduleNews = _webModuleManager.GetByTypeCode(StaticEnum.News, Lang());
                                    ContentViewModels model = new()
                                    {
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()),//Bài viết khác
                                        ListContentItemNew = await _webContentManager.GetListContent(search, 5, moduleNews != null ? moduleNews.ID : 0, "", content.ID.ToString()), //Bài viết mới nhất 
                                        ListSystemTags = _systemTagManager.GetListSystemTag(),
                                    };
                                    model.WebsiteModulesItems = _webModuleManager.GetByModuleTypeCode(module.ModuleTypeCode, Lang());
                                    var moduleProduct = _webModuleManager.GetByModuleTypeCode(StaticEnum.Product, Lang());
                                    model.ListProductModels = _productManager.GetListProduct(new SearchModel { page = search.page, lang = Lang(), sort = 5 }, 5, moduleProduct.FirstOrDefault().ID, string.Join(",", moduleProduct.Select(x => x.ID)), "0");
                                    await _webContentManager.UpdateTotalViews(content.ID);
                                    return View(@"~/Views/Detail/DetailPriceQuote.cshtml", model);
                                }

                            #endregion Báo giá chi tiết
                            #region thư viện chi tiết
                            case StaticEnum.Gallery:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    ContentViewModels model = new()
                                    {
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()), //Bài viết khác
                                        ListSystemTags = _systemTagManager.GetListSystemTag()
                                    };
                                    WebsiteModulesItem moduleParent = GetOrigin(module);
                                    model.ListModuleItems = await cacheUtils.GetListModuleChidrentAsync(moduleParent.ID);

                                    IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);

                                    IEnumerable<WebsiteContentItem> listContentVew = await _webContentManager.GetListContentViewed(3, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                                    model.ListContentItemView = listContentVew;
                                    await _webContentManager.UpdateTotalViews(content.ID);
                                    return View(@"~/Views/Detail/DetailGallery.cshtml", model);
                                }

                            #endregion thư viện chi tiết
                            #region Dự án chi tiết
                            case StaticEnum.Project:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    ContentViewModels model = new()
                                    {
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()), //Bài viết khác
                                    };
                                    await _webContentManager.UpdateTotalViews(content.ID);
                                    return View(@"~/Views/Detail/DetailProject.cshtml", model);
                                }

                            #endregion Tin tức chi tiết
                            #region Lĩnh vực hoạt động

                            case StaticEnum.BusinessAreas:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    ContentViewModels model = new()
                                    {
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 5, module != null ? module.ID : 0, "", content.ID.ToString()), //Bài viết khác
                                    };
                                    await _webContentManager.UpdateTotalViews(content.ID);
                                    return View(@"~/Views/Detail/DetailBusinessAreas.cshtml", model);
                                }

                            #endregion Lĩnh vực hoạt động

                            // ẩn
                            #region Tuyển dụng chi tiết
                            case StaticEnum.Recuitment:
                                {
                                    content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                    search.sort = 1;
                                    ContentViewModels model = new()
                                    {
                                        ContentItem = content,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        //Areas = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("Area.json", "DataJson")),
                                        ListContentItem = await _webContentManager.GetListContent(search, 5, module != null ? module.ID : 0, "", content.ID.ToString())
                                    };
                                    if (module.TypeView == StaticEnum.Recuiment1)
                                        return View(@"~/Views/Detail/DetailRecuitment1.cshtml", model);
                                    return View(@"~/Views/Detail/DetailRecuitment.cshtml", model);
                                }

                                #endregion Tuyển dụng chi tiết

                        }
                    }
                    else
                    {
                        ProductDetail product = _productManager.GetByNameAscii(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii);
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(nameAscii) && !string.IsNullOrEmpty(nameAsciiC))
                            {
                                return RedirectPermanent("/" + nameAsciiC);
                            }
                            module = await cacheUtils.GetModuleByNameAscii(product.ModuleNameAscii);
                            product.AlbumGalleryItems = !string.IsNullOrEmpty(product.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(product.AlbumPictureJson) : new List<AlbumGalleryItem>();
                            product.ColorTableItems = !string.IsNullOrEmpty(product.ColorTable) ? JsonConvert.DeserializeObject<List<ColorTableItem>>(product.ColorTable) : new List<ColorTableItem>();
                            moduleType = module != null ? module.ModuleTypeCode : StaticEnum.Product;
                            if (module != null)
                            {
                                module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                            }
                            SearchModel search = new() { page = 0, lang = Lang(), sort = 5 };
                            switch (moduleType)
                            {
                                #region Products
                                case StaticEnum.Product:
                                    {
                                        SearchModel searchModel = new() { productId = product.ID, page = 1 };
                                        string contentIds = product.ID.ToString();
                                        //searchModel.customerId = GetWebUserID();
                                        search.contentId = product.ID;
                                        search.moduleId = module != null ? module.ID : 0;
                                        search.pagesize = 5;
                                        searchModel.customerId = GetWebUserID();
                                        var commentID = Request.Query["comment"];

                                        #region Comment

                                        //int size = 5;
                                        //IEnumerable<CommentItem> comments = await _commentManager.GetListCommentByPage(searchModel, size);
                                        //if (!string.IsNullOrEmpty(commentID) && !comments.Any(x => x.ID == commentID))
                                        //{
                                        //    comments = await GetCommentPreview(searchModel, size, Convert.ToInt32(commentID));
                                        //}
                                        //await comments.ToAsyncEnumerable().ForEachAsync(async x =>
                                        //{
                                        //    x.Replies = await _commentManager.GetListReplyByPage(new SearchModel { productId = product.ID, parentId = x.ID, page = 1, pagesize = 1 }, 5);
                                        //    x.GridHtml = Common.GetAjaxPage(1, x.TotalReply.Value, size);
                                        //});
                                        //int Total = comments.Any() ? comments.FirstOrDefault().TotalRecord : 0;
                                        //ViewBag.GridPageHtml = Common.GetAjaxPage(searchModel.page, Total, size);

                                        #endregion Comment

                                        #region rate

                                        searchModel.page = 1;
                                        //IEnumerable<CommentItem> rate = await _commentManager.GetListRateByPage(searchModel, size);
                                        //if (!string.IsNullOrEmpty(commentID) && !rate.Any(x => x.ID == commentID))
                                        //{
                                        //    rate = await GetRatePreview(searchModel, size, Convert.ToInt32(commentID));
                                        //}
                                        //await rate.ToAsyncEnumerable().ForEachAsync(async x =>
                                        //{
                                        //    x.Replies = await _commentManager.GetListReplyRateByPage(new SearchModel { productId = product.ID, parentId = x.ID, page = 1, pagesize = 1 }, 5);
                                        //    x.GridHtml = Common.GetAjaxPage(1, x.TotalReply.Value, size);
                                        //});
                                        //int Totalrate = rate.Any(x => x.IsApproved == true) ? rate.Count(x => x.IsApproved == true) : 0;
                                        ////double TotalStar = 0;
                                        //ViewBag.GridRatePageHtml = Common.GetAjaxPage(searchModel.page, Totalrate, size);
                                        //product.Rate1 = rate.Any(x => x.Rate == 1) ? rate.Count(x => x.Rate == 1) : 0;
                                        //product.Rate2 = rate.Any(x => x.Rate == 2) ? rate.Count(x => x.Rate == 2) : 0;
                                        //product.Rate3 = rate.Any(x => x.Rate == 3) ? rate.Count(x => x.Rate == 3) : 0;
                                        //product.Rate4 = rate.Any(x => x.Rate == 4) ? rate.Count(x => x.Rate == 4) : 0;
                                        //product.Rate5 = rate.Any(x => x.Rate == 5) ? rate.Count(x => x.Rate == 5) : 0;
                                        //product.TotalRate = Totalrate;

                                        #endregion rate

                                        #region Attribute

                                        //attr
                                        //IEnumerable<AttributeItem> attr = new List<AttributeItem>();
                                        //List<AttributeItem> attrCal = new();
                                        //List<Attribute_WebsiteContentItem> attrCalPrice = new();
                                        //if (!string.IsNullOrEmpty(product.AttributeProductIds))
                                        //{
                                        //    attr = await _productManager.GetAttributeByListIds(product.AttributeProductIds);
                                        //    foreach (AttributeItem item in attr)
                                        //    {
                                        //        string temp = string.Empty;
                                        //        if (module != null)
                                        //        {
                                        //            temp = "," + module.AttributeModuleIdsCal + ",";
                                        //        }
                                        //        if (temp.Contains("," + item.ID + ","))
                                        //        {
                                        //            attrCal.Add(item);
                                        //        }
                                        //        if (item.ParentID != null && temp.Contains("," + item.ParentID.Value + ","))
                                        //        {
                                        //            attrCal.Add(item);
                                        //        }
                                        //    }
                                        //    if (attrCal.Any())
                                        //    {
                                        //        attrCalPrice = _productManager.GetAttributeWebsiteContentItemByListAttrIdsAndProductId(string.Join(",", attrCal.Select(c => c.ID.ToString())), product.ID);
                                        //    }
                                        //}

                                        #endregion Attribute

                                        #region Group Product

                                        //IEnumerable<ProductItem> related = !string.IsNullOrEmpty(product.ProductGroupCode) ? await _productManager.GetListByListProductGroupCode(product.ProductGroupCode) : new List<ProductItem>();
                                        //await related.ToAsyncEnumerable().ForEachAsync(x =>
                                        //{
                                        //    if ((!x.TypeSaleValue.HasValue || x.TypeSaleValue == 0) && (!x.DiscountAmount.HasValue || x.DiscountAmount == 0))
                                        //    {
                                        //        var ListIds = ListHelper.GetValuesArray(x.ModuleIds);
                                        //        if (allpromotion.Any(m => ("," + m.ProductIds + ",").Contains("," + x.ID + ",") || (m.ForAll == true && ListHelper.GetValuesArray(m.ModuleIds).Intersect(ListIds).Count() > 0)))
                                        //        {
                                        //            var listSale = allpromotion.Where(n => ("," + n.ProductIds + ",").Contains("," + x.ID + ",") || (n.ForAll == true && ListHelper.GetValuesArray(n.ModuleIds).Intersect(ListIds).Count() > 0)).OrderBy(n => n.OrderDisplay).FirstOrDefault();
                                        //            if (listSale.TypeSale == 2 && listSale.DiscountAmount > 0)
                                        //            {
                                        //                x.TypeSale = listSale.TypeSale;
                                        //                x.TypeSaleValue = 0;
                                        //                x.Price = x.PriceOld - listSale.DiscountAmount;
                                        //            }
                                        //            else if (listSale.TypeSale == 1 && listSale.SaleValue > 0)
                                        //            {
                                        //                x.TypeSale = listSale.TypeSale;
                                        //                x.TypeSaleValue = listSale.SaleValue;
                                        //                x.Price = x.PriceOld - x.PriceOld * (listSale.SaleValue.HasValue ? listSale.SaleValue.Value : 0) / 100;
                                        //            }
                                        //            x.UrlLogoSale = !string.IsNullOrEmpty(listSale.UrlPicture) ? listSale.UrlPicture : string.Empty;
                                        //            x.HtmlSale = !string.IsNullOrEmpty(listSale.ShortDescription) ? listSale.ShortDescription : string.Empty;
                                        //        }
                                        //    }
                                        //});

                                        #endregion Group Product

                                        #region Attach Product

                                        //IEnumerable<ProductItem> attachProduct = !string.IsNullOrEmpty(product.AttachedProductIds) ? await _productManager.GetListByArrId(product.AttachedProductIds) : new List<ProductItem>();

                                        #endregion Attach Product

                                        #region Gift Product

                                        //IEnumerable<ProductItem> giftProduct = !string.IsNullOrEmpty(product.GiftIds) ? await _productManager.GetListByArrId(product.GiftIds) : new List<ProductItem>();

                                        #endregion Gift Product

                                        #region Replace Product

                                        //IEnumerable<ProductItem> replaces = new List<ProductItem>();
                                        //if (product.Status == 2)
                                        //{
                                        //    replaces = !string.IsNullOrEmpty(product.ReplaceIds) ? await _productManager.GetListByArrId(product.ReplaceIds) : new List<ProductItem>();
                                        //    await replaces.ToAsyncEnumerable().ForEachAsync(x =>
                                        //    {
                                        //        if ((!x.TypeSaleValue.HasValue || x.TypeSaleValue == 0) && (!x.DiscountAmount.HasValue || x.DiscountAmount == 0))
                                        //        {
                                        //            var ListIds = ListHelper.GetValuesArray(x.ModuleIds);
                                        //            if (allpromotion.Any(m => ("," + m.ProductIds + ",").Contains("," + x.ID + ",") || (m.ForAll == true && ListHelper.GetValuesArray(m.ModuleIds).Intersect(ListIds).Count() > 0)))
                                        //            {
                                        //                var listSale = allpromotion.Where(n => ("," + n.ProductIds + ",").Contains("," + x.ID + ",") || (n.ForAll == true && ListHelper.GetValuesArray(n.ModuleIds).Intersect(ListIds).Count() > 0)).OrderBy(n => n.OrderDisplay).FirstOrDefault();
                                        //                if (listSale.TypeSale == 2 && listSale.DiscountAmount > 0)
                                        //                {
                                        //                    x.TypeSale = listSale.TypeSale;
                                        //                    x.TypeSaleValue = 0;
                                        //                    x.Price = x.PriceOld - listSale.DiscountAmount;
                                        //                }
                                        //                else if (listSale.TypeSale == 1 && listSale.SaleValue > 0)
                                        //                {
                                        //                    x.TypeSale = listSale.TypeSale;
                                        //                    x.TypeSaleValue = listSale.SaleValue;
                                        //                    x.Price = x.PriceOld - x.PriceOld * (listSale.SaleValue.HasValue ? listSale.SaleValue.Value : 0) / 100;
                                        //                }
                                        //                x.HtmlSale = !string.IsNullOrEmpty(listSale.ShortDescription) ? listSale.ShortDescription : string.Empty;
                                        //            }
                                        //        }
                                        //    });
                                        //}

                                        #endregion Replace Product

                                        IEnumerable<WebsiteModulesItem> listModule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                        product.AlbumGalleryItems = !string.IsNullOrEmpty(product.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(product.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                        var ModuleTypesNews = cacheUtils.GetByModuleTypeCode(StaticEnum.News, Lang());
                                        var ModuleProduct = cacheUtils.GetByModuleTypeCode(StaticEnum.Product, Lang());
                                        var related = !string.IsNullOrEmpty(product.ProductGroupCode) ? _productManager.GetListByListProductGroupCode(product.ProductGroupCode) : new List<ProductItem>();
                                        ContentViewModels model = new()
                                        {
                                            Schema = await _otherContentManager.GetByCodeLang("SchemaDetailProduct", Lang()),
                                            BreadcrumbList = await cacheUtils.GetListBreadcrumb(module.ID, Lang()),
                                            CustomerItem = _customerManager.GetId(GetWebUserID()),
                                            ProductItem = product,
                                            ModuleItem = module ?? new WebsiteModulesItem(),
                                            ListProductModels = related,
                                            SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                            ListProductItem = await _productManager.GetListProductJson(search, 5, module.ID, string.Join(",", listModule.Select(x => x.ID)), product.ID.ToString()),
                                            ListProductItemNew = await _productManager.GetListProductJson(search, 5, module.ID, string.Join(",", ModuleProduct.Where(x => x.TypeView == StaticEnum.TypeProductI).Select(x => x.ID)), product.ID.ToString()),
                                            WebsiteModulesItem = _webModuleManager.GetByTypeCode(StaticEnum.Contact, Lang()),
                                            RelatedContent = _webContentManager.GetListByArrId(product.RelatedIds),
                                            ListProductAttach = await _productManager.GetListByArrId(product.AttachedProductIds),
                                            RelatedDocumentContent = _webContentManager.GetListByArrId(product.DocumentIds),
                                            ListFileDownload = !string.IsNullOrEmpty(product.LinkDownload) ? JsonConvert.DeserializeObject<List<FileDownloadAdmin>>(product.LinkDownload) : new List<FileDownloadAdmin>(),
                                            //CommentItems = comments.Where(x => x.IsApproved == true),
                                            //TotalComment = comments.Count() > 0 ? comments.FirstOrDefault().TotalRecord : 0,
                                            //RateItems = rate.Where(x => x.IsApproved == true),
                                            ListSubItems = _subItemManager.GetAll(Lang(), product.ID),
                                            ListModulesItem = ModuleProduct.Where(x => x.ID != module.ID && x.ParentID != 0)
                                        };

                                        //if (model.ListProductSub != null)
                                        //{
                                        //    model.ListProductSub.ForEach(x =>
                                        //    {
                                        //        if (!string.IsNullOrEmpty(x.AttributeProductIds))
                                        //        {
                                        //            search.lang = Lang();
                                        //            string rs = Regex.Replace(x.AttributeProductIds, "^,", "");
                                        //            rs = Regex.Replace(rs, ",$", "");
                                        //            string[] arr = rs.Split(",");
                                        //            List<string> lstattr = new List<string>(arr);
                                        //            search.ListAttr = lstattr;
                                        //            x.ListColorItems = _productManager.GetListColorProductSub(search);
                                        //        }
                                        //        x.ListSubItems = _subItemManager.GetAll(Lang(), x.ID);
                                        //    });
                                        //}

                                        var listAds = cacheUtils.GetListAdvertisingItemByCode("ADSProductDetail", Lang());
                                        model.ListAds = listAds;

                                        model.ListModuleManufacturers = _webModuleManager.GetByModuleTypeCode(StaticEnum.Trademark, Lang());

                                        var listProductCheck = await _productManager.GetListProductJson(search, 10, module.ID, string.Join(",", listModule.Select(x => x.ID)), product.ID.ToString());

                                        var listProductHot = _productManager.GetListProductBestSelling(search, module.ID, string.Join(",", listModule.Select(x => x.ID)), 8);
                                        model.ListProductItemHot = listProductHot;
                                        //danh sách comment
                                        //model.ProductItem.AverageRate = AverageRates / Convert.ToDouble(listComment.Count());

                                        //model.ListContentItem2 = _webContentManager.GetListContentHighlights(3, ContactType.FirstOrDefault().ID, string.Join(",", ContactType.Select(x => x.ID)));

                                        #region SimilarProducts

                                        //search.price = product.Price.HasValue ? product.Price.Value : 0;
                                        //IEnumerable<WebsiteProductItemJson> listProductSale = await _productManager.GetRandomProductMore(search, product.ID, module != null ? module.ID.ToString() : "0");
                                        //await listProductSale.ToAsyncEnumerable().ForEachAsync(x =>
                                        //{
                                        //    if ((!x.TypeSaleValue.HasValue || x.TypeSaleValue == 0) && (!x.DiscountAmount.HasValue || x.DiscountAmount == 0))
                                        //    {
                                        //        List<int> ListIds = ListHelper.GetValuesArray(x.ModuleIds);
                                        //        if (allpromotion.Any(m => ("," + m.ProductIds + ",").Contains("," + x.ID + ",") || (m.ForAll == true && ListHelper.GetValuesArray(m.ModuleIds).Intersect(ListIds).Count() > 0)))
                                        //        {
                                        //            var listSale = allpromotion.Where(n => ("," + n.ProductIds + ",").Contains("," + x.ID + ",") || (n.ForAll == true && ListHelper.GetValuesArray(n.ModuleIds).Intersect(ListIds).Count() > 0)).OrderBy(n => n.OrderDisplay).FirstOrDefault();
                                        //            if (listSale.TypeSale == 2 && listSale.DiscountAmount > 0)
                                        //            {
                                        //                x.TypeSale = listSale.TypeSale;
                                        //                x.TypeSaleValue = 0;
                                        //                x.Price = x.PriceOld - listSale.DiscountAmount;
                                        //            }
                                        //            else if (listSale.TypeSale == 1 && listSale.SaleValue > 0)
                                        //            {
                                        //                x.TypeSale = listSale.TypeSale;
                                        //                x.TypeSaleValue = listSale.SaleValue;
                                        //                x.Price = x.PriceOld - x.PriceOld * (listSale.SaleValue.HasValue ? listSale.SaleValue.Value : 0) / 100;
                                        //            }
                                        //            x.UrlLogoSale = !string.IsNullOrEmpty(listSale.UrlPicture) ? listSale.UrlPicture : string.Empty;
                                        //            x.HtmlSale = !string.IsNullOrEmpty(listSale.ShortDescription) ? listSale.ShortDescription : string.Empty;
                                        //        }
                                        //    }
                                        //});
                                        //model.ListProductItem = listProductSale;

                                        #endregion SimilarProducts

                                        // Câu hỏi thường gặp
                                        var ModuleQa = cacheUtils.GetByModuleTypeCode(StaticEnum.QA, Lang());
                                        if (!string.IsNullOrEmpty(product.TagValue))
                                        {
                                            model.ListQA = _webContentManager.GetSearchQA(product.TagValue, Lang(), string.Join(",", ModuleQa.Select(x => x.ID)));
                                        }
                                        else
                                        {
                                            model.ListQA = new List<WebsiteContentItem>();
                                        }
                                        await _productManager.UpdateTotalViews(product.ID);
                                        if (model.ModuleItem.TypeView == StaticEnum.TypeProductII)
                                            return View(@"~/Views/Product/DetailProductHotel.cshtml", model);
                                        return View(@"~/Views/Product/DetailProduct.cshtml", model);
                                    }

                                    #endregion Products
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            //return Redirect("/error");
            Response.StatusCode = 404;
            return View(@"~/Views/Error/Error404.cshtml");
        }

        #region Quickview

        public async Task<IActionResult> Quickview(string nameAscii, string nameAsciiC)
        {
            if (string.IsNullOrEmpty(GetAdminUserId()))
            {
                Response.StatusCode = 404;
                return View(@"~/Views/Error/Error404.cshtml");
            }
            if (nameAscii != null && nameAscii.Contains(".htm"))
            {
                return RedirectPermanent("/" + nameAscii.Replace(".htm", "").ToLower());
            }
            WebsiteModulesItem module = new();
            module = nameAscii == "all" && !string.IsNullOrEmpty(nameAsciiC)
                ? await cacheUtils.GetModuleByNameAscii(nameAsciiC)
                : await cacheUtils.GetModuleByNameAscii(nameAscii);
            string moduleType = string.Empty;
            try
            {
                ViewBag.NoIndex = true;
                WebsiteContentItem content = new();
                SessionBase session = new(HttpContext);
                _ = !string.IsNullOrEmpty(session.GetAdminUserId()) ? content = _webContentManager.GetByNameAsciiPending(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii) : content = _webContentManager.GetByNameAsciiPending(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii);
                if (content != null)
                {
                    if (!string.IsNullOrEmpty(content.ModuleNameAscii))
                    {
                        module = await cacheUtils.GetModuleByNameAscii(content.ModuleNameAscii);
                    }
                    else
                    {
                        List<WebsiteModulesItem> modules = _webModuleManager.GetListByArrId(content.ModuleIds);
                        module = modules.Any() ? modules.FirstOrDefault() : new WebsiteModulesItem();
                    }
                    module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                    moduleType = module != null ? module.ModuleTypeCode : StaticEnum.News;
                    SearchModel search = new()
                    {
                        page = 0,
                        lang = Lang(),
                        sort = 0
                    };
                    switch (moduleType)
                    {
                        #region Tin tức

                        case StaticEnum.News:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();

                                search.sort = 1;
                                ContentViewModels model = new()
                                {
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()),
                                    ListModulesItem = await _webModuleManager.GetAllModuleByCode(moduleType, Lang())
                                };
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailNews.cshtml", model);
                            }

                        #endregion Tin tức

                        #region Dự án

                        case StaticEnum.Project:
                            {
                                search.sort = 4;
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                SearchModel searchModel = new() { contentId = content.ID, page = 1, sort = 0 };
                                ContentViewModels model = new()
                                {
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()),
                                    ListContentHightlightsItem = _webContentManager.GetListContentHighlights(3, module != null ? module.ID : 0, ""),
                                    //WebsiteModulesItem = _webModuleManager.GetByTypeCode(StaticEnum.Contact, Lang())
                                    ListProductAttach = await _productManager.GetListByArrId(content.RelatedIds),
                                    ListModuleProducts = _webModuleManager.GetByModuleTypeCode(StaticEnum.Product, Lang())
                                };
                                //model.WebsiteModulesItems = await cacheUtils.GetListModuleChildID(module.ID, Lang());
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailProject.cshtml", model);
                            }

                        #endregion Dự án

                        #region Video

                        case StaticEnum.Video:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                search.sort = 1;
                                ContentViewModels model = new()
                                {
                                    Schema = await _otherContentManager.GetByCodeLang("Schema", Lang()),
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItem = await _webContentManager.GetListContent(search, 6, module != null ? module.ID : 0, "", content.ID.ToString()),
                                    ListModulesItem = await _webModuleManager.GetAllModuleByCode(moduleType, Lang())
                                    //ListModulesItem = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang())
                                };
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailVideo.cshtml", model);
                            }

                        #endregion Video

                        #region Chi tiết dịch vụ

                        case StaticEnum.Services:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                SearchModel searchModel = new() { contentId = content.ID, page = 1, sort = 0 };
                                IEnumerable<WebsiteModulesItem> listModule = await _webModuleManager.GetAllModuleByCode(StaticEnum.News, Lang());
                                search.sort = 4;
                                ContentViewModels model = new()
                                {
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItem = await _webContentManager.GetListContent(search, 5, module != null ? module.ID : 0, "", content.ID.ToString()),
                                };
                                IEnumerable<WebsiteModulesItem> AllModule = cacheUtils.GetListModuleChidrentNotAsync(module.ID);
                                model.ListModulesItem = AllModule.Count() <= 1 && module.ParentID > 0 ? await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang()) : await cacheUtils.GetListModuleChildID(module.ID, Lang());
                                model.WebsiteModulesItem = cacheUtils.GetModuleById(model.ListModulesItem.Any(x => x.ParentID == module.ID) ? module.ID : (module.ParentID.HasValue && module.ParentID > 0 ? module.ParentID.Value : module.ID));
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailServices.cshtml", model);
                            }

                        #endregion Chi tiết dịch vụ

                        #region Tuyển dụng

                        case StaticEnum.Recuitment:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                SearchModel searchModel = new() { contentId = content.ID, page = 1, sort = 0 };
                                IEnumerable<WebsiteModulesItem> listModule = await _webModuleManager.GetAllModuleByCode(StaticEnum.News, Lang());
                                search.sort = 1;
                                ContentViewModels model = new()
                                {
                                    Schema = await _otherContentManager.GetByCodeLang("Schema", Lang()),
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItemView = await _webContentManager.GetListContent(search, 5, listModule != null ? listModule.FirstOrDefault().ID : 0, string.Join(",", listModule.Select(x => x.ID)), content.ID.ToString()),
                                };

                                model.ListModulesItem = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailRecuitment.cshtml", model);
                            }

                        #endregion Tuyển dụng

                        #region Document

                        case StaticEnum.Document:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                SearchModel searchModel = new() { contentId = content.ID, page = 1, sort = 0 };
                                search.sort = 1;
                                ContentViewModels model = new()
                                {
                                    Schema = await _otherContentManager.GetByCodeLang("Schema", Lang()),
                                    SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                    ContentItem = content,
                                    ModuleItem = module ?? new WebsiteModulesItem(),
                                    ListContentItem = await _webContentManager.GetListContent(search, 5, module != null ? module.ID : 0, "", content.ID.ToString())
                                };
                                model.ListModulesItem = module.ParentID == 0 ? await cacheUtils.GetListModuleChildID(module.ID, Lang()) : await cacheUtils.GetListModuleChildID(module.ParentID.Value, Lang());
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailDocument.cshtml", model);
                            }

                        #endregion Document

                        #region Gallery

                        case StaticEnum.Gallery:
                            {
                                content.AlbumGalleryItems = !string.IsNullOrEmpty(content.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(content.AlbumPictureJson) : new List<AlbumGalleryItem>();
                                ContentViewModels model = new()
                                {
                                    ContentItem = content,
                                    ModuleItem = module,
                                    ListContentItem = await _webContentManager.GetListContent(search, 6, module.ID, "", content.ID.ToString())
                                    //ListModulesItem = await cacheUtils.GetListModuleChildIDSimple(module.ParentID == 0 ? module.ID : module.ParentID.Value, Lang()),
                                };
                                await _webContentManager.UpdateTotalViews(content.ID);
                                return View(@"~/Views/Detail/DetailGallery.cshtml", model);
                            }

                            #endregion Gallery
                    }
                }
                else
                {
                    ProductDetail product = _productManager.GetByNameAscii(!string.IsNullOrEmpty(nameAsciiC) ? nameAsciiC : nameAscii);
                    if (product != null)
                    {
                        if (!string.IsNullOrEmpty(nameAscii) && !string.IsNullOrEmpty(nameAsciiC))
                        {
                            return RedirectPermanent("/" + nameAsciiC);
                        }
                        module = await cacheUtils.GetModuleByNameAscii(product.ModuleNameAscii);
                        product.AlbumGalleryItems = !string.IsNullOrEmpty(product.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(product.AlbumPictureJson) : new List<AlbumGalleryItem>();
                        product.ColorTableItems = !string.IsNullOrEmpty(product.ColorTable) ? JsonConvert.DeserializeObject<List<ColorTableItem>>(product.ColorTable) : new List<ColorTableItem>();
                        moduleType = module != null ? module.ModuleTypeCode : StaticEnum.Product;
                        if (module != null)
                        {
                            module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                        }
                        SearchModel search = new() { page = 0, lang = Lang(), sort = 5 };
                        switch (moduleType)
                        {
                            #region Products

                            case StaticEnum.Sale:
                            case StaticEnum.Product:
                            case StaticEnum.Trademark:
                                {
                                    SearchModel searchModel = new() { productId = product.ID, page = 1 };
                                    string contentIds = product.ID.ToString();
                                    searchModel.customerId = GetWebUserID();
                                    var commentID = Request.Query["comment"];
                                    search.contentId = product.ID;
                                    search.moduleId = module != null ? module.ID : 0;
                                    //search.pagesize = 5;
                                    IEnumerable<WebsiteModulesItem> listModule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                                    var ModuleTypesNews = cacheUtils.GetByModuleTypeCode(StaticEnum.News, Lang());
                                    var ModuleTypesProject = cacheUtils.GetByModuleTypeCode(StaticEnum.Project, Lang());
                                    ContentViewModels model = new()
                                    {
                                        CustomerItem = _customerManager.GetId(GetWebUserID()),
                                        ProductItem = product,
                                        ModuleItem = module ?? new WebsiteModulesItem(),
                                        SystemConfigJson = cacheUtils.SystemConfigItem(Lang()),
                                        ColorTableItems = !string.IsNullOrEmpty(product.ColorTable) ? JsonConvert.DeserializeObject<List<ColorTableItem>>(product.ColorTable) : new List<ColorTableItem>(),
                                        ListContentItem = await _webContentManager.GetListContent(search, 3, module.ID, string.Join(",", ModuleTypesNews.Select(x => x.ID)), product.ID.ToString()),
                                        ListProductItem = await _productManager.GetListProductJson(search, 3, module.ID, string.Join(",", listModule.Select(x => x.ID)), product.ID.ToString()),
                                        WebsiteModulesItem = _webModuleManager.GetByTypeCode(StaticEnum.Contact, Lang()),
                                        ListContentItem2 = _webContentManager.GetListContentHighlights(3, module.ID, string.Join(",", ModuleTypesNews.Select(x => x.ID))),
                                        ListContentItemProjects = _webContentManager.GetListContentHighlights(3, module.ID, string.Join(",", ModuleTypesProject.Select(x => x.ID)))
                                    };
                                    if (module.TypeView == StaticEnum.TypeProductII)
                                    {
                                        return View(@"~/Views/Product/DetailProductII.cshtml", model);
                                    }

                                    if (module.TypeView == StaticEnum.TypeProductIII)
                                    {
                                        return View(@"~/Views/Product/DetailProductIII.cshtml", model);
                                    }
                                    var ModuleTypes = cacheUtils.GetByModuleTypeCode(StaticEnum.Contact, Lang());
                                    model.ListContentItem = await _webContentManager.GetListContent(new SearchModel { page = 1, sort = 4, lang = Lang() }, 0, ModuleTypes.FirstOrDefault().ID, string.Join(",", ModuleTypes.Select(x => x.ID)), "0");
                                    await _productManager.UpdateTotalViews(product.ID);
                                    return View(@"~/Views/Product/DetailProduct.cshtml", model);
                                }

                                #endregion Products
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            Response.StatusCode = 404;
            return View(@"~/Views/Error/Error404.cshtml");
        }

        #endregion Quickview

        #region Redirect301

        public IActionResult Redirect301(string moduleId, string contentId, string nameAscii, string nameAsciiC)
        {
            try
            {
                if (!string.IsNullOrEmpty(moduleId) && !string.IsNullOrEmpty(contentId) && !string.IsNullOrEmpty(nameAscii) && nameAscii.Contains(".htm") && string.IsNullOrEmpty(nameAsciiC))
                {
                    string newLink = "/" + Utility.ValidString(nameAscii.Replace(".htm", ""), "301", true).ToLower();
                    return RedirectPermanent(newLink);
                }
                if (!string.IsNullOrEmpty(moduleId) && !string.IsNullOrEmpty(contentId) && !string.IsNullOrEmpty(nameAscii) && !string.IsNullOrEmpty(nameAsciiC) && nameAsciiC.Contains(".htm"))
                {
                    string newNameAscii = Utility.ValidString(nameAsciiC.Replace(".htm", ""), "301", true).ToLower();
                    WebsiteContentItem content = _webContentManager.GetByNameAscii(newNameAscii);
                    ProductDetail product = _productManager.GetByNameAscii(newNameAscii);
                    if (content != null)
                    {
                        string newLink = Utility.Link(content.ModuleNameAscii, content._NameAscii, content.LinkUrl);
                        return RedirectPermanent(newLink);
                    }
                    if (product != null)
                    {
                        return RedirectPermanent("/" + product._NameAscii);
                    }
                    Response.StatusCode = 404;
                    return View(@"~/Views/Error/Error404.cshtml");
                }
                else
                {
                    Response.StatusCode = 404;
                    return View(@"~/Views/Error/Error404.cshtml");
                }
            }
            catch
            {
                Response.StatusCode = 404;
                return View(@"~/Views/Error/Error404.cshtml");
            }
        }

        #endregion Redirect301

        #region Apply

        public async Task<ActionResult> Apply(int? id)
        {
            try
            {
                WebsiteContentItem content = _webContentManager.GetContentById(id ?? 0);
                WebsiteModulesItem module = await cacheUtils.GetModuleByNameAscii(content.ModuleNameAscii);
                if (module != null)
                {
                    module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                }
                if (content != null)
                {
                    SearchModel search = new() { page = 1, sort = 0, lang = Lang() };
                    ContentViewModels model = new()
                    {
                        ContentItem = content,
                        ModuleItem = module,
                        ListContentItem = await _webContentManager.GetListContent(search, 5, module.ID, "", content.ID.ToString()),
                        Areas = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("Area.json", "DataJson"))
                    };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return Redirect("/error");
            //Response.StatusCode = 404;
            //return View(@"~/Views/Error/Error404.cshtml");
        }

        #endregion Apply

        #region BookTour

        public async Task<ActionResult> BookTour(int? id)
        {
            try
            {
                ProductDetail product = _productManager.GetId(id ?? 0);
                WebsiteModulesItem module = await cacheUtils.GetModuleByNameAscii(product.ModuleNameAscii);
                if (module != null)
                {
                    module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
                }
                if (product != null)
                {
                    SearchModel search = new() { page = 1, sort = 0, lang = Lang() };
                    ModuleViewModels model = new()
                    {
                        ProductItem = product,
                        ModuleItem = module
                    };
                    List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
                    List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
                    if (model.ProductItem != null)
                    {
                        model.ProductItem.ListSubItems = _subItemManager.GetAll(Lang(), model.ProductItem.ID);
                        if (!string.IsNullOrEmpty(model.ProductItem.Times))
                            model.ProductItem.TimesValue = listItem != null ? listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(model.ProductItem.Times)).Name : null;
                        if (!string.IsNullOrEmpty(model.ProductItem.AddressId))
                            model.ProductItem.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(model.ProductItem.AddressId)).Name;

                    }
                    ViewBag.NoIndex = true;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return Redirect("/error");
            //Response.StatusCode = 404;
            //return View(@"~/Views/Error/Error404.cshtml");
        }

        #endregion BookTour

        //public IActionResult TradeMark()
        //{
        //    try
        //    {
        //        ModuleViewModels model = new()
        //        {
        //            ModuleItem = new WebsiteModulesItem
        //            {
        //                Name = ResourceData.Resource("NameTradeMark", Lang()),
        //                SEOTitle = ResourceData.Resource("TitleTradeMark", Lang()),
        //                SeoDescription = ResourceData.Resource("DescriptionTradeMark", Lang()),
        //                SeoKeyword = ResourceData.Resource("KeywordTradeMark", Lang())
        //            },
        //            WebsiteModulesItems = cacheUtils.GetByModuleTypeCode(StaticEnum.Trademark, Lang())
        //        };
        //        return View(@"~/Views/Content/TradeMark.cshtml", model);
        //    }
        //    catch
        //    {
        //        Response.StatusCode = 404;
        //        return View(@"~/Views/Error/Error404.cshtml");
        //    }
        //}
        public async Task<IActionResult> TagsProducts(string nameAscii, int? p, string code)
        {
            string path = HttpUtility.UrlDecode(Request.Path).ToLower();
            if (!string.IsNullOrEmpty(path))
            {
                string json = await ReadFileAsync("Redirect.json", "DataJson");
                List<RedirectJson> listredirect = JsonConvert.DeserializeObject<List<RedirectJson>>(json);
                if (listredirect != null && listredirect.Any(x => HttpUtility.UrlDecode(x.OldUrl).ToLower() == path))
                {
                    RedirectJson newurl = listredirect.FirstOrDefault(x => HttpUtility.UrlDecode(x.OldUrl).ToLower() == path);
                    return newurl.TypeRedirect == "302" ? Redirect(newurl.NewUrl) : RedirectPermanent(newurl.NewUrl);
                }
            }
            if (nameAscii != null && nameAscii.Contains(".htm"))
            {
                nameAscii = nameAscii.Replace(".htm", "");
                return RedirectPermanent("/" + nameAscii.ToLower());
            }
            TagItem module = new();
            module = await cacheUtils.GetTagByNameAscii(nameAscii);
            try
            {
                if (module != null)
                {
                    SearchModel search = new();
                    await TryUpdateModelAsync(search);
                    search.page = p ?? 1;
                    int total = 0;
                    search.pagesize = 6;
                    search.lang = Lang();
                    TagViewModel model = new()
                    {
                        TagItem = module,
                        SearchModel = search,
                    };
                    var listProduct = _productManager.GetListProductByTagItem(module.Name, search.lang, search.page, search.pagesize);
                    total = listProduct.Any() ? listProduct.FirstOrDefault().TotalRecord : 0;
                    model.ListProductItemAsync = listProduct;
                    List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
                    List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
                    if (model.ListProductItemAsync.Any())
                    {
                        foreach (var item in model.ListProductItemAsync)
                        {
                            if (!string.IsNullOrEmpty(item.Times))
                                item.TimesValue = listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(item.Times)).Name;
                            if (!string.IsNullOrEmpty(item.AddressId))
                                item.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(item.AddressId)).Name;
                        }
                    }
                    model.Total = total;
                    model.Page = search.page;
                    model.PageSize = search.pagesize;
                    ViewBag.NameAscii = nameAscii;
                    if (code == "pagging")
                        return View(@"~/Views/PartialContent/PartialTag.cshtml", model);
                    return View(@"~/Views/Product/ListByTag.cshtml", model);
                }
                else
                {
                    Response.StatusCode = 404;
                    return View(@"~/Views/Error/Error404.cshtml");
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            Response.StatusCode = 404;
            return View(@"~/Views/Error/Error404.cshtml");
        }

        #region Partial

        [HttpPost]
        public async Task<IActionResult> PartialSameProduct(int id, int module)
        {
            try
            {
                SearchModel search = new();
                await TryUpdateModelAsync(search);
                if (!string.IsNullOrEmpty(search.q))
                {
                    search.q = Utility.RemoveSpecialCharacter(search.q);
                }
                IEnumerable<WebsiteProductItemJson> product = await _productManager.GetSameProductMore(search, id, module.ToString());
                if (string.IsNullOrEmpty(search.q))
                {
                    ProductDetail productItem = _productManager.GetId(id);
                    search.price = productItem.Price;
                    product = await _productManager.GetRandomProductMore(search, id, module.ToString());
                }
                ModuleViewModels model = new();
                model.ListProductItemJson = product;
                return View(@"~/Views/PartialContent/PartialSameProduct.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> ProductByListIds(string lstIds, int no)
        {
            ViewBag.No = no;
            IEnumerable<ProductItem> products = await _productManager.GetListByIds(lstIds);
            ModuleViewModels model = new();
            model.ListProductItem = products;
            return View(model);
        }

        public ActionResult ProductByID(int id, int no)
        {
            ViewBag.No = no;
            ProductItem product = _productManager.GetById(id);
            product.AlbumGalleryItems = !string.IsNullOrEmpty(product.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(product.AlbumPictureJson) : new List<AlbumGalleryItem>();
            return View(product);
        }

        [HttpPost]
        public IActionResult PopupVideo(int id)
        {
            try
            {
                WebsiteContentItem content = _webContentManager.GetContentById(id);
                return View(@"~/Views/PartialContent/PopupVideo.cshtml", content);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Good()
        {
            try
            {
                SearchModel search = new();
                await TryUpdateModelAsync(search);
                if (search.productId > 0)
                {
                    _productManager.UpdateGoodViews(search.productId);
                }
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Bad()
        {
            try
            {
                SearchModel search = new();
                await TryUpdateModelAsync(search);
                if (search.productId > 0)
                {
                    _productManager.UpdateBadViews(search.productId);
                }
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialProduct()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                search.view = string.IsNullOrEmpty(search.view) ? "grid" : search.view;
                int pageSize = 6;
                WebsiteModulesItem module = await cacheUtils.GetModuleByNameAscii(search.seoUrl);
                if (module.ModuleTypeCode == StaticEnum.Trademark)
                {
                    search.brandId = module.ID;
                }
                IEnumerable<WebsiteProductItemJson> listProduct = new List<WebsiteProductItemJson>();
                if (module != null)
                {
                    IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                    List<string> lstattr = new();
                    for (int idx = 0; idx < Request.Query.Keys.Count; idx++)
                    {
                        string key = Request.Query.Keys.ToList()[idx];
                        bool checkattr = _productManager.CheckAttrByNameAscii(key);
                        if (key != "sort" && key != "seoUrl" && key != "page" && key != "gia" && key != "hsx" && key != "size" && key != "view" && Utility.IsArrIds(Request.Query[key]) && checkattr == true)
                            lstattr.Add(Request.Query[key]);
                    }
                    search.attr = string.Join(",", lstattr);
                    search.ListAttr = lstattr;
                    search.sort = search.sort;
                    if (search.page == 1)
                    {
                        search.start = 0;
                    }
                    else
                    {
                        search.start = (6 + (search.page - 2) * pageSize);
                    }
                    SearchModel searchproduct = new()
                    {
                        attr = search.attr,
                        ListAttr = search.ListAttr,
                        page = search.page,
                        gia = search.gia,
                        moduleId = search.moduleId,
                        q = search.q,
                        lang = search.lang,
                        sort = search.sort,
                        hsx = search.hsx,
                        brandId = search.brandId
                    };
                    listProduct = await _productManager.GetListProductJson(searchproduct, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                    List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
                    List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
                    if (listProduct.Any())
                    {
                        foreach (var item in listProduct)
                        {
                            if (!string.IsNullOrEmpty(item.Times))
                                item.TimesValue = listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(item.Times)).Name;
                            if (!string.IsNullOrEmpty(item.AddressId))
                                item.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(item.AddressId)).Name;
                        }
                    }
                }
                model.ListProductItemJson = listProduct;
                model.Total = listProduct.Any() ? listProduct.FirstOrDefault().TotalRecord.Value : 0;
                model.PageSize = pageSize;
                model.Page = search.page > 1 ? search.page : 1;
                model.ModuleItem = module;
                model.SearchModel = search;
                ViewBag.GridHtml = Common.GetAjaxPage(search.page, model.Total.Value, pageSize);
                //if (model.SearchModel.view == "list")
                //{
                //    return View(@"~/Views/PartialContent/PartialProductII.cshtml", model);
                //}
                return View(@"~/Views/PartialContent/PartialProduct.cshtml", model);
            }
            catch (Exception ex)
            {
                AddLogError(ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialSearchTour()
        {
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                int total = 0;
                await TryUpdateModelAsync(search);
                search.pagesize = 6;
                search.q = Utility.RemoveHTMLTag(search.q);
                search.TourType = Utility.RemoveHTMLTag(search.TourType);
                search.AddressStart = Utility.RemoveHTMLTag(search.AddressStart);
                search.Times = Utility.RemoveHTMLTag(search.Times);
                SearchViewModel model = new()
                {
                    SearchModel = search,
                    ListProducts = await _productManager.GetListProductTour(search)
                };

                List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
                List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
                if (model.ListProducts.Any())
                {
                    foreach (var item in model.ListProducts)
                    {
                        if (!string.IsNullOrEmpty(item.Times))
                            item.TimesValue = listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(item.Times)).Name;
                        if (!string.IsNullOrEmpty(item.AddressId))
                            item.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(item.AddressId)).Name;
                    }
                }
                total = model.ListProducts.Any() ? model.ListProducts.FirstOrDefault().TotalRecord : 0;
                model.Total = total;
                model.PageSize = search.pagesize;
                model.Page = search.page > 1 ? search.page : 1;
                model.SearchModel = search;
                model.Keyword = search.q;
                model.ModuleItem = _webModuleManager.GetByTypeCode(StaticEnum.Search, Lang());
                model.ModuleItem.AlbumGalleryItems = !string.IsNullOrEmpty(model.ModuleItem.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(model.ModuleItem.AlbumPictureJson) : new List<AlbumGalleryItem>();
                return View(@"~/Views/PartialContent/PartialSearchTour.cshtml", model);
            }
            catch (Exception ex)
            {
                AddLogError(ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialNews()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                foreach (var prop in search.GetType().GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        var val = search.GetType().GetProperty(prop.Name).GetValue(search, null);
                        if (val != null)
                        {
                            prop.SetValue(search, Utility.RemoveSpecialCharacterSQLInjection2(val.ToString()));
                        }
                    }
                }
                var module = await cacheUtils.GetModuleByNameAscii(search.seoUrl);
                if (module != null)
                {
                    int pageSize = 6;
                    if (search.page == 1)
                    {
                        search.start = 0;
                    }
                    else
                    {
                        search.start = module.ModuleTypeCode == StaticEnum.Video ? (10 + (search.page - 2) * pageSize) : (6 + (search.page - 2) * pageSize);
                    }
                    IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                    IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                    model.ListContentItem = listContent;
                    model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                    model.PageSize = pageSize;
                    model.Page = search.page > 1 ? search.page : 1;
                    model.ModuleItem = module;
                    return View(@"~/Views/PartialContent/PartialNews.cshtml", model);
                }
                return NotFound();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialDocument()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                int pageSize = 0;
                IEnumerable<WebsiteModulesItem> childmodule = new List<WebsiteModulesItem>();
                WebsiteModulesItem module = new();
                if (search.moduleId > 0)
                {
                    module = _webModuleManager.GetById(search.moduleId);
                    childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                }
                else
                {
                    module = new WebsiteModulesItem { ID = 0 };
                    childmodule = await _webModuleManager.GetAllModuleByCode(StaticEnum.Document, Lang());
                }
                IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                model.ListContentItemAsync = listContent;
                model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                model.PageSize = pageSize;
                model.Page = search.page > 1 ? search.page : 1;
                return View(@"~/Views/PartialContent/PartialDocument.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialRecuitment()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                //foreach (var prop in search.GetType().GetProperties())
                //{
                //    if (prop.PropertyType == typeof(string))
                //    {
                //        var val = search.GetType().GetProperty(prop.Name).GetValue(search, null);
                //        if (val != null)
                //        {
                //            prop.SetValue(search, Utility.RemoveSpecialCharacterSQLInjection2(val.ToString()));
                //        }
                //    }
                //}
                var module =  cacheUtils.GetModuleById(search.moduleId);
                if (module != null)
                {
                    int pageSize = 4;
                    if (search.page == 1)
                    {
                        search.start = 0;
                    }
                    else
                    {
                        search.start = module.ModuleTypeCode == StaticEnum.Video ? (4 + (search.page - 2) * pageSize) : (4 + (search.page - 2) * pageSize);
                    }
                    
                    IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, module.ID, "0", "0");
                    model.ListContentItem = listContent;
                    model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
                    model.PageSize = pageSize;
                    model.Page = search.page > 1 ? search.page : 1;
                    model.ModuleItem = module;
                    model.Number = search.number;
                    return View(@"~/Views/PartialContent/PartialRecuitment.cshtml", model);
                }
                return NotFound();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> RateStar(int star, int ProductId)
        {
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                var product = _productManager.GetById(ProductId);
                var totalStar = product.Star + star;
                var totalRate = product.TotalRate == null ? 1 : product.TotalRate + 1;
                product.TotalStars = Math.Round(Double.Parse(totalStar.ToString()) / Double.Parse(totalRate.ToString()), 1);
                await _productManager.UpdateTotalRate(ProductId, totalStar, product.TotalStars, totalRate.Value);
                return Ok(product.TotalStars);
            }
            catch
            {
                return NotFound();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> PartialNewsTag()
        //{
        //    try
        //    {
        //        var model = new TagViewModel();
        //        Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
        //        Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
        //        SearchModel search = new()
        //        {
        //            lang = Lang()
        //        };
        //        await TryUpdateModelAsync(search);
        //        var module = await cacheUtils.GetTagByNameAscii(search.seoUrl);
        //        model.TagItem = module;
        //        search.pagesize = 10;
        //        search.page = search.page > 1 ? search.page : 1;
        //        model.SearchModel = search;
        //        IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContentByTagItem(search, module.ID.ToString());
        //        model.ListContentItemAsync = listContent;
        //        model.Total = listContent.Any() ? listContent.FirstOrDefault().TotalRecord : 0;
        //        return View(@"~/Views/PartialContent/PartialNewsTag.cshtml", model);
        //    }
        //    catch
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost]
        [ResponseCache(VaryByHeader = "Content Home", Duration = 30)]
        public async Task<IActionResult> PartialContentIndex()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                ModulePositionItem position = _modulePositionManager.GetByIdAsync(search.type);
                model.PositionItem = position;
                int pageSize = position.NumberContent.HasValue ? position.NumberContent.Value : 6;
                search.strWhere = !string.IsNullOrEmpty(position.SqlContent) ? position.SqlContent : string.Empty;
                search.strOrder = !string.IsNullOrEmpty(position.SqlContentOrderBy) ? position.SqlContentOrderBy : string.Empty;
                IEnumerable<WebsiteModulesItem> modules = new List<WebsiteModulesItem>();
                if (search.moduleId > 0)
                {
                    modules = await cacheUtils.GetListModuleChidrentAsync(search.moduleId);
                    model.ModuleItem = cacheUtils.GetModuleById(search.moduleId);
                    model.WebsiteModulesItems = new List<WebsiteModulesItem> { model.ModuleItem };
                    IEnumerable<WebsiteContentItem> listContent = await _webContentManager.GetListContent(search, pageSize, model.ModuleItem.ID, Convert.ToString(model.ModuleItem.ID), "0");
                    model.ListContentItemAsync = listContent;
                }
                else
                {
                    modules = cacheUtils.GetListModuleView(position.Code, new List<ModulePositionItem> { position }, Lang());
                    search.moduleId = modules.FirstOrDefault().ID;
                    model.ModuleItem = modules.FirstOrDefault();
                    model.WebsiteModulesItems = modules;
                }
                if (position.TypeView == StaticEnum.Content)
                {
                    model.ListContentItemAsync = await _webContentManager.GetListContentHome(search, pageSize, search.moduleId, string.Join(",", modules.Select(x => x.ID)));
                    model.PageSize = pageSize;
                    await model.ListContentItemAsync.ToAsyncEnumerable().ForEachAsync(x =>
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(x.LinkDownload))
                            {
                                x.FileDownloadAdmins = JsonConvert.DeserializeObject<List<FileDownloadAdmin>>(x.LinkDownload);
                            }
                        }
                        catch
                        {
                            x.FileDownloadAdmins = new List<FileDownloadAdmin>();
                        }
                    });
                }
                //if (Utility.IsMobile(HttpContext.Request.Headers["User-Agent"]))
                //{
                //    return View(@"~/Views/Mobile/PartialContentIndex.cshtml", model);
                //}
                return View(@"~/Views/PartialContent/PartialContent.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> PartialFillterProduct()
        {
            try
            {
                ModuleViewModels model = new();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                int pageSize = 16;
                WebsiteModulesItem module = await cacheUtils.GetModuleByNameAscii(search.seoUrl);

                IEnumerable<WebsiteModulesItem> childmodule = await cacheUtils.GetListModuleChidrentAsync(module.ID);
                IEnumerable<WebsiteProductItemJson> listProduct = new List<WebsiteProductItemJson>();
                if (module != null)
                {
                    SearchModel searchproduct = new()
                    {
                        page = search.page,
                        moduleId = search.moduleId,
                        q = search.q,
                        lang = search.lang,
                        wattage = search.wattage,
                        sort = search.sort
                    };
                    //if (search.attr != "")
                    //{
                    //    listProduct = await _productManager.GetListProductFilterBrand(searchproduct, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)));
                    //}
                    //else
                    //{
                    //    listProduct = await _productManager.GetListProductJson(searchproduct, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)), "0");
                    //}
                    listProduct = await _productManager.GetListProductFilter(searchproduct, pageSize, module.ID, string.Join(",", childmodule.Select(x => x.ID)));
                }
                model.ListProductItemJson = listProduct;
                model.Total = listProduct.Any() ? listProduct.FirstOrDefault().TotalRecord.Value : 0;
                model.PageSize = pageSize;
                model.Page = search.page > 1 ? search.page : 1;
                model.ModuleItem = module;
                model.SearchModel = search;
                ViewBag.GridHtml = Common.GetAjaxPage(search.page, model.Total.Value, pageSize);
                return View(@"~/Views/PartialContent/PartialProductFillter.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [HttpPost]
        [ResponseCache(VaryByHeader = "Product Home", Duration = 30)]
        public async Task<IActionResult> PartialProductIndex()
        {
            try
            {
                var model = new ModuleViewModels();
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new()
                {
                    lang = Lang()
                };
                await TryUpdateModelAsync(search);
                ModulePositionItem position = _modulePositionManager.GetByIdAsync(search.type);
                model.PositionItem = position;
                int pageSize = position.NumberContent.HasValue ? position.NumberContent.Value : 10;
                search.strWhere = !string.IsNullOrEmpty(position.SqlContent) ? position.SqlContent : string.Empty;
                search.strOrder = !string.IsNullOrEmpty(position.SqlContentOrderBy) ? position.SqlContentOrderBy : string.Empty;
                IEnumerable<WebsiteModulesItem> modules = await cacheUtils.GetListModuleChidrentAsync(search.moduleId);
                IEnumerable<ProductItem> products = await _webContentManager.GetListProductHomeAsync(search, pageSize, search.moduleId, string.Join(",", modules.Select(x => x.ID)));
                model.ListProductItemAsync = products;
                model.PageSize = pageSize;
                model.SearchModel = search;
                model.Total = products.Any() ? products.FirstOrDefault().TotalRecord : 0;
                model.SystemConfig = cacheUtils.SystemConfigItem(search.lang);
                return await Task.Run(() => View(@"~/Views/PartialContent/PartialProductIndex.cshtml", model));
            }
            catch
            {
                return NotFound();
            }
        }

        #endregion Partial

        #region Search

        [HttpPost]
        [Route("/tim-kiem")]
        public async Task<ActionResult> Search()
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            SearchModel search = new()
            {
                lang = Lang()
            };
            await TryUpdateModelAsync(search);
            if (Utility.CheckNumber(search.page.ToString()) == false || Utility.CheckNumber(search.type.ToString()) == false)
            {
                return NotFound();
            }
            search.page = 1;
            search.pagesize = 25;
            WebsiteModulesItem module = new()
            {
                Name = $"{ResourceData.Resource("KetQuaTimKiemTuKhoa", Lang())}"
            };
            SearchViewModel model = new()
            {
                SearchModel = search,
                ModuleItem = module,
                //news
                ListArticle = ListContentSearch(search),
                ListProductItemJson = ListProductSearch(search),
                PageSize = search.pagesize
            };
            module.AlbumGalleryItems = !string.IsNullOrEmpty(module.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(module.AlbumPictureJson) : new List<AlbumGalleryItem>();
            model.Total = model.ListArticle.Any() ? model.ListArticle.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = Common.GetAjaxPage(search.page, model.Total.Value, search.pagesize);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            ViewBag.Elapsed = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return View(model);
        }

        [HttpPost]
        [Route("/tim-kiem-tour")]
        public async Task<ActionResult> SearchTour(int? p)
        {
            SearchModel search = new()
            {
                lang = Lang()
            };
            int total = 0;
            await TryUpdateModelAsync(search);
            search.pagesize = 6;
            search.q = Utility.RemoveHTMLTag(search.q);
            search.TourType = Utility.RemoveHTMLTag(search.TourType);
            search.AddressStart = Utility.RemoveHTMLTag(search.AddressStart);
            search.Times = Utility.RemoveHTMLTag(search.Times);
            SearchViewModel model = new()
            {
                SearchModel = search,
                ListProducts = await _productManager.GetListProductTour(search)
            };

            List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
            List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
            if (model.ListProducts.Any())
            {
                foreach (var item in model.ListProducts)
                {
                    if (!string.IsNullOrEmpty(item.Times))
                        item.TimesValue = listItem.FirstOrDefault(y => y.ID == Convert.ToInt32(item.Times)).Name;
                    if (!string.IsNullOrEmpty(item.AddressId))
                        item.Address = listItem2.FirstOrDefault(y => y.ID == Convert.ToInt32(item.AddressId)).Name;
                }
            }
            total = model.ListProducts.Any() ? model.ListProducts.FirstOrDefault().TotalRecord : 0;
            model.Total = total;
            model.PageSize = search.pagesize;
            model.Page = p ?? 1;
            model.SearchModel = search;
            model.Keyword = search.q;
            model.ModuleItem = _webModuleManager.GetByTypeCode(StaticEnum.Search, Lang());
            model.ModuleItem.AlbumGalleryItems = !string.IsNullOrEmpty(model.ModuleItem.AlbumPictureJson) ? JsonConvert.DeserializeObject<List<AlbumGalleryItem>>(model.ModuleItem.AlbumPictureJson) : new List<AlbumGalleryItem>();
            return View(@"~/Views/Content/SearchTour.cshtml", model);
        }

        public async Task<ActionResult> SearchNews(int? p)
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            SearchModel search = new()
            {
                lang = Lang()
            };
            await TryUpdateModelAsync(search);
            search.sort = 1;
            if (Utility.CheckNumber(search.page.ToString()) == false || Utility.CheckNumber(search.type.ToString()) == false || Utility.CheckNumber(search.sort.ToString()) == false)
            {
                return NotFound();
            }
            search.q = Utility.RemoveSpecialCharacter(search.q);
            search.page = p ?? 1;
            search.pagesize = 10;
            WebsiteModulesItem module = new()
            {
                Name = $"{ResourceData.Resource("KetQuaTimKiemTuKhoa", Lang())} '{search.q}'"
            };
            ModuleViewModels model = new()
            {
                SearchModel = search,
                ModuleItem = module,
                SystemConfig = cacheUtils.SystemConfigItem(Lang())
            };
            List<WebsiteContentItem> contentList = ListContentSearch(search);
            int total = contentList != null && contentList.Count > 0 ? contentList.FirstOrDefault().TotalRecord : 0;
            model.ListContentItem = contentList;
            model.ListProductItem = new List<ProductItem>();
            model.Total = total;
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            ViewBag.Elapsed = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListSearch()
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            SearchModel search = new()
            {
                lang = Lang(),
                pagesize = 10
            };
            ViewBag.Lang = Lang();
            await TryUpdateModelAsync(search);
            search.q = Utility.RemoveSpecialCharacter(search.q);
            if (Utility.CheckNumber(search.page.ToString()) == false || Utility.CheckNumber(search.type.ToString()) == false)
            {
                return NotFound();
            }
            SearchViewModel model = new()
            {
                SearchModel = search
            };
            List<WebsiteContentItem> contentList = ListContentSearch(search);
            int total = contentList != null && contentList.Count > 0 ? contentList.FirstOrDefault().TotalRecord : 0;
            model.ListArticle = contentList;
            model.ListProductItemJson = new List<WebsiteProductItemJson>();
            model.Total = total;
            model.PageSize = search.pagesize;
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            ViewBag.Elapsed = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return PartialView("~/Views/Content/ListSearch.cshtml", model);
        }

        public List<WebsiteContentItem> ListContentSearch(SearchModel search)
        {
            List<WebsiteContentItem> contentList = _webContentManager.GetSearchContent(search.q, search.lang, search.page, search.pagesize);
            return contentList;

            #region Remove

            //List<string> codes = new List<string>{
            //    StaticEnum.News, StaticEnum.Video,StaticEnum.Gallery
            //};
            //List<WebsiteModulesItem> modules = _webModuleManager.GetByListModuleTypeCode(string.Join(",", codes), Lang());
            //StringBuilder sqlWhere = new StringBuilder();
            //StringBuilder keyView = new StringBuilder();
            //StringBuilder totalNameAscii = new StringBuilder();
            //List<string> key = new List<string>();
            //string sqlOrder = " ORDER BY CreatedDate DESC";
            //if (search.sort == 2)
            //{
            //    sqlOrder = " ORDER BY TotalViews DESC, CreatedDate DESC";
            //}
            //if (!string.IsNullOrEmpty(search.q))
            //{
            //    key = search.q.Trim().ToLower().Split(' ').ToList();
            //    if (search.type != 1)
            //    {
            //        sqlOrder = string.Format(" ORDER BY case when Lower(Name) LIKE N'%{0}%' then 99999999 when NameAscii LIKE N'%{1}%'" +
            //      " then 99999998 end DESC, TotalNameAscii DESC, TotalViews DESC, CreatedDate DESC ", search.q, Utility.ConvertRewrite(search.q));
            //    }
            //}
            //if (key.Count > 0)
            //{
            //    var listColum = new List<string>();
            //    listColum.Add(nameof(WebsiteContentItem.Name));
            //    listColum.Add(nameof(WebsiteContentItem.Description));
            //    sqlWhere.Append(SqlUtility.WherAndNLikeListSearch(key, listColum));
            //    foreach (var item in key)
            //    {
            //        keyView.Append(string.Format(SqlConst.TotalNameAscii, Utility.ConvertRewrite(item)));
            //    }
            //    totalNameAscii.Append(",(0" + keyView + ") TotalNameAscii");
            //}
            //if (modules.Any() && modules.Count() > 0)
            //{
            //    sqlWhere.Append(SqlUtility.WhereOrLikeList(modules.Select(x => x.ID.ToString()).ToList(), "ModuleIds"));
            //}
            //if (!string.IsNullOrEmpty(sqlFix))
            //{
            //    sqlWhere.Append(" " + sqlFix);
            //}
            //sqlWhere.Append(" AND CreatedDate <= GETDATE()");
            //sqlWhere.Append(" AND NameAscii is not null and LinkUrl is null");
            //sqlWhere.Append(" AND Lang = '" + search.lang + "'");
            //StringBuilder sqlContent = new StringBuilder();
            //sqlContent.Append("Select * from(");
            //sqlContent.Append(string.Format(SqlContent.SqlByListSearch, totalNameAscii, sqlWhere));
            //sqlContent.Append(") c");
            //sqlContent.Append(sqlOrder);
            //var _dapperDa = new DapperDA(WebConfig.ConnectionString);
            //if (search.pagesize > 0)
            //{
            //    return _dapperDa.SelectPage<WebsiteContentItem>(sqlContent.ToString(), search.page, search.pagesize).ToList();
            //}
            //else
            //{
            //    return _dapperDa.Select<WebsiteContentItem>(sqlContent.ToString()).ToList();
            //}

            #endregion Remove
        }

        public List<WebsiteContentItem> ListDoctorSearch(SearchModel search)
        {
            List<WebsiteContentItem> contentList = _webContentManager.GetSearchDoctor(search.q, search.specialistid, search.addressid, search.moduleid, search.lang);
            return contentList;

            #region Remove

            //List<string> codes = new List<string>{
            //    StaticEnum.News, StaticEnum.Video,StaticEnum.Gallery
            //};
            //List<WebsiteModulesItem> modules = _webModuleManager.GetByListModuleTypeCode(string.Join(",", codes), Lang());
            //StringBuilder sqlWhere = new StringBuilder();
            //StringBuilder keyView = new StringBuilder();
            //StringBuilder totalNameAscii = new StringBuilder();
            //List<string> key = new List<string>();
            //string sqlOrder = " ORDER BY CreatedDate DESC";
            //if (search.sort == 2)
            //{
            //    sqlOrder = " ORDER BY TotalViews DESC, CreatedDate DESC";
            //}
            //if (!string.IsNullOrEmpty(search.q))
            //{
            //    key = search.q.Trim().ToLower().Split(' ').ToList();
            //    if (search.type != 1)
            //    {
            //        sqlOrder = string.Format(" ORDER BY case when Lower(Name) LIKE N'%{0}%' then 99999999 when NameAscii LIKE N'%{1}%'" +
            //      " then 99999998 end DESC, TotalNameAscii DESC, TotalViews DESC, CreatedDate DESC ", search.q, Utility.ConvertRewrite(search.q));
            //    }
            //}
            //if (key.Count > 0)
            //{
            //    var listColum = new List<string>();
            //    listColum.Add(nameof(WebsiteContentItem.Name));
            //    listColum.Add(nameof(WebsiteContentItem.Description));
            //    sqlWhere.Append(SqlUtility.WherAndNLikeListSearch(key, listColum));
            //    foreach (var item in key)
            //    {
            //        keyView.Append(string.Format(SqlConst.TotalNameAscii, Utility.ConvertRewrite(item)));
            //    }
            //    totalNameAscii.Append(",(0" + keyView + ") TotalNameAscii");
            //}
            //if (modules.Any() && modules.Count() > 0)
            //{
            //    sqlWhere.Append(SqlUtility.WhereOrLikeList(modules.Select(x => x.ID.ToString()).ToList(), "ModuleIds"));
            //}
            //if (!string.IsNullOrEmpty(sqlFix))
            //{
            //    sqlWhere.Append(" " + sqlFix);
            //}
            //sqlWhere.Append(" AND CreatedDate <= GETDATE()");
            //sqlWhere.Append(" AND NameAscii is not null and LinkUrl is null");
            //sqlWhere.Append(" AND Lang = '" + search.lang + "'");
            //StringBuilder sqlContent = new StringBuilder();
            //sqlContent.Append("Select * from(");
            //sqlContent.Append(string.Format(SqlContent.SqlByListSearch, totalNameAscii, sqlWhere));
            //sqlContent.Append(") c");
            //sqlContent.Append(sqlOrder);
            //var _dapperDa = new DapperDA(WebConfig.ConnectionString);
            //if (search.pagesize > 0)
            //{
            //    return _dapperDa.SelectPage<WebsiteContentItem>(sqlContent.ToString(), search.page, search.pagesize).ToList();
            //}
            //else
            //{
            //    return _dapperDa.Select<WebsiteContentItem>(sqlContent.ToString()).ToList();
            //}

            #endregion Remove
        }

        public IEnumerable<WebsiteProductItemJson> ListProductSearch(SearchModel search)
        {
            List<WebsiteProductItemJson> contentList = _productManager.GetSearchProduct(search.q, search.lang, search.page, search.pagesize);
            return contentList;

            #region Remove

            //StringBuilder sqlWhere = new StringBuilder();
            //StringBuilder keyView = new StringBuilder();
            //StringBuilder totalNameAscii = new StringBuilder();
            //List<string> key = new List<string>();
            //string sqlOrder = " ORDER BY OrderDisplay Asc";
            //if (!string.IsNullOrEmpty(search.q))
            //{
            //    key = search.q.Trim().ToLower().Split(' ').ToList();
            //    sqlOrder = string.Format(" ORDER BY case when Lower(Name) LIKE N'%{0}%' then 99999999 when NameAscii LIKE N'%{1}%'" +
            //  " then 99999998 end DESC, TotalNameAscii DESC, TotalViews DESC, CreatedDate DESC ", search.q, Utility.ConvertRewrite(search.q));
            //}
            //if (key.Count > 0)
            //{
            //    List<string> listColum = new List<string>();
            //    listColum.Add(nameof(WebsiteProductItemJson.Name));
            //    listColum.Add(nameof(WebsiteProductItemJson.ProductCode));
            //    sqlWhere.Append(SqlUtility.WherAndNLikeListSearch(key, listColum));
            //    foreach (string item in key)
            //    {
            //        keyView.Append(string.Format(SqlConst.TotalNameAscii, Utility.ConvertRewrite(item)));
            //    }
            //    totalNameAscii.Append(",(0" + keyView + ") TotalNameAscii");
            //}
            //if (!string.IsNullOrEmpty(sqlFix))
            //{
            //    sqlWhere.Append(" " + sqlFix);
            //}
            //sqlWhere.Append(" AND CreatedDate <= GETDATE()");
            //sqlWhere.Append(" AND NameAscii is not null and LinkUrl is null");
            //sqlWhere.Append(" AND Lang = '" + search.lang + "'");
            //StringBuilder sqlContent = new StringBuilder();
            //sqlContent.Append("Select * from(");
            //sqlContent.Append(string.Format(SqlContent.SqlByProduct, totalNameAscii, sqlWhere));
            //sqlContent.Append(") c");
            //sqlContent.Append(sqlOrder);
            //DapperDA _dapperDa = new DapperDA(WebConfig.ConnectionString);
            //return await _dapperDa.SelectPageAsync<WebsiteProductItemJson>(sqlContent.ToString(), search.page, search.pagesize);

            #endregion Remove
        }

        #endregion Search

        #region Download

        [Route("tai-ve/{id}")]
        [Route("tai-ve/{id}/{n}")]
        public async Task<IActionResult> Download(int? id, int? n)
        {
            string download = string.Empty;
            if (id.HasValue && id > 0)
            {
                WebsiteContentItem content = _webContentManager.GetContentById(id.Value);
                if (!string.IsNullOrEmpty(content.LinkDownload))
                {
                    List<FileDownloadAdmin> ListFile = new();
                    try
                    {
                        if (!string.IsNullOrEmpty(content.LinkDownload))
                        {
                            ListFile = JsonConvert.DeserializeObject<List<FileDownloadAdmin>>(content.LinkDownload);
                        }
                    }
                    catch
                    {
                    }
                    if (ListFile != null && ListFile.Count > 0)
                    {
                        int skip = n ?? 1;
                        download = ListFile.Skip(skip - 1).FirstOrDefault().FileUrl;
                    }
                    string path = WebConfig.PathServer + download;
                    MemoryStream memory = new();
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetContentType(path), Path.GetFileName(path));
                }
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
            else
            {
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
        }

        #region Download nội bộ

        [Route("tai-ve-noi-bo/{id}")]
        [Route("tai-ve-noi-bo/{id}/{n}")]
        public async Task<IActionResult> DownloadInternal(int? id, int? n)
        {
            string download = string.Empty;
            if (id.HasValue && id > 0)
            {
                WebsiteContentItem content = _webContentManager.GetContentById(id.Value);
                if (!string.IsNullOrEmpty(content.LinkDownload))
                {
                    if (UserId == 0)
                    {
                        TempData["ReturnUrl"] = $"/tai-ve-noi-bo/{content.ID}";
                        if (n.HasValue && n > 0)
                        {
                            TempData["ReturnUrl"] = $"/tai-ve-noi-bo/{content.ID}/{n.Value}";
                        }
                        TempData.Keep();
                        return Redirect("/dang-nhap");
                    }
                    List<FileDownloadAdmin> ListFile = new();
                    try
                    {
                        if (!string.IsNullOrEmpty(content.LinkDownload))
                        {
                            ListFile = JsonConvert.DeserializeObject<List<FileDownloadAdmin>>(content.LinkDownload);
                        }
                    }
                    catch
                    {
                    }
                    if (ListFile != null && ListFile.Count > 0)
                    {
                        int skip = n ?? 1;
                        download = ListFile.Skip(skip - 1).FirstOrDefault().FileUrl;
                    }
                    string path = WebConfig.PathServer + download;
                    MemoryStream memory = new();
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetContentType(path), Path.GetFileName(path));
                }
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
            else
            {
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
        }

        #endregion Download nội bộ

        public async Task<IActionResult> DownloadCatalog(int? id)
        {
            if (id.HasValue && id > 0)
            {
                ProductItem product = _productManager.GetById(id.Value);
                if (product != null && !string.IsNullOrEmpty(product.LinkFile))
                {
                    string path = WebConfig.PathServer + product.LinkFile;
                    MemoryStream memory = new();
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetContentType(path), Path.GetFileName(path));
                }
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
            else
            {
                Response.StatusCode = 404;
                return View("~/Views/Error/Error404.cshtml");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewerPdf()
        {
            Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
            Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
            SearchModel search = new();
            await TryUpdateModelAsync(search);
            ViewBag.LinkDowload = string.Empty;
            ViewBag.Number = search.type;
            WebsiteContentItem content = new();
            if (search.contentId > 0)
            {
                content = _webContentManager.GetContentById(search.contentId);
                if (!string.IsNullOrEmpty(content.LinkDownload))
                {
                    List<string> ListFile = ListHelper.GetValuesArrayTag(content.LinkDownload);
                    if (ListFile != null && ListFile.Count > 0)
                    {
                        ViewBag.LinkDowload = ListFile[search.type];
                    }
                }
            }
            return View("~/Views/PartialContent/ViewerPdf.cshtml", content);
        }

        #endregion Download

        public WebsiteModulesItem GetOrigin(WebsiteModulesItem module)
        {
            if (module.ParentID > 0)
            {
                WebsiteModulesItem origin = _webModuleManager.GetById(module.ParentID.Value);
                return origin.ParentID > 0 ? GetOrigin(origin) : origin;
            }
            return module;
        }

        public WebsiteModulesItem GetModuleOrigin(WebsiteModulesItem module)
        {
            if (module.ParentID > 0)
            {
                WebsiteModulesItem origin = _webModuleManager.GetById(module.ParentID.Value);
                return origin.ParentID == 0 ? module : GetModuleOrigin(origin);
            }
            return module;
        }

        public async Task<IEnumerable<CommentItem>> GetCommentPreview(SearchModel search, int size, int id)
        {
            search.page++;
            IEnumerable<CommentItem> comments = await _commentManager.GetListCommentByPage(search, size);
            if (comments.Any() && comments.Any(x => x.ID == id))
            {
                return comments;
            }
            else if (!comments.Any())
            {
                search.page = 1;
                comments = await _commentManager.GetListCommentByPage(search, size);
                return comments;
            }
            else
            {
                return await GetCommentPreview(search, size, id);
            }
        }

        public async Task<IEnumerable<CommentItem>> GetRatePreview(SearchModel search, int size, int id)
        {
            search.page++;
            IEnumerable<CommentItem> rates = await _commentManager.GetListRateByPage(search, size);
            if (rates.Any() && rates.Any(x => x.ID == id))
            {
                return rates;
            }
            else if (!rates.Any())
            {
                search.page = 1;
                rates = await _commentManager.GetListRateByPage(search, size);
                return rates;
            }
            else
            {
                return await GetRatePreview(search, size, id);
            }
        }
    }
}