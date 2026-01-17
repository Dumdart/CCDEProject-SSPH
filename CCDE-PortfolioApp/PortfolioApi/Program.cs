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

builder.Services.AddSingleton<CosmosClient>(serviceProvider => {
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration["COSMOSDB_CONNECTION_STRING"] ??
                           Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
    return new CosmosClient(connectionString);
});

builder.Build().Run();