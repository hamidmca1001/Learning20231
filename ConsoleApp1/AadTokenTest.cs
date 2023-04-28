using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ConsoleApp1
{
    internal class AadTokenTest
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Tenant is the name or Id of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //
        internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0"; //ConfigurationManager.AppSettings["ida:AADInstance"];
        internal static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";//ConfigurationManager.AppSettings["ida:Tenant"];
        internal static string clientId = "9796d507-cced-453e-a27e-c2d97a264cc8"; //ConfigurationManager.AppSettings["ida:ClientId"];
        internal static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware/"; //ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        internal static string[] scopes = new string[] { "9796d507-cced-453e-a27e-c2d97a264cc8/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

        // MSAL Public client app
        private static IPublicClientApplication application;

        public static async Task TestToken()
        {

            try
            {
                var authResult = await SignInUserAndGetTokenUsingMSAL(scopes);

                // Create authorization header of the form "Bearer {AccessToken}"
                var authHeader = authResult.CreateAuthorizationHeader();

                ListProjects(authHeader);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong.");
                Console.WriteLine("Message: " + ex.Message + "\n");
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Sign-in user using MSAL and obtain an access token for Azure DevOps
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns>AuthenticationResult</returns>
        private static async Task<Microsoft.Identity.Client.AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId)
                                       .WithAuthority(authority)
                                       .WithDefaultRedirectUri()
                                       .Build();

            Microsoft.Identity.Client.AuthenticationResult result;

            try
            {
                var accounts = await application.GetAccountsAsync();
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // If the token has expired, prompt the user with a login prompt
                result = await application.AcquireTokenInteractive(scopes)
                        .WithClaims(ex.Claims)
                        .ExecuteAsync();
            }
            return result;
        }

        /// <summary>
        /// Get all projects in the organization that the authenticated user has access to and print the results.
        /// </summary>
        /// <param name="authHeader"></param>
        private static void ListProjects1(string authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Add("Authorization", authHeader);

                // connect to the REST endpoint
                // var uri = azureDevOpsOrganizationUrl + "_apis/projects?api-version=6.0";
                //HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;
                HttpResponseMessage response = client.GetAsync("_apis/projects?api-version=6.0").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Succesful REST call");
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




        public static void GetAuthorizationToken()
        {
            Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential cc =
                new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(AzureDetails.ClientID, AzureDetails.ClientSecret);
            var context = new AuthenticationContext("https://login.microsoftonline.com/" + AzureDetails.TenantID);
            var result = context.AcquireTokenAsync("https://management.azure.com/", cc);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the Access token");
            }
            AzureDetails.AccessToken = result.Result.AccessToken;

            try
            {
                ListProjects(AzureDetails.AccessToken);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }


        private async static void ListProjects(string authHeader)
        {
            authHeader = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IjNaUHNPQzJJeEtwWWQtVDBnWlVTUkRWMVVzSERLUE53WGRRdm9KMmlFOUUiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9lMDNkYzhiMC00NDBhLTRlZDAtYWQ5Yi05MjY1MjM3MTU3MzUvIiwiaWF0IjoxNjgyNTkzOTIzLCJuYmYiOjE2ODI1OTM5MjMsImV4cCI6MTY4MjU5ODgxOSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQWQySVphZkthYmJZSmI4L3BqcGoyVmxaNHRYRURZUlVHNmFCeVNzU3prd3VmNmEvc0Y4RVNGTUpYN1o4d0VZN0dScHdCU08yU3p2UDdzeHBsUE0wL0Q5KzErc3IxSVFUajhubDlOSzNrTm9zPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiYXV0aEZ1bmNUZXN0MTIzIiwiYXBwaWQiOiI5Nzk2ZDUwNy1jY2VkLTQ1M2UtYTI3ZS1jMmQ5N2EyNjRjYzgiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IkgiLCJnaXZlbl9uYW1lIjoiSGFtaWRhbGkiLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIxMDMuMjguMjUzLjEzNyIsIm5hbWUiOiJIYW1pZGFsaS5oIiwib2lkIjoiYjAzNTRiNzUtOTI1Yi00NDJhLTgxNjktOWFmMjM1ZTRhZDZhIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAyODY0NzhEN0MiLCJyaCI6IjAuQVhZQXNNZzk0QXBFMEU2dG01SmxJM0ZYTlFNQUFBQUFBQUFBd0FBQUFBQUFBQUNaQVBzLiIsInNjcCI6ImVtYWlsIG9wZW5pZCBwcm9maWxlIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoiTDlZbC1IT0x3NDRsdEs2SjVlNlpWdUNvRFRwZzBpLWlVOWIycGtyV1o1QSIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJOQSIsInRpZCI6ImUwM2RjOGIwLTQ0MGEtNGVkMC1hZDliLTkyNjUyMzcxNTczNSIsInVuaXF1ZV9uYW1lIjoiaGFtaWRhbGkuaEB1c2Vyc21zZnR0ZWxjby5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJoYW1pZGFsaS5oQHVzZXJzbXNmdHRlbGNvLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6InYxRUE3eFc2YlV5UjNOMzlmY04zQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoiUUlkNUk0LUhveXg4eTFUVDc1UGMtVEQ4cnlCbDJwVDdtcnF4aERhZ2FoMCJ9LCJ4bXNfdGNkdCI6MTY1MjQ3MTE5OX0.jvzkDL_KiR-fcwiruP4vxMbP901yGGgcvv0hi0zYfQTqAA9_pqUPCWaZ-1v6lfsnlAEPg88yYQa1Iat6AwQdGpF7YK7yM26cpyXRAN0KyFUde7js2pR52E9SA0n2m32BWvYj6ViTcn-4NYLfWkxZcXdGxVjQL6QKBlAjNDnDogZZWimP-olcU6q1nodwrZIPuYVM1tDy0G4-3A8KJ5VKSCPnHbvu6KcBI7z_US47aoGVOYOrhIOpMxG1nAG14UTC3OIud-8UZWsBaheRnQ7L5cXpmLgGLcJlhHiD-pHkeBE7Etw7QB92bMVw4Bs06Z_YMftn3xkMZrbE-a53ojlqEg";
            var client = GetClient(authHeader);
            var uri = azureDevOpsOrganizationUrl + "_apis/projects?api-version=6.0";


            try
            {
                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                    response.EnsureSuccessStatusCode();
                    var result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        private static HttpClient GetClient(string tkn)
        {
            // Create and initialize HttpClient instance.
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", tkn))));

            return client;
        }
    }







    public static class AzureDetails
    {
        //internal static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";//ConfigurationManager.AppSettings["ida:Tenant"];
        //internal static string clientId = "9796d507-cced-453e-a27e-c2d97a264cc8"; //ConfigurationManager.AppSettings["ida:ClientId"];

        public static string ClientID = "9796d507-cced-453e-a27e-c2d97a264cc8";
        public static string ClientSecret = "Kyu8Q~UWF32Nfkh5UbYnATODrMeb27NkwgjeFamO";
        public static string TenantID = "e03dc8b0-440a-4ed0-ad9b-926523715735";
        public static string AccessToken { get; internal set; }
    }
}
