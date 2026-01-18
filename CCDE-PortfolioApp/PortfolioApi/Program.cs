using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<CosmosClient>(sp => {
    var endpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                  ?? throw new InvalidOperationException("COSMOS_ENDPOINT missing.");

    var key = Environment.GetEnvironmentVariable("COSMOS_KEY")
              ?? throw new InvalidOperationException("COSMOS_KEY missing.");
    
    return new CosmosClient(endpoint, key);
});

builder.Build().Run();