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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, 
            [CosmosDB(
                databaseName: "Music-database", 
                collectionName: "songs",
                ConnectionStringSetting = "CosmosDbConnectionString")]
                IAsyncCollector<dynamic> DbSongs, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try 
            {
                //Fill with more desired variables below if you want to expand with more properties for a song
                string artist = req.Query["artist"];
                string title = req.Query["title"];
                DateTime dateTime = DateTime.UtcNow;

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                artist = artist ?? data?.artist;
                title = title ?? data?.title;

                if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(title))
                {
                    // Add a JSON document to the output container.
                    await DbSongs.AddAsync(new
                    {
                        // create a random ID
                        id = System.Guid.NewGuid().ToString(),
                        artist = artist,
                        title = title,
                        created = dateTime                    
                    });
                }
                // string responseMessage = $"Perfect, {artist} - {title} have now been added into the database";
                // return new OkObjectResult(responseMessage);
                string responseMessage = string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(title)
                    ? "You have to pass in an artist AND a song in order to post this song into the database"
                    : $"Perfect, '{artist} - {title}' have now been added into the database";
                return new OkObjectResult(responseMessage);
            }
            catch 
            {
                return new BadRequestObjectResult("Invalid input values. You have add an artist AND a song value/input.");
            }
        }
    }
}
