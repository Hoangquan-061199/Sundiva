using ADCOnline.DA.Dapper;
using System.Collections.Generic;
using ADCOnline.Simple.Item;
using System.Linq;
using System.Text;
using ADCOnline.Utils;
using System.Threading.Tasks;

namespace ADCOnline.Business.Implementation.ClientManager
{
    public class AgencyManager : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public AgencyManager(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<AgenciesItem> GetListByArrId(string relateIds) => _dapperDa.Select<AgenciesItem>($"select * from Agencies where 1=1 And IsDeleted = 0 And IsShow = 1 AND ID IN({relateIds.Trim(',')})").ToList();
        public AreaAgencyItem GetAreaById(int id) => _dapperDa.Select<AreaAgencyItem>($"SELECT * FROM AreaAgency where IsDeleted = 0 AND Show =1 AND ID={id}").FirstOrDefault();
        public List<AgencyItem> GetListStore(string lang) => _dapperDa.Select<AgencyItem>($"select * from Agencies where ',' + TypeIds +',' not like N'%,7,%' and IsDeleted = 0 and IsShow = 1 and Lang = '{lang}' order by Name asc").ToList();
        public List<AgenciesItem> GetAll() => _dapperDa.Select<AgenciesItem>("select * from Agencies where IsDeleted = 0 And IsShow = 1").ToList();
        public List<AgenciesItem> GetByOption(string areaIds, string typeIds, string idThis)
        {
            string sql = string.Format("select * from Agencies where IsDeleted = 0 And IsShow = 1");
            if (!string.IsNullOrEmpty(areaIds) && areaIds != ",")
            {
                sql += $" and ',{areaIds},' like '%,'+AreaIds+',%'";
            }
            if (!string.IsNullOrEmpty(typeIds))
            {
                StringBuilder whereAttr = new StringBuilder();
                List<string> ids = new List<string>();
                ids = typeIds.Trim().ToLower().Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();
                int st = 1;
                foreach (var item in ids)
                {
                    if (st == 1)
                    {
                        whereAttr.Append($" And (','+TypeIds+',' LIKE '%,{item},%'");
                    }
                    else
                    {
                        whereAttr.Append($" Or ','+TypeIds+',' LIKE '%,{item},%'");
                    }
                    st++;
                }
                if (!string.IsNullOrEmpty(idThis))
                {
                    sql += $"and ID={idThis}";
                }
                whereAttr.Append(")");
                sql += whereAttr;
            }
            return _dapperDa.Select<AgenciesItem>(sql).ToList();
        }
        public AgenciesItem GetByNameAscii(string NameAscii) => _dapperDa.Select<AgenciesItem>($"select * from Agencies where IsDeleted = 0 and IsShow = 1 and NameAscii = N'{NameAscii}' AND CreatedDate <= GETDATE() Order By ID Desc").FirstOrDefault();
        public AgenciesItem GetByID(string id) => _dapperDa.Select<AgenciesItem>($"select * from Agencies where IsDeleted = 0 and IsShow = 1 and ID = {id}  Order By ID Desc").FirstOrDefault();
        public List<AgenciesItem> GetListById(string contentIds) => !string.IsNullOrEmpty(contentIds) ? _dapperDa.Select<AgenciesItem>($"select * from Agencies where IsDeleted = 0 and IsShow = 1 and ID in({contentIds.Trim(',')}) AND CreatedDate <= GETDATE() Order By ID Desc").ToList() : new List<AgenciesItem>();
        public List<AgenciesItem> GetListByAreaIds(string areaIds, string typeIds, string ts)
        {
            string sql = string.Format("select * from Agencies where IsDeleted = 0 and IsShow = 1");
            if (!string.IsNullOrEmpty(areaIds))
            {
                sql += $" and ',{areaIds},' like '%,'+AreaIds+',%'";
            }
            if (!string.IsNullOrEmpty(ts) && (ts == "doji" || ts == "tgkc"))
            {
                sql += $" and Type = '{ts}'";
            }
            if (!string.IsNullOrEmpty(typeIds))
            {
                StringBuilder whereAttr = new StringBuilder();
                List<string> ids = new List<string>();
                ids = typeIds.Trim().ToLower().Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();
                int st = 1;
                foreach (var item in ids)
                {
                    if (st == 1)
                    {
                        whereAttr.Append($" And (','+TypeIds+',' LIKE '%,{item},%'");
                    }
                    else
                    {
                        whereAttr.Append($" Or ','+TypeIds+',' LIKE '%,{item},%'");
                    }
                    st++;
                }
                whereAttr.Append(")");
                sql += whereAttr;
            }
            sql += " Order By Name";
            return _dapperDa.Select<AgenciesItem>(sql).ToList();
        }
        public List<AgenciesItem> GetListByAreaIdsOnlyStore(string areaIds)
        {
            string sql = string.Format("select * from Agencies where IsDeleted = 0 and IsShow = 1");
            if (!string.IsNullOrEmpty(areaIds))
            {
                sql += $" and ',{areaIds},' like '%,'+AreaIds+',%'";
            }
            sql += " Order By Name";
            return _dapperDa.Select<AgenciesItem>(sql).ToList();
        }
        public List<AreaAgencyItem> GetByParentId(int paId) => _dapperDa.Select<AreaAgencyItem>($"SELECT * FROM AreaAgency where IsDeleted = 0 AND Show =1 AND ParentID='{paId}'").ToList();
        public List<AreaAgencyItem> GetAllArea() => _dapperDa.Select<AreaAgencyItem>("SELECT * FROM AreaAgency where IsDeleted = 0 AND Show =1").ToList();
        public async Task<IEnumerable<AreaAgencyItem>> GetByParentArrId(string paId) => await _dapperDa.SelectAsync<AreaAgencyItem>(string.Format("SELECT * FROM AreaAgency where IsDeleted = 0 AND Show =1 AND ',{0},' LIKE '%,'+CONVERT(varchar(10), ParentID)+',%' Order By ID desc", paId));
        public List<AgencyTypeItem> GetAllType() => _dapperDa.Select<AgencyTypeItem>("select * from AgencyType where IsDeleted = 0 And IsShow = 1").ToList();
        public List<AgenciesItem> GetListAgencyPage(SearchModel search, int pageZise = 6)
        {
            string order = " ORDER BY OrderDisplay ASC";
            if (search.sort == 1)
            {
                order = " ORDER BY OrderDisplay Asc";
            }
            var sql = string.Format("SELECT * FROM Agencies WHERE IsDeleted = 0 AND IsShow = 1");
            if (!string.IsNullOrEmpty(search.nottypeId))
            {
                sql += $" AND ','+TypeIds+',' not like ',%{search.nottypeId}%,'";
            }
            if (!string.IsNullOrEmpty(search.typeId))
            {
                sql += $" AND ','+TypeIds+',' like ',%{search.typeId}%,'";
            }           
            if (!string.IsNullOrEmpty(search.areaIds) && search.areaIds != ",")
            {
                sql += $" and ',{search.areaIds},' like '%,'+AreaIds+',%'";
            }
            if (!string.IsNullOrEmpty(search.code))
            {
                sql += $" and Type = '{search.code}'";
            }
            if (!string.IsNullOrEmpty(search.store) && search.store != ",")
            {
                sql += $" and ID={search.store}";
            }
            sql += order;
            return _dapperDa.SelectPage<AgenciesItem>(sql, search.page, pageZise).ToList();
        }
    }
}
