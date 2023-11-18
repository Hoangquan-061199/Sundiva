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

namespace Website.ViewComponents
{                       
    public class FormQuestionComponent : BaseComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}