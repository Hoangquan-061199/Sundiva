using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using ADCOnline.Simple.Item;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class OtherContentManager : BaseDa
    {
        private readonly DapperDA _dapperDa;

        public OtherContentManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public async Task<OtherContentItem> GetByCodeLang(string code, string lang)
        {
            IEnumerable<OtherContentItem> obj = await _dapperDa.SelectAsync<OtherContentItem>(string.Format("select ID,Name,Code,Content from OtherContent where IsDeleted = 0 and IsShow = 1 and Lang = '{0}' and Code like '%{1}%'", lang, code));
            return obj.FirstOrDefault();
        }
        public async Task<IEnumerable<OtherContentItem>> GetListByCodeLang(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<OtherContentItem> result = await connect.QueryAsync<OtherContentItem>("select ID,Name,Code,Content,UrlAvatar from OtherContent where IsDeleted = 0 and IsShow = 1 and Lang = @lang and ',' + @code + ',' like N'%,'+Code+',%'", new { lang, code });
                connect.Close();
                return result.ToList();
            }
        }
    }
}
