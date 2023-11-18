using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using System.Linq;
using System;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class MemberManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public MemberManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public int Insert(Member obj)
        {
            obj.CreatedDate = DateTime.Now;
            obj.IsDeleted = false;
            return _dapperDa.Insert(obj);
        }
    }
}
