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
    try {
        var config = sp.GetRequiredService<IConfiguration>();
        var connStr = config.GetValue<string>("COSMOSDB_CONNECTION_STRING")
                      ?? Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connStr))
            throw new InvalidOperationException("COSMOSDB_CONNECTION_STRING missing or empty.");

        var client = new CosmosClient(connStr);
        // Lazy test connection (optional, but catches issues early)
        // var db = client.GetDatabase("test"); // Uncomment after DB exists
        return client;
    }
    catch (Exception ex) {
        // Log via root logger or throw to expose in host startup
        Console.Error.WriteLine($"CosmosClient init failed: {ex}");
        throw;
    }
});

builder.Build().Run();