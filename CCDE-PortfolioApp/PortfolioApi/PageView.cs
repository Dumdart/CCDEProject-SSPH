using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;

namespace PortfolioApi;

public class PageView {
    private readonly ILogger<PageView> _logger;
    private readonly DatabaseContext _dbContext;

    public PageView(ILogger<PageView> logger, DatabaseContext dbContext) {
        _logger = logger;
        _dbContext = dbContext;
    }

    [Function("PageView")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "visitor-count")] HttpRequestData req) {
        _logger.LogInformation("PageView called with URL: {Url}", req.Url);

        var query = HttpUtility.ParseQueryString(req.Url.Query);
        var pageId = ( query["pageId"] ?? "home" ).ToString();
        var id = $"{pageId}-page-counter";

        _logger.LogInformation("Cosmos: DB={Db}, Container={Container}",
        Environment.GetEnvironmentVariable("COSMOS_DATABASE_ID"),
        Environment.GetEnvironmentVariable("COSMOS_CONTAINER_ID"));

        try {
            _logger.LogInformation("Querying for id={Id}, pageId={PageId}", id, pageId);
            var doc = await _dbContext.Counters
                .WithPartitionKey(pageId) 
                .FirstOrDefaultAsync(x => x.Id == id);

            _logger.LogInformation("Found doc: {Doc}", doc != null ? "exists" : "null");

            if (doc == null) {
                doc = new CounterDoc(id, pageId, 0, DateTimeOffset.UtcNow.ToString("O"));
                _dbContext.Counters.Add(doc);
                _logger.LogInformation("Added new doc");
            }
            else {
                doc.ViewCount += 1;
                doc.LastUpdated = DateTimeOffset.UtcNow.ToString("O");
                _dbContext.Counters.Update(doc);
                _logger.LogInformation("Updated existing doc");
            }

            await _dbContext.SaveChangesAsync();

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(doc);
            return res;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error processing counter");
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteAsJsonAsync(new { error = ex.Message });
            return errorRes;
        }
    }
}