using System;
using System.Text;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace FunctionApp1
{
    internal class VSOProject
    {

        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Tenant is the name or Id of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //

        //internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0"; //ConfigurationManager.AppSettings["ida:AADInstance"];
        //internal static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";//ConfigurationManager.AppSettings["ida:Tenant"];
        //internal static string clientId = "6c7667fe-f76a-410f-88ad-09f9d728d839"; //ConfigurationManager.AppSettings["ida:ClientId"];
        //internal static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        ////URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware/"; //ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        //internal static string[] scopes = new string[] { "9796d507-cced-453e-a27e-c2d97a264cc8/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

        // MSAL Public client app
       // private static IPublicClientApplication application;

      
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

                    Console.WriteLine("\tSuccesful REST call");
                    result = response.Content.ReadAsStringAsync().Result;


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


