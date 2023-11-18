using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADCOnline.Business.Implementation.AdminManager;
using Website.Utils;
using ADCOnline.Simple.Json;
using Newtonsoft.Json;
using ADCOnline.Utils;
using ADCOnline.Simple.Admin;
using Website.Areas.Admin.ViewModels;
using Website.Models;
using Microsoft.AspNetCore.Http;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;

namespace Website.Areas.Admin.Controllers
{
    public class SitemapController : BaseController
    {
        private readonly WebsiteModuleDa _websiteModuleDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly SystemTagDa _systemTagDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly ProductDa _productDa;
        private readonly DapperDA _dapperDa;
        public SitemapController()
        {
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _systemTagDa = new SystemTagDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Sitemap");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            string moduleIds = _websiteModuleDa.GetModuleIds(role, userId);
            HomeAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                WebsiteModuleAdmins = _websiteModuleDa.GetAdminAll(false, Lang(),"","",moduleIds),
                Module = module
            };
            return View(model);
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            int page = seach.page > 1 ? seach.page : 1;
            List<SitemapJson> sitemap = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
            if (!string.IsNullOrEmpty(seach.keyword))
            {
                sitemap = sitemap.Where(x => x.Url.Contains(seach.keyword)).ToList();
            }
            if (seach.ModuleId.HasValue && seach.ModuleId.Value > 0) 
            {
                sitemap = sitemap.Where(x =>  ("," + x.ModuleIds + ",").Contains("," + seach.ModuleId + ",")).ToList();
            }
            if (!string.IsNullOrEmpty(seach.type))
            {
                if (seach.type == "100")
                {
                    sitemap = sitemap.Where(x => x.Priority == 100).ToList();
                }
                if (seach.type == "80-60")
                {
                    sitemap = sitemap.Where(x => x.Priority <= 80 && x.Priority >= 60).ToList();
                }
                if (seach.type == "60-40")
                {
                    sitemap = sitemap.Where(x => x.Priority <= 60 && x.Priority >= 40).ToList();
                }
            }
            if (!string.IsNullOrEmpty(seach.Status))
            {
                sitemap = sitemap.Where(x => x.ChangeFrequency == seach.Status).ToList();
            }
            HomeAdminViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                SitemapJsons = sitemap.ToList(),
                SitemapJson = seach.contentId.HasValue ? sitemap.FirstOrDefault(x => x.ID == seach.contentId.Value) : new SitemapJson(),
                SearchModel = seach
            };
            ViewBag.Total = sitemap.Count;
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            HomeAdminViewModel model = new()
            {
                SitemapJson = new SitemapJson(),
                SystemActionAdmin = SystemActionAdmin,
                SitemapJsons = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"))
            };
            if (action.Do == ActionType.Edit)
            {
                model.SitemapJson = model.SitemapJsons.FirstOrDefault(c => c.ID == ConvertUtil.ToInt32(action.ItemId));
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
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
            SitemapJson obj = new();
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
                        obj.Url = Utility.ValidString(obj.Url, Link, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.ChangeFrequency = Utility.ValidString(obj.ChangeFrequency, Code, true);
                        List<SitemapJson> listCommon = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                        if (listCommon.Any(x => x.Url == obj.Url))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Link này đã tồn tại." };
                            return Ok(msg);
                        }
                        if (listCommon == null)
                        {
                            listCommon = new List<SitemapJson>();
                        }
                        obj.ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000);
                        obj.CreatedDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(obj.Code))
                        {
                            await UpdateSitemapByCode(obj.Code);
                        }
                        listCommon.Add(obj);
                        listCommon = listCommon.OrderByDescending(x => x.CreatedDate).ToList();
                        Common.CreateFileJson(0, listCommon, "Sitemap", "DataJson");
                        AddLogEdit(Request.Path, "Add", obj.ID.ToString(), listCommon);
                        AddLogAdmin(Request.Path, "Thêm mới sitemap", "Actions-Add");
                        msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
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
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        List<SitemapJson> listCommon = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(x => x.ID == ConvertUtil.ToInt32(action.ItemId));
                        SitemapJson oldObj = obj;
                        await TryUpdateModelAsync(obj);
                        obj.Url = Utility.ValidString(obj.Url, Link, true);
                        obj.Code = Utility.ValidString(obj.Code, Code, true);
                        obj.ChangeFrequency = Utility.ValidString(obj.ChangeFrequency, Code, true);
                        if (listCommon.Any(x => x.Url == obj.Url && x.ID != obj.ID))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Link này đã tồn tại", Obj = obj };
                            return Ok(msg);
                        }
                        if (obj.Url != oldObj.Url)
                        {
                            await RemoveSitemapByCode(oldObj.Code);
                        }
                        if (listCommon != null)
                        {
                            listCommon.RemoveAll(x => x.ID == ArrID.FirstOrDefault());
                            listCommon.Add(obj);
                        }
                        else
                        {
                            listCommon = new List<SitemapJson>();
                            listCommon.Add(obj);
                        }
                        if (!string.IsNullOrEmpty(obj.Code))
                        {
                            await UpdateSitemapByCode(obj.Code);
                        }
                        listCommon = listCommon.OrderByDescending(x => x.CreatedDate).ToList();
                        Common.CreateFileJson(0, listCommon, "Sitemap", "DataJson");
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), listCommon);
                        AddLogAdmin(Request.Path, "Sửa sitemap", "Actions-Edit");
                        msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công." };
                        return Ok(msg);
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
                        List<SitemapJson> listCommon = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                        obj = listCommon.FirstOrDefault(c => c.ID == ArrID.FirstOrDefault());
                        IEnumerable<SitemapJson> child = listCommon.Where(x => x.ParentID == obj.ParentID);
                        if (obj != null)
                        {
                            listCommon.Remove(obj);
                            if (!string.IsNullOrEmpty(obj.Code))
                            {
                                await UpdateSitemapByCode(obj.Code);
                            }
                            if (child.Any())
                            {
                                foreach (SitemapJson item in child)
                                {
                                    listCommon.Remove(item);
                                    if (!string.IsNullOrEmpty(item.Code))
                                    {
                                        await RemoveSitemapByCode(item.Code);
                                    }
                                }
                            }
                            Common.CreateFileJson(0, listCommon, "Sitemap", "DataJson");
                            AddLogEdit(Request.Path, "Delete", obj.ID.ToString(), listCommon);
                            AddLogAdmin(Request.Path, "Xóa sitemap", "Actions-Delete");
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
                        List<SitemapJson> listCommon = JsonConvert.DeserializeObject<List<SitemapJson>>(ReadFile("Sitemap.json", "DataJson"));
                        foreach (int item in ArrID)
                        {
                            SitemapJson site = listCommon.FirstOrDefault(c => c.ID == item);
                            if (site != null)
                            {
                                listCommon.Remove(site);
                                if (!string.IsNullOrEmpty(site.Code))
                                {
                                    await UpdateSitemapByCode(site.Code);
                                }
                                IEnumerable<SitemapJson> child = listCommon.Where(x => x.ParentID == site.ParentID);
                                if (child.Any())
                                {
                                    foreach (SitemapJson sitechild in child)
                                    {
                                        listCommon.Remove(sitechild);
                                        if (!string.IsNullOrEmpty(sitechild.Code))
                                        {
                                            await RemoveSitemapByCode(sitechild.Code);
                                        }
                                    }
                                }
                                Common.CreateFileJson(0, listCommon, "Sitemap", "DataJson");
                                AddLogEdit(Request.Path, "Delete", site.ID.ToString(), listCommon);
                                AddLogAdmin(Request.Path, "Xóa sitemap", "Actions-Delete");
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
        [HttpPost]
        public IActionResult ResetAllSitemap()
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            try
            {
                List<SitemapJson> sitemap = new();
                //misc
                SitemapJson misc = new()
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                    Url = WebConfig.Website + "/sitemap-misc.xml",
                    Priority = 100,
                    ChangeFrequency = "Daily",
                    LastModified = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    ParentID = 0,
                    Code = "sitemap-misc"
                };
                sitemap.Add(misc);
                //index
                sitemap.Add(new SitemapJson
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                    Url = WebConfig.Website,
                    Priority = 100,
                    ChangeFrequency = "Daily",
                    LastModified = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    ParentID = misc.ID,
                    Code = "index"
                });
                //sitemap
                // sitemap.Add(new SitemapJson
                // {
                //     ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                //     Url = WebConfig.Website + "/sitemap.xml",
                //     Priority = 80,
                //     ChangeFrequency = "Monthly",
                //     LastModified = DateTime.Now,
                //     CreatedDate = DateTime.Now,
                //     ParentID = misc.ID,
                //     Code = "sitemap"
                // });
                //category
                SitemapJson cateogory = new()
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                    Url = WebConfig.Website + "/sitemap-category.xml",
                    Priority = 80,
                    ChangeFrequency = "Monthly",
                    LastModified = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    ParentID = 0,
                    Code = "sitemap-category"
                };
                sitemap.Add(cateogory);               
                //brand
                // var brand = new SitemapJson
                // {
                //     ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                //     Url = WebConfig.Website + "/sitemap-brand.xml",
                //     Priority = 80,
                //     ChangeFrequency = "Monthly",
                //     LastModified = DateTime.Now,
                //     CreatedDate = DateTime.Now,
                //     ParentID = 0,
                //     Code = "sitemap-brand"
                // };
                //sitemap.Add(brand);
                //news
                SitemapJson news = new()
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                    Url = WebConfig.Website + "/sitemap-news.xml",
                    Priority = 80,
                    ChangeFrequency = "Monthly",
                    LastModified = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    ParentID = 0,
                    Code = "sitemap-news"
                };
                sitemap.Add(news);
                //product
                var product = new SitemapJson
                {
                    ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                    Url = WebConfig.Website + "/sitemap-product.xml",
                    Priority = 80,
                    ChangeFrequency = "Monthly",
                    LastModified = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    ParentID = 0,
                    Code = "sitemap-product"
                };
                sitemap.Add(product);
                #region All category
                var allCategoryParent = _websiteModuleDa.GetCategory();
                if (allCategoryParent != null)
                {
                    foreach (WebsiteModulesJson module in allCategoryParent)
                    {
                        sitemap.Add(new SitemapJson
                        {
                            ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                            Url = WebConfig.Website + "/" + module.NameAscii,
                            Priority = 80,
                            ChangeFrequency = "Monthly",
                            LastModified = DateTime.Now,
                            CreatedDate = DateTime.Now,
                            ParentID = cateogory.ID,
                            Code = module.NameAscii
                        });
                    }
                }
                #endregion
                #region All brand
                // var allBrand = _websiteModuleDa.GetTradeMarkSitemap();
                // if (allBrand != null)
                // {
                //     foreach (var item in allBrand)
                //     {
                //         sitemap.Add(new SitemapJson
                //         {
                //             ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                //             Url = WebConfig.Website + "/" + item.NameAscii,
                //             Priority = 80,
                //             ChangeFrequency = "Monthly",
                //             LastModified = DateTime.Now,
                //             CreatedDate = DateTime.Now,
                //             ParentID = brand.ID,
                //             Code = item.NameAscii
                //         }
                //         );
                //     }
                // }
                #endregion
                #region All product
                var products = _productDa.GetAllSitemap();
                var parentP = sitemap.FirstOrDefault(x => x.Code == "sitemap-product");
                foreach (var item in products)
                {
                    var itemSitemap = new SitemapJson
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/" + item._NameAscii,
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = parentP.ID,
                        Code = item._NameAscii,
                        ModuleIds = item.ModuleIds
                    };
                    sitemap.Add(itemSitemap);
                }
                #endregion
                #region All News
                List<WebsiteContentAdmin> newsall = _websiteContentDa.GetAllSitemap();
                SitemapJson parentN = sitemap.FirstOrDefault(x => x.Code == "sitemap-news");
                foreach (WebsiteContentAdmin item in newsall)
                {
                    SitemapJson itemSitemap = new()
                    {
                        ID = ConvertUtil.ToInt32(DateTime.Now.ToString("ddHHmmss")) + Utility.RandomNumber(1, 10000),
                        Url = WebConfig.Website + "/" + item.ModuleNameAscii + "/" + item._NameAscii,
                        Priority = 80,
                        ChangeFrequency = "Monthly",
                        LastModified = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ParentID = parentN.ID,
                        Code = item._NameAscii,
                        ModuleIds = item.ModuleIds
                    };
                    sitemap.Add(itemSitemap);
                }
                #endregion
                Common.CreateFileJson(0, sitemap, "Sitemap", "DataJson");
                msg = new JsonMessage { Errors = false, Message = "Reset sitemap thành công" };
                return Ok(msg);
            }
            catch(Exception e)
            {
                msg = new JsonMessage { Errors = true, Message = e.Message };
                return Ok(msg);
            }
        }
        private async Task<int> UpdateSitemapByCode(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    string sql = "update WebsiteModule set IsSitemap = 1 where NameAscii = '" + code + "' and IsDeleted = 0 and IsShow = 1 and LinkUrl is null" +
                    " update WebsiteContent set IsSitemap = 1 where NameAscii = '" + code + "' and ModuleNameAscii is not null and IsDeleted = 0 and IsShow = 1 and LinkUrl is null" +
                    " update Product set IsSitemap = 1 where NameAscii = '" + code + "' and ModuleNameAscii is not null and IsDeleted = 0 and IsShow = 1 and LinkUrl is null";
                    await _dapperDa.ExecuteSqlAsync(sql);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        private async Task<int> RemoveSitemapByCode(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    string sql = "update WebsiteModule set IsSitemap = 0 where NameAscii = '" + code + "' and IsDeleted = 0 and IsShow = 1 and LinkUrl is null" +
                    " update WebsiteContent set IsSitemap = 0 where NameAscii = '" + code + "' and ModuleNameAscii is not null and IsDeleted = 0 and IsShow = 1 and LinkUrl is null" +
                    " update Product set IsSitemap = 0 where NameAscii = '" + code + "' and ModuleNameAscii is not null and IsDeleted = 0 and IsShow = 1 and LinkUrl is null";
                    await _dapperDa.ExecuteSqlAsync(sql);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
