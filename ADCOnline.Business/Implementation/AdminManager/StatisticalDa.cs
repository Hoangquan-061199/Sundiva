using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ADCOnline.Business.Implementation.AdminManager
{
    public class StatisticalDa
    {
        private readonly DapperDA _dapperDa;
        public StatisticalDa(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public List<StatisticalAdmin> StatisticalAdmins()
        {
            string sql = "select 'Order' Name, COUNT(Id) as Total from [Order] where IsDeleted = 0 And Status in (1,2,3)" +
                " union all" +
                " select 'Product' Name,COUNT(Id) as Total from Product where IsDeleted = 0" +
                " union all" +
                " select 'WebsiteModuleProduct' Name,COUNT(ID) Total from WebsiteModule where IsDeleted = 0 And ModuleTypeCode in ('Product','Sale')" +
                " union all" +
                " select 'TradeMark' Name,COUNT(ID) Total from WebsiteModule where IsDeleted = 0 And ModuleTypeCode in ('TradeMark') and ParentID = 0" +
                " union all" +
                " select 'Comment' Name,COUNT(Id) as Total from Comment where IsDeleted = 0 And IsApproved = 0 And Act = 'Comment'" +
                " union all" +
                " select 'Rate' Name,COUNT(Id) as Total from Comment where IsDeleted = 0 And IsApproved = 0 And Act = 'Rate'" +
                " union all" +
                " select 'WebsiteContent' Name,COUNT(ID) as Total from WebsiteContent where IsDeleted = 0 and NameAscii is not null And (Status!=0 Or Status Is Null)" +
                " union all" +
                " select 'WebsiteModuleContent' Name,COUNT(ID) as Total from WebsiteModule where IsDeleted = 0" +
                " union all" +
                " select 'ContactUs' Name, COUNT(ID) from ContactUs where Status = 1 And Code = 'Contact'" +
                " union all" +
                " select 'Advisor' Name, COUNT(ID) from ContactUs where Status = 1 And Code = 'Advisor'" +
                " union all" +
                " select 'Question' Name,COUNT(ID) as Total from WebsiteContent where IsDeleted = 0 and NameAscii is not null And Status=0";
            var result = _dapperDa.Select<StatisticalAdmin>(sql);
            return result.ToList();
        }
    }
}
