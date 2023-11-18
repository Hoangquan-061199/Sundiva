using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using System;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class ContactUsManager
    {
        private readonly DapperDA _dapperDa;
        public ContactUsManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public int Insert(ContactUs obj)
        {
            obj.CreatedDate = DateTime.Now;
            obj.Status = 1;
            return _dapperDa.Insert(obj);
        }
    }
}
