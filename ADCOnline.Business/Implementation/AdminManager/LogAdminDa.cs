using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class LogAdminDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public LogAdminDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<LogAdminAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            page = page > 1 ? page : 1;
            rowPage = rowPage > 1 ? rowPage : 10;
            int start = ((page - 1) * rowPage);
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<LogAdminAdmin>("select COUNT(ID) OVER () as TotalRecord, * FROM (SELECT l.*, m.NameModule FROM LogAdmin l, Module m WHERE Lower(l.ClassControl) = Lower(m.Tag) AND (l.Content LIKE '%' + @key + '%' ESCAPE N'~' OR l.UserLogin LIKE '%' + @key + '%' ESCAPE N'~' OR m.NameModule LIKE '%' + @key + '%' ESCAPE N'~')) c ORDER BY ID DESC OFFSET @start ROWS FETCH NEXT @size ROWS ONLY", new { @key = SqlUtility.CharacterSpecail(search.keyword), start, @size = rowPage });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<LogAdminAdmin>("select COUNT(ID) OVER () as TotalRecord, * FROM (SELECT l.*, m.NameModule FROM LogAdmin l, Module m WHERE Lower(l.ClassControl) = Lower(m.Tag)) c ORDER BY ID DESC OFFSET @start ROWS FETCH NEXT @size ROWS ONLY", new { start, @size = rowPage });
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public LogAdmin GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<LogAdmin>("select * from LogAdmin where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(LogAdmin obj) => _dapperDa.Insert(obj);
        public int Update(LogAdmin obj) => _dapperDa.Update(obj);
        public void Delete(LogAdmin obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}

