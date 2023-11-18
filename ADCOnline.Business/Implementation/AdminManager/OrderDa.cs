using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using ADCOnline.Utils;
using System.Data;
using ADCOnline.Simple.Item;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class OrderDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public OrderDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<OrderAdmin> ListSearch(Simple.Admin.SearchModel search, int page, int rowPage, bool isExport)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    page = page > 1 ? page : 1;
                    rowPage = rowPage > 1 ? rowPage : 10;
                    int start = ((page - 1) * rowPage);
                    var paras = new DynamicParameters();
                    var from = !string.IsNullOrEmpty(search.from) ? SqlUtility.ConvertToDate(search.from, false) : null;
                    var to = !string.IsNullOrEmpty(search.to) ? SqlUtility.ConvertToDate(search.to, false) : null;
                    paras.AddDynamicParams(new
                    {
                        @keyword = SqlUtility.CharacterSpecail(search.keyword),
                        @status = search.Status,
                        search.paymenttype,
                        search.ExportBill,
                        search.customerId,
                        @CityID = ConvertUtil.ToInt32(search.CityID),
                        @DistrictID = ConvertUtil.ToInt32(search.DistrictID),
                        @WardID = ConvertUtil.ToInt32(search.WardID),
                        from,
                        to,
                        start,
                        @size = rowPage,
                        isExport
                    });
                    var result = connect.Query<OrderAdmin>("dbo.AdminOrderListSearch", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {
                return new List<OrderAdmin>();
            }
        }
        public Order GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Order>("select * from [Order] where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ProductAdmin> GetListProductByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductAdmin>("SELECT * FROM Product where ',' + @listIds + ',' LIKE '%,' + CONVERT(varchar(10),ID) + ',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Attributes> GetAttributeByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attributes>("SELECT * FROM Attributes where ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<OrderDetail> GetListOrderDetailByListOrderIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OrderDetail>("SELECT * FROM OrderDetail where IsDeleted = 0 and OrderID is not null AND ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10), OrderID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public OrderAdmin GetById(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OrderAdmin>("SELECT * FROM [Order] where IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<OrderDetailItem> GetOrderDetailByOrder(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OrderDetailItem>("select od.*,p.Name ProductName,p.NameAscii,p.ModuleNameAscii, p.LinkUrl from OrderDetail od inner join Product p on od.ProductID = p.ID where od.OrderID = @id and od.IsDeleted = 0", new { id });
                connect.Close();
                return result.ToList();
            }
            //return _dapperDa.Select<OrderDetailItem>($"select od.*,p.Name ProductName,p.NameAscii,p.ModuleNameAscii, p.LinkUrl from OrderDetail od inner join Product p on od.ProductID = p.ID where od.OrderID = {id} and od.IsDeleted = 0").ToList();
        }
        public List<StatisticalOrder> StatisticalOrder()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<StatisticalOrder>("select Status, COUNT(ID) Total from [Order] where IsDeleted = 0 group by Status");
                connect.Close();
                return result.ToList();
            }
        }
        public Order GetCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Order>("select * from [Order] where OrderCode = @code", new {code});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(Order obj) => _dapperDa.Insert(obj);
        public int Update(Order obj)
        => _dapperDa.Update(obj);
    }
}
