using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using ADCOnline.Simple.Item;
using System.Linq;
using System.Text;
using ADCOnline.Utils;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class ProductManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ProductManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public ProductDetail GetByNameAscii(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductDetail> result = connect.Query<ProductDetail>("select *," +
                    " (select Count(a.ID) from Comment a where a.IsDeleted = 0 and a.IsShow = 1 and a.IsApproved = 1 and a.Act = 'Rate' and a.ParentID is null and a.Rate = 5 and a.ProductID = p.ID) Rate5," +
                " (select Count(b.ID) from Comment b where b.IsDeleted = 0 and b.IsShow = 1 and b.IsApproved = 1 and b.Act = 'Rate' and b.ParentID is null and b.Rate = 4 and b.ProductID = p.ID) Rate4," +
                " (select Count(c.ID) from Comment c where c.IsDeleted = 0 and c.IsShow = 1 and c.IsApproved = 1 and c.Act = 'Rate' and c.ParentID is null and c.Rate = 3 and c.ProductID = p.ID) Rate3," +
                " (select Count(d.ID) from Comment d where d.IsDeleted = 0 and d.IsShow = 1 and d.IsApproved = 1 and d.Act = 'Rate' and d.ParentID is null and d.Rate = 2 and d.ProductID = p.ID) Rate2," +
                " (select Count(e.ID) from Comment e where e.IsDeleted = 0 and e.IsShow = 1 and e.IsApproved = 1 and e.Act = 'Rate' and e.ParentID is null and e.Rate = 1 and e.ProductID = p.ID) Rate1" +
                " from Product p where p.IsDeleted = 0 and p.IsShow = 1 and p.NameAscii = @NameAscii AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE() And IsApproved = 1 Order By p.ID Desc", new { NameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ProductItem> GetListProduceProcess(string moduleids, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("select ID, Name, Content, UrlPicture, CreatedDate from Product where IsDeleted = 0 and IsShow = 1 and Lang = @lang and ModuleIds = @moduleids", new { moduleids, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListByListProductGroupCode(string codes)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductItem>("SELECT ID,Name,NameAscii,ModuleNameAscii,UrlPicture,LinkUrl,Price,PriceOld,IsShowPrice FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 AND PublishDate <= GETDATE() And ',' + @codes + ',' LIKE '%,'+ProductGroupCode+',%'", new { codes });
                connect.Close();
                return result.ToList();
            }
        }
        public ProductDetail GetByNameAsciiQuickView(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductDetail> result = connect.Query<ProductDetail>("select *," +
                    " (select Count(a.ID) from Comment a where a.IsDeleted = 0 and a.IsShow = 1 and a.IsApproved = 1 and a.Act = 'Rate' and a.ParentID is null and a.Rate = 5 and a.ProductID = p.ID) Rate5," +
                " (select Count(b.ID) from Comment b where b.IsDeleted = 0 and b.IsShow = 1 and b.IsApproved = 1 and b.Act = 'Rate' and b.ParentID is null and b.Rate = 4 and b.ProductID = p.ID) Rate4," +
                " (select Count(c.ID) from Comment c where c.IsDeleted = 0 and c.IsShow = 1 and c.IsApproved = 1 and c.Act = 'Rate' and c.ParentID is null and c.Rate = 3 and c.ProductID = p.ID) Rate3," +
                " (select Count(d.ID) from Comment d where d.IsDeleted = 0 and d.IsShow = 1 and d.IsApproved = 1 and d.Act = 'Rate' and d.ParentID is null and d.Rate = 2 and d.ProductID = p.ID) Rate2," +
                " (select Count(e.ID) from Comment e where e.IsDeleted = 0 and e.IsShow = 1 and e.IsApproved = 1 and e.Act = 'Rate' and e.ParentID is null and e.Rate = 1 and e.ProductID = p.ID) Rate1" +
                " from Product p where p.IsDeleted = 0 and p.IsShow = 1 and p.NameAscii = @NameAscii AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE() Order By p.ID Desc", new { NameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public ProductItem GetById(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("select *," +
                    " (select Count(a.ID) from Comment a where a.IsDeleted = 0 and a.IsShow = 1 and a.IsApproved = 1 and a.Act = 'Rate' and a.ParentID is null and a.Rate = 5 and a.ProductID = p.ID) Rate5," +
                " (select Count(b.ID) from Comment b where b.IsDeleted = 0 and b.IsShow = 1 and b.IsApproved = 1 and b.Act = 'Rate' and b.ParentID is null and b.Rate = 4 and b.ProductID = p.ID) Rate4," +
                " (select Count(c.ID) from Comment c where c.IsDeleted = 0 and c.IsShow = 1 and c.IsApproved = 1 and c.Act = 'Rate' and c.ParentID is null and c.Rate = 3 and c.ProductID = p.ID) Rate3," +
                " (select Count(d.ID) from Comment d where d.IsDeleted = 0 and d.IsShow = 1 and d.IsApproved = 1 and d.Act = 'Rate' and d.ParentID is null and d.Rate = 2 and d.ProductID = p.ID) Rate2," +
                " (select Count(e.ID) from Comment e where e.IsDeleted = 0 and e.IsShow = 1 and e.IsApproved = 1 and e.Act = 'Rate' and e.ParentID is null and e.Rate = 1 and e.ProductID = p.ID) Rate1" +
                " from Product p where 1 = 1 And p.ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public ProductDetail GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductDetail> result = connect.Query<ProductDetail>("select * from Product where 1 = 1 And ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<SubItem> GetSubItemByProductId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<SubItem> result = connect.Query<SubItem>("select * from SubItem where 1=1 And IsDeleted = 0 and IsShow = 1 and ProductID = @id Order By ID Desc", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteProductItemJson> GetViewedProduct(List<int> ids)
        {
            if (ids.Count(x => x != 0) > 0)
            {
                DataTable TableIds = new DataTable();
                TableIds.Columns.Add("ID");
                TableIds.Columns.Add("OrderIndex");
                for (int i = 0; i < ids.Count; i++)
                {
                    TableIds.Rows.Add(ids[i], (ids.Count - i));
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("select p.* from Product p inner join @TableIds i on p.ID = i.ID order by i.OrderIndex asc", new { TableIds = TableIds.AsTableValuedParameter("[dbo].[KeySort]") });
                    connect.Close();
                    return result.ToList();
                }
                #region Remove
                //StringBuilder sql = new StringBuilder();
                //sql.Append("select c.*,x.ordering OrderDisplay from Product c");
                //sql.Append(" join ( values");
                //for (int i = 0; i < ids.Count; i++)
                //{
                //    if (i == (ids.Count - 1))
                //    {
                //        sql.Append("(" + ids[i] + "," + (ids.Count - i) + ")");
                //    }
                //    else
                //    {
                //        sql.Append("(" + ids[i] + "," + (ids.Count - i) + "),");
                //    }
                //}
                //sql.Append(") as x (id, ordering) on c.id = x.id order by x.ordering");
                //return _dapperDa.Select<WebsiteProductItemJson>(sql.ToString()).ToList();
                #endregion
            }
            else
            {
                return new List<WebsiteProductItemJson>();
            }
        }
        public List<ProductItem> GetProductXml(int p, int pagesize = 10)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("select ProductCode,Name, " +
                "(select m.Name from WebsiteModule m where m.IsDeleted = 0 and m.IsShow = 1 and m.NameAscii = p.ModuleNameAscii) ModuleName, " +
                "Description, PromotionText, Status, UrlPicture,Price,PriceOld, NameAscii,ModuleNameAscii from " +
                "Product p where IsDeleted = 0 and IsShow = 1 and CreatedDate <= GETDATE() And PublishDate <= GETDATE() and LinkUrl is null order by ID desc OFFSET @start ROWS FETCH NEXT @size ROWS ONLY", new { @start = (p - 1) * pagesize, @size = pagesize });
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //string Sql = "select ProductCode,Name, " +
            //    "(select m.Name from WebsiteModule m where m.IsDeleted = 0 and m.IsShow = 1 and m.NameAscii = p.ModuleNameAscii) ModuleName, " +
            //    "Description, PromotionText, Status, UrlPicture,Price,PriceOld, NameAscii,ModuleNameAscii from " +
            //    "Product p where IsDeleted = 0 and IsShow = 1 and CreatedDate <= GETDATE() And PublishDate <= GETDATE() and LinkUrl is null order by ID desc OFFSET " + ((p - 1) * pagesize) + " ROWS FETCH NEXT " + pagesize + " ROWS ONLY";
            //return _dapperDa.Select<ProductItem>(Sql).ToList();
            #endregion
        }
        public int CountAllProduct()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<int> result = connect.Query<int>("select Count(ID) from Product where IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and PublishDate <= GETDATE() and LinkUrl is null");
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public async Task<IEnumerable<ProductItem>> GetListByArrId(string listArray)
        {
            listArray = "'," + listArray + ",'";
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = await connect.QueryAsync<ProductItem>("SELECT * FROM Product where IsDeleted = 0 AND IsShow =1" +
                " AND @Arr LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { Arr = listArray });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListProductWattage(string productGroupCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("select ID, Name, Wattage, NameAscii, LinkUrl from Product where IsDeleted = 0 and IsShow = 1 and ProductGroupCode = @productGroupCode order by Wattage desc", new
                {
                    productGroupCode
                });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<WebsiteProductItemJson>> GetListByArrIdJson(string listArray)
        {
            listArray = "'," + listArray + ",'";
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteProductItemJson> result = await connect.QueryAsync<WebsiteProductItemJson>("SELECT * FROM Product where IsDeleted = 0 AND IsShow =1 And @Arr LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { Arr = listArray });
                connect.Close();
                return result.ToList();
            }
        }
        public List<SubItem> GetSubItemByContentId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SubItem>("select * from SubItem where IsDeleted = 0 and IsShow = 1 and ContentID = @id Order By ID Desc", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteProductItemJson> GetListProductViewedPage(SearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            if (!string.IsNullOrEmpty(search.productids))
            {
                List<int> ids = ListHelper.GetValuesArray(search.productids);
                sql.Append("select COUNT(c.ID) OVER () AS TotalRecord, c.*,x.ordering OrderDisplay from Product c");
                sql.Append(" join ( values");
                for (int i = 0; i < ids.Count; i++)
                {
                    if (i == (ids.Count - 1))
                    {
                        sql.Append("(" + ids[i] + "," + (ids.Count - i) + ")");
                    }
                    else
                    {
                        sql.Append("(" + ids[i] + "," + (ids.Count - i) + "),");
                    }
                }
                sql.Append(") as x (id, ordering) on c.id = x.id order by x.ordering");
                sql.Append(" OFFSET " + ((search.page - 1) * search.pagesize) + " ROWS FETCH NEXT " + search.pagesize + " ROWS ONLY");
                return _dapperDa.Select<WebsiteProductItemJson>(sql.ToString()).ToList();
            }
            else
            {
                return new List<WebsiteProductItemJson>();
            }

        }
        public decimal? MaxPrice(SearchModel search, int moduleId = 0, string listChidrent = "")
        {
            try
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    DynamicParameters paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        search.lang
                    });
                    IEnumerable<decimal?> result = connect.Query<decimal?>("dbo.GetMaxPrice", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result != null ? result.FirstOrDefault() : 0;
                }
                #region Remove
                //StringBuilder where = new StringBuilder();
                //if (moduleId > 0)
                //{
                //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                //    lstId.Add(moduleId.ToString());
                //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds"));
                //    var result = _dapperDa.Select<decimal>(string.Format("SELECT MAX(p.Price) FROM Product p WHERE p.ID NOT IN(0) And IsDeleted = 0 AND IsShow = 1 And Price is not null And Price > 0 And IsApproved = 1 AND Lang = '{0}'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE()", search.lang, where.ToString())).ToList();
                //    return result != null ? result.FirstOrDefault() : 0;
                //}
                //else
                //{
                //    return 0;
                //}
                #endregion
            }
            catch
            {
                return 0;
            }
        }
        public decimal? MinPrice(SearchModel search, int moduleId = 0, string listChidrent = "")
        {
            try
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    var lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    DynamicParameters paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        search.lang
                    });
                    IEnumerable<decimal?> result = connect.Query<decimal?>("dbo.GetMinPrice", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result != null ? result.FirstOrDefault() : 0;
                }
                #region Remove
                //StringBuilder where = new StringBuilder();
                //if (moduleId > 0)
                //{
                //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                //    lstId.Add(moduleId.ToString());
                //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds"));
                //    string sql = string.Format("SELECT MIN(p.Price) FROM Product p  WHERE p.ID NOT IN(0) And IsDeleted = 0 AND IsShow = 1 And Price is not null And Price > 0 And IsApproved = 1 AND Lang = '{0}'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE()", search.lang, where.ToString());
                //    List<decimal> result = _dapperDa.Select<decimal>(sql).ToList();
                //    return result != null ? result.FirstOrDefault() : 0;
                //}
                //else
                //{
                //    return 0;
                //}
                #endregion
            }
            catch
            {
                return 0;
            }
        }
        public List<ProductItem> GetListProduct(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "", string contentIds = "0")
        {
            try
            {
                decimal? giafrom = null, giato = null;
                DataTable ModuleIds = new();
                ModuleIds.Columns.Add("KeyValue");
                DataTable AttrIds = new();
                AttrIds.Columns.Add("ID");
                AttrIds.Columns.Add("KeyItem");
                DataTable HsxIds = new();
                HsxIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    var lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                if (search.ListAttr != null && search.ListAttr.Count > 0)
                {
                    int i = 1;
                    foreach (var item in search.ListAttr)
                    {
                        AttrIds.Rows.Add(i, item);
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(search.hsx))
                {
                    List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
                    if (hsx.Any())
                    {
                        hsx.ForEach(x =>
                        {
                            HsxIds.Rows.Add(x);
                        });
                    }
                }
                if (!string.IsNullOrEmpty(search.gia))
                {
                    List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
                    decimal from = ConvertUtil.ToDecimal(giaids[0]);
                    decimal to = ConvertUtil.ToDecimal(giaids[1]);
                    if (from > 0) giafrom = Math.Abs(from * 1000000);
                    if (to > 0) giato = Math.Abs(to * 1000000);
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    search.page = search.page > 1 ? search.page : 1;
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @AttrIds = AttrIds.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @HsxIds = HsxIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @giafrom = (int?)giafrom,
                        @giato = (int?)giato,
                        search.sort,
                        search.lang,
                        contentIds,
                        @start = (search.page - 1) * pageZise,
                        @size = pageZise
                    });
                    var result = connect.Query<ProductItem>("dbo.GetListProduct", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return new List<ProductItem>();
            }
        }
        public async Task<IEnumerable<ProductItem>> GetListProductTour(SearchModel search)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    search.page = search.page > 1 ? search.page : 1;
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @keyword = search.q,
                        @TypeTour = search.TourType,
                        @AddressStart = search.AddressStart,
                        @Times = search.Times,
                        @lang = search.lang,
                        @start = (search.page - 1) * search.pagesize,
                        @size = search.pagesize
                    });
                    var result = await connect.QueryAsync<ProductItem>("[dbo].[GetListTours]", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            } catch
            {
                return new List<ProductItem>();
            }
        }
        public List<ColorItem> GetListColorProductSub(SearchModel search)
        {
            try
            {
                DataTable AttrIds = new();
                AttrIds.Columns.Add("ID");
                AttrIds.Columns.Add("KeyItem");
                if (search.ListAttr != null && search.ListAttr.Count > 0)
                {
                    int i = 1;
                    foreach (var item in search.ListAttr)
                    {
                        AttrIds.Rows.Add(i, item);
                        i++;
                    }
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @AttrIds = AttrIds.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        search.lang,
                    });
                    var result = connect.Query<ColorItem>("[dbo].[GetListColorProductSub]", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return new List<ColorItem>();
            }
        }
        public async Task<IEnumerable<WebsiteProductItemJson>> GetListProductJson(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "", string contentIds = "0")
        {
            try
            {
                decimal? giafrom = null, giato = null;
                DataTable ModuleIds = new();
                ModuleIds.Columns.Add("KeyValue");
                DataTable AttrIds = new();
                AttrIds.Columns.Add("ID");
                AttrIds.Columns.Add("KeyItem");
                DataTable HsxIds = new();
                HsxIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    var lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                if (search.ListAttr != null && search.ListAttr.Count > 0)
                {
                    int i = 1;
                    foreach (var item in search.ListAttr)
                    {
                        AttrIds.Rows.Add(i, item);
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(search.hsx))
                {
                    List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
                    if (hsx.Any())
                    {
                        hsx.ForEach(x =>
                        {
                            HsxIds.Rows.Add(x);
                        });
                    }
                }
                if (!string.IsNullOrEmpty(search.gia))
                {
                    List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
                    decimal from = ConvertUtil.ToDecimal(giaids[0]);
                    decimal to = ConvertUtil.ToDecimal(giaids[1]);
                    if (from > 0) giafrom = Math.Abs(from * 1000000);
                    if (to > 0) giato = Math.Abs(to * 1000000);
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    search.page = search.page > 1 ? search.page : 1;
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @AttrIds = AttrIds.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @HsxIds = HsxIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @giafrom = (int?)giafrom,
                        @giato = (int?)giato,
                        search.sort,
                        search.lang,
                        contentIds,
                        @start = (search.page - 1) * pageZise,
                        @size = pageZise
                    });
                    var result = await connect.QueryAsync<WebsiteProductItemJson>("dbo.GetListProduct", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result;
                }
            }
            catch
            {
                return null;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //List<string> key = new List<string>();
            //if (!string.IsNullOrEmpty(search.q))
            //{
            //    key = search.q.Trim().ToLower().Split(' ').ToList();
            //}
            //if (key.Count > 0)
            //{
            //    List<string> listColum = new List<string>();
            //    listColum.Add(nameof(WebsiteProductItemJson.Name));
            //    listColum.Add(nameof(WebsiteProductItemJson.NameAscii));
            //    where.Append(SqlUtility.WhereOrNLikeList(key, listColum));
            //}
            //string order = " ORDER BY OrderDisplay Asc";
            //if (search.sort == 0)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,0,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 1)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,2,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 2)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,5,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 3)
            //{
            //    order = " ORDER BY Price Desc,OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 4)
            //{
            //    order = " ORDER BY Price Asc,OrderDisplay asc, ID desc";
            //}
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    string other = "";
            //    if (search.brandId > 0)
            //    {
            //        other = " Or BrandId = " + search.brandId;
            //    }
            //    where.Append(SqlUtility.WhereOrLikeListWithOther(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds", other));
            //    if (search.ListAttr != null && search.ListAttr.Count > 0)
            //    {
            //        foreach (var item in search.ListAttr)
            //        {
            //            List<string> ids = ListHelper.GetValuesArrayTag(item);
            //            where.Append(SqlUtility.WhereOrLikeList(ids.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.AttributeProductIds"));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.gia))
            //    {
            //        List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
            //        decimal from = ConvertUtil.ToDecimal(giaids[0]);
            //        decimal to = ConvertUtil.ToDecimal(giaids[1]);
            //        if (from == to)
            //        {
            //            where.Append(" And p.Price=" + from);
            //        }
            //        else if (from > 0 && to == 0)
            //        {
            //            where.Append(" And p.Price >= " + from);
            //        }
            //        else
            //        {
            //            where.Append(" And p.Price>=" + from);
            //            where.Append(" And p.Price<=" + to);
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.hsx))
            //    {
            //        List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
            //        if (hsx.Count > 0)
            //        {
            //            where.Append(" And (");
            //            int i = 1;
            //            foreach (var item in hsx)
            //            {
            //                if (i < hsx.Count)
            //                {
            //                    where.Append(" BrandID = " + item + " Or ");
            //                }
            //                else
            //                {
            //                    where.Append(" BrandID = " + item);
            //                }
            //                i++;
            //            }
            //            where.Append(")");
            //        }
            //    }
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,*," +
            //        "(select b.Icon from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) LogoBrand, " +
            //        "(select b.Name from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) NameBrand, " +
            //        "(SELECT SUBSTRING((SELECT ',' + Convert(varchar,p.BrandID) FROM Product p WHERE p.ID NOT IN ({0}) And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE() FOR XML PATH('')), 2, 200000)) AS AllModule " +
            //        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p " +
            //" WHERE p.ID NOT IN ({0}) And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE())" + order + "", contentIds, where.ToString()); ;
            //    return pageZise>0 ? await _dapperDa.SelectPageAsync<WebsiteProductItemJson>(sql, search.page, pageZise) : await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            //}
            //else
            //{
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND Lang = '" + search.lang + "' AND ID NOT IN ({0}) AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() " + where.ToString() + order + "", contentIds);
            //    return pageZise > 0 ? await _dapperDa.SelectPageAsync<WebsiteProductItemJson>(sql, search.page, pageZise) : await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            //}
            #endregion
        }

        public async Task<IEnumerable<WebsiteProductItemJson>> GetListProductFilter(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "")
        {
            try
            {
                DataTable ModuleIds = new();
                ModuleIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    var lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    search.page = search.page > 1 ? search.page : 1;
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @wattage = search.wattage,
                        @sort = search.sort,
                        @lang = search.lang,
                        @start = (search.page - 1) * pageZise,
                        @size = pageZise
                    });
                    var result = await connect.QueryAsync<WebsiteProductItemJson>("dbo.GetListProductFilter", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result;
                }
            }
            catch
            {
                return null;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //List<string> key = new List<string>();
            //if (!string.IsNullOrEmpty(search.q))
            //{
            //    key = search.q.Trim().ToLower().Split(' ').ToList();
            //}
            //if (key.Count > 0)
            //{
            //    List<string> listColum = new List<string>();
            //    listColum.Add(nameof(WebsiteProductItemJson.Name));
            //    listColum.Add(nameof(WebsiteProductItemJson.NameAscii));
            //    where.Append(SqlUtility.WhereOrNLikeList(key, listColum));
            //}
            //string order = " ORDER BY OrderDisplay Asc";
            //if (search.sort == 0)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,0,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 1)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,2,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 2)
            //{
            //    order = " ORDER BY case when ',' + ViewHome + ',' like '%,5,%' THEN 0 ELSE 1 END, OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 3)
            //{
            //    order = " ORDER BY Price Desc,OrderDisplay asc, ID desc";
            //}
            //if (search.sort == 4)
            //{
            //    order = " ORDER BY Price Asc,OrderDisplay asc, ID desc";
            //}
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    string other = "";
            //    if (search.brandId > 0)
            //    {
            //        other = " Or BrandId = " + search.brandId;
            //    }
            //    where.Append(SqlUtility.WhereOrLikeListWithOther(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds", other));
            //    if (search.ListAttr != null && search.ListAttr.Count > 0)
            //    {
            //        foreach (var item in search.ListAttr)
            //        {
            //            List<string> ids = ListHelper.GetValuesArrayTag(item);
            //            where.Append(SqlUtility.WhereOrLikeList(ids.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.AttributeProductIds"));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.gia))
            //    {
            //        List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
            //        decimal from = ConvertUtil.ToDecimal(giaids[0]);
            //        decimal to = ConvertUtil.ToDecimal(giaids[1]);
            //        if (from == to)
            //        {
            //            where.Append(" And p.Price=" + from);
            //        }
            //        else if (from > 0 && to == 0)
            //        {
            //            where.Append(" And p.Price >= " + from);
            //        }
            //        else
            //        {
            //            where.Append(" And p.Price>=" + from);
            //            where.Append(" And p.Price<=" + to);
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.hsx))
            //    {
            //        List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
            //        if (hsx.Count > 0)
            //        {
            //            where.Append(" And (");
            //            int i = 1;
            //            foreach (var item in hsx)
            //            {
            //                if (i < hsx.Count)
            //                {
            //                    where.Append(" BrandID = " + item + " Or ");
            //                }
            //                else
            //                {
            //                    where.Append(" BrandID = " + item);
            //                }
            //                i++;
            //            }
            //            where.Append(")");
            //        }
            //    }
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,*," +
            //        "(select b.Icon from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) LogoBrand, " +
            //        "(select b.Name from WebsiteModule b where b.IsShow = 1 and b.IsDeleted = 0 and b.ID = BrandId) NameBrand, " +
            //        "(SELECT SUBSTRING((SELECT ',' + Convert(varchar,p.BrandID) FROM Product p WHERE p.ID NOT IN ({0}) And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE() FOR XML PATH('')), 2, 200000)) AS AllModule " +
            //        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p " +
            //" WHERE p.ID NOT IN ({0}) And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE())" + order + "", contentIds, where.ToString()); ;
            //    return pageZise>0 ? await _dapperDa.SelectPageAsync<WebsiteProductItemJson>(sql, search.page, pageZise) : await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            //}
            //else
            //{
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND Lang = '" + search.lang + "' AND ID NOT IN ({0}) AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() " + where.ToString() + order + "", contentIds);
            //    return pageZise > 0 ? await _dapperDa.SelectPageAsync<WebsiteProductItemJson>(sql, search.page, pageZise) : await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            //}
            #endregion
        }

        public List<WebsiteProductItemJson> GetListProductRss(SearchModel search, int moduleId = 0, string listChidrent = "")
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (moduleId > 0)
                {
                    DataTable ModuleIds = new DataTable();
                    ModuleIds.Columns.Add("KeyValue");
                    ModuleIds.Rows.Add(moduleId.ToString());
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                    IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("SELECT COUNT(ID) OVER () AS TotalRecord,*" +
                        " FROM Product p WHERE ID IN(SELECT p.ID FROM Product p inner join @ModuleIds k on '','' + p.ModuleIds + '','' like N''%,'' + k.KeyValue + '',%''" +
                        " WHERE IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 And IsSitemap = 1 AND Lang = @lang AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE()) ORDER BY OrderDisplay Asc", new { @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"), @lang = search.lang });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 And IsSitemap = 1 AND Lang = @lang AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() ORDER BY OrderDisplay Asc", new { @lang = search.lang });
                    connect.Close();
                    return result.ToList();
                }
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //string order = " ORDER BY OrderDisplay Asc";
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    where.Append(SqlUtility.WhereOrLikeListWithOther(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds", ""));
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,*" +
            //        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p " +
            //" WHERE p.ID NOT IN ({0}) And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 And IsSitemap = 1 AND Lang = '" + search.lang + "'{1} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE())" + order + "", "0", where.ToString());
            //    return _dapperDa.Select<WebsiteProductItemJson>(sql).ToList();
            //}
            //else
            //{
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 And IsSitemap = 1 AND Lang = '" + search.lang + "' AND ID NOT IN ({0}) AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() " + where.ToString() + order + "", "0");
            //    return _dapperDa.Select<WebsiteProductItemJson>(sql).ToList();
            //}
            #endregion
        }
        public async Task<IEnumerable<int>> GetListIdsProduct(SearchModel search, int moduleId = 0, string listChidrent = "")
        {
            try
            {
                decimal? giafrom = null, giato = null;
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                DataTable AttrIds = new DataTable();
                AttrIds.Columns.Add("ID");
                AttrIds.Columns.Add("KeyItem");
                DataTable HsxIds = new DataTable();
                HsxIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                if (search.ListAttr != null && search.ListAttr.Count > 0)
                {
                    int i = 1;
                    foreach (string item in search.ListAttr)
                    {
                        AttrIds.Rows.Add(i, item);
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(search.hsx))
                {
                    List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
                    if (hsx.Any())
                    {
                        hsx.ForEach(x =>
                        {
                            HsxIds.Rows.Add(x);
                        });
                    }
                }
                if (!string.IsNullOrEmpty(search.gia))
                {
                    List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
                    decimal from = ConvertUtil.ToDecimal(giaids[0]);
                    decimal to = ConvertUtil.ToDecimal(giaids[1]);
                    if (from > 0) giafrom = Math.Abs(from * 1000000);
                    if (to > 0) giato = Math.Abs(to * 1000000);
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    search.page = search.page > 1 ? search.page : 1;
                    DynamicParameters paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        @AttrIds = AttrIds.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @HsxIds = HsxIds.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @giafrom = (int?)giafrom,
                        @giato = (int?)giato,
                        search.lang
                    });
                    IEnumerable<int> result = await connect.QueryAsync<int>("dbo.GetListIdProduct", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result;
                }
            }
            catch
            {
                return null;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //string order = " ORDER BY OrderDisplay Asc";
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    string other = "";
            //    if (search.brandId > 0)
            //    {
            //        other = " Or BrandId = " + search.brandId;
            //    }
            //    where.Append(SqlUtility.WhereOrLikeListWithOther(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds", other));
            //    if (search.ListAttr != null && search.ListAttr.Count > 0)
            //    {
            //        foreach (var item in search.ListAttr)
            //        {
            //            var ids = ListHelper.GetValuesArrayTag(item);
            //            where.Append(SqlUtility.WhereOrLikeList(ids.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.AttributeProductIds"));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.gia))
            //    {
            //        List<string> giaids = ListHelper.GetValuesArrayTag(search.gia);
            //        decimal from = ConvertUtil.ToDecimal(giaids[0]);
            //        decimal to = ConvertUtil.ToDecimal(giaids[1]);
            //        if (from == to)
            //        {
            //            where.Append(" And p.Price=" + Math.Abs(from * 1000000));
            //        }
            //        else if (from > 0 && to == 0)
            //        {
            //            where.Append(" And p.Price >= " + Math.Abs(from * 1000000));
            //        }
            //        else
            //        {
            //            where.Append(" And p.Price>=" + Math.Abs(from * 1000000));
            //            where.Append(" And p.Price<=" + Math.Abs(to * 1000000));
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(search.hsx))
            //    {
            //        List<string> hsx = search.hsx.Trim().ToLower().Split(',').ToList();
            //        if (hsx.Count > 0)
            //        {
            //            where.Append(" And (");
            //            int i = 1;
            //            foreach (string item in hsx)
            //            {
            //                if (i < hsx.Count)
            //                {
            //                    where.Append(" BrandID = " + item + " Or ");
            //                }
            //                else
            //                {
            //                    where.Append(" BrandID = " + item);
            //                }
            //                i++;
            //            }
            //            where.Append(")");
            //        }
            //    }
            //    string sql = string.Format("SELECT ID" +
            //        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p " +
            //" WHERE IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "'{0} AND p.CreatedDate <= GETDATE() AND p.PublishDate <= GETDATE())" + order + "", where.ToString());
            //    return await _dapperDa.SelectAsync<int>(sql);
            //}
            //else
            //{
            //    return await _dapperDa.SelectAsync<int>(string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND Lang = '" + search.lang + "' AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() " + where.ToString() + order + ""));
            //}
            #endregion
        }
        public async Task<IEnumerable<int>> ListIdsTradeMark(SearchModel search, int moduleId = 0, string listChidrent = "")
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (moduleId > 0)
                {
                    DataTable ModuleIds = new DataTable();
                    ModuleIds.Columns.Add("keyValue");
                    ModuleIds.Rows.Add(moduleId.ToString());
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                    IEnumerable<int> result = await connect.QueryAsync<int>("SELECT distinct BrandID" +
                        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p inner join @ModuleIds k on ',' + p.ModuleIds + ',' like N'%,' + Convert(varchar,k.keyValue) + ',%'" +
                        " WHERE IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 And IsSitemap = 1 AND Lang = @lang AND p.CreatedDate <= GETDATE() And p.BrandId is not null AND p.PublishDate <= GETDATE()) ORDER BY BrandId Asc", new { @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"), @lang = search.lang });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    IEnumerable<int> result = await connect.QueryAsync<int>("SELECT distinct BrandID FROM Product WHERE IsDeleted = 0 AND IsShow = 1 And IsSitemap = 1 and IsApproved = 1 AND Lang = @lang And BrandId is not null AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE()", new { @lang = search.lang });
                    connect.Close();
                    return result.ToList();
                }
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //string order = " ORDER BY BrandId Asc";
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    where.Append(SqlUtility.WhereOrLikeListWithOther(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds", string.Empty));
            //    string sql = string.Format("SELECT distinct BrandID" +
            //        " FROM Product WHERE ID IN(SELECT p.ID FROM Product p " +
            //" WHERE IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 And IsSitemap = 1 AND Lang = '" + search.lang + "'{0} AND p.CreatedDate <= GETDATE() And BrandId is not null AND p.PublishDate <= GETDATE())" + order + "", where.ToString()); ;
            //    return await _dapperDa.SelectAsync<int>(sql);
            //}
            //else
            //{
            //    string sql = string.Format("SELECT distinct BrandID FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND Lang = '" + search.lang + "' And BrandId is not null AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() " + where.ToString() + order + "");
            //    return await _dapperDa.SelectAsync<int>(sql);
            //}
            #endregion
        }
        public async Task<IEnumerable<WebsiteProductItemJson>> GetRandomProductMore(SearchModel search, int contentId, string moduleId)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    DynamicParameters paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        search.price,
                        contentId,
                        search.lang,
                        moduleId
                    });
                    IEnumerable<WebsiteProductItemJson> result = await connect.QueryAsync<WebsiteProductItemJson>("dbo.GetRandomProductMore", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
            #region Remove
            //StringBuilder sql = new StringBuilder();
            //sql.Append("select * from (");
            //sql.Append("select * from (" +
            //    "SELECT Top(5) * FROM Product WHERE IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang = 'vi' and CreatedDate <= GETDATE() And PublishDate <= GETDATE() and ',' + ModuleIds + ',' like N'%," + moduleId + ",%' and ID not in (" + contentId + ") And Price <= " + search.price + " ORDER BY Price desc) c");
            //sql.Append(" union all ");
            //sql.Append("select * from (" +
            //    "SELECT Top(5) * FROM Product WHERE IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang = 'vi' and CreatedDate <= GETDATE() And PublishDate <= GETDATE() and ',' + ModuleIds + ',' like N'%," + moduleId + ",%' and ID not in (" + contentId + ") And Price > " + search.price + " ORDER BY Price asc) d");
            //sql.Append(") e order by Price asc");
            //return await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql.ToString());
            #endregion
        }
        public async Task<IEnumerable<WebsiteProductItemJson>> GetSameProductMore(SearchModel search, int contentId, string moduleId)
        {
            StringBuilder sql = new StringBuilder();
            List<string> key = new List<string>();
            StringBuilder keyView = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder totalNameAscii = new StringBuilder();
            if (!string.IsNullOrEmpty(search.q))
            {
                key = search.q.Trim().ToLower().Split(' ').ToList();
            }
            if (key.Count > 0)
            {
                List<string> listColum = new List<string>
                {
                    nameof(WebsiteProductItemJson.Name)
                };
                sqlWhere.Append(SqlUtility.WherAndNLikeListSearch(key, listColum));
                foreach (string item in key)
                {
                    keyView.Append(string.Format(SqlConst.TotalNameAscii, Utility.ConvertRewrite(item)));
                }
                totalNameAscii.Append(",(0" + keyView + ") TotalNameAscii");
            }
            sql.Append("select * from Product where ID != " + contentId + " and IsDeleted = 0 and IsShow = 1 and ',' + ModuleIds + ',' like N'%," + moduleId + ",%' and IsApproved = 1 and Lang = 'vi' and CreatedDate <= GETDATE() And PublishDate <= GETDATE()" + sqlWhere.ToString() + " order by Price desc");
            return await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql.ToString());
        }
        public List<ProductItem> GetRandomProductCompare(SearchModel search)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search.price.HasValue)
                {
                    IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT Top(30) * FROM Product WHERE IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang= @lang and ID IN(" +
                        "SELECT ID FROM Product WHERE CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ','+ModuleIds+',' like N'%,' + Convert(varchar, @moduleId) + ',%' and ABS(Price - Convert(decimal(18,0),@price)) <= 1000000 and ',' + @ids + ',' not like N'%,' + Convert(varchar,ID) + ',%') ORDER BY NEWID()", new { search.moduleId, search.lang, @ids = search.contentId });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT Top(30) * FROM Product WHERE IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang= @lang and ID IN(" +
                        "SELECT ID FROM Product WHERE CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ','+ModuleIds+',' like N'%,' + Convert(varchar, @moduleId) + ',%' and ',' + @ids + ',' not like N'%,' + Convert(varchar,ID) + ',%') ORDER BY NEWID()", new { search.moduleId, search.lang, @ids = search.contentId });
                    connect.Close();
                    return result.ToList();
                }
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //if (search.price.HasValue)
            //{
            //    where.Append(" and ABS(Price - " + search.price + ") <= 1000000");
            //}
            //string sql = string.Format("SELECT Top(30) * FROM Product WHERE IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang='" + search.lang + "' and ID IN(" +
            //    " SELECT ID FROM Product WHERE CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ','+ModuleIds+',' like N'%," + search.moduleId + ",%'" + where.ToString() + " and ID not in ({0})) ORDER BY NEWID()", search.contentId);
            //return _dapperDa.Select<ProductItem>(sql).ToList();
            #endregion
        }
        public List<ProductItem> GetProductSeen(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and Lang=@lang AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ',' + @ids + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%')", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<ProductItem>> GetListCompareByIds(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = await connect.QueryAsync<ProductItem>("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ',' + @ids + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' ORDER BY OrderDisplay Asc", new { ids });
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //string order = " ORDER BY OrderDisplay Asc";
            //if (search.sort == 1)
            //{
            //    order = " ORDER BY CreatedDate DESC, ID Desc";
            //}
            //if (search.sort == 2)
            //{
            //    order = " ORDER BY Price Asc,CreatedDate DESC, ID Desc";
            //}
            //if (search.sort == 3)
            //{
            //    order = " ORDER BY Price Desc,CreatedDate DESC, ID Desc";
            //}
            //string sql = string.Format("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ',{0},' LIKE '%,'+CONVERT(varchar(10), ID)+',%'{1}", ids, order);
            //return await _dapperDa.SelectAsync<ProductItem>(sql);
            #endregion
        }
        public async Task<IEnumerable<ProductItem>> GetListByIds(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = await connect.QueryAsync<ProductItem>("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and CreatedDate <= GETDATE() AND PublishDate <= GETDATE()" +
                " AND Amount is not null and Amount > 0 and ',' + @ids + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%'", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<ProductItem>> GetListByContentID(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = await connect.QueryAsync<ProductItem>("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ','+ ContentIds +',' LIKE '%,' + Convert(varchar,@id) + ',%'", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListByListNameAscii(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT Id,Name,ProductCode,NameAscii, LinkUrl,Description,ModuleNameAscii,PriceOld,Price,UrlPicture FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 and CreatedDate <= GETDATE() AND PublishDate <= GETDATE() and ',' + @ids + ',' like N'%,' + NameAscii + ',%'", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public List<string> GetListWattage()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<string> result = connect.Query<string>("select DISTINCT Wattage from Product Where Wattage is not null Order by Wattage desc");
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListByListProductCode(string codes)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT * FROM Product WHERE 1=1 And IsDeleted = 0 and IsShow = 1 and IsApproved = 1 AND PublishDate <= GETDATE() and ',' + @codes + ',' LIKE '%,'+ProductCode+',%'", new { codes });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListEachModule(List<WebsiteModulesItem> moduleIds, string lang, int size = 100)
        {
            int i = 1;
            StringBuilder sql = new StringBuilder();
            sql.Append("select  DISTINCT Id,Name, ModuleIds, UrlPicture, NameAscii,ModuleNameAscii,LinkUrl,OrderDisplay,CreatedDate from (");
            foreach (WebsiteModulesItem item in moduleIds)
            {
                if (i == 1)
                {
                    sql.Append("select Top(" + size + ") *  from Product where IsDeleted = 0 and IsShow = 1 and IsApproved = 1 And Lang = '" + lang + "' and ID in (select ID from Product where ','+ModuleIds+',' like N'%," + item.ID + ",%' AND CreatedDate <= GETDATE()) AND PublishDate <= GETDATE() ORDER BY CreatedDate DESC");
                }
                else
                {
                    sql.Append(" union all");
                    sql.Append(" select Top(" + size + ") *  from Product where IsDeleted = 0 and IsShow = 1 and IsApproved = 1 And Lang = '" + lang + "' and ID in (select ID from Product where ','+ModuleIds+',' like N'%," + item.ID + ",%' AND CreatedDate <= GETDATE()) AND PublishDate <= GETDATE() ORDER BY CreatedDate DESC");
                }
                i++;
            }
            sql.Append(") c");
            return _dapperDa.Select<ProductItem>(sql.ToString()).ToList();
        }
        //san pham moi nhat
        public IEnumerable<WebsiteProductItemJson> GetListProductNew(SearchModel search, int moduleId = 0, string listChidrent = "", int pageZise = 20)
        {
            DataTable ModuleIds = new DataTable();
            ModuleIds.Columns.Add("KeyValue");
            if (moduleId > 0)
            {
                ModuleIds.Rows.Add(moduleId.ToString());
                List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    search.lang,
                    @start = ((search.page - 1) * pageZise),
                    pageZise
                });
                IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("dbo.GetListProductNew", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //string sql = "";
            //StringBuilder where = new StringBuilder();
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds"));
            //    sql = string.Format("SELECT TOP {1} * FROM Product WHERE ID IN(SELECT ID FROM Product " +
            //  " WHERE 1=1 and IsApproved = 1 And IsDeleted = 0 AND IsShow = 1{0} AND CreatedDate <= GETDATE()) AND PublishDate <= GETDATE() ORDER BY CreatedDate DESC, ID Desc", where, pageZise);
            //}
            //else
            //{
            //    sql = string.Format("SELECT TOP {0} * FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() ORDER BY CreatedDate DESC", pageZise);
            //}
            //return await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            #endregion
        }
        //san pham noi bat
        public IEnumerable<WebsiteProductItemJson> GetListProductHotAsync(SearchModel search, int moduleId = 0, string listChidrent = "", int pageZise = 20)
        {
            DataTable ModuleIds = new DataTable();
            ModuleIds.Columns.Add("KeyValue");
            if (moduleId > 0)
            {
                ModuleIds.Rows.Add(moduleId.ToString());
                List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    search.lang,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("dbo.GetListProductHot", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //string sql = "";
            //StringBuilder where = new StringBuilder();
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds"));
            //    sql = string.Format("SELECT TOP {1} * FROM Product WHERE ID IN(SELECT ID FROM Product " +
            //  " WHERE 1=1 And IsDeleted = 0 AND IsShow = 1 And ',' +  ViewHome + ',' LIKE '%," + StaticEnum.IsHot + ",%'{0} AND CreatedDate <= GETDATE()) AND PublishDate <= GETDATE() ORDER BY OrderDisplay Asc, CreatedDate Desc", where, pageZise);
            //}
            //else
            //{
            //    sql = string.Format("SELECT TOP {0} * FROM Product WHERE 1=1 And IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 And ',' +  ViewHome + ',' LIKE '%," + StaticEnum.IsHot + ",%' AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() ORDER BY OrderDisplay Asc, CreatedDate Desc", pageZise);
            //}
            //return await _dapperDa.SelectAsync<WebsiteProductItemJson>(sql);
            #endregion
        }
        //sản phẩm bán chạy nhất
        public IEnumerable<WebsiteProductItemJson> GetListProductBestSelling(SearchModel search, int moduleId = 0, string listChidrent = "", int pageZise = 20)
        {
            DataTable ModuleIds = new DataTable();
            ModuleIds.Columns.Add("KeyValue");
            if (moduleId > 0)
            {
                ModuleIds.Rows.Add(moduleId.ToString());
                List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    search.lang,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<WebsiteProductItemJson> result = connect.Query<WebsiteProductItemJson>("dbo.GetListProductSelling", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //    string sql = string.Empty;
            //StringBuilder where = new StringBuilder();
            //if (!string.IsNullOrEmpty(listChidrent))
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds"));
            //    sql = string.Format("SELECT TOP {1} * FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND ID IN(SELECT ID FROM Product " +
            //  " WHERE 1=1{0} And CreatedDate <= GETDATE()) ORDER BY OrderDisplay Asc, CreatedDate DESC", where, pageZise);
            //    return _dapperDa.Select<ProductItem>(sql).ToList();
            //}
            //else
            //{
            //    sql = string.Format("SELECT TOP {0} * FROM Product WHERE IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 AND CreatedDate <= GETDATE() AND PublishDate <= GETDATE() And IsBestSell = 1 ORDER BY OrderDisplay Asc, CreatedDate DESC", pageZise);
            //    return _dapperDa.Select<ProductItem>(sql).ToList();
            //}
            #endregion
        }
        public List<ProductItem> GetListProductRelate(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("select * from Product where 1=1 And IsDeleted = 0 And IsShow = 1 and IsApproved = 1 AND PublishDate <= GETDATE() AND ',' + ContentIds + ',' like N'%,' + Convert(varchar,@id) + ',%'", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task UpdateTotalViews(int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("Update Product SET TotalViews = ISNULL(TotalViews,0) + 1 WHERE ID = @id", new { @id = productId });
                connect.Close();
            }
        }

        public async Task UpdateTotalRate(int productId, int star, double totalstar, int totalrate)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("Update Product SET TotalRate = @totalrate, Star = @star, TotalStars = @totalstar WHERE ID = @id", new { @id = productId, star, totalstar, totalrate });
                connect.Close();
            }
        }

        public async Task<IEnumerable<AttributeItem>> GetAttributeByListIds(string listArray)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AttributeItem> result = await connect.QueryAsync<AttributeItem>("SELECT * FROM Attributes where IsDeleted = 0 AND IsShow =1 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { listArray });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Attribute_WebsiteContentItem> GetAttributeWebsiteContentItemByListAttrIdsAndProductId(string listAttrIds, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<Attribute_WebsiteContentItem> result = connect.Query<Attribute_WebsiteContentItem>("SELECT * FROM Attribute_WebsiteContent where AttributeID is not null and ContentID is not null and ContentID= @productId  AND ',' + @listAttrIds + ',' LIKE '%,'+CONVERT(varchar(10), AttributeID)+',%' Order By ID desc", new { listAttrIds, productId });
                connect.Close();
                return result.ToList();
            }
        }
        public List<AttributeItem> GetAllAttributeByListIdsAndAllParent(string listArray, int moduleId, string listChidrent = "")
        {
            StringBuilder where = new StringBuilder();
            string total = string.Empty;
            if (moduleId > 0)
            {
                List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                lstId.Add(moduleId.ToString());
                where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "p.ModuleIds"));
                total = string.Format(",(select COUNT(attr.AttributeID) Total from Attribute_WebsiteContent attr where 1=1 And attr.AttributeID = c.ID and " +
                    "attr.ContentID in (select p.ID from Product p where p.IsDeleted = 0 And p.IsShow = 1 And p.IsApproved = 1 and p.PublishDate < GETDATE(){0}) group by attr.AttributeID) Total", where);
            }
            string sql = string.Format("SELECT *{1} FROM Attributes c where c.IsDeleted = 0 AND c.IsShow =1 And c.IsAllowsFillter = 1 AND ',{0},' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By c.ID desc", listArray, total);
            return _dapperDa.Select<AttributeItem>(sql).GroupBy(c => c.ID).Select(c => c.First()).ToList();
        }
        public List<AttributeItem> GetAllAttributeShowContent(string listArray)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AttributeItem> result = connect.Query<AttributeItem>("SELECT * FROM Attributes c where c.IsDeleted = 0 AND c.IsShow =1 And ParentID = 0 And c.IsShowContent = 1 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By c.ID desc", new { listArray });
                connect.Close();
                return result.GroupBy(c => c.ID).Select(c => c.First()).ToList();
            }
        }
        public List<ProductItem> GetListProductByTagItem(string keyword, string lang, int page = 1, int size = 20)
        {
            try
            {
                page = page > 1 ? page : 1;
                int start = (page - 1) * size;
                DataTable keys = new();
                keys.Columns.Add("ID");
                keys.Columns.Add("KeyItem");
                if (!string.IsNullOrEmpty(keyword))
                {
                    var keyids = ListHelper.GetValuesArrayTag(keyword);
                    for (int i = 0; i < keyids.Count; i++)
                    {
                        keys.Rows.Add((keyids.Count - i), keyids[i]);
                    }
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        @TagValues = keys.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        lang,
                        start,
                        size
                    });
                    var result = connect.Query<ProductItem>("dbo.GetTagValues", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }
        public bool CheckAttrByNameAscii(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AttributeItem> result = connect.Query<AttributeItem>("SELECT * FROM Attributes c where c.IsDeleted = 0 AND c.IsShow =1 And ParentID = 0 AND NameAscii = @NameAscii", new { NameAscii });
                connect.Close();
                return result.Any();
            }
        }
        public async void UpdateGoodViews(int contentId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("Update Product SET Satisfied = ISNULL(Satisfied,0) + 1 WHERE ID = @contentId", new { contentId });
                connect.Close();
            }
        }
        public async void UpdateBadViews(int contentId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("Update Product SET UnSatisfied = ISNULL(UnSatisfied,0) + 1 WHERE ID =@contentId", new { contentId });
                connect.Close();
            }
        }
        public List<WebsiteProductItemJson> GetSearchProduct(string keyword, string lang, int page = 1, int size = 20)
        {
            try
            {
                page = page > 1 ? page : 1;
                int start = (page - 1) * size;
                DataTable keys = new();
                keys.Columns.Add("ID");
                keys.Columns.Add("KeyItem");
                if (!string.IsNullOrEmpty(keyword))
                {
                    var keyids = ListHelper.GetValuesArrayTag(keyword);
                    for (int i = 0; i < keyids.Count; i++)
                    {
                        keys.Rows.Add((keyids.Count - i), keyids[i]);
                    }
                }
                using SqlConnection connect = _dapperDa.GetOpenConnection();
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keys = keys.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                    keyword,
                    lang,
                    start,
                    size
                });
                var result = connect.Query<WebsiteProductItemJson>("dbo.GetSearchProduct", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}