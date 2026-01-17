using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        // Log BEFORE CosmosClient creation
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Program");

        var constring = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
        logger.LogInformation("COSMOSDB_CONNECTION_STRING starts with: {Prefix}", constring?.Substring(0, Math.Min(30, constring?.Length ?? 0)));

        var dbId = Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID");
        var containerId = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID");
        logger.LogInformation("Using DB={DbId}, Container={ContainerId}", dbId, containerId);

        if (string.IsNullOrEmpty(constring)) {
            logger.LogError("COSMOSDB_CONNECTION_STRING is NULL or EMPTY!");
            throw new InvalidOperationException("Missing COSMOSDB_CONNECTION_STRING");
        }

        services.AddSingleton(new CosmosClient(constring));
    })
    .Build();

host.Run();

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(_ => {
    var constring = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
    return new CosmosClient(constring);
});

builder.Build().Run();
