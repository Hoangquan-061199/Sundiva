using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class WebsiteContentDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public WebsiteContentDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<WebsiteModule> GetListModuleByArrId(List<int> lstId)
        {
            try
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                lstId.ForEach(x =>
                {
                    ModuleIds.Rows.Add(x.ToString());
                });
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<WebsiteModule>("SELECT w.* FROM WebsiteModule w inner join @ModuleIds m on ','+w.ModuleIds+',' LIKE '%,'+ m.KeyValue +',%'  WHERE 1=1 And w.IsDeleted = 0 AND w.IsShow =1", new { @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]") });
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }
        public List<WebsiteContentAdmin> GetListByModuleTypeCode(string moduletypecode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentAdmin> result = connect.Query<WebsiteContentAdmin>("select a.ID, a.Name, a.OrderDisplay from WebsiteContent as a left join WebsiteModule as b on ',' + CONVERT(varchar,b.ID) + ',' like '%,' + a.ModuleIds + ',%' where b.ModuleTypeCode = @moduletypecode and a.IsDeleted = 0 and a.IsShow = 1", new { moduletypecode });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteContentAdmin> GetListByArrId(string relateIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteContentAdmin>("select * from WebsiteContent where IsDeleted = 0 And IsShow = 1 AND ',' + @relateIds + ',' like N'%,' + Convert(varchar,ID) + ',%'", new { relateIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteContentAdmin> GetListContentGroupModule(string Ids)
        {
            try
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                if (!string.IsNullOrEmpty(Ids))
                {
                    List<string> lstId = ListHelper.GetValuesArrayTag(Ids);
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
                    DynamicParameters paras = new();
                    paras.AddDynamicParams(new
                    {
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]")
                    });
                    IEnumerable<WebsiteContentAdmin> result = connect.Query<WebsiteContentAdmin>("dbo.GetListContentGroupModule", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }
        public List<WebsiteContentAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport, string moduleIds, string userLogin)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                page = page > 1 ? page : 1;
                rowPage = rowPage > 1 ? rowPage : 10;
                int start = ((page - 1) * rowPage);
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = search.keyword,
                    @keyAscii = SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)),
                    @Id = ConvertUtil.ToInt32(search.keyword),
                    search.ModuleId,
                    search.lang,
                    search.sort,
                    search.Show,
                    search.Status,
                    search.type,
                    @from = !string.IsNullOrEmpty(search.from) ? SqlUtility.ConvertDate(search.from, false) : null,
                    @to = !string.IsNullOrEmpty(search.to) ? SqlUtility.ConvertDate(search.to, false) : null,
                    search.company,
                    moduleIds,
                    userLogin,
                    start,
                    @size = rowPage,
                    isExport
                });
                var result = connect.Query<WebsiteContentAdmin>("dbo.AdminContentListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }        
        public List<WebsiteContentAdmin> ListSearchOther(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<WebsiteContentAdmin>("SELECT * FROM(SELECT c.* FROM WebsiteContent c, WebsiteModule m WHERE ','+c.ModuleIds+',' LIKE '%,'+Convert(nvarchar,m.ID)+',%' AND m.ModuleTypeCode !='News' AND Name LIKE '%' + @key + '%' ESCAPE N'~') c ORDER BY  CreatedDate DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<WebsiteContentAdmin>("SELECT * FROM(SELECT c.* FROM WebsiteContent c, WebsiteModule m WHERE ','+c.ModuleIds+',' LIKE '%,'+Convert(nvarchar,m.ID)+',%' AND m.ModuleTypeCode !='News') c ORDER BY  CreatedDate DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        #region sitemap
        public List<WebsiteContentAdmin> GetAllSitemap()
        {
            string sql = string.Format("select a.ID,a.NameAscii,a.ModuleNameAscii,a.ModuleIds, a.ModifiedDate," +
                " (select Top(1) b.ModuleTypeCode from WebsiteModule b where b.IsDeleted = 0 and b.NameAscii = a.ModuleNameAscii) ModuleType" +
                " from WebsiteContent a" +
                " where a.IsDeleted = 0 and a.IsShow = 1 and a.NameAscii is not null and a.LinkUrl is null and a.IsSitemap = 1 and a.ModuleNameAscii is not null order by a.CreatedDate desc");
            return _dapperDa.Select<WebsiteContentAdmin>(sql).ToList();
        }       
        #endregion
        public int CountContentyModuleIds(string ids, string lang)
        {
            DataTable ModuleIds = new DataTable();
            ModuleIds.Columns.Add("KeyValue");
            List<string> lstId = ListHelper.GetValuesArrayTag(ids);
            lstId.ForEach(x =>
            {
                ModuleIds.Rows.Add(x.ToString());
            });
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<int>("SELECT w.* FROM WebsiteModule w inner join @ModuleIds m on ','+w.ModuleIds+',' LIKE '%,'+ m.KeyValue +',%'  WHERE 1=1 And w.IsDeleted = 0 AND w.IsShow =1 And w.Lang = @lang", new { @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"), lang });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteContent GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteContent>("select * from WebsiteContent where ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteContent GetByNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteContent>("select * from WebsiteContent where IsDeleted = 0 AND NameAscii=@nameAscii", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(WebsiteContent obj) => _dapperDa.Insert(obj);
        public int Update(WebsiteContent obj) => _dapperDa.Update(obj);
        public void Delete(WebsiteContent obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
