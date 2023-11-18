using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class MembershipDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public MembershipDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public AspnetMembership GetLogin(string userName, string password)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetMembership>("SELECT m.* FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId AND u.UserName=@userName AND IsApproved =1", new { userName });
                connect.Close();
                var membership = result.FirstOrDefault();
                if (membership != null)
                {
                    var sha1PasswordHash = Utility.CreatePasswordHash(password, membership.PasswordSalt);
                    if (sha1PasswordHash == membership.Password)
                    {
                        return membership;
                    }
                }
            }
            return null;
        }
        public AspnetMembership GetUserName(string userName)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetMembership>("SELECT m.* FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId AND u.UserName=@userName AND IsApproved =1", new { userName });
                connect.Close();
                return result.FirstOrDefault();
            } 
        }
        public List<MembershipAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if(search!=null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<MembershipAdmin>("SELECT m.*, u.UserName, u.DepartmentID FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId AND u.UserName LIKE '%' + @key + '%' ESCAPE N'~'", new {@key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<MembershipAdmin>("SELECT m.*, u.UserName, u.DepartmentID FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId");
                    connect.Close();
                    return result.ToList();
                }
                
            }
        }
        public AspnetMembership GetId(Guid id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetMembership>("SELECT * FROM aspnetMembership m WHERE  UserId=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            } 
        }
        public MembershipAdmin GetAdminId(Guid guidId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<MembershipAdmin>("SELECT m.*, u.UserName, u.DepartmentID FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId AND u.UserId=@guidId", new { guidId });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public MembershipAdmin GetAdminUserName(string username)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<MembershipAdmin>("SELECT m.*, u.UserName, u.DepartmentID FROM aspnetMembership m, aspnetUsers u WHERE m.UserId = u.UserId AND u.UserName=@username", new { username });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(AspnetMembership obj) => _dapperDa.InsertUserNoId(obj);
        public int Update(AspnetMembership obj, Guid guidId) => _dapperDa.UpdateNoId(obj, $" UserId ='{guidId}'");
        public void Delete(AspnetMembership obj) => _dapperDa.Delete(obj);
        #region thong tin user
        public AspnetUsers GetUserId(Guid id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AspnetUsers>("SELECT * FROM AspnetUsers m WHERE  UserId=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int InsertUser(AspnetUsers obj) => _dapperDa.InsertUserNoId(obj);
        public int UpdateUser(AspnetUsers obj, Guid guidId)
        => _dapperDa.UpdateNoId(obj, $" UserId ='{guidId}'");
        public void DeleteUser(AspnetUsers obj)
        => _dapperDa.Delete(obj);
        #endregion
    }
}
