using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class MemberDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public MemberDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<MemberAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
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
                    @from = !string.IsNullOrEmpty(search.from) ? SqlUtility.ConvertDate(search.from, false) : null,
                    @to = !string.IsNullOrEmpty(search.to) ? SqlUtility.ConvertDate(search.to, false) : null,
                    start,
                    @size = rowPage,
                    isExport
                });
                var result = connect.Query<MemberAdmin>("dbo.AdminMemberListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public Member GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Member>("select * from Member WHERE ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Member GetUserName(string userName)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Member>("select * from Member WHERE UserName = @userName", new { userName });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(Member obj)
        => _dapperDa.Insert(obj);
        public int Update(Member obj) => _dapperDa.Update(obj);
        public int ExecuteSql(string sql) => _dapperDa.ExecuteSql(sql);
        public void Delete(Member obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}