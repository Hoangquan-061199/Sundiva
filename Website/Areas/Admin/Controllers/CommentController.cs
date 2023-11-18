using System;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class CommentController : BaseController
    {
        private readonly CommentAdminDa _commentAdminDa;
        private readonly WebsiteContentDa _websiteContentDa;
        private readonly ProductDa _productDa;
        private readonly MembershipDa _membershipDa;
        private readonly ModuleAdminDa _moduleAdminDa;
        public CommentController()
        {
            _commentAdminDa = new CommentAdminDa(WebConfig.ConnectionString);
            _websiteContentDa = new WebsiteContentDa(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
        }
        public IActionResult Index()
        {
            if (!SystemActionAdmin.View)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("CustomerCategory");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            CommentViewmodel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module,
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = search,
                Comment = new Comment
                {
                    ProductID = search.productId,
                    ContentID = search.contentId
                }
            };
            if (search.contentId.HasValue)
            {
                model.WebsiteContent = _websiteContentDa.GetId(search.contentId.Value);
            }
            if (search.productId.HasValue)
            {
                model.Product = _productDa.GetId(search.productId.Value);
            }
            return View(model);
        }
        public IActionResult IndexReply()
        {
            SearchModel search = new();
            TryUpdateModelAsync(search);
            search.keyword = Utility.ValidString(search.keyword, "", true);
            Comment comment = _commentAdminDa.GetId(search.parentId.Value);
            CommentViewmodel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                SearchModel = search,
                Comment = comment
            };
            return View(model);
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            List<CommentAdmin> list = _commentAdminDa.ListSearch(seach, seach.page, 20);
            list.ForEach(x =>
            {
                if (x.ProductID.HasValue)
                {
                    x.Replies = _commentAdminDa.GetRepliesByID(x.ID, x.ProductID.Value);

                    x.Replies.ForEach(y =>
                    {
                        if(y.AdminId != Guid.Empty)
                            y.membershipAdmin = _membershipDa.GetAdminId(y.AdminId);
                    });
                }
                else if (x.ContentID.HasValue)
                {
                    x.Replies = _commentAdminDa.GetRepliesByContentID(x.ID, x.ContentID.Value);
                    x.Replies.ForEach(y =>
                    {
                        if (y.AdminId != Guid.Empty)
                            y.membershipAdmin = _membershipDa.GetAdminId(y.AdminId);
                    });
                }
                else
                {
                    x.Replies = new List<CommentAdmin>();
                    x.membershipAdmin = new MembershipAdmin();
                }
            });
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            Guid aGuid = new(userId);
            CommentViewmodel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Member = _membershipDa.GetAdminId(aGuid),
                ListItem = list,
                SearchModel = seach
            };
            int total = model.ListItem.Any() ? model.ListItem.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 20);
            return View(model);
        }
        public ActionResult AjaxForm()
        {
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            ActionViewModel action = UpdateModelAction();
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            CommentViewmodel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                Comment = new Comment
                {
                    ProductID = seach.productId,
                    ContentID = seach.contentId
                }
            };
            if (!string.IsNullOrEmpty(action.ItemId))
            {
                model.Comment.ParentID = ConvertUtil.ToInt32(action.ItemId);
            }
            if (action.Do == ActionType.Edit)
            {
                model.Comment = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        public IActionResult MoreNotification()
        {
            SearchModel seach = new() { pagesize = 15 };
            TryUpdateModelAsync(seach);
            HomeAdminViewModel model = new()
            {
                CommentAdmins = _commentAdminDa.GetAllCommentNotIsApproved(seach),
                CurrentLanguage = Lang(),
                SearchModel = seach
            };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Không có hành động nào được thực hiện."
            };
            Comment obj = new();
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        #region Valid Input
                        obj.Email = Utility.ValidString(obj.Email, Link, true);
                        obj.Title = Utility.ValidString(obj.Title, "", true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        #endregion
                        MembershipAdmin member = _membershipDa.GetAdminId(ConvertUtil.ToGuid(session.GetAdminUserId()));
                        obj.CreatedDate = DateTime.Now;
                        obj.IsApproved = true;
                        obj.IsDeleted = false;
                        if (string.IsNullOrEmpty(obj.Act))
                        {
                            obj.Act = "Comment";
                        }
                        obj.AdminId = ConvertUtil.ToGuid(session.GetAdminUserId());
                        obj.IsShow = true;
                        obj.Fullname = Utility.ValidString(member.FullName, "", true);
                        if (obj.ParentID != null)
                        {
                            Comment comment = _commentAdminDa.GetId(obj.ParentID.Value);
                            obj.ProductID = comment.ProductID;
                            obj.ContentID = comment.ContentID;
                            obj.Act = comment.Act;
                        }
                        int result = _commentAdminDa.Insert(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Thêm mới thành công."
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
                        if (SystemActionAdmin.Edit != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        AddLogEdit(Request.Path, "Edit", obj.ID.ToString(), obj);
                        if (obj == null)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Dữ liệu không tồn tại."
                            };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        #region Valid Input
                        obj.Email = Utility.ValidString(obj.Email, Link, true);
                        obj.Title = Utility.ValidString(obj.Title, "", true);
                        obj.UrlPicture = Utility.ValidString(obj.UrlPicture, Link, true);
                        obj.Fullname = Utility.ValidString(obj.Fullname, "", true);
                        obj.Act = Utility.ValidString(obj.Act, Code, true);
                        #endregion
                        int result = _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Cập nhật bình luận của:" + obj.Fullname, "Actions-Edit");
                        if (result > 0)
                        {
                            msg = new JsonMessage
                            {
                                Errors = false,
                                Message = "Cập nhật bình luận."
                            };
                            return Ok(msg);
                        }

                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Display:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = true;
                        int result = _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Hiển thị bình luận:" + obj.Fullname, "Actions-Display");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Hiện thị bình luận."
                        };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Hidden:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsShow = obj.IsShow == true ? false : true;
                        string message = ConvertUtil.ToBool(obj.IsShow) ? "Hiển thị bình luận" : "Ẩn bình luận";
                        int result = _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Ẩn bình luận:" + obj.Fullname, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    try
                    {
                        if (SystemActionAdmin.Delete != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(Convert.ToInt32(action.ItemId));
                        obj.IsDeleted = true;
                        _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Xóa bình luận:" + action.ItemId, "Actions-Xóa");
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
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            Comment content = _commentAdminDa.GetId(item);
                            content.IsShow = true;
                            _commentAdminDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Hiện thị bình luận:" + string.Join(",", ArrID), "Actions-ShowAll");
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
                        if (SystemActionAdmin.Active != true)
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
                            Comment content = _commentAdminDa.GetId(item);
                            content.IsShow = false;
                            _commentAdminDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Ẩn quản lý nội dung:" + string.Join(",", ArrID), "Actions-HiddenAll");
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
                        if (SystemActionAdmin.Delete != true)
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
                            Comment content = _commentAdminDa.GetId(item);
                            content.IsDeleted = true;
                            _commentAdminDa.Update(content);
                        }
                        AddLogAdmin(Request.Path, "Xóa bình luận:" + string.Join(",", ArrID), "Actions-DeleteAll");
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
                case ActionType.Approved:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Bạn chưa được phân quyền cho chức năng này."
                            };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsApproved = true;
                        int result = _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Duyệt bình luận:" + obj.Fullname, "Actions-Display");
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Duyệt thành công."
                        };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotApproved:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _commentAdminDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.IsApproved = obj.IsApproved == true ? false : true;
                        var message = ConvertUtil.ToBool(obj.IsApproved) ? "Duyệt thành công" : "Bỏ duyệt thành công";
                        int result = _commentAdminDa.Update(obj);
                        AddLogAdmin(Request.Path, "Bỏ duyệt bình luận:" + obj.Fullname, "Actions-Ẩn");
                        msg = new JsonMessage { Errors = false, Message = message };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.ApprovedAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            Comment content = _commentAdminDa.GetId(item);
                            content.IsApproved = true;
                            _commentAdminDa.Update(content);
                        }
                        msg = new JsonMessage { Errors = false, Message = "Duyệt thành công." };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.NotApprovedAll:
                    try
                    {
                        if (SystemActionAdmin.Active != true)
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
                            Comment content = _commentAdminDa.GetId(item);
                            content.IsApproved = false;
                            _commentAdminDa.Update(content);
                        }
                        msg = new JsonMessage
                        {
                            Errors = false,
                            Message = "Bỏ duyệt thành công."
                        };
                        return Ok(msg);
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
            }
            return Ok(msg);
        }
    }
}