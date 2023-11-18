using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using Dapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class WebsiteMenuManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public WebsiteMenuManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public List<WebsiteMenuItem> GetMainMenu(string Lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteMenuItem> result = connect.Query<WebsiteMenuItem>("select a.ID,a.Name,a.Title,a.Link,a.LinkBanner, a.UrlPicture,a.UrlVideo,A.ParentID, b.ModuleTypeCode,b.TypeView,b.NameAscii,b.LinkUrl, a.OrderDisplay,a.IsShowAll from WebsiteMenu a left join WebsiteModule b on a.ModuleID = b.ID where a.IsDeleted = 0 and a.IsShow = 1 and 1=1 And a.Lang = @Lang", new { Lang });
                connect.Close();
                return result.ToList();
            }
        }
    }
}
