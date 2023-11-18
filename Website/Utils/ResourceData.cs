using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace Website.Utils
{
    public class ResourceData
    {
        public static string Resource(string code, string lang)
        {
            Dictionary<string, string> Json = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + lang + ".json", "DataJson/Resource"));
            if (Json != null && Json.ContainsKey(code))
            {
                return Json[code];
            }
            return "[" + code + "]";
        }
        private static string ReadFile(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {

            }
            return fileContent;
        }
    }
}
