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
    public class DepartmentDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public DepartmentDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<Department> GetAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Department>("select * from Department");
                connect.Close();
                return result.ToList();
            }
        }
        public List<DepartmentAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<DepartmentAdmin>("SELECT * FROM Department WHERE 1=1 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<DepartmentAdmin>("SELECT * FROM Department WHERE 1=1 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public Department GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Department>("select * from Department where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Department GetName(string name)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Department>("select * from Department where Name = @name", new { name });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(Department obj) => _dapperDa.Insert(obj);
        public int Update(Department obj) => _dapperDa.Update(obj);
        public void Delete(Department obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
