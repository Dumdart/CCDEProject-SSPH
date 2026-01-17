using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(_ => {
    var constring = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
    Console.WriteLine(constring);
    Console.WriteLine(Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID"));
    Console.WriteLine(Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID"));

    return new CosmosClient(constring);
});

builder.Build().Run();