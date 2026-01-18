namespace PortfolioApi;

public record CounterDoc {
    public CounterDoc(string id, string pageId, int viewCount, string? lastUpdated) {
        this.Id = id;
        this.PageId = pageId;
        this.ViewCount = viewCount;
        this.LastUpdated = lastUpdated;
    }

    public string Id { get; set; }
    public string PageId { get; set; }
    public int ViewCount { get; set; }
    public string? LastUpdated { get; set; }

}