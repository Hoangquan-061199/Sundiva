using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Admin;
using ADCOnline.Simple.Base;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class ModuleAdminDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ModuleAdminDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<Module> GetMenu(string role, string userId)
        {
            try
            {
                role = role.ToUpper();
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    if(role == "ALL")
                    {
                        var result = connect.Query<Module>("SELECT * from Module where IsShow = 1");
                        connect.Close();
                        return result.ToList();
                    }
                    else if (role == "ADMIN")
                    {
                        var result = connect.Query<Module>("SELECT * from Module where IsShow = 1 AND ID NOT IN(21)");
                        connect.Close();
                        return result.ToList();
                    }
                    else
                    {
                        var memberShip = connect.Query<AspnetMembership>("SELECT ModuleIds FROM aspnetMembership WHERE UserId=@userId", new { userId }).FirstOrDefault();
                        if (memberShip != null)
                        {
                            var result = connect.Query<Module>("SELECT * from Module where IsShow = 1 AND ',' + @ModuleIds + ',' Like N'%,' + Convert(varchar,ID) + ',%'", new { @ModuleIds = memberShip.ModuleIds });
                            connect.Close();
                            return result.ToList();
                        }
                    }
                }                
            }
            catch
            {

            }
            return null;
        }
        public List<Module> GetTabMenu(string role, string userId)
        {
            try
            {
                role = role.ToUpper();
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    if(role == "ALL")
                    {
                        var result = connect.Query<Module>("SELECT * from Module where IsShow = 1");
                        connect.Close();
                        return result.ToList();
                    }
                    else if (role == "ADMIN")
                    {
                        var result = connect.Query<Module>("SELECT * from Module where IsShow = 1 AND ID NOT IN(21)");
                        connect.Close();
                        return result.ToList();
                    }
                    else
                    {
                        var memberShip = connect.Query<AspnetMembership>("SELECT ModuleIds FROM aspnetMembership WHERE UserId=@userId", new { userId }).FirstOrDefault();
                        if (memberShip != null)
                        {
                            var result = connect.Query<Module>("SELECT * from Module where IsShow = 1 AND ',' + @ModuleIds + ',' Like N'%,' + Convert(varchar,ID) + ',%'", new { @ModuleIds = memberShip.ModuleIds });
                            connect.Close();
                            return result.ToList();
                        }
                    }
                }                
            }
            catch
            {

            }
            return null;
        }
        public List<ModuleAdmin> GetAdminAll(bool isShow=false)
        {
            try
            {
                using (SqlConnection connect = _dapperDa.GetOpenConnection())
                {
                    if (isShow)
                    {
                        var result = connect.Query<ModuleAdmin>("SELECT * from Module WHERE 1=1 AND IsShow =1");
                        connect.Close();
                        return result.ToList();
                    }
                    else
                    {
                        var result = connect.Query<ModuleAdmin>("SELECT * from Module WHERE 1=1");
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
        public Module GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Module>("SELECT * from Module WHERE ID =@id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public Module GetTag(string tag)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<Module>("SELECT * from Module WHERE Tag =@tag", new { tag });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
        public List<ModuleAdmin> GetListAdminByArrId(string moduleIds)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ModuleAdmin>("SELECT * from Module WHERE ',' + @moduleIds +',' Like N'%,'+Convert(varchar,ID)+',%'", new { moduleIds });
                connect.Close();
                return result.ToList();
            }
        }
        public void BuildTreeViewCheckBox(List<ModuleAdmin> ltsSource, int moduleID, bool checkShow, List<int> ltsValues, ref StringBuilder treeViewHtml)
        {
            var tempModule = ltsSource.OrderBy(o => o.Ord).Where(m => m.ParentID == moduleID && m.ID > 1);
            if (checkShow)
                tempModule = tempModule.Where(m => m.IsShow == true);
            foreach (var module in tempModule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == true);
                int totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Content + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"folder\"> <input id=\"Category_" + module.ID + "\" class=\"moduleTree\" name=\"Category_" + module.ID + "\" value=\"" + module.ID + "\" type=\"checkbox\" title=\"" + module.NameModule + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (!module.IsShow == true)
                        treeViewHtml.Append("<strike>" + module.NameModule + "</strike>");
                    else
                        treeViewHtml.Append(module.NameModule);
                    treeViewHtml.Append("</span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeViewCheckBox(ltsSource, module.ID, checkShow, ltsValues, ref treeViewHtml);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Content + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"> <input id=\"Category_" + module.ID + "\" class=\"moduleTree\" name=\"Category_" + module.ID + "\" value=\"" + module.ID + "\" type=\"checkbox\" title=\"" + module.NameModule + "\" " + (ltsValues.Contains(module.ID) ? " checked" : string.Empty) + "/> ");
                    if (!module.IsShow == true)
                        treeViewHtml.Append("<strike>" +module.NameModule + "</strike>");
                    else
                        treeViewHtml.Append(module.NameModule);
                    treeViewHtml.Append("</span></li>\r\n");
                }
            }
        }
        public void BuildTreeView(List<ModuleAdmin> ltsSource, int moduleID, bool checkShow, ref StringBuilder treeViewHtml, bool add, bool delete, bool edit, bool show, bool order)
        {
            var tempModule = ltsSource.OrderBy(o => o.Ord).Where(m => m.ParentID == moduleID);
            if (checkShow)
                tempModule = tempModule.Where(m => m.IsShow == true);

            foreach (var module in tempModule)
            {
                var countQuery = ltsSource.Where(m => m.ParentID == module.ID && m.ID > 1);
                if (checkShow)
                    countQuery = countQuery.Where(m => m.IsShow == true);
                var totalChild = countQuery.Count();
                if (totalChild > 0)
                {
                    treeViewHtml.Append("<li title=\"" + module.Content + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"folder\"><a class=\"tool\" href=\"javascript:;\">");
                    if (!module.IsShow == true)
                        treeViewHtml.Append("<strike>" + module.NameModule + "</strike>");
                    else
                        treeViewHtml.Append(module.NameModule);
                    treeViewHtml.Append("</a>\r\n");
                    if(totalChild > 0)
                    {
                        treeViewHtml.AppendFormat("<div class=\"badge badge-danger\">{0}</div>\r\n",totalChild);
                    }
                    treeViewHtml.Append(BuildEditToolByID(module, add, delete, edit, show, order) + "\r\n");
                    treeViewHtml.Append("</span>\r\n");
                    treeViewHtml.Append("<ul>\r\n");
                    BuildTreeView(ltsSource, module.ID, checkShow, ref treeViewHtml, add, delete, edit, show, order);
                    treeViewHtml.Append("</ul>\r\n");
                    treeViewHtml.Append("</li>\r\n");
                }
                else
                {
                    treeViewHtml.Append("<li title=\"" + module.Content + "\" class=\"unselect\" id=\"" + module.ID.ToString() + "\"><span class=\"file\"><a class=\"tool\" href=\"javascript:;\">");
                    if (!module.IsShow == true)
                        treeViewHtml.Append("<strike>" +module.NameModule + "</strike>");
                    else
                        treeViewHtml.Append(module.NameModule);
                    treeViewHtml.Append("</a>" + BuildEditToolByID(module, add, delete, edit, show, order) + "</span></li>\r\n");
                }
            }
        }
        private string BuildEditToolByID(ModuleAdmin moduleItem, bool add, bool delete, bool edit, bool show, bool order)
        {
            var strTool = new StringBuilder();
            strTool.Append("<div class=\"quickTool\">\r\n");
            //strTool.AppendFormat("    <a title=\"Xem tính năng: {1}\" class=\"showModule\" href=\"#{0}\">\r\n", moduleItem.Id, moduleItem.NameModule);
            //strTool.Append("        <img border=\"0\" title=\"Xem tính năng\" src=\"/Content/Admin/images/gridview/show.gif\">\r\n");
            //strTool.Append("    </a>");
            //xét active
            strTool.AppendFormat("    <a title=\"Active: {1}\" href=\"#{0}\" class=\"AddActive\">\r\n", moduleItem.ID, moduleItem.NameModule);
            strTool.Append("        <i class=\"fa fa-check-circle-o\"></i>\r\n");
            strTool.Append("    </a>\r\n");

            strTool.AppendFormat("    <a title=\"Gán Role: {1}\" class=\"roleModule\" href=\"#{0}\">\r\n", moduleItem.ID, moduleItem.NameModule);
            strTool.Append("        <i class=\"fa fa-user\"></i>\r\n");
            strTool.Append("    </a>");

            strTool.AppendFormat("    <a title=\"Gán User: {1}\" class=\"userModule\" href=\"#{0}\">\r\n", moduleItem.ID, moduleItem.NameModule);
            strTool.Append("        <i class=\"fa fa-users\"></i>\r\n");
            strTool.Append("    </a>");

            if (add)
            {
                strTool.AppendFormat("<a title=\"Thêm mới Module: {1}\" class=\"add\" href=\"#{0}\">\r\n",
                                     moduleItem.ID, moduleItem.NameModule);
                strTool.Append("        <i class=\"fa fa-plus-square-o\"></i>\r\n");
                strTool.Append("    </a>");
            }
            if (edit)
            {
                strTool.AppendFormat("    <a title=\"Chỉnh sửa: {1}\" class=\"edit\" href=\"#{0}\">\r\n", moduleItem.ID, moduleItem.NameModule);
                strTool.Append("        <i class=\"fa fa-pencil\"></i>\r\n");
                strTool.Append("    </a>");
            }
            if (show)
            {
                if (moduleItem.IsShow != null && moduleItem.IsShow.Value)
                {
                    strTool.AppendFormat("    <a title=\"Ẩn: {1}\" href=\"#{0}\" class=\"hiddens\">\r\n", moduleItem.ID, moduleItem.NameModule);
                    strTool.Append("        <i class=\"fa fa-exclamation-circle\"></i>\r\n");
                    strTool.Append("    </a>\r\n");
                }
                else
                {
                    strTool.AppendFormat("    <a title=\"Hiển thị: {1}\" href=\"#{0}\" class=\"show\">\r\n",
                                         moduleItem.ID, moduleItem.NameModule);
                    strTool.Append("        <i class=\"fa fa-check-square-o\"></i>\r\n");
                    strTool.Append("    </a>\r\n");
                }
            }
            if (delete)
            {
                strTool.AppendFormat("    <a title=\"Xóa: {1}\" href=\"#{0}\" class=\"delete\">\r\n", moduleItem.ID,
                                     moduleItem.NameModule);
                strTool.Append("        <i class=\"fa fa-trash\"></i>\r\n");
                strTool.Append("    </a>\r\n");
            }
            if (order)
            {
                strTool.AppendFormat("    <a title=\"Sắp xếp các Module con: {1}\" href=\"#{0}\" class=\"sort\">\r\n",
                                     moduleItem.ParentID, moduleItem.NameModule);
                strTool.Append("        <i class=\"fa fa-sort\"></i>\r\n");
                strTool.Append("    </a>\r\n");
            }
            strTool.Append("</div>\r\n");
            return strTool.ToString();
        }
        public int Insert(Module obj) => _dapperDa.Insert(obj);
        public int Update(Module obj) => _dapperDa.Update(obj);
        public void Delete(Module obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
    }
}
