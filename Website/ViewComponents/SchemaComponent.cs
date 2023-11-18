using ADCOnline.Business.Implementation.ClientManager;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Item;

namespace Website.ViewComponents
{
    public class SchemaComponent : BaseComponent
    {
        private readonly OtherContentManager _otherContentManager;
        public SchemaComponent()
        {
            _otherContentManager = new OtherContentManager(WebConfig.ConnectionString);
        }
        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync(bool? isSchema,string code, string logo, string title, string description, string url, string More)
        {
            OtherContentItem scheme = await _otherContentManager.GetByCodeLang(!string.IsNullOrEmpty(code) ? code : "Schema", Lang());
            if (scheme != null && isSchema == true)
            {
                scheme.Content = scheme.Content.Replace("{URL}", url).Replace("{IMG}", logo).Replace("{TITLE}", title).Replace("{DESCRIPTION}", description).Replace("{More}", More);
            }
            else
            {
                scheme = new OtherContentItem();
            }
            return View(scheme);
        }
    }
}
