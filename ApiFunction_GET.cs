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

namespace AzureFunctionCosmosDb
{
    public static class ApiFunction_GET
    {
        [FunctionName("ApiFunction_GET")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(
            databaseName: "Music-database", 
            collectionName: "songs",
            ConnectionStringSetting = "CosmosDbConnectionString",
            SqlQuery = "SELECT c.artist, c.title FROM c")]
            IAsyncCollector<dynamic> DbSongs, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try 
            {
                string artist = req.Query["artist"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                artist = artist ?? data.artist;

                string responseMessage = string.IsNullOrEmpty(artist)
                    ? "You have to pass in an artist value in order to receive information about the song."
                    : $"Perfect, '{artist} ' have now been added into the database";
                return new OkObjectResult(artist);
            }
            catch 
            {
                return new BadRequestObjectResult("Invalid input values. You have add an artist AND a song value/input.");
            }
        }
    }
}
