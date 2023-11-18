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

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class CustomerDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public CustomerDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public Customer GetByCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Customer>("Select * from Customer where Code = @code And IsActivated = 0 And IsDeleted = 0 And IsLocked = 0", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Customer GetByEmail(string email)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Customer>("Select * from Customer where Email = @email And IsActivated = 1 and IsLocked = 0 And IsDeleted = 0 And IsLocked = 0", new { email });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public CustomerAdmin GetByPhoneExisted(string code, int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CustomerAdmin>("select * from Customer where IsDeleted = 0 and Mobile = @code And ID != @id", new { code, id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public CustomerAdmin CheckByNewEmail(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CustomerAdmin>("select * from Customer where IsDeleted = 0 and Email = @code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public CustomerAdmin CheckByEmailExisted(string code, int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<CustomerAdmin>("select * from Customer where IsDeleted = 0 and Email = @code And ID != @id", new { code, id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<CustomerAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                page = page > 1 ? page : 1;
                rowPage = rowPage > 1 ? rowPage : 10;
                int start = ((page - 1) * rowPage);
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    search.dateb,
                    search.monthb,
                    search.yearb,
                    @from = !string.IsNullOrEmpty(search.from) ? SqlUtility.ConvertDate(search.from, false) : null,
                    @to = !string.IsNullOrEmpty(search.to) ? SqlUtility.ConvertDate(search.to, false) : null,
                    @start = start,
                    @status = ConvertUtil.ToInt32(search.Status),
                    @size = rowPage,
                    @isExport = isExport
                });
                var result = connect.Query<CustomerAdmin>("dbo.AdminCustomerListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<CustomerAdmin> ListExport(SearchModel search)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    @keyword = SqlUtility.CharacterSpecail(search.keyword),
                    @dateb = search.dateb,
                    @monthb = search.monthb,
                    @yearb = search.yearb,
                    @from = !string.IsNullOrEmpty(search.from) ? SqlUtility.ConvertDate(search.from, false) : null,
                    @to = !string.IsNullOrEmpty(search.to) ? SqlUtility.ConvertDate(search.to, false) : null,
                    @status = search.Status,
                    @start = 0,
                    @size = 10,
                    @isExport = 0
                });
                var result = connect.Query<CustomerAdmin>("dbo.AdminCustomerListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<OrderAdmin> GetListOrderByCustomerId(int idCustomer)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OrderAdmin>("SELECT * FROM [Order] WHERE IsDeleted = 0 AND CustomerID = @idCustomer", new { idCustomer });
                connect.Close();
                return result.ToList();
            }
        }
        public List<OrderDetail> GetListOrderDetailByListOrderIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<OrderDetail>("SELECT * FROM OrderDetail where IsDeleted = 0 AND ',' + @listIds+ ',' like N'%,' + Convert(varchar,OrderID) + ',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ProductAdmin> GetListProductByListIds(string listIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ProductAdmin>("SELECT * FROM Product where IsDeleted = 0 AND ',' + @listIds + ',' LIKE '%,'+CONVERT(varchar(10),ID)+',%' Order By ID desc", new { listIds });
                connect.Close();
                return result.ToList();
            }
        }
        public Customer GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Customer>("select * from Customer where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<Product> GetListProductByArrId(string relateIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Product>("select * from Product where IsDeleted = 0 And IsShow = 1 AND ','+@relateIds+',' like N'%,'+CONVERT(varchar,ID)+',%'", new { relateIds });
                connect.Close();
                return result.ToList();
            }
        }
        public Customer GetUserName(string userName)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Customer>("select * from Customer where UserName = @userName", new { userName });
                connect.Close();
                return result.FirstOrDefault();
            }
        }        
        public int Insert(Customer obj) => _dapperDa.Insert(obj);
        public int Update(Customer obj) => _dapperDa.Update(obj);
        public void Delete(Customer obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
