using Azure.Core;
using Azure.Identity;
using Google.GenAI;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PortfolioApi;
using System.Text.Json;


var builder = FunctionsApplication.CreateBuilder(args);

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

    options.UseCosmos(endpoint, key, databaseId)        
        .EnableDetailedErrors()           
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Debug);
});

// Gemini API client
var key = Environment.GetEnvironmentVariable("GEMINI_API_KEY") 
    ?? throw new InvalidOperationException("GEMINI_API_KEY is missing.");

builder.Services.AddScoped(sp => {
    return new Client(apiKey: key);
});


builder.Build().Run();