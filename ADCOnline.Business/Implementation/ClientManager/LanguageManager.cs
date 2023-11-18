using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class LanguageManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public LanguageManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<LanguageItem> GetListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<LanguageItem>("select ID, Name, Code, OrderDisplay, UrlPicture, IsShow from [Language] where IsShow = 1 and IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
    }
}
