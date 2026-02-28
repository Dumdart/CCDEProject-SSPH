namespace PortfolioApi;

public record CounterDoc {
    public CounterDoc() { }
    public CounterDoc(string id, string pageId, int viewCount, string? lastUpdated) {
        this.Id = id;
        this.PageId = pageId;
        this.ViewCount = viewCount;
        this.LastUpdated = lastUpdated;
    }

    public string Id { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty; 
    public int ViewCount { get; set; } 
    public string? LastUpdated { get; set; }
}