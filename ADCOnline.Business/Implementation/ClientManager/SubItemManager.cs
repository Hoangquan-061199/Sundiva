using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class SubItemManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public SubItemManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }

        public List<SubItem> GetAll(string lang, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SubItem>("SELECT * FROM SubItem WHERE IsDeleted = 0 and Lang = @lang and ProductID = @productId Order By OrderDisplay", new { lang, productId });
                connect.Close();
                return result.ToList();
            }
        }

        public List<ProductItem> GetProductByParentID(string lang, int productId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductItem>("select * from Product where ParentID = @productId and IsDeleted = 0 and IsShow = 1 and Lang = @lang", new { lang, productId });
                connect.Close();
                return result.ToList();
            }
        }


    }
}
