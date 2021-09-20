using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionCosmosDb
{
    public static class ApiFunction
    {
        [FunctionName("ApiFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            [CosmosDB(
                databaseName: "Music-database", 
                collectionName: "songs",
                ConnectionStringSetting = "CosmosDbConnectionString",
                SqlQuery = "SELECT c.artist, c.song FROM c ORDER BY c.artist")]
                IAsyncCollector<dynamic> documentsOut, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try 
            {
                string artist = req.Query["artist"];
                string song = req.Query["song"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                artist = artist ?? data?.artist;
                song = song ?? data?.song;

                if (!string.IsNullOrEmpty(artist))
                {
                    // Add a JSON document to the output container.
                    await documentsOut.AddAsync(new
                    {
                        // create a random ID
                        id = System.Guid.NewGuid().ToString(),
                        artist = artist,
                        song = song,
                    });
                }

                string responseMessage = string.IsNullOrEmpty(artist)
                    ? "This HTTP triggered function executed successfully. Pass a value in the query string or in the request body for a personalized response."
                    : $"This HTTP triggered function executed successfully\nArtist: {artist}\nSong: {song}";

                return new OkObjectResult(responseMessage);
            }
            catch 
            {
                return new BadRequestObjectResult("Invalid input values. Try again!");
            }
        }
    }
}
