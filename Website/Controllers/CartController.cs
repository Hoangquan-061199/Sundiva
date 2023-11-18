using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Website.Models;
using Website.Utils;
using Website.ViewModels;
using ADCOnline.Business.Implementation.AdminManager;
using Newtonsoft.Json;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;

namespace Website.Controllers
{
    public class CartController : BaseController
    {
        private readonly CacheUtils cacheUtils;
        private readonly CartManager _cartManager;
        private readonly ProductManager _productManager;
        private readonly AgencyManager _agencyManager;
        private readonly CustomerManager _customerManager;
        private readonly CustomerDa _customerDa;
        private readonly OrderDa _orderDa;
        public CartController(IDistributedCache distributedCache)
        {
            cacheUtils = new CacheUtils(distributedCache);
            _agencyManager = new AgencyManager(WebConfig.ConnectionString);
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _customerManager = new CustomerManager(WebConfig.ConnectionString);
            _cartManager = new CartManager(WebConfig.ConnectionString);
            _customerDa = new CustomerDa(WebConfig.ConnectionString);
            _orderDa = new OrderDa(WebConfig.ConnectionString);
        }
        public IActionResult CartData()
        {
            string cartCookie = Request.Cookies["shopping_cart"];
            CartViewModel objCartViewModel = new CartViewModel
            {
                ListCartItem = _cartManager.GetCartData(cartCookie, ""),
                TotalPriceCart = _cartManager.GetSumPrice(cartCookie),
                TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
                TotalPriceCartAfterVoucher = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
                DisCountModule = _cartManager.GetDiscountMoudle(cartCookie),
                DisCountCombo = _cartManager.GetDiscountCombo(cartCookie)
            };
            return View(objCartViewModel);
        }
        public IActionResult CartAjax(string vorchercode)
        {
            ViewBag.Back = GetWebBack();
            string cartCookie = Request.Cookies["shopping_cart"];
            //customer                                
            CustomerItem customer = _customerManager.GetId(UserId.Value);
            CartViewModel objCartViewModel = new CartViewModel
            {
                ListCartItem = _cartManager.GetCartData(cartCookie, vorchercode),
                TotalPriceCart = _cartManager.GetSumPrice(cartCookie),
                TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
                DisCountModule = _cartManager.GetDiscountMoudle(cartCookie),
                CustomerItem = customer,
            };
            return View(objCartViewModel);
        }
        public IActionResult TotalPriceCart(string vorchercode)
        {
            string cartCookie = Request.Cookies["shopping_cart"];
            CartViewModel objCartViewModel = new CartViewModel
            {
                ListCartItem = _cartManager.GetCartData(cartCookie, vorchercode),
                TotalPriceCart = _cartManager.GetSumPrice(cartCookie),
                TotalPriceCartAfterDisCount = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
                DisCountVoucher = _cartManager.GetDiscountVoucher(cartCookie),
                TotalPriceCartAfterVoucher = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
                DisCountCombo = _cartManager.GetDiscountCombo(cartCookie)
            };
            return View(objCartViewModel);
        }
        #region check voucher
        //public ActionResult CheckVorcherCode(string vorchercode)
        //{
        //    //check va hien thi thong tin
        //    JsonMessage msg = new JsonMessage{ Errorcode = 0, Errormessage = "" };
        //    if (!string.IsNullOrEmpty(vorchercode) && Utility.CheckSpecial(vorchercode))
        //    {
        //        msg.Errorcode = 0;
        //        msg.Errormessage = "Mã không tồn tại";
        //        return Ok(msg);
        //    }
        //    CampaignItem campain = _cartManager.GetCampaignByVoucher(vorchercode);
        //    if (campain != null)
        //    {
        //        if (campain.IsDeleted == true)
        //        {
        //            msg.Errorcode = 0;
        //            msg.Errormessage = "Voucher này đã được sử dụng";
        //        }
        //        else
        //        {
        //            DateTime today = DateTime.Now;
        //            DateTime? activeday = campain.ActivedDate;
        //            DateTime? expireday = campain.ExpireDate;
        //            if (campain.Used < campain.TotalUse)
        //            {
        //                if (activeday != null && expireday != null)
        //                {
        //                    int afterResult = DateTime.Compare(today, activeday.Value);
        //                    int beforeResult = DateTime.Compare(expireday.Value, today);
        //                    if (afterResult > 0 && beforeResult > 0)
        //                    {
        //                        decimal TotalVat = 0;
        //                        var cartCookie = Request.Cookies["shopping_cart"];
        //                        List<CartItem> ListCartItem = _cartManager.GetCartData(cartCookie, "");
        //                        decimal valueOrderFrom = campain.ValueOrderFrom == null ? 0 : campain.ValueOrderFrom.Value;
        //                        decimal valueOrderTo = campain.ValueOrderTo == null ? 100000000000 : campain.ValueOrderTo.Value;
        //                        TotalVat = ListCartItem.Sum(x => x.VATPrice);
        //                        decimal sumP = _cartManager.GetSumPrice(cartCookie) + TotalVat;
        //                        switch (campain.Type)
        //                        {
        //                            case 2:
        //                            case 3:
        //                                if (valueOrderTo != 0)
        //                                {
        //                                    if (sumP >= valueOrderFrom && sumP <= valueOrderTo)
        //                                    {
        //                                        msg.Errorcode = 1;
        //                                        msg.Errormessage = "Đơn hàng của bạn đủ tiêu chuẩn sử dụng mã này";
        //                                    }
        //                                    else
        //                                    {
        //                                        msg.Errorcode = 0;
        //                                        msg.Errormessage = "Đơn hàng của bạn không đủ tiêu chuẩn sử dụng mã này";
        //                                    }
        //                                }
        //                                break;
        //                            case 6:
        //                            case 7:
        //                                foreach (CartItem item in ListCartItem)
        //                                {
        //                                    List<string> listModuleIds = !string.IsNullOrEmpty(campain.ListModuleIds) ? ListHelper.GetValuesArrayString(campain.ListModuleIds) : new List<string>();
        //                                    string listProductIds = campain.ListProductIds;
        //                                    if (("," + listProductIds + ",").Contains("," + item.ProductId + ",") || listModuleIds.Any(p => ("," + item.ModuleIds + ",").Contains("," + p + ",")))
        //                                    {
        //                                        msg.Errorcode = 1;
        //                                        msg.Errormessage = "Đơn hàng của bạn đủ tiêu chuẩn sử dụng mã này";
        //                                    }
        //                                    else
        //                                    {
        //                                        msg.Errorcode = 0;
        //                                        msg.Errormessage = "Không có sản phẩm nào thỏa mãn hoặc mã chỉ dùng cho danh mục sản phẩm khác";
        //                                    }
        //                                }
        //                                break;
        //                        }
        //                        //var cartCookie = Request.Cookies["shopping_cart"];
        //                        //var ListCartItem = _cartManager.GetCartData(cartCookie, vorchercode);
        //                        //var totalPriceCart = _cartManager.GetSumPrice(cartCookie);
        //                        //var totalMoneyAfterSale = _cartManager.GetSumPriceAfterDiscountModule("");
        //                        //var typeOther = totalPriceCart;
        //                        //var idCampainId = thisVoucher.CampaignID == null ? 0 : thisVoucher.CampaignID.Value;
        //                        //campain = _cartManager.GetCampaignById(idCampainId);
        //                        //if (campain == null)
        //                        //    campain = new CampaignItem();
        //                        //if (typeOther != totalMoneyAfterSale)
        //                        //{
        //                        //    errorcode = 1; //co the dung duoc va dung cho don hang dang mua 
        //                        //    typeAllOther = totalPriceCart - totalMoneyAfterSale;
        //                        //}
        //                        //else
        //                        //{
        //                        //    errorcode = 2;
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        msg.Errorcode = 0;
        //                        msg.Errormessage = "Voucher này đã hết hạn hoặc chưa diễn ra";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                msg.Errorcode = 0;
        //                msg.Errormessage = "Voucher này đã đạt giới hạn số lần sử dụng";
        //            }

