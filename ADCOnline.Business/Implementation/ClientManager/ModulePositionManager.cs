using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Item;
using System.Collections.Generic;
using System.Linq;
using ADCOnline.DA.Dapper.SqlView;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using ADCOnline.Simple.Json;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class ModulePositionManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ModulePositionManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public ModulePositionItem GetByIdAsync(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("SELECT * FROM ModulePosition WHERE ID=@id And IsShow = 1 And IsDeleted = 0", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ModulePositionItem> GetListView(string view)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("SELECT * FROM ModulePosition WHERE ParentId = (SELECT ID FROM ModulePosition WHERE Code=@view) OR Code=@view", new { view });
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModulePositionItem> GetListViewIndex(string view)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("with name_tree as (" +
                    " select *,OrderTree =0 from ModulePosition where Code = @view union all" +
                    " select c.*,OrderTree=OrderTree+1 from ModulePosition c join name_tree p on p.ID = c.ParentId  AND c.IsDeleted=0 And c.IsShow = 1)" +
                    " select * from name_tree ORDER BY OrderTree ASC", new { view });
                connect.Close();
                return result.ToList();
            }
        }
        public ModulePositionItem GetByCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("SELECT * FROM ModulePosition WHERE Code=@code And IsShow = 1 And IsDeleted = 0", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public ModulePositionItem GetPositionViewID(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("SELECT * FROM ModulePosition WHERE ID = @id And IsShow = 1 And IsDeleted = 0", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ModulePositionItem> GetListByCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<ModulePositionItem> result = connect.Query<ModulePositionItem>("SELECT * FROM ModulePosition WHERE Code=@code And TypeView = 'Advertising' And IsShow = 1 And IsDeleted = 0", new { code });
                connect.Close();
                return result.ToList();
            }
        }
        public List<AdvertisingItem> GetListAdvertisingByCode(string code, string Lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AdvertisingItem> result = connect.Query<AdvertisingItem>("SELECT * FROM Advertising WHERE ',' + PositionCode + ',' like N'%,'+ @code + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang", new { code, Lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<AdvertisingItem> GetListAdvertisingInPositionIds(string id, string Lang, int take = 0)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (take == 0)
                {
                    IEnumerable<AdvertisingItem> result = connect.Query<AdvertisingItem>("SELECT ID,Name, Title, UrlPicture, UrlPictureMobile, LinkUrl, Description, Video FROM Advertising WHERE ',' + PositionIds + ',' like N'%,'+ @id + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang Order By OrderDisplay Asc", new { id, Lang });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    IEnumerable<AdvertisingItem> result = connect.Query<AdvertisingItem>("SELECT TOP(@take) ID,Name, Title, UrlPicture, UrlPictureMobile, LinkUrl, Description, Video FROM Advertising WHERE ',' + PositionIds + ',' like N'%,'+ @id + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang Order By OrderDisplay Asc", new { id, Lang, take });
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public async Task<IEnumerable<AdvertisingItem>> GetListAdvertisingByCodeAsync(string code, string Lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<AdvertisingItem> result = await connect.QueryAsync<AdvertisingItem>("SELECT * FROM Advertising WHERE ',' + PositionCode + ',' like N'%,'+ @code + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang", new { code, Lang });
                connect.Close();
                return result;
            }
        }
        public List<WebsiteModulesJson> GetListModuleInPositionCode(string code, string Lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                IEnumerable<WebsiteModulesJson> result = connect.Query<WebsiteModulesJson>("SELECT * FROM WebsiteModule WHERE ',' + PositionCode + ',' like N'%,'+ @code + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang order by OrderDisplay asc", new { code, Lang });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesJson> GetListModuleInPositionIds(string id, string Lang, int take = 0)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (take == 0)
                {
                    IEnumerable<WebsiteModulesJson> result = connect.Query<WebsiteModulesJson>("SELECT ID,Name, Title, ParentID, NameAscii, LinkUrl, UrlPicture, AlbumPictureJson, Description, Content, Video, ModuleTypeCode FROM WebsiteModule WHERE ',' + PositionIds + ',' like N'%,'+ @id + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang Order By OrderDisplay Asc", new { id, Lang });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    IEnumerable<WebsiteModulesJson> result = connect.Query<WebsiteModulesJson>("SELECT Top(@take) ID, Name, ParentID, Title, NameAscii, LinkUrl, UrlPicture, AlbumPictureJson, Description, Content, Video, ModuleTypeCode FROM WebsiteModule WHERE ',' + PositionIds + ',' like N'%,'+ @id + ',%' And IsShow = 1 And IsDeleted = 0 And Lang = @Lang Order By OrderDisplay Asc", new { id, take, Lang });
                    connect.Close();
                    return result.ToList();
                }
            }
        }
    }
}
