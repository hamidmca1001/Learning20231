using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.Identity.Client;
using System.Linq;
using System.Globalization;
using System.Text;

namespace FunctionApp1
{
    public class AadTokenFunctionTest1
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Tenant is the name or Id of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //
        internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0";  //ConfigurationManager.AppSettings["ida:AADInstance"];
        internal static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";      //ConfigurationManager.AppSettings["ida:Tenant"];
        internal static string clientId = "80836f79-d00c-4f46-a789-89b276a0f82e";     //ConfigurationManager.AppSettings["ida:ClientId"];
        internal static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware"; //ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        internal static string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

        //api://9796d507-cced-453e-a27e-c2d97a264cc8/user_impersonation

        // MSAL Public client app
        private static IPublicClientApplication application;


        [FunctionName("AadTokenFunctionTest1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var sb = new StringBuilder();

            var tokenValue = GetToken(log).Result;
            if (!string.IsNullOrWhiteSpace(tokenValue))
            {
                log.LogInformation("Token Receied.");
                sb.AppendLine("Token Receied.");

                var vsoProjectList = VSOProject.VsoProjectList(tokenValue, sb);

                if (!string.IsNullOrWhiteSpace(vsoProjectList))
                {
                    sb.AppendLine(vsoProjectList);
                    log.LogInformation("Project List.");
                }
            }
            else
            {
                sb.AppendLine("No Token found to get the project list");
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(sb);
        }

        public async Task<string> GetToken(ILogger log)
        {
            log.LogInformation("Token Calling.");
            var authResult = await SignInUserAndGetTokenUsingMSAL(scopes, log);

            // Create authorization header of the form "Bearer {AccessToken}"
            var authHeader = authResult.CreateAuthorizationHeader();

            return authHeader;
        }

        /// <summary>
        /// Sign-in user using MSAL and obtain an access token for Azure DevOps
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns>AuthenticationResult</returns>
        private static async Task<AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes, ILogger log)
        {
            AuthenticationResult result = null;

            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId)
                                       .WithAuthority(authority)
                                       .WithDefaultRedirectUri()
                                       .Build();

            try
            {
                var accounts = await application.GetAccountsAsync();

                log.LogInformation($"SignInUserAndGetTokenUsingMSAL Calling. : {accounts.Count()}");
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    log.LogInformation($"checking for MSFT Login.");
                    
                        // If the token has expired, prompt the user with a login prompt
                        result = await application.AcquireTokenInteractive(scopes)
                                .WithClaims(ex.Claims)
                                .ExecuteAsync();

                    if (result != null)
                    {
                        log.LogInformation($"result : {result.Account}");
                        log.LogInformation($"After MSFT Response. {result.IdToken}");
                    }
                    else
                    {
                        log.LogInformation($"Result received null");
                    }
                    return result;
                }
                catch (Exception ex1)
                {
                    log.LogInformation($"Exception : {ex1.Message}");
                    Console.WriteLine(ex1);
                }
            }

            return result;
        }

    }



}