        //        }
        //    }
        //    else
        //    {
        //        msg.Errorcode = 0;
        //        msg.Errormessage = "Voucher này không tồn tại";
        //    }
        //    return Ok(msg);
        //}
        //public ActionResult ApplyVorcherCode(string vorchercode)
        //{
        //    string cartCookie = Request.Cookies["shopping_cart"];
        //    VoucherViewModel model = new VoucherViewModel
        //    {
        //        IsApplyOtherCampaign = _cartManager.GetIsApplyOtherCampaign(cartCookie),
        //        TotalMoney = _cartManager.GetSumPrice(cartCookie),
        //        TotalMoneyAfterModule = _cartManager.GetSumPriceAfterDiscountModule(cartCookie),
        //        TotalMoneyAfterVoucher = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
        //        DiscountModule = _cartManager.GetDiscountMoudle(cartCookie),
        //        DiscountVoucher = _cartManager.GetDiscountVoucher(cartCookie),
        //        VoucherCode = vorchercode,
        //    };
        //    return View(model);
        //}
        #endregion 
        public async Task<IActionResult> OrderInformation(string ordercode)
        {
            Order order = _cartManager.GetOrderByCodePageSucces(ordercode);
            List<OrderDetail> details = new List<OrderDetail>();
            List<Product> products = new List<Product>();
            IEnumerable<AttributeItem> attrs = new List<AttributeItem>();
            bool cancelByCustomer = false;
            if (order != null)
            {
                details = _cartManager.GetListOrderDetailByListOrderIds(order.ID.ToString());
                if (details.Any())
                {
                    string lstIdPros = string.Join(",", details.Select(c => c.ProductID).ToList());
                    products = _cartManager.GetListProductsByListIds(lstIdPros);
                    //attr
                    string lstIdProsAttr = string.Join(",", details.Select(c => c.AttrIds));
                    attrs = await _productManager.GetAttributeByListIds(lstIdProsAttr);
                }
            }
            else
            {
                cancelByCustomer = true;
            }
            CartViewModel objCartViewModel = new CartViewModel
            {
                Order = order ?? new Order(),
                OrderDetails = details,
                Products = products,
                AttributeItems = attrs,
                SystemConfig = await cacheUtils.SystemConfig("vi"),
                CancelByCustomer = cancelByCustomer
            };
            return View(objCartViewModel);
        }
        public async Task<ViewResult> LoadDistrict(string id) => View(new CartViewModel
        {
            DistrictItems = await  _cartManager.GetDistrictById("district.json", id)
        });
        public async Task<ViewResult> LoadWard(string id) => View(new CartViewModel
        {
            WardItems = await _cartManager.GetWardById("ward.json", id)
        });
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> SendPayment()
        {
            JsonMessage msg = new JsonMessage();
            PaymentModels paymentModel = new PaymentModels();
            await TryUpdateModelAsync(paymentModel);
            foreach (var prop in paymentModel.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var val = paymentModel.GetType().GetProperty(prop.Name).GetValue(paymentModel, null);
                    if (val != null)
                    {
                        prop.SetValue(paymentModel, Utility.RemoveSpecialCharacterSQLInjection2(val.ToString()));
                    }
                }
            }
            SystemConfigJson systemConfig = cacheUtils.SystemConfigItem(Lang());
            if(string.IsNullOrEmpty(paymentModel.fullname) || string.IsNullOrEmpty(paymentModel.paymentmobile))
            {
                msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập đẩy đủ họ tên" };
                return Ok(msg);
            }
            #region Insert DB
            //order
            string cartCookie = Request.Cookies["shopping_cart"];
            List<CartItem> ListCartData = _cartManager.GetCartData(cartCookie, paymentModel.vouchercode);
            paymentModel.fullname = Utility.RemoveValidFullname(paymentModel.fullname);
            paymentModel.paymentmobile = Utility.RemoveValidNumber(paymentModel.paymentmobile);
            paymentModel.note = Utility.RemoveValidFullname(paymentModel.note);
            //paymentModel.PhoneBill = Utility.RemoveValidFullname(paymentModel.PhoneBill);
            //paymentModel.EmailBill = Utility.RemoveValidFullname(paymentModel.EmailBill);
            //paymentModel.FullNameBill = Utility.RemoveValidNumber(paymentModel.FullNameBill);
            //paymentModel.EmailBill1 = Utility.RemoveValidUserName(paymentModel.EmailBill1);
            //paymentModel.FullNameReceive = Utility.RemoveValidFullname(paymentModel.FullNameReceive);
            //paymentModel.PhoneReceive = Utility.RemoveValidNumber(paymentModel.PhoneReceive);
            if (string.IsNullOrEmpty(paymentModel.fullname))
            {
                msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập đẩy đủ họ tên" };
                return Ok(msg);
            }
            if (string.IsNullOrEmpty(paymentModel.paymentmobile))
            {
                msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập số điện thoại" };
                return Ok(msg);
            }
            if (string.IsNullOrEmpty(paymentModel.paymentemail))
            {
                msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập email" };
                return Ok(msg);
            }
            //bool HasVoucher = _cartManager.CheckHasVoucher();
            Order order = new Order
            {
                FullName = $"{paymentModel.gen} {paymentModel.fullname}",
                Mobile = paymentModel.paymentmobile,
                Address = paymentModel.paymentadd,
                Email = paymentModel.paymentemail,
                //Facebook = paymentModel.paymentfacebook,
                //Zalo = paymentModel.paymentzalo,
                Addr = paymentModel.addr,
                Note = paymentModel.note,
                FullNameReceive = $"{paymentModel.otherreceiveaf} {paymentModel.FullNameReceive}",
                PhoneReceive = paymentModel.PhoneReceive,
                IsPayment = false,
                PaymentType = paymentModel.paymentype,
                Status = 1,
                IsCancerByCustomer = false,
                IsDeleted = false,
                OrderCode = $"TL{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}",
                TotalMoney = _cartManager.GetSumPrice(cartCookie),
                TotalVAT = ListCartData.Sum(x => x.VATPrice),
                DiscountCombo = _cartManager.GetDiscountCombo(cartCookie),
                TotalAfterSale = _cartManager.GetSumPriceAfterDiscountAll(cartCookie),
                DiscountModule = _cartManager.GetDiscountMoudle(cartCookie)
            };
            //if (HasVoucher == true)
            //{
            //    order.DiscountVoucher = _cartManager.GetDiscountVoucher(cartCookie);
            //    order.VoucherCode = paymentModel.vouchercode;
            //}
            if (UserId.HasValue && UserId != 0)
            {
                order.CustomerID = UserId.Value;
            }
            //paymentModel.paymentreceive = "1";
            //if (paymentModel.paymentreceive == "1")
            //{
                order.Paymentreceive = "GHTN";
                if(string.IsNullOrEmpty(paymentModel.CityID) || string.IsNullOrEmpty(paymentModel.DistrictID) || string.IsNullOrEmpty(paymentModel.WardID) || string.IsNullOrEmpty(paymentModel.paymentadd))
                {
                    msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập đẩy đủ thông tin nhận hàng." };
                    return Ok(msg);
                }
                if (!string.IsNullOrEmpty(paymentModel.CityID))
                {
                    order.CityId = Convert.ToInt32(paymentModel.CityID);
                    CityJson city = await _cartManager.GetCityByCityId("city.json", paymentModel.CityID);
                    order.CityName = city.name;
                }
                if (!string.IsNullOrEmpty(paymentModel.DistrictID))
                {
                    order.DistrictId = Convert.ToInt32(paymentModel.DistrictID);
                    DistrictJson distr = await _cartManager.GetDistrictByDistrictId("district.json", paymentModel.DistrictID);
                    order.DistrictName = distr.name;
                }
                if (!string.IsNullOrEmpty(paymentModel.WardID))
                {
                    order.WardId = Convert.ToInt32(paymentModel.WardID);
                    WardJson ward = await _cartManager.GetWardByWardId("ward.json", paymentModel.WardID);
                    order.WardName = ward.name;
                }
            //}
            //else
            //{
            //    order.Paymentreceive = "NTCH";
            //    if (string.IsNullOrEmpty(paymentModel.AreaAgencyParentID) || string.IsNullOrEmpty(paymentModel.AreaAgencyChildID))
            //    {
            //        msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập người nhận hàng." };
            //        return Ok(msg);
            //    }
            //    if (!string.IsNullOrEmpty(paymentModel.AreaAgencyParentID))
            //    {
            //        order.AreaAgencyIDParent = Convert.ToInt32(paymentModel.AreaAgencyParentID);
            //        AreaAgencyItem AreaAgencyIDParentN = _agencyManager.GetAreaById(order.AreaAgencyIDParent.Value);
            //        order.AreaAgencyIDParentName = AreaAgencyIDParentN.Name;
            //    }
            //    if (!string.IsNullOrEmpty(paymentModel.AreaAgencyChildID))
            //    {
            //        order.AreaAgencyIDChild = Convert.ToInt32(paymentModel.AreaAgencyChildID);
            //        AreaAgencyItem AreaAgencyIDChildN = _agencyManager.GetAreaById(order.AreaAgencyIDChild.Value);
            //        order.AreaAgencyIDChildName = AreaAgencyIDChildN.Name;
            //    }
            //    if (!string.IsNullOrEmpty(paymentModel.store))
            //    {
            //        order.AgenciesID = Convert.ToInt32(paymentModel.store);
            //        AgenciesItem store = _agencyManager.GetByID(paymentModel.store);
            //        order.AgenciesIDName = store.Name;
            //    }
            //}
            #region Hình thức nhận hàng
            //if (paymentModel.timereceivecheck == "1")
            //{
            //    order.Timereceivecheck = "Nhận hàng tiêu chuẩn.";
            //    order.Timereceive = DateTime.Now;
            //}
            //else
            //{
            //    order.Timereceivecheck = "Thời gian nhận cụ thể.";
            //    if (!string.IsNullOrEmpty(paymentModel.timereceive))
            //    {

