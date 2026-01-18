using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfolioApi;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddDbContext<DatabaseContext>(options => {
    var endpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                   ?? throw new InvalidOperationException("COSMOS_ENDPOINT missing.");

    var key = Environment.GetEnvironmentVariable("COSMOS_KEY")
               ?? throw new InvalidOperationException("COSMOS_KEY missing.");

    var databaseId = Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID")
                      ?? throw new InvalidOperationException("COSMOS_DATABASE_ID missing.");

    options.UseCosmos(endpoint, key, databaseId);
});


builder.Build().Run();