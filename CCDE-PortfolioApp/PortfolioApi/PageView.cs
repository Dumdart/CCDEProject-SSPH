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
        var query = HttpUtility.ParseQueryString(req.Url.Query);
        var pageId = ( query["pageId"] ?? "home" ).ToString();
        var id = $"{pageId}-page-counter";

        var databaseId = Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID");
        var containerId = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID");

        var container = _cosmos.GetContainer(databaseId, containerId);

        CounterDoc doc;

        try {
            var read = await container.ReadItemAsync<CounterDoc>(id, new PartitionKey(pageId));
            doc = read.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
            doc = new CounterDoc(id, pageId, 0, DateTimeOffset.UtcNow.ToString("O"));
        }

        var updated = doc with {
            viewCount = doc.viewCount + 1,
            lastUpdated = DateTimeOffset.UtcNow.ToString("O")
        };

        // Upsert: updates if (id + partitionKey) exists, else inserts
        var upsert = await container.UpsertItemAsync(updated, new PartitionKey(pageId));
        
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(upsert.Resource);
        return res;
    }
}