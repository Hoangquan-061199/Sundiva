using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using ADCOnline.Simple.Item;
using System.Linq;
using System.Text;
using ADCOnline.Utils;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class WebsiteContentManager : BaseDa
    {
        private readonly DapperDA _dapperDa;

        public WebsiteContentManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public List<WebsiteContentItem> GetListContentGroupModule(string listChidrent = "")
        {
            DataTable ModuleIds = new();
            ModuleIds.Columns.Add("KeyValue");           
            if (!string.IsNullOrEmpty(listChidrent))
            {
                List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                if (lstId.Any())
                {
                    lstId.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
            }
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            DynamicParameters paras = new();
            paras.AddDynamicParams(new
            {
                @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]")
            });
            var result = connect.Query<WebsiteContentItem>("dbo.GetListContentGroupModule", paras, commandType: CommandType.StoredProcedure);
            connect.Close();
            return result.ToList();
        }
        
        public WebsiteContentItem GetContentById(int Ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select * from WebsiteContent where 1=1 and IsDeleted = 0 and IsShow = 1 and ID = @Ids", new { Ids });
                connect.Close();
                return result.FirstOrDefault();
            }
        }

        public List<WebsiteContentItem> GetSearchQA(string keyword, string lang, string listChidrent)
        {
            try
            {
                DataTable ModuleIds = new();
                ModuleIds.Columns.Add("KeyValue");
                if (!string.IsNullOrEmpty(listChidrent))
                {
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
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
                        @keys = keys.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                        keyword,
                        lang,
                    });
                    var result = connect.Query<WebsiteContentItem>("dbo.[GetSearchQA]", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<WebsiteContentItem>> GetListContentHome(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "")
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
            search.page = search.page > 1 ? search.page : 1;
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    search.lang,
                    @sqlOrder = search.strOrder,
                    @sqlWhere = search.strWhere,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<WebsiteContentItem> result = await connect.QueryAsync<WebsiteContentItem>("dbo.GetListContentHome", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }

            #region Remove

            //StringBuilder where = new StringBuilder();
            //string order = " ORDER BY CreatedDate DESC, OrderDisplay Asc, ID Desc";
            //if (!string.IsNullOrEmpty(search.strOrder))
            //{
            //    order = " " + search.strOrder;
            //}
            //if (!string.IsNullOrEmpty(search.strWhere))
            //{
            //    where.Append(" " + search.strWhere);
            //}
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds"));
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,*" +
            //        " FROM WebsiteContent WHERE 1=1 And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "' AND ID IN(SELECT ID FROM WebsiteContent " +
            //    " WHERE PublishDate <= GETDATE()) " + where.ToString() + order + "");
            //    return pageZise == 0 ? await _dapperDa.SelectAsync<WebsiteContentItem>(sql) : await _dapperDa.SelectPageAsync<WebsiteContentItem>(sql, search.page, pageZise);
            //}
            //else
            //{
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM WebsiteContent WHERE IsDeleted = 0 AND IsShow = 1 AND Lang = '" + search.lang + "' AND 1=1 AND PublishDate <= GETDATE() " + where.ToString() + order + "");
            //    return pageZise == 0 ? await _dapperDa.SelectAsync<WebsiteContentItem>(sql) : await _dapperDa.SelectPageAsync<WebsiteContentItem>(sql, search.page, pageZise);
            //}

            #endregion Remove
        }

        public async Task<IEnumerable<ProductItem>> GetListProductHomeAsync(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "")
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
            search.page = search.page > 1 ? search.page : 1;
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    search.lang,
                    @sqlOrder = search.strOrder,
                    @sqlWhere = search.strWhere,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<ProductItem> result = await connect.QueryAsync<ProductItem>("dbo.GetListProductHome", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }

            #region Remove

            //StringBuilder where = new StringBuilder();
            //string order = " ORDER BY CreatedDate DESC, OrderDisplay Asc, ID Desc";
            //if (!string.IsNullOrEmpty(search.strOrder))
            //{
            //    order = " " + search.strOrder;
            //}
            //if (!string.IsNullOrEmpty(search.strWhere))
            //{
            //    where.Append(" " + search.strWhere);
            //}
            //if (moduleId > 0)
            //{
            //    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
            //    lstId.Add(moduleId.ToString());
            //    where.Append(SqlUtility.WhereOrLikeList(lstId.Where(x => !string.IsNullOrEmpty(x)).ToList(), "ModuleIds"));
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,*" +
            //        " FROM Product WHERE 1=1 And IsDeleted = 0 AND IsShow = 1 And IsApproved = 1 AND Lang = '" + search.lang + "' AND ID IN(SELECT ID FROM Product " +
            //    " WHERE PublishDate <= GETDATE()) " + where.ToString() + order + "");
            //    return pageZise == 0 ? await _dapperDa.SelectAsync<ProductItem>(sql) : await _dapperDa.SelectPageAsync<ProductItem>(sql, search.page, pageZise);
            //}
            //else
            //{
            //    string sql = string.Format("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM Product WHERE IsDeleted = 0 AND IsShow = 1 AND Lang = '" + search.lang + "' AND 1=1 AND PublishDate <= GETDATE() " + where.ToString() + order + "");
            //    return pageZise == 0 ? await _dapperDa.SelectAsync<ProductItem>(sql) : await _dapperDa.SelectPageAsync<ProductItem>(sql, search.page, pageZise);
            //}

            #endregion Remove
        }

        public List<WebsiteContentItem> GetListByArrId(string relateIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select * from WebsiteContent where 1=1 And IsDeleted = 0 And IsShow = 1 and ',' + @relateIds + ',' like N'%,' + Convert(varchar,ID) + ',%'", new { relateIds });
                connect.Close();
                return result.ToList();
            }
        }

        public List<WebsiteContentItem> GetListByModuleTypeCode(string moduleTypeCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select * from WebsiteContent where 1=1 And IsDeleted = 0 And IsShow = 1 and ModuleIds in  (select ID from WebsiteModule where ModuleTypeCode = @moduleTypeCode)", new { moduleTypeCode });
                connect.Close();
                return result.ToList();
            }
        }

        public WebsiteContentItem GetByNameAscii(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select c.*, (select m.Comment from AspnetMembership m where c.CreatorID = m.UserId and m.IsLockedOut = 0 and m.IsApproved = 1) CommentCreator" +
                " from WebsiteContent c where c.IsDeleted = 0 and c.IsShow = 1 and c.IsApproved = 1 and c.NameAscii = @NameAscii AND c.CreatedDate <= GETDATE() Order By c.ID Desc", new { NameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }

        public WebsiteContentItem GetByNameAsciiPending(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select * from WebsiteContent where IsDeleted = 0 and IsShow = 1 and NameAscii = @NameAscii AND CreatedDate <= GETDATE() Order By ID Desc", new { NameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }

        public List<WebsiteContentItem> GetListById(string contentIds)
        {
            if (string.IsNullOrEmpty(contentIds))
            {
                return new List<WebsiteContentItem>();
            }
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("select * from WebsiteContent where IsDeleted = 0 and IsShow = 1 and ',' + @contentIds + ',' like N'%,' + Convert(varchar,ID) + ',%' AND CreatedDate <= GETDATE() Order By ID Desc", new { contentIds });
                connect.Close();
                return result.ToList();
            }
        }

        //tin tuc moi nhat
        public async Task<IEnumerable<WebsiteContentItem>> GetListContent(SearchModel search, int pageZise = 20, int moduleId = 0, string listChidrent = "", string contentIds = "0")
        {
            DataTable ModuleIds = new();
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
            search.start = search.page > 1 ? (search.page - 1) * pageZise : 0;
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                DynamicParameters paras = new();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    contentIds,
                    search.code,
                    search.sort,
                    search.lang,
                    @start = search.start,
                    @size = pageZise
                });
                IEnumerable<WebsiteContentItem> result = await connect.QueryAsync<WebsiteContentItem>("dbo.GetListContent", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }            
        }
        public List<WebsiteContentItem> GetTopListContentGroupModule(SearchModel search, int pageZise = 3, int moduleId = 0, string listChidrent = "")
        {
            DataTable ModuleIds = new();
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
                DynamicParameters paras = new();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    contentIds = "0",
                    search.code,
                    search.sort,
                    search.lang,
                    @start = 0,
                    @size = pageZise
                });
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("dbo.GetListContent", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<int> GetAllYearNews(int ModuleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<int> result = connect.Query<int>("select DISTINCT Year(CreatedDate) as Year from WebsiteContent where ',' + ModuleIds + ',' like N'%,' + Convert(varchar,@ModuleId) + ',%' and IsDeleted = 0 and IsShow = 1 order by Year desc", new { ModuleId });
                connect.Close();
                return result.ToList();
            }
        }

        //tin tuc xem nhieu nhat
        public async Task<IEnumerable<WebsiteContentItem>> GetListContentViewed(int pageZise = 10, int moduleId = 0, string listChidrent = "", string contentIds = "0")
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                DataTable ModuleIds = new();
                ModuleIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                }
                if (!string.IsNullOrEmpty(listChidrent))
                {
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    @start = 0,
                    @size = pageZise
                });
                IEnumerable<WebsiteContentItem> result = await connect.QueryAsync<WebsiteContentItem>("dbo.GetListContentViewed", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
        }

        //tin tuc Hot
        public List<WebsiteContentItem> GetListContentHot(int pageZise = 5, int moduleId = 0, string listChidrent = "")
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                if (moduleId > 0)
                {
                    ModuleIds.Rows.Add(moduleId.ToString());
                }
                if (!string.IsNullOrEmpty(listChidrent))
                {
                    List<string> lstId = ListHelper.GetValuesArrayTag(listChidrent);
                    if (lstId.Any())
                    {
                        lstId.ForEach(x =>
                        {
                            ModuleIds.Rows.Add(x);
                        });
                    }
                }
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]"),
                    @start = 0,
                    @size = pageZise
                });
                IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("dbo.GetListContentHot", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }

        //tin tuc Noi bat
        public List<WebsiteContentItem> GetListContentHighlights(int pageZise = 5, int moduleId = 0, string listChidrent = "")
        {
            if (moduleId > 0)
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("SELECT * FROM WebsiteContent WHERE IsDeleted = 0 AND IsShow = 1 AND ID IN(SELECT ID FROM WebsiteContent " +
              " WHERE (','+ModuleIds+',' LIKE '%,' + Convert(varchar,@moduleId) + ',%' OR ',' + @listChidrent + ',' LIKE '%,'+ModuleIds+',%') AND ',' + ViewHome + ',' LIKE '%,3,%' AND CreatedDate <= GETDATE()) ORDER BY TotalViews DESC, CreatedDate DESC OFFSET 0 ROWS FETCH NEXT @pageZise ROWS ONLY", new { moduleId, listChidrent, pageZise });
                    connect.Close();
                    return result.ToList();
                }
            }
            else
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    IEnumerable<WebsiteContentItem> result = connect.Query<WebsiteContentItem>("SELECT * FROM WebsiteContent WHERE IsDeleted = 0 AND IsShow = 1 AND ',' + ViewHome + ',' LIKE '%,2,%' AND CreatedDate <= GETDATE() ORDER BY TotalViews DESC, CreatedDate DESC OFFSET 0 ROWS FETCH NEXT @pageZise ROWS ONLY", new { pageZise });
                    connect.Close();
                    return result.ToList();
                }
            }
        }

        public async Task UpdateTotalViews(int contentId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                await connect.ExecuteAsync("UPdate WebsiteContent SET TotalViews = ISNULL(TotalViews,0) + 1 WHERE ID =@contentId ", new { contentId });
                connect.Close();
            }
        }

        public List<SubItem> GetSubItemsByListID(string Ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<SubItem> result = connect.Query<SubItem>("select * from SubItem where 1=1 and IsDeleted = 0 and IsShow = 1 and ',' + @Ids + ',' LIKE '%,'+CONVERT(varchar(10), ContentID)+',%'", new { Ids });
                connect.Close();
                return result.ToList();
            }
        }

        public async Task<IEnumerable<SubItem>> GetSubItemsByContentID(string id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<SubItem> result = await connect.QueryAsync<SubItem>("select * from SubItem where 1=1 and IsDeleted = 0 and IsShow = 1 and ContentID = @id and ModuleTypeCode = 'Content'", new { id });
                connect.Close();
                return result.ToList();
            }
        }

        public async Task<IEnumerable<TagItem>> GetAllTagItems(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<TagItem> result = await connect.QueryAsync<TagItem>("select * from SystemTag where IsDeleted = 0 and IsShow =1 and ',' + @ids + ',' like N'%,'+CONVERT(varchar,ID)+',%'", new { ids });
                connect.Close();
                return result;
            }
        }

        public List<WebsiteContentItem> GetListContentByTagItem(string keyword, string lang, int page = 1, int size = 20)
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
                    var result = connect.Query<WebsiteContentItem>("dbo.GetTagValues", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<WebsiteContentItem> GetSearchContent(string keyword, string lang, int page = 1, int size = 20)
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
                        @keys = keys.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        keyword,
                        lang,
                        start,
                        size
                    });
                    var result = connect.Query<WebsiteContentItem>("dbo.GetSearchContent", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<WebsiteContentItem> GetSearchDoctor(string keyword, int specialistid, int addressid, int moduleid, string lang)
        {
            try
            {
                DataTable keys = new();
                keys.Columns.Add("ID");
                keys.Columns.Add("KeyItem");
                if (!string.IsNullOrEmpty(keyword))
                {
                    var keyids = ListHelper.GetValuesArrayTag2(keyword);
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
                        @keys = keys.AsTableValuedParameter("[dbo].[KeysSearchSort]"),
                        @specialistid = specialistid,
                        @addressid = addressid,
                        @moduleid = moduleid,
                        @lang = lang
                    });
                    var result = connect.Query<WebsiteContentItem>("dbo.GetSearchDoctor", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}