using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class ProductDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ProductDa(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public List<ProductAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport, string moduleIds)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            page = page > 1 ? page : 1;
            rowPage = rowPage > 1 ? rowPage : 10;
            int start = ((page - 1) * rowPage);
            DataTable ModuleIds = new();
            ModuleIds.Columns.Add("KeyValue");
            if (!string.IsNullOrEmpty(search.ModuleIds))
            {
                var lstId = ListHelper.GetValuesArrayTag(search.ModuleIds);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            DynamicParameters paras = new();
            paras.AddDynamicParams(new
            {
                @keyword = SqlUtility.CharacterSpecail(search.keyword),
                @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                @brand = search.BrandId,
                search.Show,
                search.lang,
                search.type,
                search.sort,
                @status = (!string.IsNullOrEmpty(search.Status) ? ConvertUtil.ToInt32(search.Status) : -1),
                start,
                @size = rowPage,
                isExport
            });
            var result = connect.Query<ProductAdmin>("dbo.AdminProductListSearch", paras, commandType: CommandType.StoredProcedure);
            connect.Close();
            return result.ToList();

        }

        public List<ProductAdmin> ListSearchByParentId(SearchModel search, int page, int rowPage, bool isExport, string moduleIds, int parentId)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            page = page > 1 ? page : 1;
            rowPage = rowPage > 1 ? rowPage : 10;
            int start = ((page - 1) * rowPage);
            DataTable ModuleIds = new();
            ModuleIds.Columns.Add("KeyValue");
            if (!string.IsNullOrEmpty(search.ModuleIds))
            {
                var lstId = ListHelper.GetValuesArrayTag(search.ModuleIds);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            DynamicParameters paras = new();
            paras.AddDynamicParams(new
            {
                @keyword = SqlUtility.CharacterSpecail(search.keyword),
                @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                @brand = search.BrandId,
                search.Show,
                search.lang,
                search.type,
                search.sort,
                @status = (!string.IsNullOrEmpty(search.Status) ? ConvertUtil.ToInt32(search.Status) : -1),
                start,
                @size = rowPage,
                isExport,
                parentId
            });
            var result = connect.Query<ProductAdmin>("[dbo].[AdminProductParentIdListSearch]", paras, commandType: CommandType.StoredProcedure);
            connect.Close();
            return result.ToList();

        }

        public List<ProductAdmin> ListProductSort(Simple.Admin.SearchModel search, string moduleIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    search.ModuleId,
                    @brand = search.BrandId,
                    search.Show,
                    search.lang,
                    search.type,
                    moduleIds,
                    search.sort,
                    @status = (!string.IsNullOrEmpty(search.Status) ? ConvertUtil.ToInt32(search.Status) : -1),
                    @start = 0,
                    @size = 0,
                    @isExport = 1
                });
                var result = connect.Query<ProductAdmin>("dbo.AdminProductListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductAdmin> ListExport(Simple.Admin.SearchModel search, string moduleIds)
        {
            int page = search.page > 1 ? search.page : 1;
            int size = search.pagesize > 1 ? search.pagesize : 10;
            int start = ((page - 1) * size);
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    search.ModuleId,
                    @brand = search.BrandId,
                    search.Show,
                    search.lang,
                    search.type,
                    moduleIds,
                    search.sort,
                    @status = (!string.IsNullOrEmpty(search.Status) ? ConvertUtil.ToInt32(search.Status) : -1),
                    start,
                    size,
                    @isExport = size > 0 ? 0 : 1
                });
                var result = connect.Query<ProductAdmin>("dbo.AdminProductListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            //return search.pagesize > 0 ? _dapperDa.SelectPage<ProductAdmin>($"{sql}", search.page, search.pagesize).ToList() : _dapperDa.Select<ProductAdmin>($"{sql}").ToList();
        }
        public List<ProductAdmin> GetListByArrId(string relateIds)
        {
            relateIds = relateIds.Trim(',');
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductAdmin>("select * from Product where IsDeleted = 0 And IsShow = 1 AND ',' + @relateIds + ',' like N'%,'+CONVERT(varchar,ID)+',%'", new { relateIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Product> GetListAllProducts(string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                string sqll = string.Format("select * from Product where IsDeleted = 0 And IsShow = 1 AND Lang = @lang And ProductCode is not null", new { lang });
                var result = connect.Query<Product>("select * from Product where IsDeleted = 0 And IsShow = 1 AND Lang = @lang And ProductCode is not null", new { lang });
                connect.Close();
                return result.ToList();
            }
        }
        public int CountProductByModuleIds(string ids, string lang)
        {
            string sql = $"SELECT Count(ID) FROM Product WHERE 1=1 AND IsDeleted = 0 And Lang = '{lang}'";
            if (!string.IsNullOrEmpty(ids))
            {
                List<string> lstId = ListHelper.GetValuesArrayTag(ids);
                sql += SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds");
            }
            return _dapperDa.Select<int>(sql).FirstOrDefault();
        }
        public Product GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Product>("SELECT * FROM Product WHERE id=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Product GetByProductCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Product>("SELECT * FROM Product WHERE 1=1 and IsDeleted = 0 and ProductCode=@code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<Simple.Base.SubItem> GetSubItemByProductId(int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Simple.Base.SubItem>("SELECT * FROM SubItem WHERE IsDeleted = 0 and ProductID=@productId", new { productId });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Attribute_WebsiteContent> GetAttrByProductId(int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attribute_WebsiteContent>("SELECT * FROM Attribute_WebsiteContent WHERE ContentID=@productId", new { productId });
                connect.Close();
                return result.ToList();
            }
        }
        public Product GetNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Product>("SELECT * FROM Product WHERE NameAscii=@nameAscii and IsDeleted = 0", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        #region sitemap
        public List<ProductAdmin> GetAllSitemap()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductAdmin>("select ID,NameAscii,ModuleNameAscii, ModuleIds, ModifiedDate from Product where IsDeleted = 0 and IsShow = 1 and NameAscii is not null and LinkUrl is null and IsSitemap = 1 order by CreatedDate desc");
                connect.Close();
                return result.ToList();
            }
        }
        public int RemoveFrameProduct(int id)
        {
            try
            {
                return _dapperDa.ExecuteSql(string.Format($"update Product set FramesID = NULL where FramesID = {id}"));
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        public int UpdateSaleByIds(string ids, int value)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                int result = connect.Execute("update Product set TypeSaleValue = @value where ','+ @ids + ',' like N'%,'+CONVERT(varchar,ID)+',%'", new { ids, value });
                connect.Close();
                return result;
            }
        }

        public int CheckProductCode(string code)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            var result = connect.Query<int>("SELECT ID FROM Product Where IsDeleted = 0 and ProductCode = @code", new { code });
            connect.Close();
            return result != null && result.Any() ? result.First() : 0;
        }
        //=> _dapperDa.ExecuteSql(string.Format($"update Product set TypeSaleValue = {value} where ',{ids},' like N'%,'+CONVERT(varchar,ID)+',%'"));
        public int Insert(Product obj) => _dapperDa.Insert(obj);
        public int InsertNoId(Product obj)
        => _dapperDa.InsertUserNoId(obj);
        public int InsertAttr(Attribute_WebsiteContent obj)
        => _dapperDa.Insert(obj);
        public int Update(Product obj)
        => _dapperDa.Update(obj);
        public void Delete(Product obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
        public int DeleteAttr(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                int result = connect.Execute("delete from Attribute_WebsiteContent where ContentID = @id", new { id });
                connect.Close();
                return result;
            }
        }
        //=> _dapperDa.ExecuteSql($"delete from Attribute_WebsiteContent where ContentID = {id}");
        public int UpdateSort(int sort, int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                int result = connect.Execute("update Product set OrderDisplay = @sort where ID = @id", new { sort, id });
                connect.Close();
                return result;
            }
        }
        //=> _dapperDa.ExecuteSql(string.Format($"update Product set OrderDisplay = {sort} where ID = {id}"));
    }
}
