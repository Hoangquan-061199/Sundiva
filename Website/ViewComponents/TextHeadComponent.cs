using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Website.Utils;

namespace Website.ViewComponents
{
    public class TextHeadComponent : BaseComponent
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        public TextHeadComponent(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
        }
        public IViewComponentResult Invoke() => View(cacheUtils.SystemConfigItem("vi"));
    }
}
