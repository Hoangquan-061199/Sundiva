using System.Collections.Generic;

namespace ADCOnline.DA.Dapper
{
    public interface IDapperDA
    {
        void Delete<T>(T obj);
        int Insert<T>(T obj);
        int InsertNoId<T>(T obj);
        int UpdateNoId<T>(T obj, string sqlWhere);
        void DeleteNoId<T>(T obj, string sqlWhere);
        void DeleteIsTrue<T>(T obj,string sqlWhere);

        IEnumerable<T> Select<T>(string sql);
        IEnumerable<T> SelectPage<T>(string sql, int currentPage, int rowPerPage);
        int Update<T>(T obj);
        int ExecuteSql(string sql, object parameters = null);
    }
}