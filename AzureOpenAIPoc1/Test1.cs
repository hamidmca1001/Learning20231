using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace AzureOpenAIPoc1
{
    public class Test1
    {
        //public static void M1()
        //{
        //    // load a pre-trained language model
        //    //var nlp = new EnglishLanguageModel();

        //    // define a function to fetch the textual content from a web page
        //    string FetchText(string url)
        //    {
        //        var httpClient = new HttpClient();
        //        var response = httpClient.GetAsync(url).Result;
        //        var html = response.Content.ReadAsStringAsync().Result;
        //        var document = new HtmlDocument();
        //        document.LoadHtml(html);
        //        var text = document.DocumentNode.SelectSingleNode("//body").InnerText;
        //        return text;
        //    }

        //    // define a function to generate embeddings for URLs
        //    List<List<float>> GenerateEmbeddings(List<string> urls)
        //    {
        //        var texts = new List<string>();
        //        foreach (var url in urls)
        //        {
        //            var text = FetchText(url);
        //            texts.Add(text);
        //        }
        //        var docs = nlp.Process(texts.ToArray());
        //        var embeddings = new List<List<float>>();
        //        foreach (var doc in docs)
        //        {
        //            embeddings.Add(doc.Vector.ToList());
        //        }
        //        return embeddings;
        //    }

        //    // generate embeddings for a list of URLs
        //    var urls = new List<string> { "https://www.example.com/page1", "https://www.example.com/page2", "https://www.example.com/page3" };
        //    var embeddings = GenerateEmbeddings(urls);

        //    // upload the embeddings to Pinecone
        //    //Pinecone.Init(apiKey: "YOUR_API_KEY", environment: "YOUR_ENVIRONMENT");
        //    //Pinecone.CreateIndex(indexName: "my_index", dimension: 300);
        //    //Pinecone.Index(indexName: "my_index", data: embeddings, ids: Enumerable.Range(0, embeddings.Count));

        //}
    }
}



