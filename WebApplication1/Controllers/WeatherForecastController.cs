using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.OpenAI;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using WebApplication1.DtoModels;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        public string GetResut(string input)
        {
            // Replace with your Azure OpenAI key
            string key = "d2941cc1aff14b1bbdcbc6cfe2c48e29";
            string endpoint = "https://solutionspoc.openai.azure.com/";
            OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

            ////1 - GetCompletions // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?source=recommendations&view=azure-dotnet-preview
            CompletionsOptions completionsOptions = new CompletionsOptions()
            {
                Prompts = { "5G and Next Gen  Services" }
            };
            completionsOptions.Prompts.Add("5G and Next Gen  Services");

            Response<Completions> completionsResponse = client.GetCompletions("babbage-test", completionsOptions);
            string completion = completionsResponse.Value.Choices[0].Text;
            Console.WriteLine($"GetCompletions: {completion}");

            // string output = "hhhhhhh";
            return completion;
        }

        [HttpGet]
        [Route("qna/{ques}")]
        public async Task<ActionResult<QnaResponse>> GetQna(string ques)
        {
            string ans = string.Empty;
            var Url = "https://openaiqnafun.azurewebsites.net/api/qnaSolution?name=" + ques;
            dynamic content = new { ques = ques };


            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
            using (var httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;

                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    ans = response.Content.ReadAsStringAsync().Result;

                }
            }

            if (ans == null)
            {
                return NotFound();
            }
            else
            {
                var response = new QnaResponse { Answer = ans };

                return response;
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        //[HttpGet]
        //[Route("qnaResponse/{name}")]
        //public async Task<IActionResult> GetQnaResponse(string name)
        //{
        //    return Json(model);
        //}

        //[HttpGet]
        //[Route("video-mapping")]
        //public Task<IActionResult> GetVideoMappings()
        //{

        //    return OkResult();
        //}
    }
}