using System;
using System.Linq;
using ADCOnline.Business.Implementation.AdminManager;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Areas.Admin.ViewModels;
using Website.Utils;
using System.Collections.Generic;
using ADCOnline.Simple.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Website.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly OrderDa _OrderDa;
        private readonly CartManager _cartManager;
        private readonly ModuleAdminDa _moduleAdminDa;
        private readonly MembershipDa _membershipDa;
        public OrderController()
        {
            _OrderDa = new OrderDa(WebConfig.ConnectionString);
            _cartManager = new CartManager(WebConfig.ConnectionString);
            _moduleAdminDa = new ModuleAdminDa(WebConfig.ConnectionString);
            _membershipDa = new MembershipDa(WebConfig.ConnectionString);
        }
        public async Task<IActionResult> Index()
        {
            if (SystemActionAdmin.View != true)
            {
                return Redirect("/" + WebConfig.AdminAlias);
            }
            Module module = _moduleAdminDa.GetTag("Order");
            string role = HttpContext.Session.GetString("WebAdminRole");
            string userId = HttpContext.Session.GetString("WebAdminUserID");
            OrderViewModel model = new()
            {
                ListModule = _moduleAdminDa.GetTabMenu(role, userId),
                Module = module,
                SystemActionAdmin = SystemActionAdmin,
                CityItems = await _cartManager.GetAllCity("city.json"),
                StatisticalOrders = _OrderDa.StatisticalOrder()
            };
            return View(model);
        }
        public IActionResult ListItems()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            seach.keyword = Utility.ValidString(seach.keyword, "", true);
            List<OrderAdmin> list = _OrderDa.ListSearch(seach, seach.page, 20, false);
            int total = list.Any() ? list.FirstOrDefault().TotalRecord : 0;
            ViewBag.GridHtml = GetPage(seach.page, total, 20);
            string listIds = string.Join(",", list.Select(c => c.ID));
            List<OrderDetail> lstDetail = !string.IsNullOrEmpty(listIds) ? _OrderDa.GetListOrderDetailByListOrderIds(listIds) : new List<OrderDetail>();
            List<ProductAdmin> products = new();
            if (lstDetail.Any())
            {
                string lstIdPros = string.Join(",", lstDetail.Select(c => c.ProductID).ToList());
                products = _OrderDa.GetListProductByListIds(lstIdPros);
            }
            OrderViewModel model = new()
            {
                SearchModel = seach,
                ListItem = list,
                SystemActionAdmin = SystemActionAdmin,
                ListOrderDetail = lstDetail,
                ProductAdmins = products,
                OrderAdmin = seach.contentId.HasValue ? _OrderDa.GetById(seach.contentId.Value) : new OrderAdmin()
            };
            return View(model);
        }
        public async Task<ActionResult> AjaxForm()
        {
            ActionViewModel action = UpdateModelAction();
            var tab = Request.Query["tab"];
            ViewBag.Tab = tab;
            OrderViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                OrderAdmin = new OrderAdmin(),
                CityItems = await _cartManager.GetAllCity("city.json"),
                DistrictItems = new Dictionary<string, DistrictJson>(),
                WardItems = new Dictionary<string, WardJson>()
            };
            if (action.Do == ActionType.Edit)
            {
                model.OrderAdmin = _OrderDa.GetById(ConvertUtil.ToInt32(action.ItemId));
                if (model.OrderAdmin.CityId != null)
                {
                    string id = model.OrderAdmin.CityId < 10 ? "0" + model.OrderAdmin.CityId : model.OrderAdmin.CityId.ToString();
                    model.DistrictItems = await _cartManager.GetDistrictById("district.json", id);
                }
                if (model.OrderAdmin.DistrictId != null)
                {
                    string id = model.OrderAdmin.DistrictId.ToString();
                    if (model.OrderAdmin.DistrictId < 10)
                    {
                        id = "00" + model.OrderAdmin.DistrictId;
                    }
                    if (model.OrderAdmin.DistrictId < 100 && model.OrderAdmin.DistrictId > 10)
                    {
                        id = "0" + model.OrderAdmin.DistrictId;
                    }
                    model.WardItems = await _cartManager.GetWardById("ward.json", id);
                }
                List<OrderDetail> lstDetail = _OrderDa.GetListOrderDetailByListOrderIds(model.OrderAdmin.ID.ToString());
                //product
                List<ProductAdmin> productAdmis = new();
                List<Attributes> attrs = new();
                if (lstDetail.Any())
                {
                    string lstIdPros = string.Join(",", lstDetail.Select(c => c.ProductID).ToList());
                    lstIdPros = lstIdPros + "," + string.Join(",", lstDetail.Select(c => c.GiftIds).ToList());
                    productAdmis = _OrderDa.GetListProductByListIds(lstIdPros);
                    //attr
                    string lstIdProsAttr = string.Join(",", lstDetail.Select(c => c.AttrIds));
                    attrs = _OrderDa.GetAttributeByListIds(lstIdProsAttr);
                }
                model.Attributes = attrs;
                model.ListOrderDetail = lstDetail;
                model.ProductAdmins = productAdmis;
            }
            ViewBag.Action = action.Do ?? ActionType.Add;
            ViewBag.ActionText = ActionType.ActionText(action.Do);
            return View(model);
        }
        public ActionResult HistoryOrder()
        {
            SearchModel seach = new();
            TryUpdateModelAsync(seach);
            OrderViewModel model = new()
            {
                ListItem = _OrderDa.ListSearch(seach, seach.page, 50, false),
                SystemActionAdmin = SystemActionAdmin
            };
            return View(model);
        }
        public async Task<ActionResult> UpdateOrderStatus(int id, int status, int payment)
        {
            MembershipAdmin membership = _membershipDa.GetAdminId(ConvertUtil.ToGuid(session.GetAdminUserId()));
            Order obj = _OrderDa.GetId(id);
            obj.Status = status;
            if (payment == 1)
            {
                obj.IsPayment = true;
            }
            else
            {
                obj.IsPayment = false;
            }
            obj.EditorID = ConvertUtil.ToGuid(session.GetAdminUserId());
            obj.EditorName = membership.FullName;
            obj.ModifiedDate = DateTime.Now;
            _OrderDa.Update(obj);
            List<OrderDetail> orderdetails = _OrderDa.GetListOrderDetailByListOrderIds(obj.ID.ToString());
            if (orderdetails != null)
            {
                foreach (OrderDetail item in orderdetails)
                {
                    await UpdateTotalOrderProduct(item.ProductID.Value);
                }
            }
            return Ok(new JsonMessage { Errors = false });
        }

        public ActionResult HistoryOrderDetail()
        {
            int orderId = ConvertUtil.ToInt32(Request.Query["orderId"].ToString());
            Order orderDetail = _OrderDa.GetId(orderId);
            List<OrderDetail> lstDetail = _OrderDa.GetListOrderDetailByListOrderIds(orderDetail.ID.ToString());
            //product
            List<ProductAdmin> productAdmis = new();
            List<Attributes> attrs = new();
            if (lstDetail.Any())
            {
                string lstIdPros = string.Join(",", lstDetail.Select(c => c.ProductID).ToList());
                lstIdPros = lstIdPros + "," + string.Join(",", lstDetail.Select(c => c.GiftIds).ToList());
                productAdmis = _OrderDa.GetListProductByListIds(lstIdPros);
                //attr
                string lstIdProsAttr = string.Join(",", lstDetail.Select(c => c.AttrIds));
                attrs = _OrderDa.GetAttributeByListIds(lstIdProsAttr);
            }
            OrderViewModel model = new()
            {
                SystemActionAdmin = SystemActionAdmin,
                ListOrderDetail = lstDetail,
                Order = orderDetail,
                ProductAdmins = productAdmis,
                Attributes = attrs
            };
            return PartialView(model);
        }
        [HttpPost]
        public async Task<ActionResult> Actions()
        {
            ActionViewModel action = UpdateModelAction();
            JsonMessage msg = new() { Errors = true, Message = "Không có hành động nào được thực hiện." };
            MembershipAdmin membership = _membershipDa.GetAdminId(ConvertUtil.ToGuid(session.GetAdminUserId()));
            Order obj = new();
            switch (action.Do)
            {
                case ActionType.Add:
                    try
                    {
                        if (SystemActionAdmin.Add != true)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        await TryUpdateModelAsync(obj);
                        #region Valid
                        obj.FullName = Utility.ValidString(obj.FullName, "", true);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Code, true);
                        obj.Note = Utility.RemoveHTMLTag(obj.Note);
                        obj.Address = Utility.ValidAddress(obj.Address);
                        obj.PaymentType = Utility.ValidString(obj.PaymentType, Code, true);
                        if (Utility.IsValidEmail(obj.Email))
                        {
                            msg = new JsonMessage
                            {
                                Errors = true,
                                Message = "Email không đúng."
                            };
                            return Ok(msg);
                        }
                        #endregion
                        Order checkCode = _OrderDa.GetCode(obj.OrderCode);
                        if (checkCode != null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Mã đơn hàng đã tồn tại." };
                            return Ok(msg);
                        }
                        obj.IsDeleted = false;
                        obj.CreatedDate = DateTime.Now;
                        obj.OrderCode = "OD" + Utility.CreateRandomTransaction(10);
                        int result = _OrderDa.Insert(obj);
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Thêm mới thành công.", Obj = obj };
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
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        obj = _OrderDa.GetId(ConvertUtil.ToInt32(action.ItemId));
                        obj.EditorID = ConvertUtil.ToGuid(session.GetAdminUserId());
                        obj.EditorName = membership.FullName;
                        obj.ModifiedDate = DateTime.Now;
                        Order objOld = obj;
                        await TryUpdateModelAsync(obj);
                        #region Valid
                        obj.FullName = Utility.ValidString(obj.FullName, "", true);
                        obj.Mobile = Utility.ValidString(obj.Mobile, Code, true);
                        obj.Note = Utility.RemoveHTMLTag(obj.Note);
                        obj.Address = Utility.ValidAddress(obj.Address);
                        obj.PaymentType = Utility.ValidString(obj.PaymentType, Code, true);
                        if (!string.IsNullOrEmpty(obj.Email))
                        {
                            if (Utility.IsValidEmail(obj.Email))
                            {
                                msg = new JsonMessage
                                {
                                    Errors = true,
                                    Message = "Email không đúng."
                                };
                                return Ok(msg);
                            }
                        }
                        #endregion
                        var isPaymentN = Request.Form["IsPaymentn"];
                        obj.IsPayment = isPaymentN != "0";
                        int result = _OrderDa.Update(obj);
                        List<OrderDetail> orderdetails = _OrderDa.GetListOrderDetailByListOrderIds(obj.ID.ToString());
                        if (orderdetails != null)
                        {
                            foreach (OrderDetail item in orderdetails)
                            {
                                await UpdateTotalOrderProduct(item.ProductID.Value);
                            }
                        }
                        if (result > 0)
                        {
                            msg = new JsonMessage { Errors = false, Message = "Cập nhật thành công.", Obj = obj };
                            return Ok(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogError(ex);
                    }
                    break;
                case ActionType.Delete:
                    if (SystemActionAdmin.Delete != true)
                    {
                        msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                        return Ok(msg);
                    }
                    try
                    {
                        Order content = _OrderDa.GetId(Convert.ToInt32(action.ItemId));
                        content.IsDeleted = true;
                        content.EditorID = ConvertUtil.ToGuid(session.GetAdminUserId());
                        content.EditorName = membership.FullName;
                        content.ModifiedDate = DateTime.Now;
                        _OrderDa.Update(content);
                        List<OrderDetail> orderdetails = _OrderDa.GetListOrderDetailByListOrderIds(content.ID.ToString());
                        if (orderdetails != null)
                        {
                            foreach (OrderDetail item in orderdetails)
                            {
                                await UpdateTotalOrderProduct(item.ProductID.Value);
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
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
                            msg = new JsonMessage { Errors = true, Message = "Bạn chưa được phân quyền cho chức năng này." };
                            return Ok(msg);
                        }
                        foreach (int item in ArrID)
                        {
                            Order content = _OrderDa.GetId(item);
                            content.IsDeleted = true;
                            content.EditorID = ConvertUtil.ToGuid(session.GetAdminUserId());
                            content.EditorName = membership.FullName;
                            content.ModifiedDate = DateTime.Now;
                            _OrderDa.Update(content);
                            List<OrderDetail> orderdetails = _OrderDa.GetListOrderDetailByListOrderIds(content.ID.ToString());
                            if (orderdetails != null)
                            {
                                foreach (OrderDetail od in orderdetails)
                                {
                                    await UpdateTotalOrderProduct(od.ProductID.Value);
                                }
                            }
                        }
                        msg = new JsonMessage { Errors = false, Message = "Xóa thành công." };
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
