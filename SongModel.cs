using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using System.ComponentModel;

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