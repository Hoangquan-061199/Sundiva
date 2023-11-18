using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Website.Utils;

namespace Website.ViewComponents
{
    public class AdvComponent :BaseComponent
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        public AdvComponent(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
        }
        public async Task<IViewComponentResult> InvokeAsync(string code)
        {
            ViewBag.CodeAdv = code;
            return await Task.FromResult<IViewComponentResult>(View(await cacheUtils.GetListAdvertisingItemByCodeAsync(code, Lang())));
        }
    }
}
