using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ADCOnline.DA.Dapper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperKey : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DapperIgnore : Attribute
    {
    }

    public class DapperDA : IDapperDA
    {
        private readonly string _connectionString = "";
        private readonly string _pathServer = "";
        private readonly string _host = "";

        public DapperDA(string conection,string pathServer="", string host = "")
        {
            _connectionString = conection;
            _pathServer = pathServer;
            _host = host;
        }
        public SqlConnection GetOpenConnection()
        {
            SqlConnection connection = new(_connectionString);
            connection.Open();
            return connection;
        }
        public bool CheckOpenConnection()
        {
            if (!string.IsNullOrEmpty(_host) && (_host.Contains("localhost") || _host.Contains("hoanghuuthang")))
            {
                SqlConnection connection = new(_connectionString);
                connection.Open();
                return true;
            }
            return false;
        }
        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        private IEnumerable<T> GetItems<T>(CommandType commandType, string sql)
        {
            using (var connection = GetOpenConnection())
            {
                var result = connection.Query<T>(sql, commandType: commandType);
                connection.Close();
                return result;
            }
        }
        private async Task<IEnumerable<T>> GetItemsAsync<T>(CommandType commandType, string sql)
        {
            using (var connection = await GetOpenConnectionAsync())
            {
                var result = await connection.QueryAsync<T>(sql, commandType: commandType);
                connection.Close();
                return result;
            }
        }
        public IEnumerable<T> GetListItems<T>(string sql,CommandType commandType, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                var result = connection.Query<T>(sql, parameters, commandType: commandType);
                connection.Close();
                return result;
            }
        }
        public int ExecuteSql(string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                var result = connection.Execute(sql, parameters, commandType: CommandType.Text);
                connection.Close();
                return result;
            }
        }
        public async Task<int> ExecuteSqlAsync(string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
                connection.Close();
                return result;
            }
        }

        private int Execute(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                var result = connection.Execute(sql, parameters, commandType: commandType);
                connection.Close();
                return result;
            }
        }
        private async Task<int> ExecuteAsync(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = await GetOpenConnectionAsync())
            {
                var result = await connection.ExecuteAsync(sql, parameters, commandType: commandType);
                connection.Close();
                return result;
            }
        }
        public IEnumerable<T> Select<T>(string sql)
        {
            return GetItems<T>(CommandType.Text, sql);
        }
        public async Task<IEnumerable<T>> SelectAsync<T>(string sql)
        {
            return await GetItemsAsync<T>(CommandType.Text, sql);
        }
        #region create sql
        public IEnumerable<T> SelectPage<T>(string sql, int currentPage, int rowPerPage)
        {
            string query = CreateSqlPage(sql, currentPage, rowPerPage);
            return GetItems<T>(CommandType.Text, query);
        }
        public async Task<IEnumerable<T>> SelectPageAsync<T>(string sql, int currentPage, int rowPerPage)
        {
            string query = CreateSqlPage(sql, currentPage, rowPerPage);
            return await GetItemsAsync<T>(CommandType.Text, query);
        }
        private string CreateSqlPage(string sql, int currentPage, int rowPerPage)
        {
            //int currentPage = !string.IsNullOrEmpty(request["page"]) ? Convert.ToInt32(request["page"]) : 1;
            //int rowPerPage = !string.IsNullOrEmpty(request["RowPerPage"]) ? Convert.ToInt32(request["RowPerPage"]) : 20;
            currentPage = currentPage > 1 ? currentPage : 1;
            rowPerPage = rowPerPage > 0 ? rowPerPage : 20;
            string query = ReplaceFirst(sql, "select", "select COUNT(ID) OVER () as TotalRecord,");
            query = string.Format("{0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", query, ((currentPage - 1) * rowPerPage), rowPerPage);
            return query;
        }
        #endregion
        public int Insert<T>(T obj)
        {
            try
            {
                var propertyContainer = ParseProperties(obj);
                var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES (@{2}) SELECT CAST(scope_identity() AS int)",
                    typeof(T).Name,
                    string.Join(", ", propertyContainer.ValueNames),
                    string.Join(", @", propertyContainer.ValueNames));

                using (var connection = GetOpenConnection())
                {
                    var id = connection.Query<int>
                        (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
                    SetId(obj, id, propertyContainer.IdPairs);
                    return id;
                }

            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                return 0;
            }
        }
        public int InsertNoId<T>(T obj)
        {
            try
            {
                var propertyContainer = ParseProperties(obj);
                var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES (@{2})",
                    typeof(T).Name,
                    string.Join(", ", propertyContainer.ValueNames),
                    string.Join(", @", propertyContainer.ValueNames));

                using (var connection = GetOpenConnection())
                {
                    var id = connection.Query<int>
                        (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).FirstOrDefault();
                    SetId(obj, id, propertyContainer.IdPairs);
                    return 1;
                }

            }
            catch
            {
                return 0;
            }
        }
        public int InsertUserNoId<T>(T obj)
        {
            try
            {
                var propertyContainer = ParsePropertiesAdmin(obj);
                var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES (@{2})",
                    typeof(T).Name,
                    string.Join(", ", propertyContainer.ValueNames),
                    string.Join(", @", propertyContainer.ValueNames));

                using (var connection = GetOpenConnection())
                {
                    var id = connection.Query<int>
                        (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).FirstOrDefault();
                    SetId(obj, id, propertyContainer.IdPairs);
                    return 1;
                }

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int Update<T>(T obj)
        {
            try
            {
                var propertyContainer = ParseProperties(obj);
                var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
                var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
                var sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", typeof(T).Name, sqlValuePairs, sqlIdPairs);
                var a = Execute(CommandType.Text, sql, propertyContainer.AllPairs);
                return 1;
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                return 0;
                throw;
            }
        }
        public async Task<int> UpdateAsync<T>(T obj)
        {
            try
            {
                var propertyContainer = ParseProperties(obj);
                var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
                var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
                var sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", typeof(T).Name, sqlValuePairs, sqlIdPairs);
                await ExecuteAsync(CommandType.Text, sql, propertyContainer.AllPairs);
                return 1;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return 0;
                throw;
            }
        }
        public int UpdateNoId<T>(T obj,string sqlWhere)
        {
            try
            {
                var propertyContainer = ParseProperties(obj);
                var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
                var sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", typeof(T).Name, sqlValuePairs, sqlWhere);
                var a = Execute(CommandType.Text, sql, propertyContainer.AllPairs);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public void Delete<T>(T obj)
        {
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", typeof(T).Name, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.IdPairs);
        }
        public void DeleteNoId<T>(T obj, string sqlWhere)
        {
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", typeof(T).Name, sqlWhere);
            Execute(CommandType.Text, sql);
        }
        public void DeleteAttr<T>(T obj, string sqlWhere)
        {
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", typeof(T).Name, sqlWhere);
            Execute(CommandType.Text, sql);
        }
        public void DeleteIsTrue<T>(T obj, string sqlWhere)
        {
            var propertyContainer = ParseProperties(obj);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("UPDATE [{0}] SET IsDeleted = 1 WHERE {1}", typeof(T).Name, sqlIdPairs);
            Execute(CommandType.Text, sql);
        }
        private static PropertyContainer ParseProperties<T>(T obj, string key = "")
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name.ToLower();

            var validKeyNames = new[]
                {
                    "id",
                    "userid",
                    string.Format("{0}id", typeName), string.Format("{0}_id", typeName)
                };

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;
                var type = property.PropertyType;
                type = Nullable.GetUnderlyingType(type) ?? type;
                if (type != typeof(string) && type != typeof(int) && type != typeof(bool) && type != typeof(DateTime)
                    && type != typeof(double) && type != typeof(decimal) && type != typeof(Nullable) && type != typeof(Guid))
                    continue;

                var name = property.Name;
                var value = property.GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name.ToLower()))
                {
                    propertyContainer.AddId(name, value);
                }
                else if (type == typeof(DateTime))
                {
                    if (Convert.ToDateTime(value) == DateTime.MinValue)
                    {
                        propertyContainer.AddValue(name, null);
                    }
                    else
                    {
                        propertyContainer.AddValue(name, value);
                    }
                }
                else
                {
                    propertyContainer.AddValue(name, value);
                }
            }
            return propertyContainer;
        }
        private static PropertyContainer ParsePropertiesAdmin<T>(T obj, string key = "")
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name.ToLower();

            var validKeyNames = new[]
                {
                    "Userid",
                    string.Format("{0}id", typeName), string.Format("{0}_id", typeName)
                };

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;
                var type = property.PropertyType;
                type = Nullable.GetUnderlyingType(type) ?? type;
                if (type != typeof(string) && type != typeof(int) && type != typeof(bool) && type != typeof(DateTime)
                    && type != typeof(double) && type != typeof(decimal) && type != typeof(Nullable) && type != typeof(Guid))
                    continue;

                var name = property.Name;
                var value = property.GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name.ToLower()))
                {
                    propertyContainer.AddId(name, value);
                }
                else if (type == typeof(DateTime))
                {
                    if (Convert.ToDateTime(value) == DateTime.MinValue)
                    {
                        propertyContainer.AddValue(name, null);
                    }
                    else
                    {
                        propertyContainer.AddValue(name, value);
                    }
                }
                else
                {
                    propertyContainer.AddValue(name, value);
                }
            }
            return propertyContainer;
        }
        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        private static string GetSqlPairs (IEnumerable<string> keys, string separator = ", ")
        {
            var pairs = keys.Select(key => string.Format("{0}=@{0}", key)).ToList();
            return string.Join(separator, pairs);
        }
        private void SetId<T>(T obj, int id, IDictionary<string, object> propertyPairs)
        {
            if (propertyPairs.Count == 1)
            {
                var propertyName = propertyPairs.Keys.First();
                var propertyInfo = obj.GetType().GetProperty(propertyName);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
        }
        private class PropertyContainer
        {
            private readonly Dictionary<string, object> _ids;
            private readonly Dictionary<string, object> _values;

            #region Properties

            internal IEnumerable<string> IdNames
            {
                get { return _ids.Keys; }
            }

            internal IEnumerable<string> ValueNames
            {
                get { return _values.Keys; }
            }

            internal IEnumerable<string> AllNames
            {
                get { return _ids.Keys.Union(_values.Keys); }
            }

            internal IDictionary<string, object> IdPairs
            {
                get { return _ids; }
            }

            internal IDictionary<string, object> ValuePairs
            {
                get { return _values; }
            }

            internal IEnumerable<KeyValuePair<string, object>> AllPairs
            {
                get { return _ids.Concat(_values); }
            }

            #endregion

            #region Constructor

            internal PropertyContainer()
            {
                _ids = new Dictionary<string, object>();
                _values = new Dictionary<string, object>();
            }

            #endregion

            #region Methods

            internal void AddId(string name, object value)
            {
                _ids.Add(name, value);
            }

            internal void AddValue(string name, object value)
            {
                _values.Add(name, value);
            }

            #endregion
        }
        private string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.CurrentCultureIgnoreCase);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
