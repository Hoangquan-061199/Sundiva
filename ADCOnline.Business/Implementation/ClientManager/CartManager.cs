using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using ADCOnline.Simple.Item;
using System.Linq;
using System.Text;
using System;
using ADCOnline.Utils;
using ADCOnline.Simple.Base;
using Newtonsoft.Json;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class CartManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public CartManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public int InsertOrder(Order obj)
        {
            obj.CreatedDate = DateTime.Now;
            obj.Status = 1;
            return _dapperDa.Insert(obj);
        }
        public int InsertOrderDetail(OrderDetail obj) => _dapperDa.Insert(obj);
        public ProductItem GetById(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT * FROM Product where IsDeleted = 0 AND IsShow =1 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<OrderDetail> GetListOrderDetailByListOrderIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<OrderDetail> result = connect.Query<OrderDetail>("SELECT * FROM OrderDetail where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,' + Convert(varchar,OrderID) + ',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public Order GetOrderByCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<Order> result = connect.Query<Order>("SELECT * FROM [Order] where IsDeleted = 0 AND OrderCode = @code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Order GetOrderByCodePageSucces(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<Order> result = connect.Query<Order>("SELECT * FROM [Order] where IsDeleted = 0 AND OrderCode = @code And IsCancerByCustomer = 0", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ProductItem> GetListProductByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT * FROM Product where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10),ID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Product> GetListProductsByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<Product> result = connect.Query<Product>("SELECT * FROM Product where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10),ID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public WebsiteModulesItem GetByNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT m.*,p.StartDate,p.EndDate,p.SaleValue,p.IsDeleted IsDeletedSale, p.IsShow IsShowSale FROM WebsiteModule m left join Promotion p on m.PromotionID = p.ID " +
                "where m.IsDeleted = 0 AND m.IsShow = 1 AND m.NameAscii=@nameAscii", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }                
        public decimal SumPrice = 0;//tong ban dau chua co chuong trinh kmai hoac ma giam gia
        public decimal SumVATAll = 0; // Tổng VAT
        public decimal DiscountCombo = 0;
        public decimal SumPriceAfterDisCountModule = 0; //tong sau khi giam gia chuong trinh kmai
        public decimal DiscountMoudle = 0;//so tien giam gia chuong trinh kmai
        public decimal SumPriceAfterDisCountVoucher = 0;//tong sau khi giam gia theo ma giam gia
        public decimal DiscountVoucher = 0;//so tien giam gia theo ma giam gia
        public decimal SumPriceAfterDisCountAll = 0;//tong sau cung
        public bool IsApplyOtherCampaign = false;
        public List<CartItem> GetCartData(string cartCookie, string vorchercode)
        {
            List<CartItem> listC = new List<CartItem>();
            string cookieValue = cartCookie;
            if (cookieValue != null)
            {
                //%2c97-2&48
                //%2c97-2&49
                //%2c97-1&46
                List<int> IdsIn = IdsInCart(cookieValue);
                string[] list = System.Net.WebUtility.UrlDecode(cookieValue).Split(',');
                decimal sumP = 0;
                decimal sumVAT = 0;
                decimal sumPafterDiscount = 0;
                decimal discount = 0;
                decimal discountCombo = 0;
                foreach (string data in list.Where(c => !string.IsNullOrEmpty(c)))
                {
                    string[] spPaA = data.Split('&');
                    string proPart = spPaA[0]; //proid + quantity
                    string attrPart = spPaA[1]; //attr
                    string[] temp = spPaA[0].Split('-');
                    if (temp.Length == 2)
                    {
                        int idProduct = Convert.ToInt32(temp[0]);
                        ProductItem productItem = GetById(idProduct);
                        CartItem objCart = new CartItem();
                        if (productItem != null)
                        {
                            objCart = new CartItem
                            {
                                ProductId = productItem.ID,
                                Status =productItem.Status,
                                ProductName = productItem.Name,
                                ImageProduct = productItem.UrlPicture,
                                ProductCode = productItem.ProductCode,
                                NameAscci = productItem.NameAscii,
                                ViewHome = productItem.ViewHome,
                                GiftIds = productItem.GiftIds,
                                PromotionText = productItem.PromotionText,
                                ModuleIds = productItem.ModuleIds,
                                IsVAT = productItem.IsVAT
                            };
                            double quan = Convert.ToDouble(temp[1]);
                            objCart.Quantity = quan; // so luong                              
                        }
                        // kmai theo chuong trinh khuyen mai
                        WebsiteModulesItem moduleM = GetByNameAscii(productItem.ModuleNameAscii);
                        List<int> ListIds = ListHelper.GetValuesArray(productItem.ModuleIds);
                        if (spPaA.Length == 2 && !string.IsNullOrEmpty(attrPart))
                        {
                            List<int> idValues = ListHelper.GetValuesArrayBySymbol(attrPart, '-');
                            if (idValues.Any())
                            {
                                List<Attribute_WebsiteContentItem> attroptioncontent = GetAttributeWebsiteContentItemByListAttrIdsAndProductId(string.Join(",", idValues.Select(c => c.ToString())), productItem.ID);
                                //AttributeOptionItems

                                List<AttributeItem> attrs = GetAttributeByListIds(string.Join(",", idValues.Select(c => c.ToString())));
                                //find parent
                                List<AttributeItem> attrsP = GetAttributeByListIds(string.Join(",", attrs.Where(c => c.ParentID != 0).Select(c => c.ParentID.Value)));
                                attrs.AddRange(attrsP);
                                decimal priceAll = (productItem.IsShowPrice == true ? (productItem.Price ?? 0) : 0);
                                decimal priceAllOld = (productItem.IsShowPrice == true ? (productItem.PriceOld ?? 0) : 0);
                                foreach (Attribute_WebsiteContentItem item in attroptioncontent)
                                {
                                    priceAll += item.Price.Value;
                                    priceAllOld += item.Price.Value;
                                }                                
                                objCart.AttributeItems = attrs;
                                objCart.PriceProduct = priceAll;
                                objCart.PriceOrigin = priceAllOld;
                                objCart.SumMoney = priceAll * Convert.ToDecimal(objCart.Quantity);
                                objCart.VATPrice = 0;
                                objCart.VATPrice = objCart.IsVAT != true && objCart.SumMoney > 0 ? (objCart.SumMoney * 10 / 100) : 0;
                                //objCart.DiscountModuleItem = ((objCart.SumMoney + objCart.VATPrice) * percentModuleThis) / 100;
                                objCart.TotalAfterSaleModule = objCart.SumMoney + objCart.VATPrice - objCart.DiscountModuleItem;
                            }
                        }
                        else
                        {
                            objCart.AttributeItems = new List<AttributeItem>();
                            objCart.PriceProduct = productItem.IsShowPrice == true ? (productItem.Price ?? 0) : 0;
                            objCart.PriceOrigin = productItem.IsShowPrice == true ? (productItem.PriceOld ?? 0) : 0;                            
                            objCart.PriceProduct = productItem.Price.Value;
                            objCart.SumMoney = objCart.PriceProduct * Convert.ToDecimal(objCart.Quantity);
                            objCart.VATPrice = 0;
                            objCart.VATPrice = objCart.IsVAT != true && objCart.SumMoney > 0 ? (objCart.SumMoney * 10 / 100) : 0;
                            //objCart.DiscountModuleItem = ((objCart.SumMoney + objCart.VATPrice) * percentModuleThis) / 100;
                            objCart.TotalAfterSaleModule = objCart.SumMoney + objCart.VATPrice - objCart.DiscountModuleItem;
                        }
                        sumP += objCart.SumMoney;
                        sumVAT += objCart.VATPrice;
                        discount = discount + objCart.DiscountModuleItem ?? 0;
                        if (CheckCombo(productItem.AttachedProductIds, IdsIn) == true)
                        {
                            discountCombo += productItem.DiscountAmount.HasValue ? productItem.DiscountCombo.Value : 0;
                        }
                        sumPafterDiscount = sumP + sumVAT - discount - discountCombo;
                        listC.Add(objCart);
                    }
                }
                SumPrice = sumP;
                SumVATAll = sumVAT;
                SumPriceAfterDisCountModule = sumPafterDiscount;
                DiscountMoudle = discount;
                DiscountCombo = discountCombo;
                //khuyen mai theo ma giam gia
                //decimal tempsumP = 0;
                decimal TotalWithVAT = sumP + sumVAT;                
            }
            return listC;
        }
        public List<AttributeItem> GetAttributeByListIds(string listArray)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AttributeItem> result = connect.Query<AttributeItem>("SELECT * FROM Attributes where IsDeleted = 0 AND IsShow = 1 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { listArray });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Attribute_WebsiteContentItem> GetAttributeWebsiteContentItemByListAttrIdsAndProductId(string listAttrIds, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<Attribute_WebsiteContentItem> result = connect.Query<Attribute_WebsiteContentItem>("SELECT * FROM Attribute_WebsiteContent where AttributeID is not null and ContentID is not null and ContentID=@productId AND ',' + @listAttrIds + ',' LIKE '%,'+CONVERT(varchar(10), AttributeID)+',%' Order By ID desc", new { listAttrIds, productId });
                connect.Close();
                return result.ToList();
            }
        }
        private List<int> IdsInCart(string cookieValue)
        {
            List<int> ids = new List<int>();
            if (cookieValue != null)
            {
                string[] list = System.Net.WebUtility.UrlDecode(cookieValue).Split(',');
                foreach (string data in list.Where(c => !string.IsNullOrEmpty(c)))
                {
                    string[] spPaA = data.Split('&');
                    string proPart = spPaA[0];
                    string[] temp = spPaA[0].Split('-');
                    if (temp.Length == 2)
                    {
                        ids.Add(Convert.ToInt32(temp[0]));
                    }
                }
            }
            return ids;
        }
        public bool CheckCombo(string ids, List<int> cart)
        {
            if (!string.IsNullOrEmpty(ids) && cart.Count>0)
            {
                List<int> listId = ListHelper.GetValuesArray(ids);
                foreach (int item in listId)
                {
                    if (!cart.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public decimal GetSumPrice(string cartCookie) => SumPrice;
        public bool GetIsApplyOtherCampaign(string cartCookie) => IsApplyOtherCampaign;
        public decimal GetSumPriceAfterDiscountModule(string cartCookie) => SumPriceAfterDisCountModule;
        public decimal GetDiscountMoudle(string cartCookie) => DiscountMoudle;
        public decimal GetDiscountCombo(string cartCookie) => DiscountCombo;
        public decimal GetDiscountVoucher(string cartCookie) => DiscountVoucher;
        public decimal GetTotalVAT(string cartCookie) => SumVATAll;
        public decimal GetSumPriceAfterDiscountAll(string cartCookie)
        {
            if (DiscountVoucher != 0) // neu co chuogn trinh voucher
            {
                return IsApplyOtherCampaign == true
                    ? SumPriceAfterDisCountModule - DiscountVoucher - DiscountCombo
                    : (SumPrice + SumVATAll) - DiscountVoucher - DiscountCombo;
            }
            else
            {
                return SumPriceAfterDisCountModule;
            }
        }

        public async Task<Dictionary<string, CityJson>> GetAllCity(string str) => JsonConvert.DeserializeObject<Dictionary<string, CityJson>>(await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}"));
        public async Task<Dictionary<string, DistrictJson>> GetDistrictById(string str, string id)
        {
            Dictionary<string, DistrictJson> district = JsonConvert.DeserializeObject<Dictionary<string, DistrictJson>>(await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}"));
            return district.Where(x => x.Value.parent_code.Equals(id)).ToDictionary(x => x.Key, x => x.Value);
        }
        public async Task<Dictionary<string, WardJson>> GetWardById(string str, string id)
        {
            string Str = await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}");
            Dictionary<string, WardJson> district = JsonConvert.DeserializeObject<Dictionary<string, WardJson>>(Str);
            return district.Where(x => x.Value.parent_code.Equals(id)).ToDictionary(x => x.Key, x => x.Value);
        }
        public async Task<WardJson> GetWardByWardId(string str, string id)
        {
            string Str = await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}");
            Dictionary<string, WardJson> district = JsonConvert.DeserializeObject<Dictionary<string, WardJson>>(Str);
            return district.Where(x => x.Value.code.Equals(id)).FirstOrDefault().Value;
        }
        public async Task<CityJson> GetCityByCityId(string str, string id)
        {
            string Str = await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}");
            Dictionary<string, CityJson> district = JsonConvert.DeserializeObject<Dictionary<string, CityJson>>(Str);
            return district.Where(x => x.Value.code.Equals(id)).FirstOrDefault().Value;
        }
        public async Task<DistrictJson> GetDistrictByDistrictId(string str, string id)
        {
            string Str = await Utility.GetDataTemplate($"{System.IO.Directory.GetCurrentDirectory()}/wwwroot/DataJson/{str}");
            Dictionary<string, DistrictJson> district = JsonConvert.DeserializeObject<Dictionary<string, DistrictJson>>(Str);
            return district.Where(x => x.Value.code.Equals(id)).FirstOrDefault().Value;
        }
    }
}
