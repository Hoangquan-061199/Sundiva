using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class SystemTagManager
    {
        private readonly DapperDA _dapperDa;

        public SystemTagManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<SystemTag> GetListSystemTag()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<SystemTag> result = connect.Query<SystemTag>("select Name, NameAscii, OrderBy from SystemTag where IsDeleted = 0 and IsShow = 1 Order by OrderBy asc");
                connect.Close();
                return result.ToList();
            }
        }
    }
}