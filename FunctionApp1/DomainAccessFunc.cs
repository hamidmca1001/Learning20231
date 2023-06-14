using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace FunctionApp1
{
    public static class DomainAccessFunc
    {
        [FunctionName("DomainAccessFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            StringBuilder sb = new StringBuilder();



            // Retrieve the Referer header from the HttpRequest
            string referer = req.Headers["Referer"].ToString();

          
            log.LogInformation($"Referer: {referer}");
            sb.AppendLine($"domain : {referer}");

           
            var origin = req.Headers["Origin"].ToString();
            sb.AppendLine($"origin : {origin}");

            string referer1 = req.HttpContext.Request.Headers["Referer"].ToString();
            sb.AppendLine($"From req.HttpContext.Request.Headers : {referer1}");


            return new OkObjectResult(sb);


            //// Define the allowed domains
            //string[] allowedDomains = { "example.com", "example.org", "example.net" };

            //// Retrieve the domain from the referer header
            //string referer = req.Headers["Referer"].ToString();
            //string domain = GetDomainFromUrl(referer);



            //foreach (var h in req.Headers)
            //{
            //    sb.AppendLine($"HeaderKey: {h.Key} - HeaderValue: {h.Value}");
            //}

            //return new OkObjectResult(sb);

            // Check if the domain is allowed
            //if (!string.IsNullOrEmpty(domain) && allowedDomains.Contains(domain))
            //{
            //    // Domain is allowed, continue with function logic

            //    // Your function logic...
            //    return new OkObjectResult(sb);
            //   // return new OkResult();
            //}
            //else
            //{
            //    // Domain is not allowed, return Unauthorized status code or custom message

            //    return new UnauthorizedResult();
            //}
        }

        private static string GetDomainFromUrl(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return uri.Host;
            }
            return null;
        }

    }
}
