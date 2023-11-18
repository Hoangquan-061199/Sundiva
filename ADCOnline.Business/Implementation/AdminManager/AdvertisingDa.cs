using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class AdvertisingDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public AdvertisingDa(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }

        public List<AdvertisingAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
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
                    @positionId = search.position,
                    search.lang,
                    start,
                    @size = rowPage
                });
                var result = connect.Query<AdvertisingAdmin>("dbo.AdminAdvertisingListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }

        public List<Advertising> ListAll()
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Advertising>("SELECT * FROM Advertising WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }            
        }
        public List<AdvertisingAdmin> ListAdminAll()
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AdvertisingAdmin>("SELECT * FROM Advertising WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            } 
        }
        public List<AdvertisingAdmin> ListAdminAllShow(bool isShow = true)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AdvertisingAdmin>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND IsShow=@isShow", new {isShow});
                connect.Close();
                return result.ToList();
            }
        }
        public List<AdvertisingAdmin> ListAdvertisingEmpty(bool isShow = true)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AdvertisingAdmin>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND IsShow = @isShow and PositionCode is null", new {isShow});
                connect.Close();
                return result.ToList();
            }
        }
        public List<Advertising> ListAdvertisingByModuleIds(string id)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Advertising>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND IsShow= 1 and ',' + ModuleIds + ',' like N'%,' + @id + ',%'", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public List<Advertising> ListAdvertisingByIds(string ids)
        {
            using(SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Advertising>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND IsShow= 1 and ',' + @ids + ',' like N'%,'+Convert(varchar,ID)+',%'", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public Advertising GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Advertising>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<Advertising> GetListPositionCode(string listCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Advertising>("SELECT * FROM Advertising WHERE IsDeleted = 0 AND IsShow=1 AND ',' + @listCode + ',' LIKE '%,'+Code+',%'", new { listCode });
                connect.Close();
                return result.ToList();
            }           
        }
        public int Insert(Advertising obj) => _dapperDa.Insert(obj);
        public int Update(Advertising obj) => _dapperDa.Update(obj);
        public void Delete(Advertising obj) => _dapperDa.Delete(obj);
    }
}
