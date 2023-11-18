using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.Simple.Item;
using System.Text;
using ADCOnline.Simple.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.ClientManager
{

    public class CommentManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        private readonly string _pathServer;
        public CommentManager(string connectionSql, string pathServer = "")
        {
            _dapperDa = new DapperDA(connectionSql, pathServer);
            _pathServer = pathServer;
        }
        public async Task<IEnumerable<CommentItem>> GetListCommentByPage(SearchModel search, int pageZise = 20)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @type = 1,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = await connect.QueryAsync<CommentItem>("dbo.GetListCommentByPage", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*,");
            //sql.Append("(");
            //sql.Append("select Sum(su.Total) from (");
            //sql.Append("select COUNT(csu.ID) Total from Comment csu where csu.IsDeleted = 0 AND csu.IsShow = 1 and csu.IsApproved = 1 And csu.ParentID = a.ID");
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select COUNT(csu.ID) Total from Comment csu where csu.IsDeleted = 0 AND csu.IsShow = 1 and csu.IsApproved = 0 And csu.ParentID = a.ID and csu.CustomerID = " + search.customerId);
            //}
            //sql.Append(") su");
            //sql.Append(") TotalReply");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Comment' And ParentID is null" + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Comment' And ParentID is null and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc");
            //return await _dapperDa.SelectPageAsync<CommentItem>(sql.ToString(), search.page, pageZise);
            #endregion
        }
        public async Task<IEnumerable<CommentItem>> GetListReplyByPage(SearchModel search, int pageZise = 20)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @parentId = search.parentId,
                    @type = 1,
                    @start = (search.page - 1) * pageZise,
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = await connect.QueryAsync<CommentItem>("dbo.GetListReplyByPage", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Comment' And ParentID = " + search.parentId + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Comment' And ParentID = " + search.parentId + " and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc");
            //return await _dapperDa.SelectPageAsync<CommentItem>(sql.ToString(), search.page, pageZise);
            #endregion
        }
        public List<CommentItem> GetListReplyBySkip(SearchModel search, int pageZise = 3)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @type = 1,
                    @parentID = search.parentId,
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = connect.Query<CommentItem>("dbo.GetListReplyBySkip", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Comment' And ParentID = " + search.parentId + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Comment' And ParentID = " + search.parentId + " and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc OFFSET "+pageZise+" ROWS");
            //var result = _dapperDa.Select<CommentItem>(sql.ToString()).ToList();
            //return result;
            #endregion
        }
        public List<CommentItem> GetListReplyRateBySkip(SearchModel search, int pageZise = 3)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @type = 0,
                    @parentID = search.parentId,
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = connect.Query<CommentItem>("dbo.GetListReplyBySkip", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Rate' And ParentID = " + search.parentId + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Rate' And ParentID = " + search.parentId + " and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc OFFSET " + pageZise + " ROWS");
            //var result = _dapperDa.Select<CommentItem>(sql.ToString()).ToList();
            //return result;
            #endregion
        }
        public async Task<IEnumerable<CommentItem>> GetAllCommentCategory(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CommentItem> result = await connect.QueryAsync<CommentItem>("select Fullname,CreatedDate,UrlPicture,Content,Rate from Comment where IsDeleted = 0 and IsShow = 1 and Act = 'Rate' And IsApproved = 1 And ParentID is null and ',' + @ids + ',' like N'%,'+ Convert(varchar,ProductID) +',%' order by CreatedDate desc", new { ids });
                connect.Close();
                return result;
            }
        }
            //=> await _dapperDa.SelectAsync<CommentItem>(string.Format("select Fullname,CreatedDate,UrlPicture,Content,Rate from Comment where IsDeleted = 0 and IsShow = 1 and Act = 'Rate' And IsApproved = 1 And ParentID is null and ProductID in ({0}) order by CreatedDate desc", ids));
        public async Task<IEnumerable<CommentItem>> GetListRateByPage(SearchModel search, int pageZise = 20)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @type = 0,
                    @start = ((search.page - 1) * pageZise),
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = await connect.QueryAsync<CommentItem>("dbo.GetListCommentByPage", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*,");
            //sql.Append("(");
            //sql.Append("select Sum(su.Total) from (");
            //sql.Append("select COUNT(csu.ID) Total from Comment csu where csu.IsDeleted = 0 AND csu.IsShow = 1 and csu.IsApproved = 1 And csu.ParentID = a.ID");
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select COUNT(csu.ID) Total from Comment csu where csu.IsDeleted = 0 AND csu.IsShow = 1 and csu.IsApproved = 0 And csu.ParentID = a.ID and csu.CustomerID = " + search.customerId);
            //}
            //sql.Append(") su");
            //sql.Append(") TotalReply");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Rate' And ParentID is null" + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Rate' And ParentID is null and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc");
            //return await _dapperDa.SelectPageAsync<CommentItem>(sql.ToString(), search.page, pageZise);
            #endregion
        }
        public async Task<IEnumerable<CommentItem>> GetListReplyRateByPage(SearchModel search, int pageZise = 20)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.contentId,
                    search.productId,
                    search.customerId,
                    @parentId = search.parentId,
                    @type = 0,
                    @start = ((search.page - 1) * pageZise),
                    @size = pageZise
                });
                IEnumerable<CommentItem> result = await connect.QueryAsync<CommentItem>("dbo.GetListReplyByPage", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result;
            }
            #region Remove
            //StringBuilder where = new StringBuilder();
            //StringBuilder sql = new StringBuilder();
            //if (search.contentId > 0)
            //{
            //    where.Append(" and ContentID = " + search.contentId);
            //}
            //if (search.productId > 0)
            //{
            //    where.Append(" and ProductID = " + search.productId);
            //}
            //sql.Append("SELECT COUNT(ID) OVER () AS TotalRecord,*");
            //sql.Append(" FROM Comment a WHERE ID in (");
            //sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and IsApproved = 1 and Act = 'Rate' And ParentID = " + search.parentId + where.ToString());
            //if (search.customerId > 0)
            //{
            //    sql.Append(" union all");
            //    sql.Append(" select ID from Comment where IsDeleted = 0 AND IsShow = 1 and Act = 'Rate' And ParentID = " + search.parentId + " and IsApproved = 0 and CustomerID = " + search.customerId + where.ToString());
            //}
            //sql.Append(") order by CreatedDate desc");
            //return await _dapperDa.SelectPageAsync<CommentItem>(sql.ToString(), search.page, pageZise);
            #endregion
        }
    }
}
