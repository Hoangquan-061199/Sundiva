using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;

namespace Website.Utils
{
    public class GoogleReCaptchaValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Lazy<ValidationResult> errorResult = new(() => new ValidationResult("Google reCAPTCHA validation failed", new string[] { validationContext.MemberName }));
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult.Value;
            }
            string reCaptchResponse = value.ToString();
            string reCaptchaSecret = WebConfig.SecretKeyCaptchaGoogle;
            HttpClient httpClient = new();
            HttpResponseMessage httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchResponse}").Result;
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return errorResult.Value;
            }
            string jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData = JObject.Parse(jsonResponse);
            if (jsonData.success != true.ToString().ToLower())
            {
                return errorResult.Value;
            }
            return ValidationResult.Success;
        }
    }
}