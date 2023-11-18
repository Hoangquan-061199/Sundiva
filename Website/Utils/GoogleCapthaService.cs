using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net;
using Newtonsoft.Json;

namespace Website.Utils
{
    public class GoogleCapthaService
    {
        private readonly IOptionsMonitor<GoogleCaptchaConfig> _config;
        public GoogleCapthaService(IOptionsMonitor<GoogleCaptchaConfig> config)
        {
            _config = config;
        }
        public async Task<bool> VerifyToken(string token)
        {
            try{
                string url = $"https://www.google.com/recaptcha/api/siteverify?secret={_config.CurrentValue.SecretKey}&response={token}";
                using (var client = new HttpClient())
                {
                    var httpResult = await client.GetAsync(url);
                    if(httpResult.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }
                    var responseString = await httpResult.Content.ReadAsStringAsync();
                    var googleResult = JsonConvert.DeserializeObject<GoogleCaptchaResponse>(responseString);
                    return googleResult.success && googleResult.score >= 0.5;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
