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
        internal static string aadInstance = "https://login.microsoftonline.com/{0}/v2.0"; //ConfigurationManager.AppSettings["ida:AADInstance"];
        internal static string tenant = "e03dc8b0-440a-4ed0-ad9b-926523715735";//ConfigurationManager.AppSettings["ida:Tenant"];
        internal static string clientId = "9796d507-cced-453e-a27e-c2d97a264cc8"; //ConfigurationManager.AppSettings["ida:ClientId"];
        internal static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = "https://dev.azure.com/HCLGreenSoftware/"; //ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        internal static string[] scopes = new string[] { "9796d507-cced-453e-a27e-c2d97a264cc8/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  

        // MSAL Public client app
       // private static IPublicClientApplication application;

      
        /// <summary>
        /// Get all projects in the organization that the authenticated user has access to and print the results.
        /// </summary>
        /// <param name="authHeader"></param>
        public static void ListProjects1(string token, StringBuilder sb)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                   Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", token))));

                //client.DefaultRequestHeaders.Add("Bearer", token);
                //.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = client.GetAsync("_apis/projects").Result;

                if (response.IsSuccessStatusCode)
                {
                    sb.AppendLine($"Succesful REST call:-, Status: {response.IsSuccessStatusCode}  , StatusCode : {response.StatusCode}");

                    Console.WriteLine("\tSuccesful REST call");
                    var result = response.Content.ReadAsStringAsync().Result;


                    Console.WriteLine(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }


                // use the httpclient
             //   using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    client.DefaultRequestHeaders.Add("User-Agent", "authFuncTest123");
            //    // client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
            //    // client.DefaultRequestHeaders.Add("Authorization", authHeader);
            //    // client.DefaultRequestHeaders.Add("Authorization", authHeader);
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);


            //    // connect to the REST endpoint            
            //    HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

            //    // check to see if we have a succesfull respond
            //    if (response.IsSuccessStatusCode)
            //    {
            //        Console.WriteLine("Succesful REST call");
            //        var result = response.Content.ReadAsStringAsync().Result;
            //        Console.WriteLine(result);
            //    }
            //    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //    {
            //        Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
            //        //throw new UnauthorizedAccessException();
            //    }
                else
                {
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                }
            }
        }




        //public static void getauthorizationtoken()
        //{
        //    clientcredential cc =
        //         new microsoft.identitymodel.clients.activedirectory.clientcredential(azuredetails.clientid, azuredetails.clientsecret);
        //    var context = new authenticationcontext("https://login.microsoftonline.com/" + azuredetails.tenantid);
        //    var result = context.acquiretokenasync("https://management.azure.com/", cc);
        //    if (result == null)
        //    {
        //        throw new invalidoperationexception("failed to obtain the access token");
        //    }
        //    azuredetails.accesstoken = result.result.accesstoken;
        //    //"eyj0exaioijkv1qilcjub25jzsi6imz2twzcznpnv1mywkfzby10uxn2nhned1hxqktyefdub0lxx3q3ee9kwleilcjhbgcioijsuzi1niising1dci6ii1lstnrow5oujdium9meg1lwm9ycwjiwkdldyisimtpzci6ii1lstnrow5oujdium9meg1lwm9ycwjiwkdldyj9.eyjhdwqioiiwmdawmdawmy0wmdawltawmdatyzawmc0wmdawmdawmdawmdailcjpc3mioijodhrwczovl3n0cy53aw5kb3dzlm5ldc9lmdnkyzhimc00ndbhltrlzdatywq5yi05mjy1mjm3mtu3mzuviiwiawf0ijoxnjgynjiyotawlcjuymyioje2odi2mji5mdasimv4cci6mty4mjyyode4ocwiywnjdci6mcwiywnyijoimsisimfpbyi6ikfwuufxlzhuqufbqvpduzfpexjodlntcgfdovrjtevzujh6zwy2ctbnkza4y2hmvvjcrlzgq0xpm0njz1bfufdlnstmawhizmjxohurv2p3ruzywxvxn2tpue5pr1fus0dvadlknml3aw1hzflles9tahhgtlpvpsisimftcii6wyjwd2qilcjtzmeixswiyxbwx2rpc3bsyxluyw1lijoiyxv0aez1bmnuzxn0mtiziiwiyxbwawqioii5nzk2zduwny1jy2vkltq1m2utyti3zs1jmmq5n2eynjrjyzgilcjhchbpzgfjcii6ijeilcjmyw1pbhlfbmftzsi6ikgilcjnaxzlbl9uyw1lijoisgftawrhbgkilcjpzhr5cci6invzzxiilcjpcgfkzhiioiixmdmumjgumjuzljeznyisim5hbwuioijiyw1pzgfsas5oiiwib2lkijoiyjazntrinzutoti1yi00ndjhltgxnjktowfmmjm1ztrhzdzhiiwicgxhdgyioiiziiwichvpzci6ijewmdmymdayody0nzhen0milcjyaci6ijauqvhzqxnnzzk0qxbfmeu2dg01smxjm0zytlfnqufbqufbqufbd0fbqufbqufbqunaqvbzliisinnjcci6imvtywlsig9wzw5pzcbwcm9mawxliiwic2lnbmlux3n0yxrlijpbimttc2kixswic3viijoitdlzbc1it0x3ndrsdes2sjvlnlpwdunvrfrwzzbplwlvowiycgtyv1o1qsisinrlbmfudf9yzwdpb25fc2nvcguioijoqsisinrpzci6imuwm2rjogiwltq0mgetngvkmc1hzdliltkynjuymzcxntcznsisinvuaxf1zv9uyw1lijoiagftawrhbgkuaeb1c2vyc21zznr0zwxjby5vbm1py3jvc29mdc5jb20ilcj1cg4ioijoyw1pzgfsas5oqhvzzxjzbxnmdhrlbgnvlm9ubwljcm9zb2z0lmnvbsisinv0asi6iknhtmi1yvvlbkvpznyywvfsuwlpqueilcj2zxiioiixljailcj3awrzijpbimi3owzizjrkltnlzjktndy4os04mtqzltc2yje5ngu4ntuwosjdlcj4bxnfc3qionsic3viijoiuulknuk0luhvexg4etfuvdc1ugmtveq4cnlcbdjwvddtcnf4aerhz2fomcj9lcj4bxnfdgnkdci6mty1mjq3mte5ox0.muzfa3k3muweklotgin7sym69im7xtpe-onvr-oxacib0alh4o0qhszf6p6xexnsssuusf603mtadxed996byqeuwv7tfscezxetwph8rzh4clzsxjat0n15xt_lk9lvp0izijztponbntpkezh7gideme59hkajosyymgkkqujcybxmzxeznypfngjoisixv-cgdmtej-b7g6vizucgqdk7vliruwzubqzin3fnsgvijmeeherwabyhoyc5jxtwjpboqpwvdnwa3ggc1f6-epxt7wehu6msezal3zcgqxse5jvdp2rylydj4dg86-l4zjgwyvscpgbecfddi0hzxw";
        //    try
        //    {
        //        listprojects1(azuredetails.accesstoken);
        //    }
        //    catch (exception ex)
        //    {

        //        throw;
        //    }

        //}


        private async static void ListProjects(string authHeader)
        {
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


        public static string ProjectListResponse { get;  set; }
    }
}


