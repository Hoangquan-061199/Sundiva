using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Simple.Item;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class WebsiteModuleManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        private readonly string _pathServer;
        public WebsiteModuleManager(string connectionSql, string pathServer = "")
        {
            _dapperDa = new DapperDA(connectionSql, pathServer);
            _pathServer = pathServer;
        }
        public WebsiteModulesItem GetById(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow = 1 AND ID = @id", new {id});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public async Task<WebsiteModulesItem> GetByNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow = 1 AND NameAscii=@nameAscii", new {nameAscii});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteModulesItem GetByCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND Code=@code", new {code});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public async Task<IEnumerable<WebsiteModulesItem>> GetListBreadcrumb(int moduleId, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = await connect.QueryAsync<WebsiteModulesItem>("WITH cte(ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon, OrderTree) AS" +
                                                  " (" +
                                                  " SELECT ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon,OrderTree = 0" +
                                                  " FROM WebsiteModule WHERE 1=1 And Lang = @lang and IsDeleted = 0 and ModuleTypeCode is not null and ID = @moduleId" +
                                                  " UNION ALL" +
                                                  " SELECT c.ID, c.ParentID, c.Name,c.OrderDisplay,c.ModuleTypeCode,c.NameAscii,c.IsShow,c.PositionCode,c.TotalViews,c.LinkUrl,c.IsDeleted,c.Icon, OrderTree=OrderTree+1" +
                                                  " FROM WebsiteModule c INNER JOIN cte ON cte.ParentID = c.ID)" +
                                                  " SELECT distinct ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl,IsDeleted,Icon,OrderTree," +
                                                  " (select t.Name from ModuleType t where t.Code = cte.ModuleTypeCode) ModuleType" +
                                                  " FROM cte ORDER BY OrderTree Desc", new { moduleId, lang });
                connect.Close();
                return result.ToList();
            }
            //    string sql = string.Format("WITH cte(ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon, OrderTree) AS" +
            //                                      " (" +
            //                                      " SELECT ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon,OrderTree = 0" +
            //                                      " FROM WebsiteModule WHERE 1=1 And Lang = '{1}' and IsDeleted = 0 and ModuleTypeCode is not null and ID = {0}" +
            //                                      " UNION ALL" +
            //                                      " SELECT c.ID, c.ParentID, c.Name,c.OrderDisplay,c.ModuleTypeCode,c.NameAscii,c.IsShow,c.PositionCode,c.TotalViews,c.LinkUrl,c.IsDeleted,c.Icon, OrderTree=OrderTree+1" +
            //                                      " FROM WebsiteModule c INNER JOIN cte ON cte.ParentID = c.ID)" +
            //                                      " SELECT distinct ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl,IsDeleted,Icon,OrderTree," +
            //                                      " (select t.Name from ModuleType t where t.Code = cte.ModuleTypeCode) ModuleType"+
            //                                      " FROM cte ORDER BY OrderTree Desc", moduleId, lang);
            //return await _dapperDa.SelectAsync<WebsiteModulesItem>(sql);
        }
        public WebsiteModulesItem GetByTypeCode(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ModuleTypeCode=@code AND Lang=@lang", new {code, lang});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<WebsiteModulesItem> GetByModuleTypeCode(string code, string lang)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ModuleTypeCode=@code And Lang = @lang", new { code, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesItem> GetByListModuleTypeCode(string code, string lang)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ',' + @code + ',' like '%,'+ModuleTypeCode+',%' and Lang = @lang", new { code, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesItem> GetByModuleTypeCodeID(string code, int id, string lang)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ModuleTypeCode=@code AND ParentID = @id And Lang = @lang", new { code, id, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<WebsiteModulesItem>> GetByAllTradeMark(string ids, string lang, string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("select ID,Name,NameAscii,LinkUrl,ParentID,UrlPicture from WebsiteModule wm where 1=1 and ',' + @ids + ',' like N'%,' + Convert(varchar,ID) + ',%' And Lang = @lang And ParentID = 0 and IsDeleted = 0 and IsShow = 1 and ModuleTypeCode = @code order by OrderDisplay", new { ids, lang, code });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<WebsiteModulesItem>> GetListByParentIDAsync(int? id, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("SELECT * FROM WebsiteModule where 1=1 and IsDeleted = 0 AND IsShow =1 AND ParentID=@id AND Lang = @lang", new { id, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesItem> GetListByParentID(int id, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where 1=1 and IsDeleted = 0 AND IsShow =1 AND ParentID=@id AND Lang = @lang", new { id, lang });
                connect.Close();
                return result.ToList();
            }
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetListByParentSimple(int id, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND (ParentID=@id Or ID = @id) AND Lang = @lang", new { id, lang });
                connect.Close();
                return result;
            }
        }
        public List<WebsiteModulesItem> GetListByArrId(string listArray)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { listArray });
                connect.Close();
                return result.ToList();
            }
        }       
        public async Task<IEnumerable<WebsiteModulesItem>> GetListChidrentAsync(int moduleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("with name_tree as (select ID, ParentID,Name,NameAscii,Title,ModuleTypeCode,TypeView,AlbumPictureJson,Description,OrderDisplay,LinkUrl,UrlPicture,ViewHome,Content,OrderTree =0" +
                                                  " from WebsiteModule where id = @moduleId union all" +
                                                  " select c.ID, c.ParentID,c.Name,c.NameAscii,c.Title,c.ModuleTypeCode,c.TypeView,c.AlbumPictureJson,c.Description,c.OrderDisplay,c.LinkUrl,c.UrlPicture,c.ViewHome,c.Content,OrderTree=OrderTree+1 from WebsiteModule c" +
                                                  " join name_tree p on p.id = c.parentid  AND c.IsDeleted=0 And c.IsShow = 1)" +
                                                  " select * from name_tree ORDER BY OrderTree ASC", new { moduleId });
                connect.Close();
                return result;
            }
        }
        public List<WebsiteModulesItem> GetListChidrentNotAsync(int moduleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("with name_tree as (select ID, ParentID,Name,NameAscii,Title,ModuleTypeCode,TypeView,TypeViewChild,AlbumPictureJson,Description,OrderDisplay,LinkUrl,UrlPicture,ViewHome,Content,OrderTree =0" +
                                                  " from WebsiteModule where id = @moduleId union all" +
                                                  " select c.ID, c.ParentID,c.Name,c.NameAscii,c.Title,c.ModuleTypeCode,c.TypeView,c.TypeViewChild,c.AlbumPictureJson,c.Description,c.OrderDisplay,c.LinkUrl,c.UrlPicture,c.ViewHome,c.Content,OrderTree=OrderTree+1 from WebsiteModule c" +
                                                  " join name_tree p on p.id = c.parentid  AND c.IsDeleted=0 And c.IsShow = 1)" +
                                                  " select * from name_tree ORDER BY OrderTree ASC", new { moduleId });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<WebsiteModulesItem>> GetAllModuleByCode(string code, string lang)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("select * from WebsiteModule where IsDeleted = 0 and IsShow = 1 And ModuleTypeCode = @code and Lang =@lang", new { code, lang });
                connect.Close();
                return result;
            }
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetAllModuleByCodeAndTypeView(string code, string typeview, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = await connect.QueryAsync<WebsiteModulesItem>("select Name, NameAscii, LinkUrl from WebsiteModule where IsDeleted = 0 and IsShow = 1 And ModuleTypeCode = @code and Lang = @lang and TypeView = @typeview", new { code, typeview, lang });
                connect.Close();
                return result;
            }
        }

        public async Task<IEnumerable<int>> GetAllIdsModuleByCode(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<int> result = await connect.QueryAsync<int>("select ID from WebsiteModule where IsDeleted = 0 and IsShow = 1 And ModuleTypeCode = @code and Lang =@lang", new { code, lang });
                connect.Close();
                return result;
            }
        }
        public List<WebsiteModulesItem> GetAllModuleByPositionCode(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesItem> result = connect.Query<WebsiteModulesItem>("select * from WebsiteModule where IsDeleted = 0 And IsShow = 1 and (',' + PositionCode + ',') like N'%,' + @code + ',%' And Lang = @lang", new { code, lang });
                connect.Close();
                return result.ToList();
            }
        }
        public async Task<IEnumerable<SubItem>> GetListSubItems(SearchModel search)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<SubItem> result = await connect.QueryAsync<SubItem>("select * from SubItem where IsDeleted = 0 And IsShow = 1 And ModuleID = @moduleId And Lang = @lang and ModuleTypeCode = 'Module'", new { moduleId = search.moduleId, lang = search.lang });
                connect.Close();
                return result;
            }
        }
        public async Task UpdateTotalViews(int moduleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("Update WebsiteModule SET TotalViews = ISNULL(TotalViews,0) + 1 WHERE ID =@moduleId", new { moduleId });
                connect.Close();
            }
        }
        public async Task<TagItem> GetTagByNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<TagItem> result = await connect.QueryAsync<TagItem>("SELECT Top(1) * FROM SystemTag where IsDeleted = 0 AND IsShow = 1 AND NameAscii=@nameAscii", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
    }
}
