using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCOnline.DA.Dapper
{
    public static class SqlConst
    {
        public const string UnionAll = " UNION ALL ";
        public const string TotalNameAscii = "+(Case when  CHARINDEX('{0}',NameAscii) >0 then 1 ELSE 0 END)";
        public const string LikeKey = "(Name like N'%{0}%' OR NameAscii like N'%{0}%' OR Content like N'%{0}%')";
    }
}
