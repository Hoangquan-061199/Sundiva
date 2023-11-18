using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ADCOnline.Utils
{
    public static class ListHelper
    {
        public static string[] GetValuesArrayKeyWord(string ltsSourceValues)
        {
            string[] ltsValues;
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                if (ltsSourceValues.Contains(' '))
                    ltsValues = ltsSourceValues.Split(new Char[] { ' ' }).Select(x => (!string.IsNullOrEmpty(x) && x != null && x != "") ? x.Trim().ToLower() : "").ToArray();
                else
                    ltsValues = new string[] { ltsSourceValues };
                return ltsValues;
            }
            return ltsValues = Array.Empty<string>();
        }
        public static List<int> GetValuesArrayBySymbol(string ltsSourceValues, char symbol)
        {
            List<int> ltsValues = new();
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                //var arrVal=LtsSourceValues.Split(',');
                if (ltsSourceValues.Contains(symbol))
                    //foreach(var item in arrVal){
                    //    if(!string.IsNullOrEmpty(item)){
                    //        ltsValues.Add(int.Parse(item));
                    //    }
                    //}
                    ltsValues = ltsSourceValues.Split(symbol).Select(o => Convert.ToInt32(!string.IsNullOrEmpty(o.Trim()) ? (o.Trim()) : "0")).ToList();
                else
                    ltsValues.Add(Convert.ToInt32(ltsSourceValues));
            }
            return ltsValues;
        }
        public static List<string> GetValuesArrayTagBySymbol(string symbol, string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            string lts = ltsSourceValues.Replace("" + symbol + " ", "" + symbol + "");
            if (lts.Contains("" + symbol + ""))
            {
                ltsValues = Regex.Split(lts, "" + symbol + "").ToList();
            }
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }
        public static List<int> GetEditValuesArrayTag(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                if (ltsSourceValues.Contains(',') || ltsSourceValues.Contains(' '))
                    ltsValues = Regex.Split(ltsSourceValues, ",").ToList();
                else
                    ltsValues.Add(Convert.ToString(ltsSourceValues));
            }
            List<int> newListInt = new();
            ltsValues.ForEach(f =>
            {
                int temp = Convert.ToInt32(f);
                if (!newListInt.Contains(temp))
                    newListInt.Add(temp);
            });
            return newListInt;
        }

        public static List<int> GetEditValuesArrayTagNews(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                if (ltsSourceValues.Contains(','))
                    ltsValues = Regex.Split(ltsSourceValues, ",").ToList();
                else
                    ltsValues.Add(Convert.ToString(ltsSourceValues));
            }
            List<int> newListInt = new();
            ltsValues.ForEach(f =>
            {
                int temp = Convert.ToInt32(f);
                if (!newListInt.Contains(temp))
                    newListInt.Add(temp);
            });
            return newListInt;
        }
        public static List<string> GetValuesArrayTag(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            string lts = ltsSourceValues.Replace(", ", ",");
            if (lts.Contains(','))
            {
                ltsValues = Regex.Split(lts, ",").ToList();
            }
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }
        public static List<string> GetValuesArrayTag2(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            string lts = ltsSourceValues.Replace("  ", " ");
            if (lts.Contains(' '))
            {
                ltsValues = Regex.Split(lts, " ").ToList();
            }
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }
        public static List<string> GetValuesArraAttr(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            string lts = ltsSourceValues.Replace("| ", "|");
            if (lts.Contains('|'))
            {
                ltsValues = Regex.Split(lts, "|").ToList();
            }
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }
        public static List<string> GetValuesArraAttrOption(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            string lts = ltsSourceValues.Replace("$ ", "$");
            if (lts.Contains('$'))
            {
                ltsValues = Regex.Split(lts, "$").ToList();
            }
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }

        public static List<int> GetValuesArray(string ltsSourceValues)
        {
            List<int> ltsValues = new();
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                //var arrVal=LtsSourceValues.Split(',');
                if (ltsSourceValues.Contains(','))
                    //foreach(var item in arrVal){
                    //    if(!string.IsNullOrEmpty(item)){
                    //        ltsValues.Add(int.Parse(item));
                    //    }
                    //}
                    ltsValues = ltsSourceValues.Split(',').Select(o => Convert.ToInt32(!string.IsNullOrEmpty(o.Trim()) ? (o.Trim()) : "0")).ToList();
                else
                    ltsValues.Add(Convert.ToInt32(ltsSourceValues));
            }
            return ltsValues;
        }
        public static List<string> GetValuesArrayString(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                //var arrVal=LtsSourceValues.Split(',');
                if (ltsSourceValues.Contains(','))
                    //foreach(var item in arrVal){
                    //    if(!string.IsNullOrEmpty(item)){
                    //        ltsValues.Add(int.Parse(item));
                    //    }
                    //}
                    ltsValues = ltsSourceValues.Split(',').Select(o => !string.IsNullOrEmpty(o.Trim()) ? (o.Trim()) : "0").ToList();
                else
                    ltsValues.Add(ltsSourceValues);
            }
            return ltsValues;
        }
        public static int[] GetIdsArray(string ltsSourceValues)
        {
            int[] ltsValues = new int[100];
            if (!string.IsNullOrEmpty(ltsSourceValues))
            {
                if (ltsSourceValues.Contains(','))
                    ltsValues = ltsSourceValues.Split(',').Select(o => Convert.ToInt32(!string.IsNullOrEmpty(o.Trim()) ? (o.Trim()) : "0")).ToArray();
                else
                    ltsValues[0] = (Convert.ToInt32(ltsSourceValues));
            }
            return ltsValues;
        }

        public static List<int> GetValuesArrayCheckEmpty(string LtsSourceValues)
        {
            List<int> ltsValues = new();
            if (!string.IsNullOrEmpty(LtsSourceValues))
            {
                if (LtsSourceValues.Contains(','))
                    ltsValues = LtsSourceValues.Split(',').Where(c => c != "" && c != null).Select(o => Convert.ToInt32(o)).ToList();
                else
                    ltsValues.Add(Convert.ToInt32(LtsSourceValues));
            }
            return ltsValues;
        }

        public static List<string> GetValuesArrayForTag(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            if (ltsSourceValues.Contains(',') && ltsSourceValues.Contains(' '))
                ltsValues = Regex.Split(ltsSourceValues, ", ").Select(Convert.ToString).ToList();
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }
        public static List<string> GetValuesArrayForCrawl(string ltsSourceValues)
        {
            List<string> ltsValues = new();
            if (string.IsNullOrEmpty(ltsSourceValues)) return ltsValues;
            if (ltsSourceValues.Contains(':') && ltsSourceValues.Contains(' '))
                ltsValues = Regex.Split(ltsSourceValues, ":").Select(Convert.ToString).ToList();
            else
                ltsValues.Add(Convert.ToString(ltsSourceValues));
            return ltsValues;
        }

    }
}
