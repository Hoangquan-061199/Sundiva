using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class SystemTagDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public SystemTagDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<SystemTag> GetAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemTag>("select * from SystemTag");
                connect.Close();
                return result.ToList();
            }
        }
        public List<SystemTag> GetListByArrId(string relateIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemTag>("select * from SystemTag where IsDeleted = 0 And IsShow = 1 AND ',' + @relateIds + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%'", new { relateIds });
                connect.Close();
                return result.ToList();
            }
        }
        public SystemTag GetByNameAscii(string NameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemTag>("select * from SystemTag where IsDeleted = 0 and IsShow = 1 and NameAscii = @NameAscii", new { NameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<SystemTagAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<SystemTagAdmin>("SELECT * FROM SystemTag WHERE 1=1 and IsDeleted = 0 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<SystemTagAdmin>("SELECT * FROM SystemTag WHERE 1=1 and IsDeleted = 0 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public SystemTag GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemTag>("SELECT * FROM SystemTag WHERE 1=1 AND IsDeleted = 0 And ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public SystemTag GetName(string name)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemTag>("SELECT * FROM SystemTag WHERE 1=1 AND IsDeleted = 0 And Name = @name", new { name });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(SystemTag obj)
        => _dapperDa.Insert(obj);
        public int Update(SystemTag obj)
        => _dapperDa.Update(obj);
        public void Delete(SystemTag obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
