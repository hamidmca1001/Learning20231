
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Class2Token
    {
        public static async void M1()
        {
        //     public static string ClientID = "9796d507-cced-453e-a27e-c2d97a264cc8";
        //public static string ClientSecret = "Kyu8Q~UWF32Nfkh5UbYnATODrMeb27NkwgjeFamO";
        //public static string TenantID = "e03dc8b0-440a-4ed0-ad9b-926523715735";

        var tenantId = "e03dc8b0-440a-4ed0-ad9b-926523715735";
            var clientId = "9796d507-cced-453e-a27e-c2d97a264cc8";
            var secret = "Kyu8Q~UWF32Nfkh5UbYnATODrMeb27NkwgjeFamO";
            var resourceUrl = "https://management.azure.com/";
            var requestUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/token";

            // in real world application, please use Typed HttpClient from ASP.NET Core DI
            var httpClient = new HttpClient();

            var dict = new Dictionary<string, string>
                            {
                                { "grant_type", "client_credentials" },
                                { "client_id", clientId },
                                { "client_secret", secret },
                                { "resource", resourceUrl }
                            };

            var requestBody = new FormUrlEncodedContent(dict);
            var response = await httpClient.PostAsync(requestUrl, requestBody);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var aadToken = JsonSerializer.Deserialize<AzureADToken>(responseContent);

            Console.WriteLine(aadToken?.AccessToken);
        }
    }


    public class AzureADToken
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonPropertyName("ext_expires_in")]
        public string ExtExpiresIn { get; set; }

        [JsonPropertyName("expires_on")]
        public string ExpiresOn { get; set; }

        [JsonPropertyName("not_before")]
        public string NotBefore { get; set; }

        public string Resource { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
