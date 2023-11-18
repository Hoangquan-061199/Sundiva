using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class LanguageDa :BaseDa
    {
        private readonly DapperDA _dapperDa;
        public LanguageDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<LanguageAdmin> GetListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<LanguageAdmin>("select ID, Name, Code, OrderDisplay, UrlPicture, IsShow from [Language] where IsShow = 1 and IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public Language GetLanguageByID(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Language>("select ID, Name, Code, IsShow, IsDeleted, OrderDisplay, UrlPicture from [Language] where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<LanguageAdmin> ListSearch(SearchModel search)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            if (search != null && !string.IsNullOrEmpty(search.keyword))
            {
                var result = connect.Query<LanguageAdmin>("SELECT ID, Name, Code, IsShow, OrderDisplay, UrlPicture FROM [Language] WHERE 1=1 AND IsDeleted = 0 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                connect.Close();
                return result.ToList();
            }
            else
            {
                var result = connect.Query<LanguageAdmin>("SELECT ID, Name, Code, IsShow, OrderDisplay, UrlPicture FROM [Language] WHERE 1=1 AND IsDeleted = 0 ORDER BY ID DESC");
                connect.Close();
                return result.ToList();
            }
        }
        public int Insert(Language obj) => _dapperDa.Insert(obj);
        public int Update(Language obj) => _dapperDa.Update(obj);
    }
}
