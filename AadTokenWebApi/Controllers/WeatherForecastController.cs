using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace AadTokenWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0";
        static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";
        static string clientId = "7e6988e4-b5e2-4964-afd4-41b3e70a6001";

        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //URL of your Azure DevOps account.
        string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware";

        string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

        // MSAL Public client app
        private static IPublicClientApplication application;



        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }



        [HttpGet]
        [Route("aad-test")]
        public async Task<ActionResult<WeatherForecast>> AadTokenTest1()
        {
            StringBuilder sb = new StringBuilder();
            var response = new WeatherForecast();
            try
            {
                // response.HttpContextValue = _contextAccessor.HttpContext.Response.Headers.Count().ToString();
                var a1 = _contextAccessor.HttpContext.Response.Headers;
                if (a1.Count > 0)
                {
                    foreach (var item in a1)
                    {
                        sb.Append($"Item Key : {a1.Keys}  Item Value : {a1.Values}");
                    }
                    response.HttpContextValue = response.HttpContextValue + sb.ToString();
                }

                var token = GetToken(sb).Result;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var result = VSOProject.VsoProjectList(token, sb);

                    response.Summary = result;
                    return response;
                }

                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Exception logged : {ex.Message}");
                return BadRequest($"Exception : {ex.Message} , {response}");
            }
            return Ok(response);
        }

        private async Task<string> GetToken(StringBuilder sb)
        {
            sb.AppendLine("Token Calling.");
            //  log.LogInformation("Token Calling.");
            var authResult = await SignInUserAndGetTokenUsingMSAL(scopes, sb);

            if (authResult != null)
            {
                sb.AppendLine($"authResult {authResult}");
            }
            else
            {
                sb.AppendLine($"authResult is null");
            }
            // Create authorization header of the form "Bearer {AccessToken}"
            var authHeader = authResult.CreateAuthorizationHeader();

            return authHeader;
        }

        /// <summary>
        /// Sign-in user using MSAL and obtain an access token for Azure DevOps
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns>AuthenticationResult</returns>
        private static async Task<AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes, StringBuilder sb)
        {
            AuthenticationResult result = null;

            // Initialize the MSAL library by building a public client application
            //application = PublicClientApplicationBuilder.Create(clientId)
            //                            .WithAuthority(authority)
            //                            .WithDefaultRedirectUri()
            //                            .Build();

            application = PublicClientApplicationBuilder.Create(clientId)
                                                .WithAuthority(authority)
                                                .WithRedirectUri("api://7e6988e4-b5e2-4964-afd4-41b3e70a6001")
                                               // .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                                              // .WithRedirectUri("https://aadwebservicehamid.azurewebsites.net")
                                            .Build();


            try
            {
                // log.LogInformation($"Application Called: Initialize the MSAL library by building a public client application");
                // log.LogInformation($"RedirectUri : {application.AppConfig.RedirectUri}");
                //log.LogInformation($"ClientId : {application.AppConfig.ClientId}");

                sb.AppendLine($"RedirectUri : {application.AppConfig.RedirectUri}");
                sb.AppendLine($"ClientId : {application.AppConfig.ClientId}");

                var accounts = await application.GetAccountsAsync();

                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                            .ExecuteAsync();

                return result;


            }
            catch (MsalUiRequiredException ex)
            {

                sb.AppendLine($"Exception throw in  msft redirect authentication : {ex.Message}");

                result = await application.AcquireTokenInteractive(scopes)
                          .WithClaims(ex.Claims)
                          .ExecuteAsync();

                sb.AppendLine($"Printing Result : {result}");
                // log.LogInformation($"Printing Result : {result}");

                Console.WriteLine(ex.Message);

            }

            return result;
        }
    }



    internal class VSOProject
    {
        ////URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware/"; //ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        /// <summary>
        /// Get all projects in the organization that the authenticated user has access to and print the results.
        /// </summary>
        /// <param name="authHeader"></param>
        public static string VsoProjectList(string token, StringBuilder sb)
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", token);

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?api-version=6.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    sb.AppendLine($"Succesful REST call:-, Status: {response.IsSuccessStatusCode}  , StatusCode : {response.StatusCode}");

                    // log.LogInformation($"Succesful REST call:-, Status: {response.IsSuccessStatusCode}  , StatusCode : {response.StatusCode}");
                    Console.WriteLine("\tSuccesful REST call");
                    result = response.Content.ReadAsStringAsync().Result;

                    // log.LogInformation(result);
                    sb.AppendLine(result);
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

            return result;
        }

    }
}