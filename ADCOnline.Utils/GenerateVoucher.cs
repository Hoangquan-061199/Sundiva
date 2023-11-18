using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ADCOnline.Utils
{
    public class GenerateVoucher
    {
        // public static string GenCode(int length)
        // {
        //     string strCharacters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //     char[] chars = strCharacters.ToCharArray();
        //     byte[] data = new byte[length];
        //     RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
        //     crypto.GetNonZeroBytes(data);
        //     StringBuilder result = new StringBuilder(length);
        //     foreach (byte b in data)
        //     {
        //         result.Append(chars[b % (chars.Length - 1)]);
        //     }
        //     return result.ToString();
        // }
        // public static List<string> CreateVouchers(int amountVoucher, string prefix, string suffix, int length)
        // {
        //     List<string> listVouchers = new List<string>();
        //     int _length = length - (string.IsNullOrEmpty(prefix) ? 0 : prefix.Length) - suffix.Length;
        //     while (amountVoucher > 0)
        //     {
        //         string code = GenCode(_length);
        //         if (!listVouchers.Any(x => x.Equals(code)))
        //         {
        //             string fullCode = prefix + code + suffix;
        //             listVouchers.Add(fullCode);
        //             amountVoucher--;
        //         }
        //     }
        //     return listVouchers;
        // }
    }
}
