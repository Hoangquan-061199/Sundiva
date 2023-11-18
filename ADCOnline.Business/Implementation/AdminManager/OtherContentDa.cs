using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class OtherContentDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public OtherContentDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<OtherContentAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<OtherContentAdmin>("SELECT * FROM OtherContent WHERE 1=1 AND IsDeleted = 0 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<OtherContentAdmin>("SELECT * FROM OtherContent WHERE 1=1 AND IsDeleted = 0 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public OtherContent GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OtherContent>("select * from OtherContent WHERE ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public OtherContent GetName(string name)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OtherContent>("select * from OtherContent WHERE Name =@name", new { name });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public OtherContent GetByCode(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OtherContent>("select * from OtherContent WHERE Code =@code and IsDeleted = 0 and IsShow = 1 and Lang = @lang", new { code, lang });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(OtherContent obj) => _dapperDa.Insert(obj);
        public int Update(OtherContent obj) => _dapperDa.Update(obj);
    }
}
