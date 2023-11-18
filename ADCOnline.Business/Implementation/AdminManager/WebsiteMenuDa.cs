using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class WebsiteMenuDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public WebsiteMenuDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<WebsiteMenuAdmin> GetAdminAll(bool isShow, string lang)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<WebsiteMenuAdmin>("SELECT * from WebsiteMenu WHERE 1=1 And IsDeleted = 0 and Lang = @lang And IsShow = @isShow", new { isShow, lang });
                    connect.Close();
                    return result.ToList();
                }               
            }
            catch
            {
            }
            return null;
        }        
        public List<WebsiteMenu> ListAllJson(string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteMenu>("SELECT * FROM WebsiteMenu WHERE IsDeleted = 0 And IsShow = 1 and Lang = @lang", new { lang });
                connect.Close();
                return result.ToList();
            }
        }
        public WebsiteMenu GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteMenu>("SELECT * FROM WebsiteMenu WHERE IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(WebsiteMenu obj) => _dapperDa.Insert(obj);
        public int Update(WebsiteMenu obj) => _dapperDa.Update(obj);
    }
}
