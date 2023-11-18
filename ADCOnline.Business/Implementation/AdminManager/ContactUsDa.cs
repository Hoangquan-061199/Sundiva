using ADCOnline.DA.Dapper;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Dapper;

namespace ADCOnline.Business.Implementation.AdminManager
{
    public class ContactUsDa : BaseDa
    {
        private readonly DapperDA _dapperDa;
        public ContactUsDa(string connectionSql) => _dapperDa = new DapperDA(connectionSql);
        public List<ContactUsAdmin> ListSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM ContactUs WHERE 1=1 And Code !='RequestPopup'");
            try
            {
                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.keyword))
                    {
                        sql.Append($" AND (FullName LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~')");
                    }

                    if (!string.IsNullOrEmpty(search.from))
                    {
                        sql.Append($" AND CreatedDate >={SqlUtility.ConvertDate(search.from, false)}");
                    }

                    if (!string.IsNullOrEmpty(search.to))
                    {
                        sql.Append($" AND CreatedDate <={SqlUtility.ConvertDate(search.to, false, true)}");
                    }
                    if (!string.IsNullOrEmpty(search.type))
                    {
                        sql.Append($" And Code = '{search.type}'");
                    }
                    if (!string.IsNullOrEmpty(search.Status))
                    {
                        sql.Append($" And Status = {search.Status}");
                    }
                }
            }
            catch
            {

            }
            sql.Append($" ORDER BY ID DESC");
            if (isExport)
            {
                return _dapperDa.Select<ContactUsAdmin>($"{sql}").ToList();
            }
            return _dapperDa.SelectPage<ContactUsAdmin>($"{sql}", page, rowPage).ToList();
        }
        public List<ContactUsAdmin> ListApplySearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM ContactUs WHERE 1=1 And Code ='Apply'");
            try
            {
                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.keyword))
                    {
                        sql.Append($" AND (FullName LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~'" +
                            $" Phone LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~'" +
                            $" Email LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~')");
                    }

                    if (!string.IsNullOrEmpty(search.from))
                    {
                        sql.Append($" AND CreatedDate >={SqlUtility.ConvertDate(search.from, false)}");
                    }
                    if (!string.IsNullOrEmpty(search.to))
                    {
                        sql.Append($" AND CreatedDate <={SqlUtility.ConvertDate(search.to, false, true)}");
                    }
                    if (!string.IsNullOrEmpty(search.Status))
                    {
                        sql.Append($" And Status = {search.Status}");
                    }
                }
            }
            catch
            {

            }
            sql.Append($" ORDER BY ID DESC");
            if (isExport)
            {
                return _dapperDa.Select<ContactUsAdmin>($"{sql}").ToList();
            }
            return _dapperDa.SelectPage<ContactUsAdmin>($"{sql}", page, rowPage).ToList();
        }
        public List<ContactUsAdmin> ListPopupSearch(SearchModel search, int page, int rowPage, bool isExport)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM ContactUs WHERE 1=1 And Code ='RequestPopup'");
            try
            {
                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.keyword))
                    {
                        sql.Append($" AND (FullName LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~'" +
                            $" Phone LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~'" +
                            $" Email LIKE N'%{SqlUtility.CharacterSpecail(search.keyword)}%' ESCAPE N'~')");
                    }

                    if (!string.IsNullOrEmpty(search.from))
                    {
                        sql.Append($" AND CreatedDate >={SqlUtility.ConvertDate(search.from, false)}");
                    }
                    if (!string.IsNullOrEmpty(search.to))
                    {
                        sql.Append($" AND CreatedDate <={SqlUtility.ConvertDate(search.to, false, true)}");
                    }
                    if (!string.IsNullOrEmpty(search.Status))
                    {
                        sql.Append($" And Status = {search.Status}");
                    }
                }
            }
            catch
            {

            }
            sql.Append($" ORDER BY ID DESC");
            if (isExport)
            {
                return _dapperDa.Select<ContactUsAdmin>($"{sql}").ToList();
            }
            return _dapperDa.SelectPage<ContactUsAdmin>($"{sql}", page, rowPage).ToList();
        }
        public int Update(ContactUs obj) => _dapperDa.Update(obj);
        public void Delete(ContactUs obj, string sql) => _dapperDa.DeleteNoId(obj, sql);
        public ContactUs GetId(int id)
        {
            using (SqlConnection connect = _dapperDa.GetOpenConnection())
            {
                var result = connect.Query<ContactUs>("select * from ContactUs where ID = @id", new { id });
                connect.Close();
                return result.FirstOrDefault();
            }
        }
    }
}
