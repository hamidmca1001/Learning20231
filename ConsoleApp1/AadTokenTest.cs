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
using System.Diagnostics;

namespace ConsoleApp1
{
    internal class AadTokenTest
    {
        //var tenantId = "e03dc8b0-440a-4ed0-ad9b-926523715735";
        //var clientId = "d676a254-3ec1-4749-94af-442fd836ad14";
        //var secret = "BJe8Q~2PtOi80EwtVhIB7DgSZwTedCwcPJ9fdawI";
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

        internal static string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

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
            authHeader = "AWQAm/8TAAAA4pV6g/fnlu5xULE16epNLdiUgIViaVGHC+W1P1n9ZoF+twaSgq8ziYrmmBfNsJiymLg+navmLRDAbuPXXyuuRvATX07DdiYr+B7GVyR+G6+7xch4XoIi5nFRc0o9ZzH3";
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



        public static async Task MSFTTest()
        {
            ListProjects("ff");

            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId)
                                       .WithAuthority(authority)
                                       .WithDefaultRedirectUri()
                                       .Build();

            var accounts = await application.GetAccountsAsync();

            Microsoft.Identity.Client.AuthenticationResult result = null;
            try
            {
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    result = await application.AcquireTokenInteractive(scopes)
                                      .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                  var a1 = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
               var a2 = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                return;
            }

            if (result != null)
            {
                string accessToken = result.AccessToken;
                // Use the token
            }
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
