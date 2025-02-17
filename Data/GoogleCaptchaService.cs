using System;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Data;

public class GoogleCaptchaService
{
    public virtual async Task<GoogleCaptchaResponse?> Verify(string token)
    {
        GoogleCaptchaResponse? reCaptchaResponse;
        using(var httpClient = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("secret", GoogleCaptchaSettings.SecretKey),
                new KeyValuePair<string, string>("response", token)
            });
            try 
            {
                var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var jsonString = await response.Content.ReadAsStringAsync();
                reCaptchaResponse = JsonSerializer.Deserialize<GoogleCaptchaResponse>(jsonString);
            }
            catch (Exception ex)
            {
                reCaptchaResponse = null;
                throw;
            }

            return reCaptchaResponse;
        }
    }
}
