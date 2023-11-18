using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Json;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class SystemConfigManager
    {
        private readonly DapperDA _dapperDa;
        public SystemConfigManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public SystemConfigJson GetSystemConfigBase(string lang)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            IEnumerable<SystemConfigJson> result = connect.Query<SystemConfigJson>("select * from SystemConfig where lang = @lang", new { lang });
            connect.Close();
            return result.FirstOrDefault();
        }
    }
}
