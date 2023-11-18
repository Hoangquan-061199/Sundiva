using ADCOnline.Business.Implementation.ClientManager;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Item;

namespace Website.ViewComponents
{
    public class OtherComponent : BaseComponent
    {
        private readonly OtherContentManager _otherContentManager;
        public OtherComponent()
        {
            _otherContentManager = new OtherContentManager(WebConfig.ConnectionString);
        }
        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync(string code)
        {
            OtherContentItem other = await _otherContentManager.GetByCodeLang(code,Lang());
            ViewBag.Code = code;
            return View(other);
        }
    }
}