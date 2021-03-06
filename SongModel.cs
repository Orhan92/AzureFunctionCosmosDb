using System;
using Newtonsoft.Json;

namespace AzureFunctionCosmosDb
{
    public class SongModel 
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        // [JsonProperty(PropertyName = "artist")]
        public string Artist { get;set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
    }
}