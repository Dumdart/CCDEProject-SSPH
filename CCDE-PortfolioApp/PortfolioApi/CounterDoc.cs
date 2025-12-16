namespace PortfolioApi;

public record CounterDoc {
    public CounterDoc(string id, string pageId, int viewCount, string? lastUpdated) {
        this.id = id;
        this.pageId = pageId;
        this.viewCount = viewCount;
        this.lastUpdated = lastUpdated;
    }

    public string id { get; init; }
    public string pageId { get; init; }
    public int viewCount { get; init; }
    public string? lastUpdated { get; init; }

}