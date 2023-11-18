using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Utils;
using Website.ViewModels;

namespace Website.ViewComponents
{
    public class HeadTopViewComponent : BaseComponent
    {
        private readonly CacheUtils cacheUtils;
        private readonly DapperDA _dapperDa;
        private readonly LanguageManager _languageManager;

        public HeadTopViewComponent(IDistributedCache distributedCache)
        {
            cacheUtils = new CacheUtils(distributedCache);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _languageManager = new LanguageManager(WebConfig.ConnectionString);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string view = StaticEnum.HeadTop;
            StringBuilder sqlWhere = new();
            StringBuilder sqlContent = new();
            List<string> listModuleByContent = new();
            IEnumerable<ProductItem> listProduct = new List<ProductItem>();
            IEnumerable<WebsiteContentItem> listContent = new List<WebsiteContentItem>();
            List<ModulePositionItem> listPosition = new();
            try
            {
                listPosition = cacheUtils.GetListPositionViewIndex(view);
                if (listPosition.Any())
                {
                    listPosition.ForEach(x =>
                    {
                        x.AdvertisingItems = cacheUtils.GetListAdvertisingItemByCode(x.Code, Lang());
                        x.WebsiteModulesItems = cacheUtils.GetListModuleInPositionCode(x.Code, Lang());
                        sqlWhere.Clear();
                        sqlContent.Clear();
                        listModuleByContent = x.WebsiteModulesItems.Where(x => (x.ModuleTypeCode == StaticEnum.Partner || x.ModuleTypeCode == StaticEnum.Services)).Select(c => c.ID.ToString()).ToList();
                        if (listModuleByContent.Count > 0)
                        {
                            sqlWhere.Append(SqlUtility.WhereOrLikeList(listModuleByContent, "ModuleIds"));
                        }
                        else
                        {
                            sqlWhere.Append(SqlUtility.AND("ModuleIds", 0, 1));
                        }
                        sqlWhere.Append(x.SqlContent);
                        sqlContent.Append(string.Format(SqlContent.SqlView, 1000, "," + x.Code + ",", Lang(), sqlWhere, !string.IsNullOrEmpty(x.SqlContentOrderBy) ? x.SqlContentOrderBy : " Order By CreatedDate Desc"));
                        string sql1 = string.Format(SqlCommon.SqlHome, sqlContent);
                        listContent = _dapperDa.Select<WebsiteContentItem>(sql1);
                        x.WebsiteContentItems = listContent?.ToList();
                        if (x.TypeView == StaticEnum.Product)
                        {
                            sqlWhere.Clear();
                            sqlContent.Clear();
                            listModuleByContent = x.WebsiteModulesItems.Select(c => c.ID.ToString()).ToList();
                            if (listModuleByContent.Count > 0)
                            {
                                sqlWhere.Append(SqlUtility.WhereOrLikeList(listModuleByContent, "ModuleIds"));
                            }
                            else
                            {
                                sqlWhere.Append(SqlUtility.AND("ModuleIds", 0, 1));
                            }
                            sqlWhere.Append(x.SqlContent);
                            sqlContent.Append(string.Format(SqlProduct.SqlView, x.NumberContent ?? 20, "," + x.Code + ",", Lang(), sqlWhere, " Order By OrderDisplay Asc"));
                            string sql2 = string.Format(SqlCommon.SqlHome, sqlContent);
                            listProduct = _dapperDa.Select<ProductItem>(sql2);
                            x.ProductItems = listProduct?.ToList();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            ViewBag.UserId = GetIdMember();
            ViewBag.FullName = GetFullNameMember();
            List<CommonJsonItem> CustomerOrder = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("CustomerReview.json", "DataJson"));
            IndexViewModel model = new()
            {
                ListPositionItem = listPosition,
                SystemConfigItem = cacheUtils.SystemConfigItem(Lang()),
                LanguageItems = _languageManager.GetListAll(),
                CustomerOrder = CustomerOrder,
                ModuleFormPopup = cacheUtils.GetByModuleTypeCodeSimple(StaticEnum.FreeInformationPack, Lang())
            };
            //var viewer = Request.Cookies["viewer"];
            //if (viewer != null)
            //{
            //    AddLogError(viewer);
            //    if (viewer == "mobile")
            //    {
            //        return View(@"~/Views/Mobile/HeadTop.cshtml", model);
            //    }
            //    if (viewer == "destop")
            //    {
            //        return View(model);
            //    }
            //}
            //if (Utility.IsMobile(HttpContext.Request.Headers["User-Agent"]))
            //{
            //    return await Task.FromResult<IViewComponentResult>(View(@"~/Views/Mobile/HeadTop.cshtml", model));
            //}
            return await Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}