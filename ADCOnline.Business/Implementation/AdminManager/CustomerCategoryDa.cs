using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class CustomerCategoryDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public CustomerCategoryDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<CustomerCategoryAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<CustomerCategoryAdmin>("SELECT * FROM CustomerCategory WHERE 1=1 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<CustomerCategoryAdmin>("SELECT * FROM CustomerCategory WHERE 1=1 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public List<CustomerCategoryAdmin> GetListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CustomerCategoryAdmin>("select ID,Name from CustomerCategory where IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public int Insert(CustomerCategory obj) => _dapperDa.Insert(obj);
        public int Update(CustomerCategory obj) => _dapperDa.Update(obj);
        public void Delete(CustomerCategory obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
        public CustomerCategory GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CustomerCategory>("select * from CustomerCategory where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
    }
}
