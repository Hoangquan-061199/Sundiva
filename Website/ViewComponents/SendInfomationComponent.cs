using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Website.Utils;
using Website.ViewModels;

namespace Website.ViewComponents
{
    public class SendInfomationViewComponent : BaseComponent
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        public SendInfomationViewComponent(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
        }

        public IViewComponentResult Invoke()
        {
          
            var model = new ModuleViewModels
            {
                SystemConfig = cacheUtils.GetSystemItemConfigJson(Lang())
            };
            return View(model);
        }
      
    }
}
