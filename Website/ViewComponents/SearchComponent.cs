using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Utils;
using Website.ViewModels;

namespace Website.ViewComponents
{
    public class SearchComponent : BaseComponent
    {
        private readonly DapperDA _dapperDa;
        private readonly WebsiteModuleManager _websiteModuleManager;

        public SearchComponent(IDistributedCache distributedCache)
        {
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _websiteModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CommonJsonItem> listItem = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TimeTour.json", "DataJson"));
            List<CommonJsonItem> listItem2 = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("AddressStart.json", "DataJson"));
            var listModule = await _websiteModuleManager.GetAllModuleByCodeAndTypeView(StaticEnum.Product, StaticEnum.TypeProduct, Lang());

            SearchViewModel model = new()
            {
                ListTimes = listItem != null ? listItem : new List<CommonJsonItem>(),
                ListAddressStart = listItem2 != null ? listItem2 : new List<CommonJsonItem>(),
                ListModulesItem = listModule
            };

            return View(model);
        }
    }
}
