using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Utils;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class AttributesDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public AttributesDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<AttributesAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
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
                    search.parentId,
                    search.lang,
                    search.sort,
                    start,
                    @size = rowPage,
                    isExport
                });
                var result = connect.Query<AttributesAdmin>("dbo.AdminAttributeListSearch", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public List<AttributesAdmin> GetAdminAll(bool isShow, string lang)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    if (isShow == true)
                    {
                        var result = connect.Query<AttributesAdmin>("SELECT a.*,(select COUNT(e.ID) from Attributes e where e.IsDeleted = 0 And e.ParentID = a.ID) Number FROM Attributes a WHERE 1=1 And a.IsDeleted = 0 and a.Lang = @lang", new { lang });
                        connect.Close();
                        return result.ToList();
                    }
                    else
                    {
                        var result = connect.Query<AttributesAdmin>("SELECT a.*,(select COUNT(e.ID) from Attributes e where e.IsDeleted = 0 And e.ParentID = a.ID) Number FROM Attributes a WHERE 1=1 And a.IsDeleted = 0 and a.Lang = @lang AND a.IsShow = 1", new { lang });
                        connect.Close();
                        return result.ToList();
                    }
                }
            }
            catch
            {

            }

            return null;
        }
        public List<AttributesAdmin> GetAdminByModuleIds(bool isShow, string moduleIds)
        {
            try
            {
                DataTable ModuleIds = new DataTable();
                ModuleIds.Columns.Add("KeyValue");
                var sql = string.Format("SELECT * FROM Attributes WHERE 1=1 And IsDeleted = 0 AND IsShow =1");
                if (!string.IsNullOrEmpty(moduleIds))
                {
                    List<string> ids = ListHelper.GetValuesArrayTag(moduleIds);
                    ids.ForEach(x =>
                    {
                        ModuleIds.Rows.Add(x);
                    });
                }
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<AttributesAdmin>("SELECT distinct a.* FROM Attributes a inner join @ModuleIds m on ','+a.ListModuleIds+',' LIKE '%,'+ m.KeyValue +',%' inner join WebsiteModule md on md.ID = m.keyValue WHERE 1=1 And a.IsDeleted = 0 AND a.IsShow =1 and md.IsDeleted = 0", new { @ModuleIds = ModuleIds.AsTableValuedParameter("[dbo].[keysSearch]") });
                    connect.Close();
                    return result.ToList();
                }
                #region Remove
                // string sql = string.Format("SELECT * FROM Attributes WHERE 1=1 And IsDeleted = 0");
                // if (isShow == true)
                // {
                //     sql += " AND IsShow =1";
                // }
                // StringBuilder whereAttr = new StringBuilder();
                // if (!string.IsNullOrEmpty(moduleIds))
                // {
                //     List<string> ids = new List<string>();
                //     ids = moduleIds.Trim().ToLower().Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();
                //     int st = 1;
                //     foreach (var item in ids)
                //     {
                //         if (st == 1)
                //         {
                //             whereAttr.Append($" And (','+ListModuleIds+',' LIKE '%,{item},%'");
                //         }
                //         else
                //         {
                //             whereAttr.Append($" Or ','+ListModuleIds+',' LIKE '%,{item},%'");
                //         }
                //         st++;
                //     }
                //     whereAttr.Append(")");
                //     sql += whereAttr;
                // }
                // if (!string.IsNullOrEmpty(sql))
                // {
                //     return _dapperDa.Select<AttributesAdmin>(sql).ToList();
                // }
                #endregion
            }
            catch
            {

            }
            return null;
        }
        public List<AttributeContent> GetListAttributeContent(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AttributeContent>("select aw.AttributeID,aw.ContentID,a.Name,a.ParentID,aw.Price from Attribute_WebsiteContent aw inner join Attributes a on aw.AttributeID = a.ID where ',' + @ids + ',' like N'%,' + CONVERT(varchar, aw.ContentID) + ',%' and a.IsDeleted = 0", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public List<AttributesAdmin> GetListByArrId(string listArray)
        {
            try
            {
                if (!string.IsNullOrEmpty(listArray))
                {
                    return _dapperDa.Select<AttributesAdmin>(string.Format($"SELECT * FROM Attributes where IsDeleted = 0 AND IsShow =1 AND ',{listArray},' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID Asc")).ToList();
                }
            }
            catch
            {

            }
            return new List<AttributesAdmin>();
        }
        public Attributes GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attributes>("SELECT * FROM Attributes where IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Attributes GetByName(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attributes>("SELECT * FROM Attributes where IsDeleted = 0 AND Name=@code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Attributes GetByNameOption(string code, int parentId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attributes>("SELECT * FROM Attributes where IsDeleted = 0 AND Name=@code And ParentID = @parentId", new { code, parentId });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Attributes GetByNameOptionByID(string code, int parentId, int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Attributes>("SELECT * FROM Attributes where IsDeleted = 0 AND Name=@code And ParentID = @parentId And ID != @id", new { code, parentId, id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<AttributesAdmin> GetListAttrByModuleId(string moduleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AttributesAdmin>("SELECT * FROM Attributes where IsDeleted = 0 AND IsShow =1 AND ','+ListModuleIds+',' LIKE '%,' + @moduleId + ',%' Order By ID desc", new { moduleId });
                connect.Close();
                return result.ToList();
            }
        }
        public int Insert(Attributes obj) => _dapperDa.Insert(obj);
        public int Update(Attributes obj) => _dapperDa.Update(obj);
        public int UpdateSql(string sql) => _dapperDa.ExecuteSql(sql);
        public int DeleteAttributeContent(int Id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Execute("delete from dbo.Attribute_WebsiteContent where ContentID = @id", new { Id });
                connect.Close();
                return result;
            }
        }
        //Lấy thông tin từng module 
        public List<AttributesAdmin> GetListByAttrByMoudleId(string ids)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AttributesAdmin>("SELECT * FROM Attributes where IsDeleted = 0 AND ',' + @ids + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%'", new { ids });
                connect.Close();
                return result.ToList();
            }
        }
        public List<AttributesAdmin> GetListByModule()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AttributesAdmin>("SELECT * FROM Attributes where IsDeleted = 0 AND IsShow =1");
                connect.Close();
                return result.ToList();
            }
        }
        public List<AttributesAdmin> GetListByModuleLang(string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<AttributesAdmin>("SELECT * FROM Attributes where IsDeleted = 0 AND IsShow =1 And Lang = @lang", new { lang });
                connect.Close();
                return result.ToList();
            }
        }
        public void UpdateModuleIds(string attrIdNew, string idModule)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    int result = connect.Execute("Update Attributes set ListModuleIds = @attrIdNew WHERE ID = @idModule", new { attrIdNew, idModule });
                    connect.Close();
                }
            }
            catch
            {
            }
        }
    }
}
