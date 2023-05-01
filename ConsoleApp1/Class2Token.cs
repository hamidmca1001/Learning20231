
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            var secret = "Rj18Q~QEHIBJHYPOWBarWeFv-vXgz9L8UyKN9b0s";
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
            NewMethod(aadToken?.AccessToken);
        }

        public static void NewMethod(string token)
        {
           // token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IkdiUlAzdWtOUzdscHM5alVJdmFnTEhhcDA3ZkNveUJsRHJ5NzVvSjhabTQiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9lMDNkYzhiMC00NDBhLTRlZDAtYWQ5Yi05MjY1MjM3MTU3MzUvIiwiaWF0IjoxNjgyNjU2Nzc0LCJuYmYiOjE2ODI2NTY3NzQsImV4cCI6MTY4MjY2MDcwNSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQTFmQnJQVjFiQ3B5a0RsUUYwdi9wMkw4dTdZYXFicDB2QTdCODlvViswTzFMSGFwZ3NON3hlTmc5MVhscEhKbWhMcjl1SnUzVXBkaTF4d1NqdWpNalBmUGtzTkFuNnI3Q1A5WERtNlg4TjJrPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiYXV0aEZ1bmNUZXN0MTIzIiwiYXBwaWQiOiI5Nzk2ZDUwNy1jY2VkLTQ1M2UtYTI3ZS1jMmQ5N2EyNjRjYzgiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IkgiLCJnaXZlbl9uYW1lIjoiSGFtaWRhbGkiLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIxMDMuMjguMjUzLjEzNyIsIm5hbWUiOiJIYW1pZGFsaS5oIiwib2lkIjoiYjAzNTRiNzUtOTI1Yi00NDJhLTgxNjktOWFmMjM1ZTRhZDZhIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAyODY0NzhEN0MiLCJyaCI6IjAuQVhZQXNNZzk0QXBFMEU2dG01SmxJM0ZYTlFNQUFBQUFBQUFBd0FBQUFBQUFBQUNaQVBzLiIsInNjcCI6ImVtYWlsIG9wZW5pZCBwcm9maWxlIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoiTDlZbC1IT0x3NDRsdEs2SjVlNlpWdUNvRFRwZzBpLWlVOWIycGtyV1o1QSIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJOQSIsInRpZCI6ImUwM2RjOGIwLTQ0MGEtNGVkMC1hZDliLTkyNjUyMzcxNTczNSIsInVuaXF1ZV9uYW1lIjoiaGFtaWRhbGkuaEB1c2Vyc21zZnR0ZWxjby5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJoYW1pZGFsaS5oQHVzZXJzbXNmdHRlbGNvLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6InE3bk15SVVuREU2MlMyMU9OMUdsQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoiUUlkNUk0LUhveXg4eTFUVDc1UGMtVEQ4cnlCbDJwVDdtcnF4aERhZ2FoMCJ9LCJ4bXNfdGNkdCI6MTY1MjQ3MTE5OX0.JxrSjox6hmEuuzA8zttSIDWVUlLK9UDuO9BY_1fuVISgYxHAhxlGnBgKYCa01ZH7d1pYUhzyHZb_iH5TSPvArh_jxGHreEp8YkrnwS_6xN5fsQ7YZKmPsSZqCnbgbgfYz_OQh_Idqlod8FZNeaAAI3Jt_Vne45bEOltwIwUIgjiQtz-lr2ECdRSlcIdInu9iNiK9f1nWuSz95UKTXexeIfhhxtAZ8mBuZCrITUQsA-yHorLMP7CE0WhniPJeuCJBe656RukST2TF4ThcKx2SY7c7y-dfq08KWpH-c2qZ4iqp0hNVOLvbiKZRShAK1IwRkcjnNB_z1r5fnpZBrPtT-g";
            var azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = client.GetAsync("_apis/projects").Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\tSuccesful REST call");
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                }
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
}
