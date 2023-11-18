using System;
using System.Linq;
using System.Linq.Expressions;

namespace ADCOnline.Business.Implementation
{
    public class BaseDa
    {
        protected IQueryable<T> PagedResult<T, TResult>( IQueryable<T> query,int currentpage, int pageSize, Expression<Func<T, TResult>> orderByProperty, bool isAscendingOrder, out int rowsCount)
        {
            int pageNum = currentpage > 1 ? currentpage : 1;
            if (pageSize <= 0) pageSize = 20;
            //Total result count
            rowsCount = query.Count();
            //If page number should be > 0 else set to first page
            //   if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;
            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * pageSize;
            query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);
            //Skip the required rows for the current page and take the next records of pagesize count
            return query.Skip(excludedRows).Take(pageSize);
        }

    }
}
