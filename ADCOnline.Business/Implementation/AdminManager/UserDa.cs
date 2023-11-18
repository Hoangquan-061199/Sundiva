using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class UserDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public UserDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        public MembershipAdmin GetMembership(string guidId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<MembershipAdmin>("select m.*, u.UserName from aspnetMembership m, aspnetUsers u where m.UserId = u.UserId AND m.UserId=@guidId", new { guidId });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(AspnetMembership obj)
        => _dapperDa.Insert(obj);
        public int Update(AspnetMembership obj)
        => _dapperDa.Update(obj);
        public void Delete(AspnetMembership obj)
        => _dapperDa.Delete(obj);
    }
}
