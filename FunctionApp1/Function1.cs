using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;

namespace FunctionApp1
{
    public static class Function1
    {
        public static object AadTokenTest { get; private set; }

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            var sb = new StringBuilder();
            try
            {
               // var token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IkdiUlAzdWtOUzdscHM5alVJdmFnTEhhcDA3ZkNveUJsRHJ5NzVvSjhabTQiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9lMDNkYzhiMC00NDBhLTRlZDAtYWQ5Yi05MjY1MjM3MTU3MzUvIiwiaWF0IjoxNjgyNjU2Nzc0LCJuYmYiOjE2ODI2NTY3NzQsImV4cCI6MTY4MjY2MDcwNSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQTFmQnJQVjFiQ3B5a0RsUUYwdi9wMkw4dTdZYXFicDB2QTdCODlvViswTzFMSGFwZ3NON3hlTmc5MVhscEhKbWhMcjl1SnUzVXBkaTF4d1NqdWpNalBmUGtzTkFuNnI3Q1A5WERtNlg4TjJrPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiYXV0aEZ1bmNUZXN0MTIzIiwiYXBwaWQiOiI5Nzk2ZDUwNy1jY2VkLTQ1M2UtYTI3ZS1jMmQ5N2EyNjRjYzgiLCJhcHBpZGFjciI6IjEiLCJmYW1pbHlfbmFtZSI6IkgiLCJnaXZlbl9uYW1lIjoiSGFtaWRhbGkiLCJpZHR5cCI6InVzZXIiLCJpcGFkZHIiOiIxMDMuMjguMjUzLjEzNyIsIm5hbWUiOiJIYW1pZGFsaS5oIiwib2lkIjoiYjAzNTRiNzUtOTI1Yi00NDJhLTgxNjktOWFmMjM1ZTRhZDZhIiwicGxhdGYiOiIzIiwicHVpZCI6IjEwMDMyMDAyODY0NzhEN0MiLCJyaCI6IjAuQVhZQXNNZzk0QXBFMEU2dG01SmxJM0ZYTlFNQUFBQUFBQUFBd0FBQUFBQUFBQUNaQVBzLiIsInNjcCI6ImVtYWlsIG9wZW5pZCBwcm9maWxlIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoiTDlZbC1IT0x3NDRsdEs2SjVlNlpWdUNvRFRwZzBpLWlVOWIycGtyV1o1QSIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJOQSIsInRpZCI6ImUwM2RjOGIwLTQ0MGEtNGVkMC1hZDliLTkyNjUyMzcxNTczNSIsInVuaXF1ZV9uYW1lIjoiaGFtaWRhbGkuaEB1c2Vyc21zZnR0ZWxjby5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJoYW1pZGFsaS5oQHVzZXJzbXNmdHRlbGNvLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6InE3bk15SVVuREU2MlMyMU9OMUdsQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfc3QiOnsic3ViIjoiUUlkNUk0LUhveXg4eTFUVDc1UGMtVEQ4cnlCbDJwVDdtcnF4aERhZ2FoMCJ9LCJ4bXNfdGNkdCI6MTY1MjQ3MTE5OX0.JxrSjox6hmEuuzA8zttSIDWVUlLK9UDuO9BY_1fuVISgYxHAhxlGnBgKYCa01ZH7d1pYUhzyHZb_iH5TSPvArh_jxGHreEp8YkrnwS_6xN5fsQ7YZKmPsSZqCnbgbgfYz_OQh_Idqlod8FZNeaAAI3Jt_Vne45bEOltwIwUIgjiQtz-lr2ECdRSlcIdInu9iNiK9f1nWuSz95UKTXexeIfhhxtAZ8mBuZCrITUQsA-yHorLMP7CE0WhniPJeuCJBe656RukST2TF4ThcKx2SY7c7y-dfq08KWpH-c2qZ4iqp0hNVOLvbiKZRShAK1IwRkcjnNB_z1r5fnpZBrPtT-g";
                // VSOProject.GetAuthorizationToken();
                var token = req.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    sb.AppendLine("Token Receied.");
                  
                    VSOProject.VsoProjectList(token, sb);

                    log.LogInformation("Token Receied.");
                }
                else
                {
                    sb.AppendLine("No Token found to get the project list");
                }

                if (req.Headers.Count > 0)
                {
                    sb.AppendLine($"Headers Count:  { req.Headers.Count}");
                    foreach (var item in req.Headers)
                    {
                        sb.AppendLine($"{item.Key} : {item.Value}");
                    }
                }


                var identity = req.HttpContext?.User?.Identity as ClaimsIdentity;
                sb.AppendLine($"IsAuthenticated: {identity?.IsAuthenticated}");
                sb.AppendLine($"Identity name: {identity?.Name}");
                sb.AppendLine($"AuthenticationType: {identity?.AuthenticationType}");
                foreach (var claim in identity?.Claims)
                {
                    sb.AppendLine($"Claim: {claim.Type} : {claim.Value}");
                }
                //sb.AppendLine($"Token:  { AzureDetails.AccessToken}");
                //sb.AppendLine($"ProjectList Response: {AzureDetails.ProjectListResponse}");

            }



            catch (Exception ex)
            {
                sb.AppendLine($"Error Message in SB: {ex.Message}");
                log.LogInformation("Error Message : {0}", ex.Message);
            }

            return new OkObjectResult(sb.ToString());

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);
        }
    }
}
