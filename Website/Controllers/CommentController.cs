using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Utils;
using Website.ViewModels;

namespace Website.Controllers
{
    public class CommentController : BaseController
    {
        private CommentAdminDa _commentAdminDa;
        private readonly CommentManager _commentManager;
        private readonly ProductDa _productDa;
        public CommentController()
        {
            _commentAdminDa = new CommentAdminDa(WebConfig.ConnectionString);
            _commentManager = new CommentManager(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
        }
        private const int Pagesize = 5;
        #region Comment
        [HttpPost]
        public IActionResult CommentAction()
        {
            Comment obj = new();
            JsonMessage msg = new();
            try
            {
                TryUpdateModelAsync(obj);
                var fullname = Request.Form["Fullname"];
                var email = Request.Form["Email"];
                var content = Request.Form["Content"];
                var contentId = Request.Form["ProductID"];
                if (string.IsNullOrEmpty(contentId) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                obj.CreatedDate = DateTime.Now;
                obj.IsDeleted = false;
                obj.IsShow = true;
                obj.Act = "Comment";
                obj.IsApproved = false;
                obj.CustomerID = UserId;
               

                obj.Fullname = Utility.RemoveSpecialCharacter(fullname);
                obj.Email = Utility.RemoveValidUserName(email);
                obj.Content = Utility.RemoveValidUserName(content);
                var type = "Product";
                if (!string.IsNullOrEmpty(type) && type == "Product")
                {
                    obj.ProductID = ConvertUtil.ToInt32(contentId.ToString());
                    obj.ContentID = null;
                }
                #region Avatar
                var Avatar = Request.Form.Files["AvatarFile"];
                if (Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                {
                    string processPath = "";
                    var extention = Path.GetExtension(Avatar.FileName);
                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                    {
                        processPath = Url.Content("comment/") + Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(Avatar.FileName);
                        string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                        FileStream stream = new(filePath, FileMode.Create);
                        Avatar.CopyTo(stream);
                        stream.Close();
                        obj.UrlPicture = "/" + processPath;
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("FileError", Lang()) };
                        return Ok(msg);
                    }
                }
                #endregion
                var result = _commentAdminDa.Insert(obj);
                if (result > 0)
                {
                    msg.Errors = false;
                    msg.Data = obj;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("BinhLuanThatBai", Lang());
                }
            }
            catch
            {
                msg.Errors = true;
                msg.Message = ResourceData.Resource("BinhLuanThatBai", Lang());
            }
            return Ok(msg);
        }
        [HttpPost]
        [Route("/AjaxComment")]
        public async Task<IActionResult> AjaxCommentAsync()
        {
            JsonMessage msg = new();
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new();
                int pageSize = Pagesize;
                await TryUpdateModelAsync(search);
                search.customerId = GetWebUserID();
                IEnumerable<CommentItem> comments = await _commentManager.GetListCommentByPage(search, pageSize);
                await comments.ToAsyncEnumerable().ForEachAsync(async x =>
                {
                    x.Replies = await _commentManager.GetListReplyByPage(new SearchModel { productId = search.productId, parentId = x.ID, page = 1, pagesize = 1 }, 5);
                });
                int Total = comments.Any() ? comments.FirstOrDefault().TotalRecord : 0;
                CommentViewModel model = new()
                {
                    GridHtml = Common.GetAjaxPage(search.page, Total, pageSize),
                    CommentItems = comments,
                    Page = search.page,
                    PageSize = pageSize,
                    Total = Total,
                    productId = search.productId,
                };
                ViewBag.GridPageHtml = Common.GetAjaxPage(search.page, Total, 5);
                return View(@"~/Views/PartialContent/AjaxComment.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/reply-comment")]
        public IActionResult ReplyAction()
        {
            Comment obj = new();
            JsonMessage msg = new();
            try
            {
                var productId = Request.Form["ProductID"];
                var parentId = Request.Form["ParentID"];
                var fullname = Request.Form["Fullname"];
                var email = Request.Form["Email"];
                var content = Request.Form["Content"];
                if (ConvertUtil.ToInt32(productId.ToString()) == 0 || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(content))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                obj.CreatedDate = DateTime.Now;
                obj.IsDeleted = false;
                obj.IsShow = true;
                obj.IsApproved = false;
                obj.Act = "Comment";
                obj.CustomerID = UserId;
                obj.Fullname = Utility.RemoveSpecialCharacter(fullname);
                obj.Email = email;
                obj.Content = content;
                obj.ParentID = ConvertUtil.ToInt32(parentId.ToString());
                var type = "Product";
                if (!string.IsNullOrEmpty(type) && type == "Product")
                {
                    obj.ProductID = ConvertUtil.ToInt32(productId.ToString());
                    obj.ContentID = null;
                }
                #region Avatar
                var Avatar = Request.Form.Files["AvatarFile"];
                if (Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                {
                    string processPath = "";
                    var extention = Path.GetExtension(Avatar.FileName);
                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                    {
                        processPath = Url.Content("comment/") + Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(Avatar.FileName);
                        string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                        FileStream stream = new(filePath, FileMode.Create);
                        Avatar.CopyTo(stream);
                        stream.Close();
                        obj.UrlPicture = "/" + processPath;
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("LoiFile", Lang()) };
                        return Ok(msg);
                    }
                }
                #endregion
                var result = _commentAdminDa.Insert(obj);
                if (result > 0)
                {
                    msg.Errors = false;
                    msg.Data = obj;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("BinhLuanThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Logs = e.Message;
                msg.Errors = true;
                msg.Message = ResourceData.Resource("BinhLuanThatBai", Lang());
            }
            return Ok(msg);
        }
        [HttpPost]
        public IActionResult AjaxReply()
        {
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new();
                TryUpdateModelAsync(search);
                search.customerId = GetWebUserID();
                var comments = _commentManager.GetListReplyBySkip(search, 5);
                var model = new CommentViewModel
                {
                    CommentItems = comments
                };
                return View(@"~/Views/PartialContent/AjaxReply.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult DeleteComment()
        {
            JsonMessage msg = new();
            SearchModel search = new();
            try
            {
                TryUpdateModelAsync(search);
                var cmt = _commentAdminDa.GetId(search.parentId);
                cmt.IsDeleted = true;
                var update = _commentAdminDa.Update(cmt);
                if (update > 0)
                {
                    msg.Errors = false;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("XoaThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Errors = true;
                msg.Message = ResourceData.Resource("XoaThatBai", Lang());
                msg.Logs = e.Message;
            }
            return Ok(msg);
        }
        #endregion
        #region Rate
        [HttpPost]
        public IActionResult RateAction()
        {
            Comment obj = new();
            JsonMessage msg = new();
            try
            {
                TryUpdateModelAsync(obj);
                var fullname = Request.Form["Fullname"];
                var email = Request.Form["Email"];
                var content = Request.Form["Content"];
                var phone = Request.Form["Phone"];
                var contentId = Request.Form["ContentID"];
                if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(phone))
                {
                    msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("VuiLongNhapCacTruongBatBuoc", Lang()) };
                    return Ok(msg);
                }
                obj.CreatedDate = DateTime.Now;
                obj.IsDeleted = false;
                obj.IsShow = true;
                obj.IsApproved = false;
                obj.CustomerID = UserId;
                obj.Fullname = Utility.RemoveSpecialCharacter(fullname);
                obj.Email = Utility.RemoveValidUserName(email);
                obj.Content = Utility.RemoveValidUserName(content);
                obj.Phone = phone;
                if (!string.IsNullOrEmpty(Request.Form["Star"]))
                {
                    obj.Rate = Convert.ToInt32(Request.Form["Star"]);
                    obj.Act = "Rate";
                }
                else
                {
                    obj.Act = "Comment";
                }
                var type = Request.Form["Type"];
                if (!string.IsNullOrEmpty(type) && type == "Product")
                {
                    obj.ProductID = Convert.ToInt32(contentId);
                    obj.ContentID = null;
                }
                var check = _commentAdminDa.GetRateByEmail(obj.ProductID.Value, obj.Email);
                if (check != null)
                {
                    msg.Errors = true;
                    msg.Message = "Bạn đã đánh giá rồi.";
                    return Ok(msg);
                }
                #region Avatar
                var Avatar = Request.Form.Files["AvatarFile"];
                if (Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                {
                    string processPath = "";
                    var extention = Path.GetExtension(Avatar.FileName);
                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                    {
                        processPath = Url.Content("comment/") + Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + Path.GetExtension(Avatar.FileName);
                        string filePath = Path.Combine(WebConfig.PathServer + "/" + processPath);
                        FileStream stream = new(filePath, FileMode.Create);
                        Avatar.CopyTo(stream);
                        stream.Close();
                        obj.UrlPicture = "/" + processPath;
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("FileError", Lang()) };
                        return Ok(msg);
                    }
                }
                #endregion
                var result = _commentAdminDa.Insert(obj);
                if (result > 0)
                {
                    msg.Errors = false;
                    msg.Data = obj;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
                }
            }
            catch(Exception e)
            {
                msg.Errors = true;
                msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
                msg.Logs = e.Message;
            }
            return Ok(msg);
        }
        [HttpPost]
        public async Task<IActionResult> AjaxRate()
        {
            JsonMessage msg = new();
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new();
                int pageSize = Pagesize;
                await TryUpdateModelAsync(search);
                search.customerId = GetWebUserID();
                IEnumerable<CommentItem> comments = await _commentManager.GetListRateByPage(search, pageSize);
                await comments.ToAsyncEnumerable().ForEachAsync(async x =>
                {
                    x.Replies = await _commentManager.GetListReplyRateByPage(new SearchModel { productId = search.productId, parentId = x.ID, page = 1, pagesize = 1 }, 3);
                });
                int Total = comments.Any() ? comments.FirstOrDefault().TotalRecord : 0;
                var model = new CommentViewModel
                {
                    GridHtml = Common.GetAjaxPage(search.page, Total, pageSize),
                    CommentItems = comments,
                    Page = search.page,
                    PageSize = pageSize,
                    Total = Total
                };
                return View(@"~/Views/PartialContent/AjaxRate.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult ReplyRateAction()
        {
            Comment obj = new();
            JsonMessage msg = new();
            try
            {
                TryUpdateModelAsync(obj);
                obj.CreatedDate = DateTime.Now;
                obj.IsDeleted = false;
                obj.IsShow = true;
                obj.IsApproved = false;
                obj.Act = "Rate";
                obj.CustomerID = UserId;
                obj.Fullname = Utility.RemoveSpecialCharacter(obj.Fullname);
                obj.Email = Utility.RemoveValidUserName(obj.Email);
                obj.Content = Utility.RemoveValidUserName(obj.Content);
                var type = Request.Form["Type"];
                if (!string.IsNullOrEmpty(type) && type == "Product")
                {
                    obj.ProductID = obj.ContentID;
                    obj.ContentID = null;
                }
                #region Avatar
                var Avatar = Request.Form.Files["AvatarFile"];
                if (Avatar != null && !string.IsNullOrEmpty(Avatar.ToString()))
                {
                    string processPath = "";
                    var extention = System.IO.Path.GetExtension(Avatar.FileName);
                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                    {
                        processPath = Url.Content("comment/") + System.IO.Path.GetFileNameWithoutExtension(Avatar.FileName) + "_" + Utility.RenDateFileName() + System.IO.Path.GetExtension(Avatar.FileName);
                        string filePath = System.IO.Path.Combine(WebConfig.PathServer + "/" + processPath);
                        FileStream stream = new FileStream(filePath, FileMode.Create);
                        Avatar.CopyTo(stream);
                        stream.Close();
                        obj.UrlPicture = "/" + processPath;
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = ResourceData.Resource("FileError", Lang()) };
                        return Ok(msg);
                    }
                }
                #endregion
                var result = _commentAdminDa.Insert(obj);
                if (result > 0)
                {
                    msg.Errors = false;
                    msg.Data = obj;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Logs = e.Message;
                msg.Errors = true;
                msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
            }
            return Ok(msg);
        }
        [HttpPost]
        public IActionResult AjaxReplyRate()
        {
            try
            {
                Response.Headers.Add("AMP-Access-Control-Allow-Source-Origin", Request.Scheme + "://" + Request.Host);
                Response.Headers.Add("Access-Control-Expose-Headers", "AMP-Redirect-To, AMP-Access-Control-Allow-Source-Origin");
                SearchModel search = new();
                TryUpdateModelAsync(search);
                search.customerId = GetWebUserID();
                var comments = _commentManager.GetListReplyRateBySkip(search, 5);
                var model = new CommentViewModel
                {
                    CommentItems = comments
                };
                return View(@"~/Views/PartialContent/AjaxReplyRate.cshtml", model);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult DeleteRate()
        {
            JsonMessage msg = new();
            SearchModel search = new();
            try
            {
                TryUpdateModelAsync(search);
                var cmt = _commentAdminDa.GetId(search.parentId);
                cmt.IsDeleted = true;
                var update = _commentAdminDa.Update(cmt);
                if (update > 0)
                {
                    msg.Errors = false;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("XoaThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Errors = true;
                msg.Logs = e.Message;
                msg.Message = ResourceData.Resource("XoaThatBai", Lang());
            }
            return Ok(msg);
        }
        #endregion
        [HttpPost]
        public async Task<IActionResult> Good()
        {
            JsonMessage msg = new();
            SearchModel search = new();
            try
            {
                await TryUpdateModelAsync(search);
                var cmt = _commentAdminDa.GetId(search.parentId);
                cmt.Good = cmt.Good.HasValue ? (cmt.Good + 1) : 1;
                var update = _commentAdminDa.Update(cmt);
                if (update > 0)
                {
                    msg.Errors = false;
                    msg.Number = cmt.Good.Value;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Errors = true;
                msg.Logs = e.Message;
                msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
            }
            return Ok(msg);
        }
        [HttpPost]
        public async Task<IActionResult> Bad()
        {
            JsonMessage msg = new();
            SearchModel search = new();
            try
            {
                await TryUpdateModelAsync(search);
                var cmt = _commentAdminDa.GetId(search.parentId);
                cmt.Bad = cmt.Bad.HasValue ? cmt.Bad + 1 : 1;
                var update = _commentAdminDa.Update(cmt);
                if (update > 0)
                {
                    msg.Errors = false;
                }
                else
                {
                    msg.Errors = true;
                    msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
                }
            }
            catch (Exception e)
            {
                msg.Errors = true;
                msg.Logs = e.Message;
                msg.Message = ResourceData.Resource("DanhGiaThatBai", Lang());
            }
            return Ok(msg);
        }
    }
}