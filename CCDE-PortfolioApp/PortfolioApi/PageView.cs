using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker.Http;

namespace PortfolioApi;

public class PageView {
    private readonly ILogger<PageView> _logger;
    private readonly CosmosClient _cosmos;

    public PageView(ILogger<PageView> logger, CosmosClient cosmosCosmos) {
        _logger = logger;
        _cosmos = cosmosCosmos;
    }

    [Function("PageView")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function,
            "get", Route = "visitor-count")] HttpRequestData req) {
        if (_cosmos == null) {
            _logger.LogError("CosmosClient is null");
            var errorRes = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
            await errorRes.WriteAsJsonAsync(new { error = "Cosmos client unavailable" });
            return errorRes;
        }

            var query = HttpUtility.ParseQueryString(req.Url.Query);
            var pageId = ( query["pageId"] ?? "home" ).ToString();
            var id = $"{pageId}-page-counter";  



        _logger.LogInformation("PageView called with URL: {Url}", req.Url);

        var databaseId = Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID");
        _logger.LogInformation("DatabaseID:" + databaseId);

        var containerId = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID");
        _logger.LogInformation($"{containerId}");

        var constring = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");
        Console.WriteLine(constring);


        var container = _cosmos.GetContainer(databaseId, containerId);

        CounterDoc doc;

        try {
            var read = await container.ReadItemAsync<CounterDoc>(id, new PartitionKey(pageId));
            doc = read.Resource;
            _logger.LogInformation("Found existing doc: {Doc}", doc);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
            _logger.LogInformation("Doc not found, creating new: id={Id}", id);
            doc = new CounterDoc(id, pageId, 0, DateTimeOffset.UtcNow.ToString("O"));
        }
        catch (CosmosException ex) {
            _logger.LogError(ex, "CosmosException: Status={StatusCode}, Message={Message}", ex.StatusCode, ex.Message);
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteAsJsonAsync(new { error = ex.Message });
            return errorRes;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Unexpected error");
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteAsJsonAsync(new { error = ex.Message });
            return errorRes;
        }

        var updated = doc with {
            viewCount = doc.viewCount + 1,
            lastUpdated = DateTimeOffset.UtcNow.ToString("O")
        };

        try {
            var upsert = await container.UpsertItemAsync(updated, new PartitionKey(pageId));
            _logger.LogInformation("Upsert successful: {Doc}", upsert.Resource);

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(upsert.Resource);
            return res;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Upsert failed");
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteAsJsonAsync(new { error = ex.Message });
            return errorRes;
        }
    }
}