            //        order.Timereceive = DateTime.Now.AddDays(Convert.ToInt32(paymentModel.timereceive));
            //    }
            //}
            #endregion`
            if (!string.IsNullOrEmpty(paymentModel.dateofbirth))
            {
                order.DateofBirth = Convert.ToDateTime(Utility.ConvertDateMMddyyyy(paymentModel.dateofbirth));
            }
            if (paymentModel.IsExportBill == "1")
            {
                if (string.IsNullOrEmpty(paymentModel.PhoneBill) || string.IsNullOrEmpty(paymentModel.EmailBill) || string.IsNullOrEmpty(paymentModel.EmailBill1) || string.IsNullOrEmpty(paymentModel.FullNameBill))
                {
                    msg = new JsonMessage { Errors = true, Message = "Vui lòng nhập thông tin xuất hóa đơn." };
                    return Ok(msg);
                }
                order.IsExportBill = true;
                order.FullNameBill = string.IsNullOrEmpty(paymentModel.FullNameBill) ? paymentModel.fullname : paymentModel.FullNameBill;
                order.PhoneBill = string.IsNullOrEmpty(paymentModel.PhoneBill) ? paymentModel.paymentmobile : paymentModel.PhoneBill;
                order.EmailBill = string.IsNullOrEmpty(paymentModel.EmailBill) ? paymentModel.paymentemail : paymentModel.EmailBill;
                order.EmailBill1 = string.IsNullOrEmpty(paymentModel.EmailBill1) ? "" : paymentModel.EmailBill1;
            }
            string add = string.Empty;
            if (!string.IsNullOrEmpty(paymentModel.paymentadd))
            {
                add += paymentModel.paymentadd;
            }
            if (!string.IsNullOrEmpty(order.WardName))
            {
                add += $", {order.WardName}";
            }
            if (!string.IsNullOrEmpty(order.DistrictName))
            {
                add += $", {order.DistrictName}";
            }
            if (!string.IsNullOrEmpty(order.CityName))
            {
                add += $", {order.CityName}";
            }
            order.Address = add;
            int result = 0;
            //reset cookie cart
            //Customer customer = _customerDa.GetId(UserId.Value);
            //if (customer != null)
            //{
            //    customer.CartCookies = string.Empty;
            //    _customerDa.Update(customer);
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(order.FullName) && !string.IsNullOrEmpty(order.Mobile))
            //    {
            //        ADCOnline.Simple.Admin.CustomerAdmin cusPhone = _customerDa.GetByPhone(order.Mobile);
            //        if (cusPhone != null)
            //        {
            //            order.CustomerID = cusPhone.ID;
            //        }
            //        else
            //        {
            //            string saltKey = Utility.CreateSaltKey(5);
            //            string code = Utility.CreateRandomKey(10);
            //            string pass = Utility.CreateRandomKey(10);
            //            string sha1PasswordHash = Utility.CreatePasswordHash(pass, saltKey);
            //            string codeconfirm = Utility.RandomString(20, false);
            //            Customer newCustomer = new Customer
            //            {
            //                FullName = order.FullName,
            //                Mobile = order.Mobile,
            //                IsActivated = false,
            //                IsLocked = false,
            //                IsDeleted = false,
            //                IsOnline = false,
            //                IsReceiveSale = false,
            //                Address = order.Address,
            //                CreatedOnUtc = DateTime.Now,
            //                Password = sha1PasswordHash,
            //                PasswordSalt = saltKey,
            //                ForgotPassCode = code,
            //                Code = codeconfirm,
            //            };
            //            order.CustomerID = _customerDa.Insert(newCustomer);
            //        }
            //    }
            //}
            if (!string.IsNullOrEmpty(order.FullName) && !string.IsNullOrEmpty(order.Mobile))
            {
                result = _cartManager.InsertOrder(order);
                //update campain
                //paymentModel.vouchercode = Utility.RemoveSpecialCharacter(paymentModel.vouchercode);
                //Campaign cam = _cartManager.GetCampainByVoucher(paymentModel.vouchercode);
                //if (cam != null)
                //{
                //    cam.Used++;
                //    _cartManager.Upd ateCampaign(cam);
                //}
                //detail
                foreach (CartItem v in ListCartData)
                {
                    OrderDetail objOrderDetail = new OrderDetail
                    {
                        Price = v.PriceProduct,
                        ProductID = v.ProductId,
                        Quantity = v.Quantity,
                        OrderID = order.ID,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                        Type = v.Type,
                        AttrIds = string.Join(",", v.AttributeItems.Select(c => c.ID)),
                        AttrNames = string.Join(",", v.AttributeItems.Where(c => c.ParentID != 0).Select(c => c.Name)),
                        SumMoney = v.SumMoney,
                        VATPrice = v.VATPrice,
                        SumMoneyAfterSaleModule = v.SumMoney + v.VATPrice,
                        PromotionText = v.PromotionText,
                        GiftIds = v.GiftIds,
                        IsVAT = v.IsVAT
                    };
                    _cartManager.InsertOrderDetail(objOrderDetail);
                }
            }
            else
            {
                msg = new JsonMessage { Errors = true, Message = "Vui lòng thử lại" };
                return Ok(msg);
            }
            #endregion
            #region Render Email to customer
            string lstIdPros = string.Join(",", ListCartData.Select(c => c.ProductId).ToList());
            List<ProductItem> products = _cartManager.GetListProductByListIds(lstIdPros);
            string str = Common.ReadFile("LayoutEmail.htm", "html/Configuaration");
            CommonJsonItem mainTemplate = new CommonJsonItem();
            List<CommonJsonItem> list = JsonConvert.DeserializeObject<List<CommonJsonItem>>(ReadFile("TemplateEmail.json", "DataJson"));
            if (list == null)
            {
                msg = new JsonMessage { Errors = true, Message = "Gửi thất bại", Id = order.OrderCode };
                return Ok(msg);
            }
            mainTemplate = list.Any(x => x.Code == "EmailOrder") ? list.FirstOrDefault(x => x.Code == "EmailOrder") : new CommonJsonItem();
            if (string.IsNullOrEmpty(mainTemplate.Content))
            {
                msg = new JsonMessage { Errors = true, Message = "Gửi thất bại", Id = order.OrderCode };
                return Ok(msg);
            }
            str = str.Replace("[Main]", mainTemplate.Content);
            str = str.Replace("[FullName]", order.FullName);
            str = str.Replace("[OrderCode]", order.OrderCode);
            #region info product 
            StringBuilder inforproduct = new StringBuilder();
            inforproduct.Append(@"<table width=""100%"">");
            inforproduct.Append(@"<tr><td width=""100""><strong>Tên sản phẩm</strong></td><td width=""20""></td><td width=""200"">&nbsp;</td><td width=""20""></td><td width=""140"" align=""right""><strong>Thông tin</strong></td></tr>");
            inforproduct.Append(@"<tr><td colspan=""5"">&nbsp;</td></tr><tr><td colspan=""5"" style=""border-top:1px solid #cdcdcd;""></td></tr><tr><td colspan=""5"">&nbsp;</td></tr>");
            foreach (var v in ListCartData)
            {
                var product = products.Where(c => c.ID == v.ProductId).First();
                if (product != null)
                {
                    var img = WebConfig.Website + product.UrlPicture;
                    var link = WebConfig.Website + Utility.Link(string.Empty, product.NameAscii, product.LinkUrl);
                    inforproduct.Append($@"<tr><td style=""vertical-align:top;width:100px;""><a href=""{link}"" target=""_blank""><img src=""{img}"" width=""100"" /></a></td>");
                    inforproduct.Append($@"<td width=""10"">&nbsp;</td><td align=""left"" valign=""top""><strong>{product.Name}</strong><br />Mã SP: {product.ProductCode}");
                    if (v.Type != "1")
                    {
                        foreach (var attr in v.AttributeItems.Where(c => c.ParentID == 0))
                        {
                            inforproduct.Append($"<br/>{attr.Name}: {string.Join(",", v.AttributeItems.Where(c => c.ParentID == attr.ID).Select(x => x.Name))}");
                        }
                    }
                    inforproduct.Append($@"</td><td width=""20"">&nbsp;</td><td align=""right"" valign=""top"">Giá: <span style=""color:red;"">{Utility.GetFormatPriceType(v.PriceProduct, 1, "Liên hệ")}</span><br />");
                    //if (v.IsVAT == true)
                    //{
                    //    inforproduct.Append("(Giá đã bao gồm VAT)<br />");
                    //}
                    //else
                    //{
                    //    inforproduct.Append("(Giá chưa bao gồm VAT)<br />");
                    //}
                    inforproduct.Append($"Số lượng: {v.Quantity}<br /></td></tr>");
                }
            }
            inforproduct.Append(@"<tr><td colspan=""5"">&nbsp;</td></tr><tr><td colspan=""5"" style=""border-top:1px solid #cdcdcd;""></td></tr><tr><td colspan=""5"">&nbsp;</td></tr>");
            inforproduct.Append($@"<tr><td colspan=""5"" align=""right"" valign=""top""><strong>Tạm tính: {Utility.GetFormatPriceType(order.TotalMoney, 1, "N/A")}</strong><br />");
            //if (order.DiscountModule != null && order.DiscountModule != 0)
            //{
            //    inforproduct.Append($"<strong>Giảm giá: {Utility.GetFormatPriceType(order.DiscountModule, 1, "N/A")}</strong><br/>");
            //}
            //if (order.DiscountCombo != null && order.DiscountCombo > 0)
            //{
            //    inforproduct.Append($"<strong>Giảm giá combo: {Utility.GetFormatPriceType(order.DiscountCombo, 1, "N/A")}</strong><br/>");
            //}
            //if (!string.IsNullOrEmpty(order.VoucherCode))
            //{
            //    inforproduct.Append($"<strong>Voucher: {order.VoucherCode}</strong><br/>");
            //    inforproduct.Append($"<strong>Giảm giá voucher: {Utility.GetFormatPriceType(order.DiscountVoucher, 1, "N/A")}</strong><br/>");
            //}
            //if (order.TotalVAT > 0)
            //{
            //    inforproduct.Append("<strong>Thuế VAT: " + Utility.GetFormatPriceType(order.TotalVAT, 1, "N/A") + "</strong><br/>");
            //}
            inforproduct.Append($"<strong>Tổng cộng: {Utility.GetFormatPriceType(order.TotalAfterSale, 1, "Liên hệ")}</strong></td></tr></table>");
            str = str.Replace("[ListProducts]", inforproduct.ToString());
            #endregion
            #region Information customer
            str = str.Replace("[FullName]", order.FullName);
            //str = str.Replace("[DateOfBirth]", order.DateofBirth.Value.ToString("dd/MM/yyyy"));
            str = str.Replace("[Mobile]", order.Mobile);
            str = str.Replace("[Email]", order.Email);
            str = str.Replace("[Address]", (string.IsNullOrEmpty(order.Address) ? "N/A" : order.Address));
            //str = str.Replace("[Facebook]", (string.IsNullOrEmpty(order.Facebook) ? "N/A" : order.Facebook));
            //str = str.Replace("[Zalo]", (string.IsNullOrEmpty(order.Zalo) ? "N/A" : order.Zalo));
            #endregion
            #region receiving information
            //string Paymentreceive = string.Empty;
            //if (order.Paymentreceive == "GHTN")
            //{
            //    Paymentreceive += "Giao hàng tận nơi";
            //}
            //else
            //{
            //    Paymentreceive += "Nhận tại cửa hàng";
            //}
            //if (!string.IsNullOrEmpty(order.Timereceivecheck))
            //{
            //    Paymentreceive += $", {order.Timereceivecheck}";
            //}
            //string ntch = string.Empty;
            //if (!string.IsNullOrEmpty(order.AgenciesIDName))
            //{
            //    ntch += $"{order.AgenciesIDName},";
            //}
            //if (!string.IsNullOrEmpty(order.AreaAgencyIDChildName))
            //{
            //    ntch += $"{order.AreaAgencyIDChildName},";
            //}
            //if (!string.IsNullOrEmpty(order.AreaAgencyIDParentName))
            //{
            //    ntch += order.AreaAgencyIDParentName;
            //}

