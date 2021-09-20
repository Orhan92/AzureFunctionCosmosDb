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
                databaseName: "my-database", 
                collectionName: "my-container",
                ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try 
            {
                string name = req.Query["name"];
                string description = req.Query["description"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;
                description = description ?? data?.description;

                if (!string.IsNullOrEmpty(name))
                {
                    // Add a JSON document to the output container.
                    await documentsOut.AddAsync(new
                    {
                        // create a random ID
                        id = System.Guid.NewGuid().ToString(),
                        name = name,
                        description = description,
                    });
                }

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered function executed successfully. Pass a value in the query string or in the request body for a personalized response."
                    : $"This HTTP triggered function executed successfully\nName: {name}\nDescription: {description}";

                return new OkObjectResult(responseMessage);
            }
            catch 
            {
                return new BadRequestObjectResult("Invalid input values. Try again!");
            }
        }
    }
}
