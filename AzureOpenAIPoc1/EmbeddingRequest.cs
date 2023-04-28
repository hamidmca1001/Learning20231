using Newtonsoft.Json;
using Azure.AI.OpenAI;

namespace AzureOpenAIPoc1
{
    public class EmbeddingRequest
    {

        [JsonProperty("model")]
        public string Model { get; set; }


        [JsonProperty("input")]
        public string Input { get; set; }

        public EmbeddingRequest()
        {

        }

    }

}
