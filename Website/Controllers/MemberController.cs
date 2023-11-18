using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Website.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using Website.Models;
using ADCOnline.Simple.Item;
using Website.Utils;
using System.Linq;
using Microsoft.AspNetCore.Http;
using ADCOnline.Business.Implementation.AdminManager;
using Website.Infrastructure;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ADCOnline.Simple.Json;

namespace Website.Controllers
{
    public class MemberController : BaseController
    {
        private readonly CustomerManager _customerManager;
        private readonly ProductManager _productManager;
        private readonly DapperDA _dapperDa;
        private readonly IDistributedCache _distributedCache;
        private readonly CacheUtils cacheUtils;
        private readonly CustomerDa _customerDa;
        //private readonly CommentManager _commentManager;
        private readonly ProductDa _productDa;
        private readonly OrderDa _orderDa; 
        private readonly GoogleCapthaService _captchaService;
        public MemberController(IDistributedCache distributedCache, GoogleCapthaService service)
        {
            _customerManager = new CustomerManager(WebConfig.ConnectionString);
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _distributedCache = distributedCache;
            cacheUtils = new CacheUtils(distributedCache);
            _customerDa = new CustomerDa(WebConfig.ConnectionString);
            //_commentManager = new CommentManager(WebConfig.ConnectionString);
            _productDa = new ProductDa(WebConfig.ConnectionString);
            _orderDa = new OrderDa(WebConfig.ConnectionString);
            _captchaService = service;
        }
        #region Facebook Google Zalo
        public IActionResult LoginFacebook(string id, string fullname, string email)
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Đăng nhập thất bại. Vui lòng thực hiện lại hoặc liên hệ với hệ thống. "
            };
            string emailR = email;
            if (string.IsNullOrEmpty(email))
            {
                emailR = $"{id}@facebook.com";
            }
            CustomerItem customer = _customerManager.GetCustomerItemByEmailOrPhone(emailR);
            if (customer != null)
            {
                SetWebUserID(customer.ID);
                SetWebUserName(customer.Email);
                SetWebFullName(customer.FullName);
                //ck
                CookieOptions ck = new()
                {
                    Expires = DateTime.Now.AddDays(5),
                    Domain = HttpContext.Request.Host.ToString()
                };
                Response.Cookies.Append("lsnp", emailR, ck);
                //cookie cart
                //var customerU = _customerDa.GetId(customer.ID);
                //if (customerU != null)
                //{
                //    var cartCookie = Request.Cookies["shopping_cart"];
                //    string newC = customerU.CartCookies + cartCookie;
                //    customerU.CartCookies = newC;
                //    _customerDa.Update(customerU);
                //    CookieOptions option = new CookieOptions();
                //    option.Expires = DateTime.Now.AddDays(1);
                //    option.Domain = HttpContext.Request.Host.ToString();
                //    Response.Cookies.Append("shopping_cart", System.Web.HttpUtility.UrlEncode(newC), option);
                //}
                msg = new JsonMessage
                {
                    Errors = false,
                };
            }
            else
            {
                string pass = "123456";
                string saltKey = Utility.CreateSaltKey(5);
                string code = Utility.CreateRandomKey(10);
                string sha1PasswordHash = Utility.CreatePasswordHash(pass, saltKey);
                Customer cus = new()
                {
                    UserName = emailR,
                    FullName = fullname,
                    Email = emailR,
                    FaceBookId = id,
                    Password = sha1PasswordHash,
                    PasswordSalt = saltKey,
                    ForgotPassCode = code,
                    IsActivated = true,
                    IsDeleted = false,
                    IsLocked = false
                };
                int result = _customerManager.Insert(cus);
                if (result > 0)
                {
                    var customerG = _customerManager.GetCustomerItemByEmailOrPhone(emailR);
                    SetWebUserID(customerG.ID);
                    SetWebUserName(customerG.Email);
                    SetWebFullName(customerG.FullName);
                    //ck
                    CookieOptions ck = new()
                    {
                        Expires = DateTime.Now.AddDays(5),
                        Domain = HttpContext.Request.Host.ToString()
                    };
                    Response.Cookies.Append("lsnp", emailR, ck);
                    msg = new JsonMessage
                    {
                        Errors = false,
                    };
                }

            }
            return Ok(msg);

        }       
        public IActionResult LoginGoogle(string id, string fullname, string email)
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Đăng nhập thất bại. Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            var customer = _customerManager.GetCustomerItemByEmailOrPhone(email);
            if (customer != null)
            {
                SetWebUserID(customer.ID);
                SetWebUserName(customer.Email);
                SetWebFullName(customer.FullName);
                //ck
                CookieOptions ck = new()
                {
                    Expires = DateTime.Now.AddDays(5),
                    Domain = HttpContext.Request.Host.ToString()
                };
                Response.Cookies.Append("lsnp", email, ck);
                //cookie cart
                //var customerU = _customerDa.GetId(customer.ID);
                //if (customerU != null)
                //{
                //    var cartCookie = Request.Cookies["shopping_cart"];
                //    string newC = customerU.CartCookies + cartCookie;
                //    customerU.CartCookies = newC;
                //    _customerDa.Update(customerU);
                //    CookieOptions option = new CookieOptions();
                //    option.Expires = DateTime.Now.AddDays(1);
                //    option.Domain = HttpContext.Request.Host.ToString();
                //    Response.Cookies.Append("shopping_cart", System.Web.HttpUtility.UrlEncode(newC), option);
                //}
                msg = new JsonMessage
                {
                    Errors = false,
                };
            }
            else
            {
                string pass = "123456";
                string saltKey = Utility.CreateSaltKey(5);
                string code = Utility.CreateRandomKey(10);
                string sha1PasswordHash = Utility.CreatePasswordHash(pass, saltKey);
                Customer cus = new()
                {
                    UserName = email,
                    FullName = fullname,
                    Email = email,
                    GoogleId = id,
                    Password = sha1PasswordHash,
                    PasswordSalt = saltKey,
                    ForgotPassCode = code,
                    IsActivated = true,
                    IsDeleted = false,
                    IsLocked = false
                };
                var result = _customerManager.Insert(cus);
                if (result > 0)
                {
                    CustomerItem customerG = _customerManager.GetCustomerItemByEmailOrPhone(email);
                    SetWebUserID(customerG.ID);
                    SetWebUserName(customerG.Email);
                    SetWebFullName(customerG.FullName);
                    //ck
                    CookieOptions ck = new()
                    {
                        Expires = DateTime.Now.AddDays(5),
                        Domain = HttpContext.Request.Host.ToString()
                    };
                    Response.Cookies.Append("lsnp", email, ck);
                    msg = new JsonMessage
                    {
                        Errors = false,
                    };
                }

            }
            return Ok(msg);
        }

        #endregion
        [HttpPost]
        public async Task<IActionResult> RegisterAsync()
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Đăng ký thất bại. Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            try
            {
                SystemConfig systemConfig = await cacheUtils.SystemConfig(Lang());
                CustomerModel customerModel = new();
                await TryUpdateModelAsync(customerModel);
                customerModel.Email = Utility.RemoveValidUserName(customerModel.Email);
                customerModel.Phone = Utility.RemoveValidNumber(customerModel.Phone);
                if (_customerManager.CheckExitsEmail(customerModel.Email.ToLower()) == true)
                {
                    msg.Errors = true;
                    msg.Message = "Email đã được đăng ký. Vui lòng sử dụng email khác.";
                    return Ok(msg);
                }
                if (_customerManager.CheckExitsPhone(customerModel.Phone.ToLower()) == true)
                {
                    msg.Errors = true;
                    msg.Message = "Số điện thoại đã được đăng ký. Vui lòng sử dụng số điện thoại khác.";
                    return Ok(msg);
                }
                string saltKey = Utility.CreateSaltKey(5);
                string code = Utility.CreateRandomKey(10);
                string sha1PasswordHash = Utility.CreatePasswordHash(customerModel.Pass, saltKey);
                string codeconfirm = Utility.RandomString(20, false);
                Customer cus = new()
                {
                    UserName = customerModel.Email,
                    FullName = customerModel.Fullname,
                    Email = customerModel.Email,
                    Job = customerModel.Job,
                    Mobile = customerModel.Phone,
                    Gender = customerModel.Gender ?? 0,
                    Address = customerModel.Address,
                    Password = sha1PasswordHash,
                    PasswordSalt = saltKey,
                    ForgotPassCode = code,
                    Code = codeconfirm,
                    IsActivated = false,
                    IsDeleted = false,
                    IsLocked = false,
                    IsReceiveSale = customerModel.Receivesale
                };
                string birth = $"{customerModel.Monthbrth ?? 1}/{customerModel.Daybrth ?? 1}/{customerModel.Yearbrth ?? 1900}";
                cus.Birthday = Convert.ToDateTime(birth);
                int result = _customerManager.Insert(cus);
                if (result > 0)
                {
                    string link = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/confirm?c={codeconfirm}";
                    if (WebConfig.EnableHttps == true)
                    {
                        link = link.Replace("http://", "https://");
                    }
                    string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                    string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                    IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                    CommonJsonItem mainTemplate = new();
                    if (list == null)
                    {
                        msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                        return Ok(msg);
                    }
                    mainTemplate = list.Any(x => x.Code == "TemplateRegister") ? list.FirstOrDefault(x => x.Code == "TemplateRegister") : new CommonJsonItem();
                    if (string.IsNullOrEmpty(mainTemplate.Content))
                    {
                        msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                        return Ok(msg);
                    }
                    str = str.Replace("[Main]", mainTemplate.Content);
                    str = str.Replace("[FullName]", cus.FullName);
                    str = str.Replace("[Email]", cus.Email);
                    str = str.Replace("[Mobile]", cus.Mobile);
                    str = str.Replace("[Gender]", Temp.TextGender(cus.Gender.ToString()));
                    str = str.Replace("[DateOfBirth]", (cus.Birthday.HasValue ? cus.Birthday.Value.ToString("dd/MM/yyyy") : "N/A"));
                    str = str.Replace("[IsReceiveSale]", cus.IsReceiveSale == true ? "Có" : "Không");
                    str = str.Replace("[Link]", link);
                    #region Send mail
                    try
                    {
                        SendEmailModels objEmail = new()
                        {
                            Subject = mainTemplate.Name,
                            DisName = systemConfig.Name,
                            EmailBody = str,
                            EmailSend = systemConfig.EmailCms,
                            Password = systemConfig.PassEmailCms,
                            Port = Convert.ToInt32(systemConfig.Port),
                            Servername = systemConfig.Severname,
                            EnableSSL = systemConfig.SSLEmail ?? false,
                            To = cus.Email
                        };
                        System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmail);
                        msg = new JsonMessage { Errors = false, Email = cus.Email, Message = "Đăng ký thành công. Vui lòng kiểm tra email của bạn để kích hoạt tài khoản.", Css = "text-success" };
                    }
                    catch (Exception e)
                    {
                        msg = new JsonMessage { Errors = true, Message = "Gửi không thành công", Logs = e.Message };
                    }
                    #endregion                   
                }
            }
            catch (Exception e)
            {
                msg.Logs = e.Message;
            }

            return Ok(msg);
        }
        [HttpGet]
        [CheckNoSession]
        public IActionResult Confirm(string c)
        {
            try
            {
                if (!string.IsNullOrEmpty(c))
                {
                    Customer obj = _customerDa.GetByCode(c);
                    if (obj != null)
                    {
                        obj.IsActivated = true;
                        obj.Code = null;
                        int result = _customerDa.Update(obj);
                        return result > 0 ? View() : RedirectToRoute("Error404");
                    }
                    else
                    {
                        return RedirectToRoute("Error404");
                    }
                }
                else
                {
                    return RedirectToRoute("Error404");
                }
            }
            catch
            {
                return RedirectToRoute("Error404");
            }
        }
        [Route("/dang-nhap")]
        [HttpGet]
        public IActionResult Login()
        {
            var returnUrl = TempData["ReturnUrl"]; 
            LoginModel model = new()
            {
                ReturnUrl = returnUrl != null ? returnUrl.ToString() : "/"
            };
            return View(model);
        }
        [Route("/dang-nhap")]
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [CheckNoSession]
        public async Task<IActionResult> LoginAction()
        {
            LoginModel login = new();
            try
            {
                
                await TryUpdateModelAsync(login);
                if (ModelState.IsValid)
                {
                    var captchaResult = await _captchaService.VerifyToken(login.Token);
                    if (!captchaResult)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, vui lòng thử lại");
                        return View(@"~/Views/Member/Login.cshtml", login);
                    }
                    login.Username = Utility.RemoveValidUserName(login.Username);
                    CustomerItem customer = _customerManager.GetCustomerItemByUsername(login.Username);
                    if (customer != null)
                    {
                        string sha1PasswordHash = Utility.CreatePasswordHash(login.Password, customer.PasswordSalt);
                        if (sha1PasswordHash == customer.Password)
                        {
                            SetWebUserID(customer.ID);
                            SetWebUserName(customer.UserName);
                            SetWebFullName(customer.FullName);
                            //ck
                            CookieOptions ck = new()
                            {
                                Expires = DateTime.Now.AddDays(5),
                                Domain = HttpContext.Request.Host.ToString()
                            };
                            Response.Cookies.Append("lsnp", customer.UserName, ck);
                            return Redirect(!string.IsNullOrEmpty(login.ReturnUrl) ? login.ReturnUrl : "/");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, vui lòng thử lại");
                            return View(@"~/Views/Member/Login.cshtml", login);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản này không thế đăng nhập. Vui lòng thử lại sau");
                        return View(@"~/Views/Member/Login.cshtml", login);
                    }
                }             
                else
                {
                    return View(@"~/Views/Member/Login.cshtml", login);
                }   
            }
            catch
            {
                ModelState.AddModelError("", "Đăng nhập không thành công. Vui lòng thử lại sau");
                return View(@"~/Views/Member/Login.cshtml", login);
            }
        }
        [Route("thoat")]
        public ActionResult LogOut()
        {
            TempData.Keep();
            HttpContext.Session.Clear();
            CookieOptions option = new() { Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append("shopping_cart", "", option);
            return Redirect($"/?nocahe={DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}");
        }
        public ActionResult MemberInformation()
        {
            TempData.Keep();
            if (UserId == 0)
            {
                return Redirect("/");
            }
            return View(_customerManager.GetId(UserId.Value));
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public IActionResult EditInformation()
        {
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            try
            {
                TempData.Keep();
                string fullname = Request.Form["fullname"];
                string phone = Request.Form["phone"];
                string gen = Request.Form["gender"];
                string year = Request.Form["yearbrth"];
                string month = Request.Form["monthbrth"];
                string day = Request.Form["daybrth"];
                string allowpass = Request.Form["fixpass"];
                string oldpass = Request.Form["oldpass"];
                string newpass = Request.Form["repass"];
                Customer cus = _customerDa.GetId(UserId.Value);
                if (cus.FullName != fullname)
                {
                    cus.FullName = Utility.RemoveSpecialCharacter(fullname);
                }
                if (cus.Mobile != phone)
                {
                    cus.Mobile = Utility.RemoveValidNumber(phone);
                }
                if (cus.Gender.ToString() != gen && !string.IsNullOrEmpty(gen))
                {
                    cus.Gender = Convert.ToInt32(gen);
                }
                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    string birth = $"{month}/{day}/{year}";
                    cus.Birthday = Convert.ToDateTime(birth);
                }
                if (allowpass == "1")
                {
                    string Oldsha1PasswordHash = Utility.CreatePasswordHash(oldpass, cus.PasswordSalt);
                    if (cus.Password != Oldsha1PasswordHash)
                    {
                        msg = new JsonMessage
                        {
                            Errors = true,
                            Message = "Mật khẩu cũ không đúng"
                        };
                        return Ok(msg);
                    }
                    string sha1PasswordHash = Utility.CreatePasswordHash(newpass, cus.PasswordSalt);
                    if (cus.Password == sha1PasswordHash)
                    {
                        msg = new JsonMessage
                        {
                            Errors = true,
                            Message = "Mật khẩu mới phải khác mật khẩu cũ, "
                        };
                        return Ok(msg);
                    }
                    cus.Password = sha1PasswordHash;
                    cus.ForgotPassCode = null;
                }
                int result = _customerDa.Update(cus);
                if (result > 0)
                {
                    msg = new JsonMessage
                    {
                        Errors = false,
                        Message = "Bạn đã cập nhật thành công, xin cảm ơn.",
                        Css = "text-success"
                    };
                }
            }
            catch
            {
                msg.Message = "Cập nhật không thành công.";
            }
            return Ok(msg);
        }
        public ActionResult ChangePass(int customerId)
        {
            TempData.Keep();
            if (UserId == 0)
            {
                return Redirect("/");
            }
            return View(_customerManager.GetId(customerId));
        }
        public IActionResult ChangePassAction()
        {
            TempData.Keep();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            CustomerModel customerModel = new CustomerModel();
            TryUpdateModelAsync(customerModel);
            Customer customer = _customerDa.GetId(UserId.Value);
            string Oldsha1PasswordHash = Utility.CreatePasswordHash(customerModel.Oldpass, customer.PasswordSalt);
            if (customer.Password != Oldsha1PasswordHash)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = "Mật khẩu cũ không đúng"
                };
                return Ok(msg);
            }
            string sha1PasswordHash = Utility.CreatePasswordHash(customerModel.Pass, customer.PasswordSalt);
            if (customer.Password == sha1PasswordHash)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = "Mật khẩu mới phải khác mật khẩu cũ, "
                };
                return Ok(msg);
            }
            customer.Password = sha1PasswordHash;
            customer.ForgotPassCode = null;
            int result = _customerDa.Update(customer);
            if (result > 0)
            {
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "Bạn đã cập nhật thành công, xin cảm ơn.",
                    Css = "text-success"
                };
            }
            return Ok(msg);
        }
        public IActionResult ChangeMailAction()
        {
            TempData.Keep();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            CustomerModel customerModel = new();
            TryUpdateModelAsync(customerModel);
            Customer customer = _customerDa.GetId(UserId.Value);
            string Oldsha1PasswordHash = Utility.CreatePasswordHash(customerModel.Pass, customer.PasswordSalt);
            if (customer.Password != Oldsha1PasswordHash)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = "Mật khẩu không đúng"
                };
                return Ok(msg);
            }
            if (_customerManager.CheckExitsEmail(customerModel.Email.ToLower()) == true)
            {
                msg.Errors = true;
                msg.Message = "Email đã được đăng ký. Vui lòng sử dụng email khác.";
                return Ok(msg);
            }
            customer.Email = customerModel.Email;
            customer.UserName = customerModel.Email;
            int result = _customerDa.Update(customer);
            if (result > 0)
            {
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "Bạn đã cập nhật email thành công, xin cảm ơn.",
                    Css = "text-success"
                };
            }
            return Ok(msg);
        }
        public IActionResult ChangeSocialAction()
        {
            TempData.Keep();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            CustomerModel customerModel = new();
            TryUpdateModelAsync(customerModel);
            Customer customer = _customerDa.GetId(UserId.Value);
            customer.ZaloId = customerModel.Zalo;
            customer.FaceBookId = customerModel.Fb;
            customer.GoogleId = customerModel.Gg;
            int result = _customerDa.Update(customer);
            if (result > 0)
            {
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "Bạn đã cập nhật thành công, xin cảm ơn.",
                    Css = "text-success"
                };
            }
            return Ok(msg);
        }
        public IActionResult ChangeMobileAction()
        {
            TempData.Keep();
            JsonMessage msg = new()
            {
                Errors = true,
                Message = "Vui lòng thực hiện lại hoặc liên hệ với hệ thống."
            };
            CustomerModel customerModel = new();
            TryUpdateModelAsync(customerModel);
            Customer customer = _customerDa.GetId(UserId.Value);
            string Oldsha1PasswordHash = Utility.CreatePasswordHash(customerModel.Pass, customer.PasswordSalt);
            if (customer.Password != Oldsha1PasswordHash)
            {
                msg = new JsonMessage
                {
                    Errors = true,
                    Message = "Mật khẩu không đúng"
                };
                return Ok(msg);
            }
            customer.Mobile = customerModel.Phone;
            int result = _customerDa.Update(customer);
            if (result > 0)
            {
                msg = new JsonMessage
                {
                    Errors = false,
                    Message = "Bạn đã cập nhật số điện thoại thành công, xin cảm ơn.",
                    Css = "text-success"
                };
            }
            return Ok(msg);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> SendCodeAsync()
        {
            JsonMessage msg = new();
            SystemConfig systemConfig = await cacheUtils.SystemConfig(Lang());
            try
            {
                string email = Request.Form["email"];
                email = Utility.RemoveValidUserName(email);
                Customer obj = _customerDa.GetByEmail(email.ToString().Trim());
                if (obj != null)
                {
                    string codeconfirm = Utility.RandomString(15, false);
                    obj.Code = codeconfirm;
                    obj.ExpiredTime = DateTime.Now;
                    int result = _customerDa.Update(obj);
                    if (result > 0)
                    {
                        string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                        string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                        IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                        CommonJsonItem mainTemplate = new();
                        if (list == null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                            return Ok(msg);
                        }
                        mainTemplate = list.Any(x => x.Code == "TemplateForgot") ? list.FirstOrDefault(x => x.Code == "TemplateForgot") : new CommonJsonItem();
                        if (string.IsNullOrEmpty(mainTemplate.Content))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                            return Ok(msg);
                        }
                        str = str.Replace("[Main]", mainTemplate.Content);
                        str = str.Replace("[Code]", codeconfirm);
                        #region Send mail
                        try
                        {
                            SendEmailModels objEmail = new()
                            {
                                Subject = mainTemplate.Name,
                                DisName = systemConfig.Name,
                                EmailBody = str,
                                EmailSend = systemConfig.EmailCms,
                                Password = systemConfig.PassEmailCms,
                                Port = Convert.ToInt32(systemConfig.Port),
                                Servername = systemConfig.Severname,
                                EnableSSL = systemConfig.SSLEmail ?? false,
                                To = email
                            };
                            System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmail);
                            msg = new JsonMessage { Errors = false, Email = email, Message = "Gửi thành công. Vui lòng kiểm tra meail của bạn." };
                        }
                        catch (Exception e)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi không thành công", Logs = e.Message };
                        }
                        #endregion

                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
                    }
                }
                else
                {
                    msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
                }
            }
            catch
            {
                msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
            }
            return Ok(msg);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<IActionResult> ForgotPasswordActionAsync()
        {
            JsonMessage msg = new();
            SystemConfig systemConfig = await cacheUtils.SystemConfig(Lang());
            try
            {
                string email = Request.Form["Email"];
                string code = Request.Form["code"];
                string newpassword = Request.Form["pass"];
                email = Utility.RemoveValidUserName(email);
                code = Utility.RemoveSpecialCharacter(code);
                Customer obj = _customerDa.GetByEmail(email.ToString().Trim());
                if (obj != null)
                {
                    TimeSpan span = new();
                    if (obj.ExpiredTime.HasValue)
                    {
                        span = DateTime.Now - obj.ExpiredTime.Value;
                    }
                    if (obj.Code != code)
                    {
                        msg = new JsonMessage { Errors = true, Message = "Mã xác nhận không đúng" };
                        return Ok(msg);
                    }
                    else if (obj.Code == code && span.TotalMinutes > 5)
                    {
                        msg = new JsonMessage { Errors = true, Message = "Mã xác nhận không thể sử dụng" };
                        return Ok(msg);
                    }
                    string sha1PasswordHash = Utility.CreatePasswordHash(newpassword, obj.PasswordSalt);
                    obj.Password = sha1PasswordHash;
                    obj.Code = null;
                    obj.ExpiredTime = null;
                    var result = _customerDa.Update(obj);
                    if (result > 0)
                    {
                        string str = await Common.ReadFileAsync("LayoutEmail.htm", "html/Configuaration");
                        string json = await ReadFileAsync("TemplateEmail.json", "DataJson");
                        IEnumerable<CommonJsonItem> list = JsonConvert.DeserializeObject<IEnumerable<CommonJsonItem>>(json);
                        CommonJsonItem mainTemplate = new();
                        if (list == null)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                            return Ok(msg);
                        }
                        mainTemplate = list.Any(x => x.Code == "TemplateReset") ? list.FirstOrDefault(x => x.Code == "TemplateReset") : new CommonJsonItem();
                        if (string.IsNullOrEmpty(mainTemplate.Content))
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi thất bại" };
                            return Ok(msg);
                        }
                        str = str.Replace("[Main]", mainTemplate.Content);
                        #region Send mail
                        try
                        {
                            SendEmailModels objEmail = new()
                            {
                                Subject = mainTemplate.Name,
                                DisName = systemConfig.Name,
                                EmailBody = str,
                                EmailSend = systemConfig.EmailCms,
                                Password = systemConfig.PassEmailCms,
                                Port = Convert.ToInt32(systemConfig.Port),
                                Servername = systemConfig.Severname,
                                EnableSSL = systemConfig.SSLEmail ?? false,
                                To = email
                            };
                            System.Threading.ThreadPool.QueueUserWorkItem(SendEmailModels.SendmailKit, objEmail);
                            msg = new JsonMessage { Errors = false, Message = "Lấy lại mật khẩu thành công" };
                        }
                        catch (Exception e)
                        {
                            msg = new JsonMessage { Errors = true, Message = "Gửi không thành công", Logs = e.Message };
                        }
                        #endregion
                    }
                    else
                    {
                        msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
                    }
                }
                else
                {
                    msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
                }
            }
            catch
            {
                msg = new JsonMessage { Errors = true, Message = "Gửi không thành công" };
            }
            return Ok(msg);
        }
    }
}