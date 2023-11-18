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
    public class ModuleTypeDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ModuleTypeDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<ModuleTypeAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<ModuleTypeAdmin>("SELECT * FROM ModuleType WHERE 1=1 AND IsDeleted = 0 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY  ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<ModuleTypeAdmin>("SELECT * FROM ModuleType WHERE 1=1 AND IsDeleted = 0 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }

        public List<ModuleType> ListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModuleType> ListAllIsHow()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 AND IsShow=1");
                connect.Close();
                return result.ToList();
            }
        }
        public ModuleType GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 AND ID=@id", new {id});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public ModuleType GetCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 AND Code = @code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ModuleType> GetCode(List<string> listCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 AND @code LIKE '%'+Code+'%'", new { @code = string.Join(",", listCode) });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModuleType> GetListCode(string code, string notcode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if(!string.IsNullOrEmpty(code) && string.IsNullOrEmpty(notcode))
                {
                    var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 And ',' + @code + ',' like N'%,' + Code + ',%'", new { code });
                    connect.Close();
                    return result.ToList();
                }
                else if(string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(notcode))
                {
                    var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0 And ',' + @code + ',' not like N'%,' + Code + ',%'", new { code });
                    connect.Close();
                    return result.ToList();
                }
                else{
                    var result = connect.Query<ModuleType>("SELECT * FROM ModuleType WHERE IsDeleted = 0");
                    connect.Close();
                    return result.ToList();
                }
            }            
        }
        public int Insert(ModuleType obj) => _dapperDa.Insert(obj);
        public int Update(ModuleType obj) => _dapperDa.Update(obj);
        public int ExecuteSql(string sql) => _dapperDa.ExecuteSql(sql);
    }
}
