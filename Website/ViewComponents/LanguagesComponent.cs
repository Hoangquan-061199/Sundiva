using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Website.Utils;
using Website.ViewModels;

namespace Website.ViewComponents
{
    public class LanguagesComponent : BaseComponent
    {
        private readonly LanguageManager _languageManager;
        private readonly DapperDA _dapperDa;
        public LanguagesComponent(IDistributedCache distributedCache)
        {
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _languageManager = new LanguageManager(WebConfig.ConnectionString);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            IndexViewModel model = new()
            {
                LanguageItems = _languageManager.GetListAll()
            };
            return await Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