            //if (order.Timereceive != null)
            //{
            //    Paymentreceive += $" ({order.Timereceive.Value} ngày)";
            //}
            //str = str.Replace("[HTNH]", Paymentreceive);
            //str = str.Replace("[DCNH]", !string.IsNullOrEmpty(ntch) ? ntch : "N/A");
            //str = str.Replace("[HTTT]", Temp.PaymentTypeText(paymentModel.paymentype));
            //str = str.Replace("[Note]", (string.IsNullOrEmpty(order.Note) ? "N/A" : order.Note));
            #endregion
            #region Export bill
            //str = str.Replace("[IsBill]", order.IsExportBill == true ? "Có xuất hóa đơn" : "Không xuất hóa đơn");
            //str = str.Replace("[MST]", !string.IsNullOrEmpty(order.FullNameBill) ? order.FullNameBill : "N/A");
            //str = str.Replace("[Company]", !string.IsNullOrEmpty(order.PhoneBill) ? order.PhoneBill : "N/A");
            //str = str.Replace("[CompanyAdress]", !string.IsNullOrEmpty(order.EmailBill) ? order.EmailBill : "N/A");
            //str = str.Replace("[EmailBill1]", !string.IsNullOrEmpty(order.EmailBill1) ? order.EmailBill1 : "N/A");
            #endregion
            #region Send mail
            SendEmailModels objEmail = new SendEmailModels();
            objEmail.Subject = mainTemplate.Name;
            objEmail.DisName = systemConfig.Name;
            objEmail.EmailBody = str;
            objEmail.EmailSend = systemConfig.EmailCms;
            objEmail.Password = systemConfig.PassEmailCms;
            objEmail.Port = Convert.ToInt32(systemConfig.Port);
            objEmail.Servername = systemConfig.Severname;
            objEmail.EnableSSL = systemConfig.SSLEmail.HasValue ? systemConfig.SSLEmail.Value : false;
            objEmail.To = paymentModel.paymentemail;
            if (WebConfig.DebugSend == true)
            {
                var logs = SendEmailModels.SendmailKitTest(objEmail);
            }
            else
            {
                System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmail);
            }
            #endregion
            #region Send mail to admin
            SendEmailModels objEmailAd = new SendEmailModels
            {
                Subject = mainTemplate.Name,
                DisName = systemConfig.Name,
                EmailBody = str,
                EmailSend = systemConfig.EmailCms,
                Password = systemConfig.PassEmailCms,
                Port = Convert.ToInt32(systemConfig.Port),
                Servername = systemConfig.Severname,
                EnableSSL = systemConfig.SSLEmail ?? false,
                To = systemConfig.Email
            };
            if (WebConfig.DebugSend == true)
            {
                string logs = SendEmailModels.SendmailKitTest(objEmailAd);
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "",
                    Id = order.OrderCode,
                    Logs = logs
                };
                return Ok(msg);
            }
            else
            {
                System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmailAd);
            }
            #endregion
            #endregion
            msg = new JsonMessage
            {
                Errors = false,
                Message = "",
                Id = order.OrderCode
            };
            return Ok(msg);
        }
        public ActionResult SaveCartToCustomer()
        {
            if (UserId.Value != 0)
            {
                string cartCookie = Request.Cookies["shopping_cart"];
                Customer customer = _customerDa.GetId(UserId.Value);
                if (customer != null)
                {
                    customer.CartCookies = cartCookie;
                    _customerDa.Update(customer);
                }
                return Ok(1);
            }
            else
            {
                return Ok(0);
            }
        }
        public ActionResult CancelOrder(int id)
        {
            Order order = _orderDa.GetId(id);
            if (order != null)
            {
                order.IsCancerByCustomer = true;
                order.Status = 6;
                order.LogS = $"Hủy bởi khách hàng lúc {DateTime.Now}";
                _orderDa.Update(order);
            }
            return Ok(1);
        }
        public ActionResult UpdatePaymentTypeOrder(int id, string type)
        {
            Order order = _orderDa.GetId(id);
            if (order != null)
            {
                order.IsPaymentTypeByCustomer = true;
                order.PaymentType = type;
                _orderDa.Update(order);
            }
            return View(order);
        }
    }
}