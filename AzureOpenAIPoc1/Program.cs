// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Microsoft.Identity.Client;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using System.Text.Json;

Console.WriteLine("Hello, World!");


// Replace with your Azure OpenAI key
string key = "d2941cc1aff14b1bbdcbc6cfe2c48e29";
string endpoint = "https://solutionspoc.openai.azure.com/";
OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

////1 - GetCompletions // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.openai-readme?source=recommendations&view=azure-dotnet-preview
CompletionsOptions completionsOptions = new CompletionsOptions()
{
    Prompt =
    {
        "5G and Next Gen  Services",
    }
};
completionsOptions.Prompt.Add("5G and Next Gen  Services");

Response<Completions> completionsResponse = client.GetCompletions("babbage-test", completionsOptions);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"GetCompletions: {completion}");

//List<string> examplePrompts = new()
//{
//    "What is the feedback given from your clients?",
//    "How are you today?",
//    "What is Azure OpenAI?",
//    "Why do children love dinosaurs?",
//    "Generate a proof of Euler's identity",
//    "Describe in single words only the good things that come into your mind about your mother.",
//};

//foreach (string prompt in examplePrompts)
//{
//    Console.Write($"Input: {prompt}");
//    CompletionsOptions completionsOptions = new CompletionsOptions();
//    completionsOptions.Prompt.Add(prompt);

//    Response<Completions> completionsResponse = client.GetCompletions("text-similarty-ada-001", completionsOptions);
//    string completion = completionsResponse.Value.Choices[0].Text;
//    Console.WriteLine($"Chatbot: {completion}");
//}




////2-GetEmbeddings
//EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions("Who is USA Precident?")
//{
//    Input = "What is the feedback given from your clients?"
//};
//embeddingsOptions.Input = "What is the feedback given from your clients?";

//Response<Embeddings> completionsResponse1 = client.GetEmbeddings("WebchaBot", embeddingsOptions);
//string completion1 = completionsResponse1.Value.Data[0].Embedding[0].ToString();
//Console.WriteLine($"GetEmbeddings: {completion1}");


// 3-

//EmbeddingsOptions embeddingsOptions = new EmbeddingsOptions("Who is USA Precident?")
//{
//    Input = "Who is USA Precident?"
//};
//embeddingsOptions.Input = "Who is USA Precident?";

//Response<Embeddings> embeddingResult = client.GetEmbeddings("text-embedding-ada-002", embeddingsOptions);
//var completion = embeddingResult.Value.Data[0].Embedding;

//foreach (var item in completion)
//{
//    Console.Write($"{item} ");
//}



//4

//var data = new
//{
//    user = "hamid",
//    input_type = "What is the feedback given from your clients?",
//    model = "WebchaBot",
//    input = new string[1] { "What is the feedback given from your clients?" }
//    //new { " " }
//};


//Response response = client.GetEmbeddings("WebchaBot", RequestContent.Create(data));

//JsonElement result = JsonDocument.Parse(response.ContentStream).RootElement;
//Console.WriteLine(result.GetProperty("object").ToString());
//Console.WriteLine(result.GetProperty("data")[0].GetProperty("object").ToString());
//Console.WriteLine(result.GetProperty("data")[0].GetProperty("embedding")[0].ToString());
//Console.WriteLine(result.GetProperty("data")[0].GetProperty("index").ToString());
//Console.WriteLine(result.GetProperty("model").ToString());
//Console.WriteLine(result.GetProperty("usage").GetProperty("prompt_tokens").ToString());
//Console.WriteLine(result.GetProperty("usage").GetProperty("total_tokens").ToString());



// 5
//var data = new
//{
//    prompt = new[] {
//        "Who is Bill Gates"
//    },
//    //max_tokens = 1234,
//    //temperature = 1.5f,
//    //top_p = 1,

//    //user = "1",
//    //n = 12,
//    //logprobs = 13,
//    //model = "11",
//    //echo = true,
//    //stop = new[] {
//    //    "1"
//    //},
//    completion_config = "1",
//    //cache_level = 1,
//    //presence_penalty = 1.5f,
//    //frequency_penalty = 1.5f,
//    //best_of = 11,
//};

//Response response = client.GetCompletions("text-davinci-003", RequestContent.Create(data));

//JsonElement result = JsonDocument.Parse(response.ContentStream).RootElement;
//Console.WriteLine(result.GetProperty("object").ToString());
//Console.WriteLine(result.GetProperty("usage").GetProperty("completion_tokens").ToString());
//Console.WriteLine(result.GetProperty("usage").GetProperty("prompt_tokens").ToString());
//Console.WriteLine(result.GetProperty("usage").GetProperty("total_tokens").ToString());




////https://liuhongbo.medium.com/how-to-use-azure-open-ai-service-in-c-80cfd63a5189
//var apiKey = "d2941cc1aff14b1bbdcbc6cfe2c48e29";

//var gpt3 = new OpenAIService(new OpenAiOptions()
//{
//    ProviderType = ProviderType.Azure,
//    ApiKey = apiKey,
//    DeploymentId = "babbage-test",
//    ResourceName = "https://solutionspoc.openai.azure.com/"
//});


//var completionResult = await gpt3.Completions.CreateCompletion(new CompletionCreateRequest()
//{
//    Prompt = "5G and Next Gen  Services",
//    Model = Models.Babbage,
//    Temperature = 0.5F,
//    MaxTokens = 100,
//    N = 3
//});
//if (completionResult.Successful)
//{
//    foreach (var choice in completionResult.Choices)
//    {
//        Console.WriteLine(choice.Text);
//    }
//}
//else
//{
//    if (completionResult.Error == null)
//    {
//        throw new Exception("Unknown Error");
//    }
//    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
//}

Console.ReadLine();
