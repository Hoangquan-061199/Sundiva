using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class CommentAdminDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public CommentAdminDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<CommentAdmin> ListSearch(SearchModel search, int page, int rowPage)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                page = page > 1 ? page : 1;
                rowPage = rowPage > 1 ? rowPage : 10;
                int start = ((page - 1) * rowPage);
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    @keyAscii = SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)),
                    @show = search.Show,
                    search.approal,
                    search.contentId,
                    search.productId,
                    search.parentId,
                    start,
                    @size = rowPage,
                    @type = 1
                });
                var result = connect.Query<CommentAdmin>("dbo.AdminCommentListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<CommentAdmin> ListRateSearch(SearchModel search, int page, int rowPage)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                page = page > 1 ? page : 1;
                rowPage = rowPage > 1 ? rowPage : 10;
                int start = ((page - 1) * rowPage);
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    @keyAscii = SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)),
                    @show = search.Show,
                    search.approal,
                    search.contentId,
                    search.productId,
                    search.parentId,
                    start,
                    @size = rowPage,
                    @type = 0
                });
                var result = connect.Query<CommentAdmin>("dbo.AdminCommentListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public Comment GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Comment>("select * from Comment where ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<CommentAdmin> GetAllCommentNotIsApproved(SearchModel search)
        {
            search.page = search.page > 1 ? search.page : 1;
            search.pagesize = search.pagesize > 1 ? search.pagesize : 10;
            int start = ((search.page - 1) * search.pagesize);
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CommentAdmin>("select COUNT(ID) OVER () as TotalRecord, * from (" +
                "select cm.ID,cm.Title,cm.ParentID,cm.ProductID,cm.CreatedDate,cm.Act,p.Name ProductName, p.ProductCode " +
                "from Comment cm inner join Product p on cm.ProductID = p.ID where cm.IsApproved = 0 and cm.IsDeleted = 0 and cm.IsShow = 1 and p.IsDeleted = 0 and p.IsShow = 1 and p.IsApproved = 1 " +
                ") c order by c.CreatedDate desc OFFSET @start ROWS FETCH NEXT @size ROWS ONLY", new { @start = start, @size = search.pagesize });
                connect.Close();
                return result.ToList();
            }
        }
        public int CountRate(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<int>("select COUNT(ID) from Comment where ParentID is null and ProductID = @id and IsApproved = 1 and IsDeleted = 0 and Act = 'Rate'", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int SumStar(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<int>("select SUM(Rate) from Comment where ParentID is null and ProductID = @id and IsApproved = 1 and IsDeleted = 0 and Act = 'Rate'", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Comment GetRateByEmail(int id, string email)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Comment>("select * from Comment where ProductID = @id and IsDeleted = 0 and IsApproved = 1 and Email = @email and Act = 'Rate' and ParentID is null", new { id, email });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<CommentAdmin> GetRepliesByID(int parentid, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CommentAdmin>("select * from Comment where ProductID = @productId and IsDeleted = 0 and ParentID = @parentid Order By CreatedDate Desc", new { productId, parentid });
                connect.Close();
                return result.ToList();
            }
        }
        public List<CommentAdmin> GetRepliesByContentID(int parentid, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CommentAdmin>("select * from Comment where ContentID = @productId and IsDeleted = 0 and ParentID = @parentid Order By CreatedDate Desc", new { productId, parentid });
                connect.Close();
                return result.ToList();
            }
        }
        public List<CommentAdmin> GetRepliesRateByID(int parentid, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CommentAdmin>("select * from Comment where ProductID = @productId and IsDeleted = 0 and Act = 'Rate' and ParentID = @parentid Order By CreatedDate Desc", new { productId, parentid });
                connect.Close();
                return result.ToList();
            }
        }
        public int Insert(Comment obj) => _dapperDa.Insert(obj);
        public int Update(Comment obj) => _dapperDa.Update(obj);
        public void Delete(Comment obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
