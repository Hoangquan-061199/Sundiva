using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class WebsiteModuleDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public WebsiteModuleDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);

        #region sitemap
        public List<WebsiteModulesJson> GetCategory()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModulesJson>("select Id,Name,NameAscii,ModifiedDate from WebsiteModule where IsDeleted = 0 and IsShow = 1 and IsSitemap = 1 and NameAscii is not null and LinkUrl is null and ',' + @str + ',' like N'%,' + ModuleTypeCode + ',%'", new { @str = string.Join(",", StaticEnum.ModuleSitemap) });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesJson> GetCategoryChildSitemap(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModulesJson>("select Id,Name,NameAscii from WebsiteModule where IsDeleted =  0 and IsShow = 1 and IsSitemap = 1 and NameAscii is not null and LinkUrl is null and ParentID = @id", new { id });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesJson> GetTradeMarkSitemap()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModulesJson>("select * from WebsiteModule where ParentID in (select ID from WebsiteModule where ModuleTypeCode = 'TradeMark' and IsDeleted = 0 and IsShow = 1 and IsSitemap = 1) and IsDeleted = 0 and IsShow = 1 and IsSitemap = 1 and NameAscii is not null and LinkUrl is null");
                connect.Close();
                return result.ToList();
            }
        }
        #endregion                
        public List<WebsiteModuleAdmin> GetAdminAll(bool isShow, string lang, string nottype, string intype, string ModuleIds)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var paras = new DynamicParameters();
                    paras.AddDynamicParams(new
                    {
                        isShow,
                        lang,
                        nottype,
                        intype,
                        ModuleIds
                    });
                    var result = connect.Query<WebsiteModuleAdmin>("dbo.AdminModuleAdminAll", paras, commandType: CommandType.StoredProcedure);
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {

            }
            return null;
        }
        public List<WebsiteModuleAdmin> GetAdminAllFilter(Simple.Admin.SearchModel search, string lang, string nottype, string intype)
        {
            try
            {

                if (!string.IsNullOrEmpty(search.keyword) || (!string.IsNullOrEmpty(search.Show) && search.Show.Equals("0")))
                {
                    using (SqlConnection connect = _dapperDa.GetOpenConnection())
                    {
                        var paras = new DynamicParameters();
                        paras.AddDynamicParams(new
                        {
                            @keyword = SqlUtility.CharacterSpecail(search.keyword),
                            @keyword2 = SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)),
                            @id = ConvertUtil.ToInt32(search.keyword),
                            lang,
                            nottype,
                            intype,
                            @show = search.Show,
                            search.ModuleIds
                        });
                        var result = connect.Query<WebsiteModuleAdmin>("dbo.AdminModuleAllWithFilter", paras, commandType: CommandType.StoredProcedure);
                        connect.Close();
                        return result.ToList();
                    }
                    #region Remove
                    //StringBuilder sqlStr = new StringBuilder();
                    //sqlStr.Append("WITH cte(ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon) AS");
                    //sqlStr.Append(" (SELECT ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl, IsDeleted,Icon");
                    //sqlStr.Append(" FROM WebsiteModule WHERE 1=1 and IsDeleted = 0");
                    //if (!string.IsNullOrEmpty(lang))
                    //{
                    //    sqlStr.AppendFormat(" And Lang = '{0}'", lang);
                    //}
                    //else
                    //{
                    //    sqlStr.Append(" And Lang = 'vi'");
                    //}
                    //if (!string.IsNullOrEmpty(search.keyword))
                    //{
                    //    sqlStr.AppendFormat(" And (Name like N'%{0}%' ESCAPE N'~' Or NameAscii like N'%{1}%' ESCAPE N'~' OR ID ={2})", SqlUtility.CharacterSpecail(search.keyword), SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)), ConvertUtil.ToInt32(search.keyword));
                    //}
                    //if (!string.IsNullOrEmpty(nottype))
                    //{
                    //    sqlStr.AppendFormat(" and (ModuleTypeCode not in({0}) Or ModuleTypeCode is null)", nottype);
                    //}
                    //if (!string.IsNullOrEmpty(intype))
                    //{
                    //    sqlStr.AppendFormat(" and (ModuleTypeCode in ({0}) Or ModuleTypeCode is null)", intype);
                    //}
                    //if (!string.IsNullOrEmpty(search.Show))
                    //{
                    //    sqlStr.AppendFormat(" AND IsShow = {0}", search.Show);
                    //}
                    //if (!string.IsNullOrEmpty(search.ModuleIds))
                    //{
                    //    sqlStr.AppendFormat(" And ',{0},' like N'%,'+ Convert(varchar,ID) +',%'", search.ModuleIds);
                    //}
                    //sqlStr.Append(" UNION ALL SELECT c.ID, c.ParentID, c.Name,c.OrderDisplay,c.ModuleTypeCode,c.NameAscii,c.IsShow,c.PositionCode,c.TotalViews,c.LinkUrl,c.IsDeleted,c.Icon");
                    //sqlStr.Append(" FROM WebsiteModule c INNER JOIN cte ON cte.ParentID = c.ID)");
                    //sqlStr.Append(" SELECT distinct ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl,IsDeleted,Icon,");
                    //sqlStr.Append("(select t.Name from ModuleType t where t.Code = cte.ModuleTypeCode) ModuleType");
                    //if (intype == StaticEnum.Trademark)
                    //{
                    //    sqlStr.Append(" FROM cte ORDER BY OrderDisplay ASC, Name asc");
                    //}
                    //else
                    //{
                    //    sqlStr.Append(" FROM cte ORDER BY ParentID ASC, OrderDisplay asc");
                    //}
                    //return _dapperDa.Select<WebsiteModuleAdmin>($"{sqlStr}").ToList();
                    #endregion
                }
                else
                {
                    using (SqlConnection connect = _dapperDa.GetOpenConnection())
                    {
                        var paras = new DynamicParameters();
                        paras.AddDynamicParams(new
                        {
                            lang,
                            nottype,
                            intype,
                            search.ModuleIds
                        });
                        var result = connect.Query<WebsiteModuleAdmin>("dbo.AdminModuleAllWithNotFilter", paras, commandType: CommandType.StoredProcedure);
                        connect.Close();
                        return result.ToList();
                    }
                    #region Remove
                    //StringBuilder sql = new StringBuilder("SELECT *, (select t.Name from ModuleType t where t.Code = m.ModuleTypeCode) ModuleType from WebsiteModule m WHERE 1=1 And IsDeleted = 0");
                    //if (!string.IsNullOrEmpty(lang))
                    //{
                    //    sql.AppendFormat(" And Lang = '{0}'", lang);
                    //}
                    //else
                    //{
                    //    sql.Append(" And Lang = 'vi'");
                    //}
                    //if (!string.IsNullOrEmpty(nottype))
                    //{
                    //    sql.AppendFormat("and (ModuleTypeCode not in({0}) Or ModuleTypeCode is null)", nottype);
                    //}
                    //if (!string.IsNullOrEmpty(intype))
                    //{
                    //    sql.AppendFormat("and (ModuleTypeCode in ({0}) Or ModuleTypeCode is null)", intype);
                    //}
                    //if (!string.IsNullOrEmpty(search.Show))
                    //{
                    //    sql.AppendFormat(" AND IsShow = {0}", search.Show);
                    //}
                    //if (search.parentId.HasValue && search.parentId > 0)
                    //{
                    //    sql.AppendFormat(" AND ParentID = {0}", search.parentId);
                    //}
                    //if (!string.IsNullOrEmpty(search.ModuleIds))
                    //{
                    //    sql.AppendFormat(" And ',{0},' like N'%,'+ Convert(varchar,ID) +',%'", search.ModuleIds);
                    //}
                    //if (intype == StaticEnum.Trademark)
                    //{
                    //    sql.Append(" order by OrderDisplay asc, Name asc");
                    //}
                    //else
                    //{
                    //    sql.Append(" order by ParentID asc, OrderDisplay asc");
                    //}
                    //return _dapperDa.Select<WebsiteModuleAdmin>($"{sql}").ToList();
                    #endregion
                }
            }
            catch
            {

            }
            return null;
        }
        public List<WebsiteModuleAdmin> GetListByArrId(string listArray)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID Asc", new { listArray });
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {

            }

            return new List<WebsiteModuleAdmin>();
        }
        public List<WebsiteModuleAdmin> GetListByArrIdNotShow(string listArray)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID Asc", new { listArray });
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {

            }

            return new List<WebsiteModuleAdmin>();
        }
        public List<WebsiteModule> GetListAdminByArrIdNotShow(string listArray)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND ',' + @listArray + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID Asc", new { listArray });
                    connect.Close();
                    return result.ToList();
                }
            }
            catch
            {

            }

            return new List<WebsiteModule>();
        }
        public List<WebsiteModuleAdmin> GetListModuleByPosition(ModulePosition item)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT DISTINCT * FROM(SELECT * FROM (SELECT top @NumberCount ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,','+PositionCode+',' PositionCode,ParentID,Description,@TypeView TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode,Content FROM WebsiteModule " +
             "WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionCode+',' like '%,' + @code + ',%' @sqlM) c) d", new { @NumberCount = item.NumberCount ?? 3000, @TypeView = item.TypeView, @code = item.Code, @sqlM = item.SqlModule });
                connect.Close();
                return result.ToList();
            }
            //var sqlModule = new StringBuilder();
            //sqlModule.Append(string.Format("SELECT * FROM (SELECT top {0} ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,','+PositionCode+',' PositionCode,ParentID,Description,'{1}' TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode,Content FROM WebsiteModule " +
            // "WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionCode+',' like '%,{2},%' {3}) c", item.NumberCount ?? 3000, item.TypeView, item.Code, item.SqlModule));
            //return _dapperDa.Select<WebsiteModuleAdmin>(string.Format("SELECT DISTINCT * FROM({0}) c ", sqlModule)).ToList();
        }
        public List<WebsiteModuleAdmin> GetListModuleByPositionIds(ModulePosition item)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT DISTINCT * FROM(SELECT * FROM (SELECT top @NumberCount ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,','+PositionCode+',' PositionCode,ParentID,Description,@TypeView TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode,Content FROM WebsiteModule " +
             "WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionIds+',' like '%,' + @id + ',%' @sqlM) c) d", new { @NumberCount = item.NumberCount ?? 3000, @TypeView = item.TypeView, @id = item.ID, @sqlM = item.SqlModule });
                connect.Close();
                return result.ToList();
            }
            //var sqlModule = new StringBuilder();
            //sqlModule.Append(string.Format("SELECT * FROM(SELECT top {0} ID,Name,Title,NameAscii,LinkUrl,UrlPicture,Video,','+PositionIds+',' PositionIds,ParentID,Description,'{1}' TypeView,Code Code,AlbumPictureJson,SeoDescription,SeoKeyword,SEOTitle,OrderDisplay,Rel, ModuleTypeCode,Content FROM WebsiteModule " +
            // "WHERE IsDeleted =0 AND IsShow =1 AND ','+PositionIds+',' like N'%,{2},%' {3}) c", item.NumberCount ?? 3000, item.TypeView, item.ID, item.SqlModule));
            //return _dapperDa.Select<WebsiteModuleAdmin>(string.Format("SELECT DISTINCT * FROM({0}) c ", sqlModule)).ToList();
        }
        public WebsiteModule GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteModuleAdmin GetModuleId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteModuleAdmin GetModuleAdminId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND ID=@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteModule GetByNameAscii(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND NameAscii=@nameAscii", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public WebsiteModule GetByNameAsciiMain(string nameAscii)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND NameAscii=@nameAscii And LinkUrl is null", new { nameAscii });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<WebsiteModule> GetPositionIds(string code)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ','+PositionIds+',' like '%,' + @code + ',%' Order By OrderDisplay Asc", new { code });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModule> GetByAttr(string listModule)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModule>("SELECT * FROM WebsiteModule where 1=1 And IsDeleted = 0 AND IsShow =1 AND ',' + @listModule + ',' LIKE '%,'+CONVERT(varchar(10), ID)+',%' Order By ID desc", new { listModule });
                connect.Close();
                return result.ToList();
            }
        }
        public void UpdateAttrModule(string attrIdNew, string idModule)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var result = connect.Execute("Update WebsiteModule set AttributeModuleIds =@attrIdNew  WHERE  ID = @idModule", new { attrIdNew, idModule });
                    connect.Close();
                }
            }
            catch
            {
            }
        }
        public List<WebsiteModuleAdmin> GetListByModuleProduct()
        {
            List<string> Codes = new List<string> { StaticEnum.Product, StaticEnum.Sale };
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 AND ',' + @codes + ',' Like N'%,' + ModuleTypeCode + ',%'", new { @codes = string.Join(",", Codes) });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModulesJson> GetListByExcuteSql(string sql)
        {
            var result = _dapperDa.Select<WebsiteModulesJson>(sql);
            return result.ToList();
        }
        public int Insert(WebsiteModule obj) => _dapperDa.Insert(obj);
        public int UpdatePosition(ModulePosition obj) => _dapperDa.Update(obj);
        public int Update(WebsiteModule obj) => _dapperDa.Update(obj);
        public async Task<int> UpdateAsync(WebsiteModule obj) => await _dapperDa.UpdateAsync(obj);
        public int UpdateSql(string sql) => _dapperDa.ExecuteSql(sql);
        public void Delete(WebsiteModule obj, string sql) => _dapperDa.DeleteIsTrue(obj, sql);

        public void UpdatePositionCode(string listModuleId, string positionCodeNew, string positionCodeOld)
        {
            try
            {
                string sql = "Update WebsiteModule set PositionCode = REPLACE(','+PositionCode+',', '," + positionCodeOld + ",','')  WHERE ',' + PositionCode +',' LIKE '," + positionCodeOld + ",';" +
               " Update WebsiteModule set PositionCode = CONCAT(PositionCode ,'," + positionCodeNew + "')  WHERE ID in (" + listModuleId + ");";
                _dapperDa.ExecuteSql(sql);
            }
            catch
            {
            }
        }
        public async Task UpdateModuleIdsPositoin(string id, string moduleIds)
        {
            try
            {
                await _dapperDa.ExecuteSqlAsync(string.Format("update ModulePosition set ModuleIds = '{0}' where Id = {1}", moduleIds, id));
            }
            catch
            {
            }
        }
        #region lấy tree grid
        public void BuildTreeView(List<WebsiteModuleAdmin> ltsSource, int? moduleID, bool checkShow, ref StringBuilder treeViewHtml, bool add, bool delete, bool edit, bool view, bool show, bool hide, bool order)
        {
            var tempmodule = ltsSource.OrderBy(o => o.OrderDisplay).Where(m => m.ParentID == ConvertUtil.ToInt32(moduleID));
            if (checkShow)
                tempmodule = tempmodule.Where(m => m.IsShow == checkShow);

            foreach (var module in tempmodule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == checkShow);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID + "\"><span class=\"folder\"><a class=\"tool\" href=\"javascript:;\">");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.OrderDisplay + ". " + module.Name + "</strike>");
                    else
                        treeViewHtml.Append(module.OrderDisplay + ". " + module.Name);
                    treeViewHtml.Append("</a>\r\n");
                    treeViewHtml.AppendFormat(" <div class=\"badge badge-danger\">{0}</div>\r\n", totalChild);
                    treeViewHtml.Append(buildEditToolByID(module, add, delete, edit, show, order) + "\r\n");
                    treeViewHtml.Append("</span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeView(ltsSource, module.ID, checkShow, ref treeViewHtml, add, delete, edit, view, show, hide, order);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"><a class=\"tool\" href=\"javascript:;\">");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.OrderDisplay + ". " + module.Name + " </strike>");
                    else
                        treeViewHtml.Append(module.OrderDisplay + ". " + module.Name);
                    treeViewHtml.Append("</a>" + buildEditToolByID(module, add, delete, edit, show, order) + "</span></li>\r\n");
                }
            }
        }
        private string buildEditToolByID(WebsiteModuleAdmin websiteModulesItem, bool add, bool delete, bool edit, bool show, bool order)
        {
            var strTool = new StringBuilder();
            strTool.Append("<div class=\"quickTool\">\r\n");
            if (add)
            {
                strTool.AppendFormat("<a title=\"Thêm mới module: {1}\" class=\"add\" href=\"#{0}\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                strTool.Append("        <span class=\"lnr lnr-plus-circle\"></span>\r\n");
                strTool.Append("    </a>");
                strTool.AppendFormat("<a title=\"Nội dung thêm: {1}\" class=\"sub\" href=\"/Adminadc/SubItem/Index?moduleId={0}&type=Module\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                strTool.Append("        <span class=\"lnr lnr-database\"></span>\r\n");
                strTool.Append("    </a>");
            }
            if (edit)
            {
                strTool.AppendFormat("    <a title=\"Chỉnh sửa: {1}\" class=\"edit\" href=\"#{0}\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                strTool.Append("        <span class=\"lnr lnr-pencil\"></span>\r\n");
                strTool.Append("    </a>");
            }
            if (show)
            {
                if (websiteModulesItem.IsShow == true)
                {
                    strTool.AppendFormat("    <a title=\"Ẩn: {1}\" href=\"#{0}\" class=\"hiddens\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                    strTool.Append("        <span class=\"lnr lnr-warning\"></span>\r\n");
                    strTool.Append("    </a>\r\n");
                }
                else
                {
                    strTool.AppendFormat("    <a title=\"Hiển thị: {1}\" href=\"#{0}\" class=\"show\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                    strTool.Append("       <span class=\"lnr lnr-checkmark-circle\"></span>\r\n");
                    strTool.Append("    </a>\r\n");
                }
            }
            if (delete)
            {
                strTool.AppendFormat("    <a title=\"Xóa: {1}\" href=\"#{0}\" class=\"delete\">\r\n", websiteModulesItem.ID, websiteModulesItem.Name);
                strTool.Append("         <span class=\"lnr lnr-trash\"></span>\r\n");
                strTool.Append("    </a>\r\n");
            }

            if (order)
            {
                strTool.AppendFormat("    <a title=\"Sắp xếp các module con: {1}\" href=\"#{0}\" class=\"sort\">\r\n", websiteModulesItem.ParentID, websiteModulesItem.Name);
                strTool.Append("        <i class=\"fa fa-sort\"></i>\r\n");
                strTool.Append("    </a>\r\n");
            }
            strTool.Append("</div>\r\n");
            return strTool.ToString();
        }
        #endregion
        #region lấy tree selectbox
        public void BuildTreeViewCheckBox(List<WebsiteModuleAdmin> ltsSource, int? moduleID, bool checkShow, List<int> ltsValues, ref StringBuilder treeViewHtml)
        {
            var tempmodule = ltsSource.OrderBy(o => o.OrderDisplay).Where(m => m.ParentID == ConvertUtil.ToInt32(moduleID));
            if (checkShow)
                tempmodule = tempmodule.Where(m => m.IsShow == checkShow);

            foreach (var module in tempmodule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == checkShow);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"folder\"><label for=\"module_" + module.ID + "\"> <input id=\"module_" + module.ID + "\" class=\"moduleTree\" name=\"module_" + module.ID + "\" value=\"" + module.ID + "\" data-link=\"" + Utility.Link(module.NameAscii, null, module.LinkUrl) + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + "</strike>");
                    else
                        treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</label></span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeViewCheckBox(ltsSource, module.ID, checkShow, ltsValues, ref treeViewHtml);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"><label for=\"module_" + module.ID + "\"> <input id=\"module_" + module.ID + "\"class=\"moduleTree\" name=\"module_" + module.ID + "\" value=\"" + module.ID + "\" data-link=\"" + Utility.Link(module.NameAscii, null, module.LinkUrl) + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + "</strike>");
                    else
                        treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</span></label></li>\r\n");
                }
            }
        }
        public void BuildTreeViewCheckBoxNotLang(List<WebsiteModuleAdmin> ltsSource, int? moduleID, bool checkShow, List<int> ltsValues, ref StringBuilder treeViewHtml, int? sort = null)
        {
            var tempmodule = ltsSource.OrderBy(o => o.OrderDisplay).Where(m => m.ParentID == ConvertUtil.ToInt32(moduleID));
            if (sort.HasValue && sort.Value == 1)
            {
                tempmodule = ltsSource.OrderBy(o => o.Name).Where(m => m.ParentID == ConvertUtil.ToInt32(moduleID));
            }
            if (checkShow)
                tempmodule = tempmodule.Where(m => m.IsShow == checkShow);

            foreach (var module in tempmodule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == checkShow);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"folder\"><label for=\"module_" + module.ID + "\"> <input id=\"module_" + module.ID + "\" class=\"moduleTree\" name=\"module_" + module.ID + "\" value=\"" + module.ID + "\" data-link=\"" + Utility.Link(module.NameAscii, null, module.LinkUrl) + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + " (" + module.Lang + ")</strike>");
                    else
                        treeViewHtml.Append(module.Name + "(" + module.Lang + ")");
                    treeViewHtml.Append("</label></span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeViewCheckBoxNotLang(ltsSource, module.ID, checkShow, ltsValues, ref treeViewHtml, sort);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"><label for=\"module_" + module.ID + "\"> <input id=\"module_" + module.ID + "\"class=\"moduleTree\" name=\"module_" + module.ID + "\" value=\"" + module.ID + "\" data-link=\"" + Utility.Link(module.NameAscii, null, module.LinkUrl) + "\" type=\"checkbox\" title=\"" + module.Name + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (module.IsShow == false)
                        treeViewHtml.Append("<strike>" + module.Name + " (" + module.Lang + ")</strike>");
                    else
                        treeViewHtml.Append(module.Name + "(" + module.Lang + ")");
                    treeViewHtml.Append("</span></label></li>\r\n");
                }
            }
        }

        public void BuildTreeViewNotCheckBox(List<WebsiteModuleAdmin> ltsSource, int? moduleID, bool checkShow, List<int> ltsValues, ref StringBuilder treeViewHtml)
        {
            var tempmodule = ltsSource.OrderBy(o => o.OrderDisplay).Where(m => m.ParentID == ConvertUtil.ToInt32(moduleID));
            if (checkShow)
                tempmodule = tempmodule.Where(m => m.IsShow == checkShow);

            foreach (var module in tempmodule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == checkShow);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"folder\"><a class=\"chooseModule " + (ltsValues.IndexOf(module.ID) != -1 ? "active" : string.Empty) + "\" data-id=\"" + module.ID + "\">");
                    treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</a></span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeViewNotCheckBox(ltsSource, module.ID, checkShow, ltsValues, ref treeViewHtml);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Name + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"><a class=\"chooseModule " + (ltsValues.IndexOf(module.ID) != -1 ? "active" : string.Empty) + "\" data-id=\"" + module.ID + "\">");
                    treeViewHtml.Append(module.Name);
                    treeViewHtml.Append("</a></span></li>\r\n");
                }
            }
        }

        #endregion
        //Lấy thông tin từng module 
        public List<WebsiteModuleAdmin> GetListByModule()
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1");
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModuleAdmin> GetListByModuleLang(string code, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang And ModuleTypeCode=@code", new { lang, code });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModuleAdmin> GetGridByModuleLang(Simple.Admin.SearchModel search, string code, string lang)
        {
            if (!string.IsNullOrEmpty(search.keyword) || (!string.IsNullOrEmpty(search.Show) && search.Show.Equals("0")) || !string.IsNullOrEmpty(search.selected))
            {
                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("WITH cte(ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl) AS ");
                sqlStr.Append("(SELECT ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl");
                sqlStr.Append(" FROM WebsiteModule WHERE 1=1 And Lang = 'vi' and IsDeleted = 0");
                if (!string.IsNullOrEmpty(search.keyword))
                {
                    sqlStr.AppendFormat(" And (Name like N'%{0}%' ESCAPE N'~' Or NameAscii like N'%{1}%' ESCAPE N'~' OR ID ={1})", SqlUtility.CharacterSpecail(search.keyword), SqlUtility.CharacterSpecail(Utility.ConvertRewrite(search.keyword)), ConvertUtil.ToInt32(search.keyword));
                }
                if (!string.IsNullOrEmpty(code) && code != "ALL")
                {
                    List<string> codes = ListHelper.GetValuesArrayString(code);
                    sqlStr.Append(" AND (");
                    var i = 1;
                    foreach (var item in codes)
                    {
                        if (i < codes.Count)
                        {
                            sqlStr.AppendFormat("ModuleTypeCode= '{0}' OR ", item);
                        }
                        else
                        {
                            sqlStr.AppendFormat("ModuleTypeCode= '{0}'", item);
                        }
                        i++;
                    }
                    sqlStr.Append(")");
                }
                if (!string.IsNullOrEmpty(search.selected))
                {
                    sqlStr.AppendFormat(" And ',{0},' like N'%,'+ Convert(varchar,ID) +',%'", search.selected);
                }
                if (!string.IsNullOrEmpty(search.ModuleIds))
                {
                    sqlStr.AppendFormat(" And ',{0},' like N'%,'+ Convert(varchar,ID) +',%'", search.ModuleIds);
                }
                sqlStr.Append(" UNION ALL SELECT c.ID, c.ParentID, c.Name,c.OrderDisplay,c.ModuleTypeCode,c.NameAscii,c.IsShow,c.PositionCode,c.TotalViews,c.LinkUrl");
                sqlStr.Append(" FROM WebsiteModule c INNER JOIN cte ON cte.ParentID = c.ID");
                sqlStr.Append(") SELECT distinct ID, ParentID, Name, OrderDisplay,ModuleTypeCode,NameAscii,IsShow,PositionCode,TotalViews,LinkUrl,");
                sqlStr.Append("(select t.Name from ModuleType t where t.Code = cte.ModuleTypeCode) ModuleType FROM cte ORDER BY OrderDisplay ASC, Name asc");
                return _dapperDa.Select<WebsiteModuleAdmin>($"{sqlStr}").ToList();
            }
            else
            {
                StringBuilder sql = new StringBuilder($"SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = '{lang}'");
                if (!string.IsNullOrEmpty(code) && code != "ALL")
                {
                    var codes = ListHelper.GetValuesArrayString(code);
                    sql.Append(" AND (");
                    var i = 1;
                    foreach (var item in codes)
                    {
                        if (i < codes.Count)
                        {
                            sql.AppendFormat("ModuleTypeCode= '{0}' OR ", item);
                        }
                        else
                        {
                            sql.AppendFormat("ModuleTypeCode= '{0}'", item);
                        }
                        i++;
                    }
                    sql.Append(")");
                }
                if (!string.IsNullOrEmpty(search.type))
                {
                    sql.AppendFormat(" And ModuleTypeCode != '{0}'", search.type);
                }
                if (!string.IsNullOrEmpty(search.unselected))
                {
                    sql.AppendFormat(" And ',{0},' not like N'%,'+ Convert(varchar,ID) +',%'", search.unselected);
                }
                if (!string.IsNullOrEmpty(search.ModuleIds))
                {
                    sql.AppendFormat(" And ',{0},' like N'%,'+ Convert(varchar,ID) +',%'", search.ModuleIds);
                }
                if (!string.IsNullOrEmpty(search.keyword))
                {
                    sql.AppendFormat(" AND (Lower(Name) LIKE N'%{0}%' ESCAPE N'~')", SqlUtility.CharacterSpecail(search.keyword));
                }
                sql.Append("Order By OrderDisplay Asc");
                return _dapperDa.Select<WebsiteModuleAdmin>($"{sql}").ToList();
            }
        }
        public List<WebsiteModuleAdmin> GetListByListModuleType(string code, string moduleIds, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(moduleIds))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang AND ',' + @code +',' like N'%,' + ModuleTypeCode + ',%' AND ',' + @moduleids +',' like N'%,' + Convert(varchar,ID) + ',%'", new { lang, code, @moduleids = moduleIds.Trim(',') });
                    connect.Close();
                    return result.ToList();
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang AND ',' + @code +',' like N'%,' + ModuleTypeCode + ',%'", new { lang, code });
                    connect.Close();
                    return result.ToList();
                }
                else if (!string.IsNullOrEmpty(moduleIds))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = '" + lang + "' AND ',' + @code +',' like N'%,' + Convert(varchar,ID) + ',%'", new { lang, @code = moduleIds.Trim(',') });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang", new { lang });
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public List<WebsiteModuleAdmin> GetListByNotListModuleType(string code, string moduleIds, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(moduleIds))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang AND ',' + @code +',' not like N'%,' + ModuleTypeCode + ',%' AND ',' + @moduleids +',' like N'%,' + Convert(varchar,ID) + ',%'", new { lang, code, @moduleids = moduleIds.Trim(',') });
                    connect.Close();
                    return result.ToList();
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang AND ',' + @code +',' not like N'%,' + ModuleTypeCode + ',%'", new { lang, code });
                    connect.Close();
                    return result.ToList();
                }
                else if (!string.IsNullOrEmpty(moduleIds))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = '" + lang + "' AND ',' + @code +',' like N'%,' + Convert(varchar,ID) + ',%'", new { lang, @code = moduleIds.Trim(',') });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang", new { lang });
                    connect.Close();
                    return result.ToList();
                }
            }
        }
        public List<WebsiteModuleAdmin> GetListByInListModuleType(string code, string moduleIds, string lang)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang AND ',' + @code +',' like N'%,' + ModuleTypeCode + ',%'", new { lang, code });
                    connect.Close();
                    return result.ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(moduleIds))
                    {
                        var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = '" + lang + "' AND ',' + @code +',' like N'%,' + Convert(varchar,ID) + ',%'", new { lang, @code = moduleIds.Trim(',') });
                        connect.Close();
                        return result.ToList();
                    }
                    else
                    {
                        var result = connect.Query<WebsiteModuleAdmin>("SELECT * FROM WebsiteModule where IsDeleted = 0 AND IsShow =1 And Lang = @lang", new { lang });
                        connect.Close();
                        return result.ToList();
                    }
                }
            }
        }
        public List<WebsiteModuleAdmin> GetListAdminByArrId(string moduleIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<WebsiteModuleAdmin>("SELECT * from WebsiteModule WHERE ',' + @moduleIds +',' like N'%,' + Convert(varchar,ID) + ',%'", new { moduleIds });
                connect.Close();
                return result.ToList();
            }
        }
        public List<WebsiteModuleAdmin> GetListChidrent(int moduleId)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var paras = new DynamicParameters();
                paras.AddDynamicParams(new
                {
                    moduleId
                });
                var result = connect.Query<WebsiteModuleAdmin>("dbo.AdminModuleTree", paras, commandType: CommandType.StoredProcedure);
                connect.Close();
                return result.ToList();
            }
        }
        public string GetModuleIds(string role, string userId)
        {
            try
            {
                role = role.ToUpper();
                if (role != "ALL" && role != "ADMIN")
                {
                    using (SqlConnection connect = _dapperDa.GetOpenConnection())
                    {
                        var memberShip = connect.Query<AspnetMembership>("SELECT WebsiteModuleIds FROM aspnetMembership WHERE UserId=@userId", new { userId });
                        connect.Close();
                        return memberShip.FirstOrDefault().WebsiteModuleIds;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public void UpdateProduct(string moduleNameAscii, string moudleNameAsciiOld)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var memberShip = connect.Execute("Update Product set ModuleNameAscii = @moduleNameAscii WHERE ModuleNameAscii = @moudleNameAsciiOld", new { moduleNameAscii, moudleNameAsciiOld });
                    connect.Close();
                }
            }
            catch
            {
            }
        }
        public void UpdateWebsiteContent(string moduleNameAscii, string moudleNameAsciiOld)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    var memberShip = connect.Execute("Update WebsiteContent set ModuleNameAscii = @moduleNameAscii  WHERE ModuleNameAscii = @moudleNameAsciiOld", new { moduleNameAscii, moudleNameAsciiOld });
                    connect.Close();
                }
            }
            catch
            {
            }

        }

    }
}
