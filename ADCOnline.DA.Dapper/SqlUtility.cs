using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADCOnline.DA.Dapper
{
    public class SqlUtility
    {
        public enum Type : int
        {
            Equal = 1,
            Like = 2,
            LikeNText = 3,
            In = 4,
            NotIn = 5
        }
        public static DateTime? ConvertToDate(string obj, bool isEn)
        {
            if (isEn)
            {
                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "MM/dd/yyyy HH:mm:ss", culture);
                    return dateVal;
                }
                catch
                {

                }

                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "MM/dd/yyyy", culture);
                    return dateVal;
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy HH:mm:ss", culture);
                    return dateVal;
                }
                catch
                {

                }

                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy", culture);
                    return dateVal;
                }
                catch
                {

                }

            }
            return null;
        }
        public static string WhereOrLikeList(List<string> objList, string column)
        {
            string result = "";
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0)
                {
                    str.Append(" AND (");
                    var i = 1;
                    foreach (var item in objList)
                    {
                        if (i < objList.Count)
                        {
                            str.Append("','+" + column + "+',' like N'%," + item + ",%' OR ");
                        }
                        else
                        {
                            str.Append("','+" + column + "+',' like N'%," + item + ",%' ");
                        }
                        i++;
                    }
                    str.Append(")");
                }
            }
            catch
            {
            }
            result = str.ToString();
            return result;
        }
        public static string WhereOrLikeListWithOther(List<string> objList, string column, string other)
        {
            string result = "";
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0)
                {
                    str.Append(" AND (");
                    var i = 1;
                    foreach (var item in objList)
                    {
                        if (i < objList.Count)
                        {
                            str.Append("','+" + column + "+',' like '%," + item + ",%' OR ");
                        }
                        else
                        {
                            str.Append("','+" + column + "+',' like '%," + item + ",%' ");
                        }
                        i++;
                    }
                    str.Append(other);
                    str.Append(")");
                }
            }
            catch
            {
            }
            result = str.ToString();
            return result;
        }
        public static string OrLikeList(List<string> objList, string column)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0)
                {
                    str.Append(" Or (");
                    var i = 1;
                    foreach (var item in objList)
                    {
                        if (i < objList.Count)
                        {
                            str.AppendFormat("','+{0}+',' like '%,{1},%' OR ", column, item);
                        }
                        else
                        {
                            str.AppendFormat("','+{0}+',' like '%,{1},%' ", column, item);
                        }
                        i++;
                    }
                    str.Append(")");
                }
            }
            catch
            {
            }
            return $"{str}";
        }
        public static string WhereAndLikeList(List<string> objList, string column)
        {
            string result = "";
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0)
                {
                    str.Append(" AND (");
                    var i = 1;
                    foreach (var item in objList)
                    {
                        if (i < objList.Count)
                        {
                            str.Append("','+" + column + "+',' like '%," + item + ",%' And ");
                        }
                        else
                        {
                            str.Append("','+" + column + "+',' like '%," + item + ",%' ");
                        }
                        i++;
                    }
                    str.Append(")");
                }
            }
            catch
            {
            }
            result = str.ToString();
            return result;
        }
        public static string WhereOrNLikeList(List<string> objList, List<string> columns)
        {
            var result = "";
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0)
                {
                    str.Append(" AND (");
                    var i = 1;
                    foreach (var item in objList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (i < objList.Count)
                            {
                                foreach (var column in columns)
                                {
                                    str.Append(column + " like N'%" + CharacterSpecail(item.Trim()) + "%'  ESCAPE N'~' OR ");
                                }
                            }
                            else
                            {
                                var j = 1;
                                foreach (var column in columns)
                                {
                                    if (j < columns.Count)
                                    {
                                        if (column.ToLower() == "nameascii")
                                        {
                                            str.Append(column + " like N'%" + ConvertRewrite(CharacterSpecail(item.Trim())) + "%'  ESCAPE N'~' OR ");
                                        }
                                        else
                                        {
                                            str.Append(column + " like N'%" + CharacterSpecail(item.Trim()) + "%'  ESCAPE N'~' OR ");
                                        }

                                    }
                                    else
                                    {
                                        if (column.ToLower() == "nameascii")
                                        {
                                            str.Append(column + " like N'%" + ConvertRewrite(CharacterSpecail(item.Trim())) + "%'  ESCAPE N'~' ");
                                        }
                                        else
                                        {
                                            str.Append(column + " like N'%" + CharacterSpecail(item.Trim()) + "%'  ESCAPE N'~' ");
                                        }
                                    }
                                    j++;
                                }

                            }

                        }
                        i++;
                    }
                    str.Append(" ) ");
                }

            }
            catch
            {
            }
            result = str.ToString();
            return result;
        }
        public static string WherAndNLikeListSearch(List<string> objList, List<string> columns)
        {
            var result = "";
            StringBuilder str = new StringBuilder();
            try
            {
                if (objList != null && objList.Count > 0 && columns != null && columns.Count > 0)
                {
                    int i = 1;
                    str.Append(" And (");
                    foreach (var item in columns)
                    {

                        if (i == 1)
                        {
                            str.Append("(");
                        }
                        else
                        {
                            str.Append(" Or (");
                        }
                        int j = 1;
                        foreach (var key in objList)
                        {
                            if (j == 1)
                            {
                                str.Append(item + " like N'%" + CharacterSpecail(key.Trim()) + "%'  ESCAPE N'~'");
                            }
                            else
                            {
                                str.Append(" And " + item + " like N'%" + CharacterSpecail(key.Trim()) + "%'  ESCAPE N'~'");
                            }
                            j++;
                        }
                        str.Append(")");
                        i++;
                    }
                    str.Append(")");
                }
            }
            catch
            {
            }
            result = str.ToString();
            return result;
        }
        public static string AND(string column, object value, int type)
        {
            var typeNumber = 0;
            try
            {
                var listValue = value.ToString().Split(',').Select(double.Parse).ToList();
                typeNumber = 1;
            }
            catch
            {

            }
            if (typeNumber == 0)
            {
                var listValue = value.ToString().Split(',').ToList();
                if (type == 4 || type == 5)
                {
                    listValue = listValue.Select(c => "'" + c + "'").ToList();
                    value = string.Join(",", listValue);
                }
            }

            switch (type)
            {
                case 1:
                    return " AND " + column + " = '" + value + "'";
                case 2:
                    return " AND " + column + " LIKE '%" + value + "%'";
                case 3:
                    return " AND " + column + " LIKE N'%" + value + "%'";
                case 4:
                    return " AND " + column + " IN(" + value + ")";
                case 5:
                    return " AND " + column + " NOT IN(" + value + ")";
                case 6:
                    return " AND " + column + " >='" + value + "'";
                case 7:
                    return " AND " + column + " <='" + value + "'";
                default:
                    return "";
            }

        }
        public static string WhereNLike(string column, string key)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    result.Append(" AND ");
                    result.Append(column);
                    result.Append(" like N'%");
                    result.Append(CharacterSpecail(key.Trim()));
                    result.Append("%' ESCAPE N'~' ");
                }
            }
            catch
            {
            }
            return result.ToString();
        }
        public static string CharacterSpecail(string key)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    //key = key.Replace("_", "~_");
                    key = key.Replace("'", "~''");
                    key = key.Replace("%", "~%");
                    key = key.Replace("~", "~~");
                }
            }
            catch
            {

            }
            return key;
        }
        public static string ConvertRewrite(string unicode)
        {
            if (!string.IsNullOrEmpty(unicode))
            {
                unicode = NewUnicodeToAscii(unicode);
                unicode = unicode.ToLower().Trim();
                unicode = Regex.Replace(unicode, @"\s+", " ");
                unicode = Regex.Replace(unicode, "[\\s]", "-");
                unicode = Regex.Replace(unicode, @"-+", "-");
                unicode = RemoveSpecialCharacter(unicode);
            }
            return unicode;
        }
        public static string NewUnicodeToAscii(string unicode)
        {
            string strFormD = unicode.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strFormD.Length; i++)
            {
                System.Globalization.UnicodeCategory uc =
                    System.Globalization.CharUnicodeInfo.GetUnicodeCategory(strFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(strFormD[i]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
        public static string RemoveSpecialCharacter(string unicode)
        {
            unicode = Regex.Replace(unicode, "[,|~|@|/|.|:|?|#|$|%|&|*|(|)|+|”|“|'|\"|!|`|–]", "-", RegexOptions.IgnoreCase);

            return unicode;
        }
        public static string ConvertDate(object obj, bool isEn, bool isTo = false)
        {
            var date = ConvertDate2(obj, isEn);
            if (isTo)
            {
                date = date.Split(' ')[0] + " 23:59:59";
            }
            return "'" + date + "'";
        }
        public static string ConvertDate2(object obj, bool isEn)
        {
            if (obj.GetType() == typeof(DateTime))
            {
                return string.Format("{0:MM/dd/yyyy HH:mm:ss}", obj);
            }
            if (isEn)
            {
                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "MM/dd/yyyy HH:mm:ss", culture);
                    return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
                }
                catch
                {

                }

                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "MM/dd/yyyy", culture);
                    return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy HH:mm:ss", culture);
                    return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
                }
                catch
                {

                }

                try
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);
                    var dateVal = DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy", culture);
                    return dateVal.ToString("MM/dd/yyyy HH:mm:ss");
                }
                catch
                {

                }

            }
            return obj.ToString();
        }
    }
}
