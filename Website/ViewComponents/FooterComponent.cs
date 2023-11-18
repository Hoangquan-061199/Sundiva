using ADCOnline.DA.Dapper;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.Simple.Item;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Utils;
using Website.ViewModels;
using ADCOnline.Business.Implementation.ClientManager;
using Microsoft.AspNetCore.Http;

namespace Website.ViewComponents
{
    public class FooterViewComponent : BaseComponent
    {
        private readonly CacheUtils cacheUtils;
        private readonly DapperDA _dapperDa;
        private readonly WebsiteModuleManager _websiteModuleManager;
        private readonly WebsiteContentManager _websiteContentManager;
        private readonly OtherContentManager _otherContentManager;
        public FooterViewComponent(IDistributedCache distributedCache)
        {
            cacheUtils = new CacheUtils(distributedCache);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _websiteModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
            _websiteContentManager = new WebsiteContentManager(WebConfig.ConnectionString);
            _otherContentManager = new OtherContentManager(WebConfig.ConnectionString);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string view = StaticEnum.Footer;
            List<ModulePositionItem> listPosition = new();
            StringBuilder sqlWhere = new();
            StringBuilder sqlContent = new();
            List<string> listModuleByContent = new();
            IEnumerable<WebsiteContentItem> listContent = new List<WebsiteContentItem>();
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
                        sqlContent.Append(string.Format(SqlContent.SqlView, 1000, "," + x.Code + ",", Lang(), sqlWhere, !string.IsNullOrEmpty(x.SqlContentOrderBy) ? x.SqlContentOrderBy : " Order By CreatedDate Desc"));
                        string sql1 = string.Format(SqlCommon.SqlHome, sqlContent);
                        listContent = _dapperDa.Select<WebsiteContentItem>(sql1);
                        x.WebsiteContentItems = listContent?.ToList();
                    });
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            var payment = await _otherContentManager.GetByCodeLang("Payment", Lang());
            FooterViewModel model = new()
            {
                ListPositionItem = listPosition,
                SystemConfig = cacheUtils.SystemConfigItem(Lang()),
                Payment = payment
                //AreaJsons = cacheUtils.GetAreaIndex(Lang()),
            };
            //var viewer = Request.Cookies["viewer"];
            //if (viewer != null)
            //{
            //    if (viewer == "mobile")
            //    {
            //        return View(@"~/Views/Mobile/Footer.cshtml", model);
            //    }
            //    if (viewer == "destop")
            //    {
            //        return View(model);
            //    }
            //}
            //if (Utility.IsMobile(HttpContext.Request.Headers["User-Agent"]))
            //{
            //    return View(@"~/Views/Mobile/Footer.cshtml", model);
            //}
            return await Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}