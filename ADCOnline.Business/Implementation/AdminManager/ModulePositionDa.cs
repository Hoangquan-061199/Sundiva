using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADCOnline.Simple.Item;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class ModulePositionDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ModulePositionDa(string connectionSql)
        {
            _dapperDa = new DapperDA(connectionSql);
        }

        public List<ModulePositionAdmin> ListSearch(Simple.Admin.SearchModel search, int page, int rowPage, bool isExport)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (search != null && !string.IsNullOrEmpty(search.keyword))
                {
                    var result = connect.Query<ModulePositionAdmin>("SELECT * FROM ModulePosition WHERE 1=1 AND IsDeleted = 0 AND Name LIKE N'%' + @key + '%' ESCAPE N'~' ORDER BY ID DESC", new { @key = SqlUtility.CharacterSpecail(search.keyword) });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<ModulePositionAdmin>("SELECT * FROM ModulePosition WHERE 1=1 AND IsDeleted = 0 ORDER BY ID DESC");
                    connect.Close();
                    return result.ToList();
                }
            }
        }

        public List<ModulePosition> ListAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModulePositionAdmin> ListAdminAll()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePositionAdmin>("SELECT * FROM ModulePosition WHERE IsDeleted = 0");
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModulePositionAdmin> ListAdminIsShow()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePositionAdmin>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND IsShow=1");
                connect.Close();
                return result.ToList();
            }
        }
        public ModulePosition GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND ID=@id", new {id});
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public ModulePosition GetCode(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND Code=@code", new { code });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ModulePosition> GetListPositionCode(string listCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(listCode))
                {
                    var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND IsShow=1 AND ',' + @listCode + ','  LIKE '%,'+Code+',%'", new { listCode });
                    connect.Close();
                    return result.ToList();
                }
                return new List<ModulePosition>();
            }
        }
        public List<ModulePosition> GetListPositionIds(string listCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(listCode))
                {
                    var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND IsShow=1 AND ',' + @listCode + ','  LIKE '%,'+ Convert(varchar,ID) +',%'", new { listCode });
                    connect.Close();
                    return result.ToList();
                }
                return new List<ModulePosition>();
            }
        }
        public List<ModulePosition> GetPositonTypeProduct()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModulePosition>("SELECT * FROM ModulePosition WHERE IsDeleted = 0 AND IsShow=1 And TypeView = 'Product'");
                connect.Close();
                return result.ToList();
            }
        }
        public List<ModulePosition> GetListPositionItemIds(string listCode)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(listCode))
                {
                    var result = connect.Query<ModulePosition>("SELECT ID,Name,Code,IsDeleted,IsShow,LinkBanner,ModuleIds,ModuleTypeCode,NumberContent,NumberCount,OrderDisplay,ParentId,SqlContent,SqlContentOrderBy,SqlModule,TypeView,UrlPicture,UrlPictureMobile,Video FROM ModulePosition WHERE IsDeleted = 0 AND IsShow=1 AND ',' + @listCode + ','  LIKE '%,'+Convert(varchar,ID)+',%'", new { listCode });
                    connect.Close();
                    return result.ToList();
                }
                return new List<ModulePosition>();
            }            
        }
        public int Insert(ModulePosition obj) => _dapperDa.Insert(obj);
        public int Update(ModulePosition obj) => _dapperDa.Update(obj);
        public void Delete(ModulePosition obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
        public void BuildTreeViewCheckBox(List<ModulePositionAdmin> ltsSource, int? moduleID, bool checkShow, List<string> ltsValues, ref StringBuilder treeViewHtml)
        {
            var tempmodule = ltsSource.OrderBy(o => o.OrderDisplay).Where(m => m.ParentId == (moduleID.HasValue ? moduleID : null));
            if (checkShow)
                tempmodule = tempmodule.Where(m => m.IsShow == checkShow);

            foreach (var module in tempmodule)
            {
                var countQuery = ltsSource.Where(m => m.ParentId == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == checkShow);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.Code + "\"><span class=\"folder\"> <input id=\"module_" + module.Code + "\" name=\"module_" + module.Code + "\" value=\"" + module.ID + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID.ToString()) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + "</strike>");
                    else
                        treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeViewCheckBox(ltsSource, module.ID, checkShow, ltsValues, ref treeViewHtml);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.Code + "\"><span class=\"file\"> <input id=\"module_" + module.Code + "\" name=\"module_" + module.Code + "\" value=\"" + module.ID + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID.ToString()) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + "</strike>");
                    else
                        treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</span></li>\r\n");
                }
            }
        }
        public void UpdateWebsiteModule(string code, string moduleIds)
        {
            string sql = string.Format("Update WebsiteModule SET PositionCode = REPLACE(','+PositionCode+',',',{0},',',') WHERE ','+PositionCode+',' LIKE '%,{0},%';", code);
            sql += string.Format("Update WebsiteModule SET PositionCode = REPLACE(','+PositionCode,',,','') WHERE ','+PositionCode LIKE '%,,%';");
            sql += string.Format("Update WebsiteModule SET PositionCode = REPLACE(PositionCode+',',',,','') WHERE PositionCode+',' LIKE '%,,%';");
            if (!string.IsNullOrEmpty(moduleIds))
            {
                sql += string.Format("Update WebsiteModule SET PositionCode = ISNULL(PositionCode,'') +',{0}' WHERE ID in({1});", code, moduleIds);
            }
            _dapperDa.ExecuteSql(sql);
        }
        public void UpdateWebsiteModuleIds(string id, string moduleIds)
        {
            string sql = string.Format("Update WebsiteModule SET PositionIds = REPLACE(','+PositionIds+',',',{0},',',') WHERE ','+PositionIds+',' LIKE '%,{0},%';", id);
            sql += string.Format("Update WebsiteModule SET PositionIds = REPLACE(','+PositionIds,',,','') WHERE ','+PositionIds LIKE '%,,%';");
            sql += string.Format("Update WebsiteModule SET PositionIds = REPLACE(PositionIds+',',',,','') WHERE PositionIds+',' LIKE '%,,%';");
            if (!string.IsNullOrEmpty(moduleIds))
            {
                sql += string.Format("Update WebsiteModule SET PositionIds = ISNULL(PositionIds,'') +',{0}' WHERE ID in({1});", id, moduleIds);
            }
            _dapperDa.ExecuteSql(sql);
        }
    }
}
