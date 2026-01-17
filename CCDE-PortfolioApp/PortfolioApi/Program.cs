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
    
    if (string.IsNullOrEmpty(constring)) {
        throw new InvalidOperationException(
            "COSMOSDB_CONNECTION_STRING environment variable is not set");
    }

    return new CosmosClient(constring);
});

builder.Build().Run();