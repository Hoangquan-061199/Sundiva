using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class SubItemDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public SubItemDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<SubItemAdmin> ListSearch(SearchModel search, int productId, int contentId, int moduleId, int page, int rowPage, bool isExport)
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
                    search.type,
                    productId,
                    moduleId,
                    contentId,
                    start,
                    @size = rowPage,
                    isExport
                });
                var result = connect.Query<SubItemAdmin>("dbo.AdminSubItemListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<SubItem> ListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SubItem>("SELECT * FROM SubItem WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public List<SubItemAdmin> GetAdminAll(string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SubItemAdmin>("SELECT * FROM SubItem WHERE IsDeleted = 0 and Lang = @lang", new { lang });
                connect.Close();
                return result.ToList();
            }
        }
        public SubItem GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<SubItem>("SELECT * FROM SubItem WHERE IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public int Insert(SubItem obj) => _dapperDa.Insert(obj);
        public int Update(SubItem obj) => _dapperDa.Update(obj);
        public void Delete(SubItem obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
