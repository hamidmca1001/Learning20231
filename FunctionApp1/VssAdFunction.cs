using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class VssAdFunction
    {
        public const string AdoBaseUrl = "https://dev.azure.com";

        public const string AdoOrgName = "Your organization name";

        public const string AadTenantId = "Your Azure AD tenant id";
        // ClientId for User Assigned Managed Identity. Leave null for System Assigned Managed Identity
        public const string AadUserAssignedManagedIdentityClientId = null;

        // Credentials object is static so it can be reused across multiple requests. This ensures
        // the internal token cache is used which reduces the number of outgoing calls to Azure AD to get tokens.
        // 
        // DefaultAzureCredential will use VisualStudioCredentials or other appropriate credentials for local development
        // but will use ManagedIdentityCredential when deployed to an Azure Host with Managed Identity enabled.
        // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential
        private readonly static TokenCredential credential =
            new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    TenantId = AadTenantId,
                    ManagedIdentityClientId = AadUserAssignedManagedIdentityClientId,
                    ExcludeEnvironmentCredential = true // Excluding because EnvironmentCredential was not using correct identity when running in Visual Studio
                });

        public static List<ProductInfoHeaderValue> AppUserAgent { get; } = new()
        {
            new ProductInfoHeaderValue("Identity.ManagedIdentitySamples", "1.0"),
            new ProductInfoHeaderValue("(3-AzureFunction-ManagedIdentity)")
        };




        [FunctionName("VssAdFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

           

            try
            {
                
                var responseMessage = $"Work item  This HTTP triggered function executed successfully.";
                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
        }

        
        //public static async Task<string> GetManagedIdentityAccessToken()
        //{
        //    var tokenRequestContext = new TokenRequestContext(new scope);
        //    var token = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);

        //    return token.Token;
        //}
    }
}
