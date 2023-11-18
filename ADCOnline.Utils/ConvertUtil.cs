using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ADCOnline.Utils
{
    public class ConvertUtil
    {
        public static int ToInt32(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception e)
            {
            }
            return 0;
        }
        public static int? ToInt32HasNull(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
            }
            return null;
        }
        public static Guid ToGuid(object obj)
        {
            try
            {
                return Guid.Parse(obj.ToString());
            }
            catch
            {
            }
            return new Guid();
        }
        public static double ToDouble(object obj)
        {
            double retVal = 0;
            try
            {
                retVal = Convert.ToDouble(obj);
            }
            catch
            {
            }

            return retVal;
        }

        public static decimal ToDecimal(object obj)
        {
            decimal retVal = 0;

            try
            {
                retVal = Convert.ToDecimal(obj);
            }
            catch
            {
            }

            return retVal;
        }
        public static bool ToBool(object obj)
        {
            bool retVal;
            try
            {
                retVal = Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static DateTime StringToDateTime(string obj, string format)
        {
            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                DateTime dateVal = DateTime.ParseExact(obj, format, culture);
                return dateVal;
            }
            catch
            {
            }
            return new DateTime();
        }
        public static DateTime ToDateTime(string obj)
        {
            DateTime retVal = DateTime.Now;
            string[] strArr = obj.Split(' ');
            int lenStrArr = strArr.Length;
            try
            {
                if (CheckDateTime())
                {
                    return Convert.ToDateTime(obj);
                }
                if (lenStrArr >= 1)
                {
                    string str = strArr[0];
                    string[] strTemp = str.Split('/');
                    if (strTemp.Length == 3)
                    {
                        string t = string.Empty;
                        if (lenStrArr == 2)
                        {
                            t = strArr[1];
                        }
                        string input = string.Format("{0}-{2}-{1} {3}", strTemp[2], strTemp[1], strTemp[0], t);
                        retVal = Convert.ToDateTime(input);
                    }
                }
            }
            catch
            {
                retVal = DateTime.Now;
            }
            return retVal;
        }
        public static DateTime? ToDateTimeHasNull(string obj)
        {
            string[] strArr = obj.Split(' ');
            int lenStrArr = strArr.Length;
            try
            {
                if (lenStrArr >= 1)
                {
                    string str = strArr[0];
                    string[] strTemp = str.Split('/');
                    if (strTemp.Length == 3)
                    {                        
                        string input = string.Format("{2}-{1}-{0}", strTemp[2], strTemp[0], strTemp[1]);
                        return Convert.ToDateTime(input);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }
        public static bool CheckDateTime()
        {
            try
            {
                Convert.ToDateTime("31/12/2014");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
