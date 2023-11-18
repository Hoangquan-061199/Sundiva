using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class ActiveRoleDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ActiveRoleDa(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public List<ActiveRole> GetAll()
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ActiveRole>("select * from ActiveRole where IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }          
        }
        public int Insert(ActiveRole obj) => _dapperDa.Insert(obj);
        public int Update(ActiveRole obj) => _dapperDa.Update(obj);
        public void Delete(ActiveRole obj) => _dapperDa.Delete(obj);
    }
}
