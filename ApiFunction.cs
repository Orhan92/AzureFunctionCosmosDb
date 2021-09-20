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

                if (!string.IsNullOrEmpty(artist))
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
                string responseMessage = string.IsNullOrEmpty(artist)
                    ? "This HTTP triggered function executed successfully. Pass a value in the query string or in the request body for a personalized response."
                    : $"This HTTP triggered function executed successfully\nArtist: {artist}\nTitle: {title}";
                return new OkObjectResult(responseMessage);
            }
            catch 
            {
                return new BadRequestObjectResult("Invalid input values. Try again!");
            }
        }
    }
}
