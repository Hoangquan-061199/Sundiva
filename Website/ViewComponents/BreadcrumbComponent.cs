using ADCOnline.Business.Implementation.ClientManager;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;

namespace Website.ViewComponents
{
    public class BreadCrumbComponent : BaseComponent
    {
        private readonly WebsiteModuleManager _webModuleManager;
        public BreadCrumbComponent()
        {
            _webModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
        }
        public IViewComponentResult Invoke(int? moduleId) => View(_webModuleManager.GetById(moduleId ?? 0));
    }
}