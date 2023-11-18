using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class RolesDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public RolesDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public List<RolesAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<RolesAdmin>("SELECT * FROM AspnetRoles WHERE 1=1 AND Name LIKE '%' + @key + '%' ESCAPE N'~'", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<RolesAdmin>("SELECT * FROM AspnetRoles WHERE 1=1");
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public AspnetRoles GetId(Guid id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetRoles>("select * from AspnetRoles where IsDeleted = 0 and RoleId = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<RolesAdmin> GetAdminAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<RolesAdmin>("select * from aspnetRoles");
                connect.Close();
                return result.ToList();
            }
        }
        public AspnetRoles GetRoleName(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetRoles>("select * from AspnetRoles where IsDeleted = 0 and RoleName = @code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(AspnetRoles obj)
        => _dapperDa.Insert(obj);
        public int Update(AspnetRoles obj)
        => _dapperDa.Update(obj);
    }
}
