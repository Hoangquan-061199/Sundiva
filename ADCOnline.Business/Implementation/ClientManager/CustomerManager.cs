using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using ADCOnline.Utils;
using ADCOnline.Simple.Json;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class CustomerManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public CustomerManager(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }
        public CustomerItem GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public OrderItem GetOrderItemId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<OrderItem> result = connect.Query<OrderItem>("SELECT * FROM [Order] WHERE IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<OrderDetail> GetListOrderDetailByListOrderIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<OrderDetail> result = connect.Query<OrderDetail>("SELECT * FROM OrderDetail where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,' + Convert(varchar,OrderID) + ',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductItem> GetListProductByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ProductItem> result = connect.Query<ProductItem>("SELECT * FROM Product where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10),ID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public bool CheckExitsEmail(string email)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 AND Email = @email", new { email });
                connect.Close();
                return result.Any();
            }
        }
        public bool CheckExitsPhone(string email)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 AND Mobile = @email", new { email });
                connect.Close();
                return result.Any();
            }
        }
        public CustomerItem GetCustomerItemByEmailOrPhone(string email)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 And IsActivated = 1 and IsLocked = 0 AND (Email = @email Or Mobile = @email)", new { email });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public CustomerItem GetCustomerItemByUsername(string username)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 And IsActivated = 1 and IsLocked = 0 AND UserName = @username", new { username });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public CustomerItem GetCustomerItemByZaloUid(string ZaloUid)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<CustomerItem> result = connect.Query<CustomerItem>("SELECT * FROM Customer WHERE IsDeleted = 0 And IsActivated = 1 and IsLocked = 0 AND ZaloUid = @ZaloUid", new { ZaloUid });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(Customer obj)
        {
            obj.CreatedOnUtc = DateTime.Now;
            return _dapperDa.Insert(obj);
        }
        public List<OrderItem> GetListOrderByCustomerIdPage(SearchModel search, int pageZise = 6)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                search.page = search.page > 1 ? search.page : 1;
                DynamicParameters paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    search.ordercode,
                    search.ispayment,
                    @startorder = !string.IsNullOrEmpty(search.startorder) ? SqlUtility.ConvertDate(search.startorder, false) : null,
                    @toorder = !string.IsNullOrEmpty(search.toorder) ? SqlUtility.ConvertDate(search.toorder, false) : null,
                    search.statusorder,
                    search.customerId,
                    @start = ((search.page - 1) * pageZise),
                    @size = pageZise
                });
                IEnumerable<OrderItem> result = connect.Query<OrderItem>("[dbo].[GetListOrderByCustomerIdPage]", paras, commandType: System.Data.CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
            #region Remove
            //string orderBy = " ORDER BY CreatedDate DESC";
            //StringBuilder sql = new StringBuilder("SELECT COUNT(ID) OVER () AS TotalRecord,* FROM [Order] WHERE IsDeleted = 0 AND CustomerID = {0}", search.customerId);
            //if (!string.IsNullOrEmpty(search.ordercode))
            //{
            //    sql.AppendFormat(" AND OrderCode like '%{0}%'", search.ordercode);
            //}
            //if (!string.IsNullOrEmpty(search.ispayment))
            //{
            //    if (search.ispayment == "0")
            //    {
            //        sql.Append(" AND IsPayment != 1");
            //    }
            //    else
            //    {
            //        sql.Append(" AND IsPayment == 1");
            //    }
            //}
            //if (!string.IsNullOrEmpty(search.startorder))
            //{
            //    sql.AppendFormat(" AND CreatedDate >= {0}", SqlUtility.ConvertDate(search.startorder, false));
            //}
            //if (!string.IsNullOrEmpty(search.toorder))
            //{
            //    sql.AppendFormat(" AND CreatedDate <= {0}", SqlUtility.ConvertDate(search.toorder, false, true));
            //}
            //if (!string.IsNullOrEmpty(search.statusorder))
            //{
            //    sql.AppendFormat(" and Status = {0}", search.statusorder);
            //}
            //sql.Append(orderBy);
            //return _dapperDa.SelectPage<OrderItem>($"{sql}", search.page, pageZise).ToList();
            #endregion
        }
    }
}
