using ADCOnline.Business.Implementation.AdminManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using ADCOnline.Simple.Base;
using System.Collections.Generic;
using Website.Models;
using ADCOnline.Utils;
using System;
using ADCOnline.Simple.Admin;
using Website.Areas.Admin.ViewModels;
using System.Text;
using ADCOnline.Simple;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class SubItemController : BaseController
    {
        private readonly SubItemDa _modulePositionDa;
        private readonly ProductDa _productDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly WebsiteModuleDa _websiteModuleDa;
        public SubItemController()
        {
            _modulePositionDa = new SubItemDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _websiteModuleDa = new WebsiteModuleDa(WebConfig.ConnectionString);
        }
        public IActionResult Index(int? productId, int? contentId, int? moduleId, string type)
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            SubItemViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Type = !string.IsNullOrEmpty(type) ? type : "Content"
            };
            switch (type)
            {
                case "Product":
                    {
                        model.Product = _productDa.GetId(productId ?? 0) ?? new Product();
                        model.Id = productId ?? 0;
                        model.WebsiteContent = new WebsiteContent();
                        model.WebsiteModule = new WebsiteModule();
                        break;
                    }
                case "Module":
                    {
                        model.WebsiteModule = _websiteModuleDa.GetId(moduleId ?? 0) ?? new WebsiteModule();
                        model.Id = moduleId ?? 0;
                        model.WebsiteContent = new WebsiteContent();
                        model.Product = new Product();
                        break;
                    }
                default:
                    {
                        model.WebsiteContent = _websiteContentDa.GetId(contentId ?? 0) ?? new WebsiteContent();
                        model.Id = contentId ?? 0;
                        model.WebsiteModule = new WebsiteModule();
                        model.Product = new Product();
                        break;
                    }
            }
            return View(model);
        }
        public IActionResult ListItems(int? productId, int? contentId, int? moduleId, string type)
        {
            SearchModel seach = new()
            {
                type = type
            };
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            SubItemViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListItem = _modulePositionDa.ListSearch(seach, productId ?? 0, contentId ?? 0, moduleId ?? 0, seach.page, 50, false)
            };
            switch (type)
            {
                case "Product":
                    {
                        model.Product = _productDa.GetId(productId ?? 0) ?? new Product();
                        break;
                    }
                case "Module":
                    {
                        model.WebsiteModule = _websiteModuleDa.GetId(moduleId ?? 0) ?? new WebsiteModule();
                        break;
                    }
                default:
                    {
                        model.WebsiteContent = _websiteContentDa.GetId(contentId ?? 0) ?? new WebsiteContent();
                        break;
                    }
            }
            return View(model);
        }

        public ActionResult AjaxForm(int? productId, int? contentId, int? moduleId, string type)
        {
            ActionViewModel action = UpdateModelAction();
            SubItemViewModel model = new()
            {
                SubItem = new SubItem
                {
                    ModuleTypeCode = type
                },
                SystemActionAdmin = SystemActionAdmin,
                ListBaseItem = _modulePositionDa.ListAll(),
                Type = type
            };
            switch (type)
            {
                case "Product":
                    {
                        model.Product = _productDa.GetId(productId ?? 0) ?? new Product();
                        model.WebsiteModule = new WebsiteModule();
                        model.WebsiteContent = new WebsiteContent();
                        break;
                    }
                case "Module":
                    {
                        model.WebsiteModule = _websiteModuleDa.GetId(moduleId ?? 0) ?? new WebsiteModule();
                        model.WebsiteContent = new WebsiteContent();
                        model.Product = new Product();
                        break;
                    }
                default:
                    {
                        model.WebsiteContent = _websiteContentDa.GetId(contentId ?? 0) ?? new WebsiteContent();
                        model.WebsiteModule = new WebsiteModule();
                        model.Product = new Product();
                        break;
                    }
            }
            if (action.Do == ActionType.Edit)
            {
                SubItem modulePosition = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                if (modulePosition != null)
                {
                    model.SubItem = modulePosition;
                    model.AlbumImageAdmins = !string.IsNullOrEmpty(model.SubItem.AlbumImageJson) ? JsonConvert.DeserializeObject<List<AlbumImageAdmin>>(model.SubItem.AlbumImageJson) : new List<AlbumImageAdmin>();
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }

        public ActionResult QuickAdd(int? productId, int? idSub)
        {
            ActionViewModel action = UpdateModelAction();
            SubItemViewModel model = new()
            {
                SubItem = new SubItem(),
                SystemActionAdmin = SystemActionAdmin,
                ListBaseItem = _modulePositionDa.ListAll(),
            };

            model.Product = _productDa.GetId(productId ?? 0) ?? new Product();
            model.WebsiteModule = new WebsiteModule();
            model.WebsiteContent = new WebsiteContent();
            if (idSub > 0)
                action.Do = ActionType.Edit;
            if (action.Do == ActionType.Edit)
            {
                SubItem modulePosition = _modulePositionDa.GetId(ConvertUtil.ToInt32(idSub));
                if (modulePosition != null)
                {
                    model.SubItem = modulePosition;
                    model.AlbumImageAdmins = !string.IsNullOrEmpty(model.SubItem.AlbumImageJson) ? JsonConvert.DeserializeObject<List<AlbumImageAdmin>>(model.SubItem.AlbumImageJson) : new List<AlbumImageAdmin>();
                }
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            try
            {
                ActionViewModel action = UpdateModelAction();
                List<int> idValues = new();
                SubItem obj = new();
                List<AlbumImageAdmin> imageGalleryItemList = new();
                AlbumImageAdmin imageGalleryItem = new();
                string image = string.Empty;
                switch (action.Do)
                {
                    case ActionType.Add:
                        try
                        {
                            if (!SystemActionAdmin.Add)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Bạn chưa được phân quyền cho chức năng này."
                                };
                                return Ok(msg);
                            }
                            await TryUpdateModelAsync(obj);
                            #region Valid input
                            obj.Name = Utility.ValidString(obj.Name, Title, true);
                            var urlPicture = Request.Form["UrlPictureSub"];
                            var content = Request.Form["ContentSub"];
                            obj.UrlPicture = Utility.ValidString(urlPicture, Link, true);
                            obj.Content = content;
                            obj.ProductID = obj.ProductID;
                            #endregion
                            obj.IsDeleted = false;
                            obj.IsShow = true;
                            obj.CreatedDate = DateTime.Now;
                            obj.Lang = Lang();
                            #region load image
                            imageGalleryItemList = UpdateModelLst(imageGalleryItem, imageGalleryItemList);
                            if (imageGalleryItemList != null && imageGalleryItemList.Count > 0)
                            {
                                imageGalleryItemList = imageGalleryItemList.Where(x => !string.IsNullOrEmpty(x.ImageSize)).OrderBy(c => c.ImageOrder).ToList();
                                image = JsonConvert.SerializeObject(imageGalleryItemList);
                            }
                            else
                            {
                                image = null;
                            }
                            obj.AlbumImageJson = Utility.RemoveHTML(image);
                            #endregion
                            int result = _modulePositionDa.Insert(obj);
                            if (result > 0)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = false,
                                    Message = "Thêm mới thành công.",
                                    Action = action.Do,
                                    Id = obj.ID.ToString(),
                                    Name = obj.Name,
                                    ProductId = obj.ProductID.ToString()
                                };
                                return Ok(msg);
                            }

                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.Edit:
                        try
                        {
                            if (!SystemActionAdmin.Edit)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Bạn chưa được phân quyền cho chức năng này."
                                };
                                return Ok(msg);
                            }
                            obj = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            SubItem objOld = obj;
                            await TryUpdateModelAsync(obj);
                            #region Valid input
                            obj.Name = Utility.ValidString(obj.Name, Title, true);
                            var urlPicture = Request.Form["UrlPictureSub"];
                            var content = Request.Form["ContentSub"];
                            obj.UrlPicture = Utility.ValidString(urlPicture, Link, true);
                            obj.Content = content;
                            obj.ProductID = obj.ProductID;
                            #endregion
                            #region load image
                            imageGalleryItemList = UpdateModelLst(imageGalleryItem, imageGalleryItemList);
                            if (imageGalleryItemList != null && imageGalleryItemList.Count > 0)
                            {
                                imageGalleryItemList = imageGalleryItemList.OrderBy(c => c.ImageOrder).ToList();
                                image = JsonConvert.SerializeObject(imageGalleryItemList);
                            }
                            else
                            {
                                image = null;
                            }
                            obj.AlbumImageJson = Utility.RemoveHTML(image);
                            #endregion
                            obj.IsDeleted = false;
                            int result = _modulePositionDa.Update(obj);
                            if (result > 0)
                            {
                                msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Action = action.Do, Id = obj.ID.ToString(), Name = obj.Name, ProductId = obj.ProductID.ToString() };
                            }
                            return Ok(msg);
                        }
                        catch { }
                        break;
                    case ActionType.Delete:
                        if (!SystemActionAdmin.Delete)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        try
                        {
                            obj = _modulePositionDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                            obj.IsDeleted = true;
                            _modulePositionDa.Update(obj);
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Xóa thành công."
                            };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.ShowAll:
                        try
                        {
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                obj = _modulePositionDa.GetId(item);
                                obj.IsShow = true;
                                _modulePositionDa.Update(obj);
                            }
                            msg = new JsonMessage { Errors = false, Message = "Hiện thị thành công." };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.HiddenAll:
                        try
                        {
                            if (!SystemActionAdmin.Active)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Bạn chưa được phân quyền cho chức năng này."
                                };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                obj = _modulePositionDa.GetId(item);
                                obj.IsShow = false;
                                _modulePositionDa.Update(obj);
                            }
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Ẩn thành công."
                            };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.DeleteAll:
                        try
                        {
                            if (!SystemActionAdmin.Delete)
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Bạn chưa được phân quyền cho chức năng này."
                                };
                                return Ok(msg);
                            }
                            foreach (int item in ArrID)
                            {
                                obj = _modulePositionDa.GetId(item);
                                obj.IsDeleted = true;
                                _modulePositionDa.Update(obj);
                            }
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Xóa thành công."
                            };
                            return Ok(msg);
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                    case ActionType.OrderBy:
                        try
                        {
                            if (!SystemActionAdmin.Order)
                            {
                                msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                                return Ok(msg);
                            }
                            if (!string.IsNullOrEmpty(Request.Form["OrderByValues"]))
                            {
                                var orderValues = Request.Form["OrderByValues"];
                                List<OrderByItem> listOrderByItem = JsonConvert.DeserializeObject<List<OrderByItem>>(orderValues);
                                foreach (OrderByItem item in listOrderByItem)
                                {
                                    try
                                    {
                                        SubItem content = _modulePositionDa.GetId(item.ID);
                                        content.OrderDisplay = item.OrderDisplay;
                                        _modulePositionDa.Update(content);
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogError(ex);
                                    }
                                }
                                msg = new JsonMessage
                                {
                                    Errors = false,
                                    Message = "Cập nhật thứ tự thành công."
                                };
                                return Ok(msg);
                            }
                        }
                        catch (Exception ex)
                        {
                            AddLogError(ex);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                AddLogError(ex);
            }

            return Ok(msg);
        }
    }
}
