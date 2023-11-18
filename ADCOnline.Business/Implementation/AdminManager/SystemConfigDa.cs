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
    public class SystemConfigDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public SystemConfigDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);        
        public SystemConfig GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemConfig>("select * from SystemConfig where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public SystemConfig GetByLang(string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemConfig>("select * from SystemConfig where Lang = @lang", new { lang });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public SystemConfig GetNameByLang(string name,string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SystemConfig>("select * from SystemConfig where Name = @name And Lang = @lang", new { name, lang });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Update(SystemConfig obj) => _dapperDa.Update(obj);
    }
}
