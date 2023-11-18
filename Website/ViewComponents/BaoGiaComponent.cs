using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple;
using ADCOnline.Simple.Item;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Website.Utils;
using Website.ViewModels;
using Website.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Website.ViewComponents
{
    public class BaoGiaComponent : BaseComponent
    {
        private readonly ProductManager _productManager;
        private readonly WebsiteContentManager _websiteContentManager;
        public BaoGiaComponent(IDistributedCache distributedCache)
        {
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _websiteContentManager = new WebsiteContentManager(WebConfig.ConnectionString);
        }
        public IViewComponentResult Invoke(int id, string code)
        {
            ModuleViewModels model = new() { };
            if(code == StaticEnum.Product)
                model.ProductItem = _productManager.GetId(id);
            else
                model.WebsiteContentItem = _websiteContentManager.GetContentById(id);

            ViewBag.ModuleTypeCode = code;
            return View(model);
        }
    }
}