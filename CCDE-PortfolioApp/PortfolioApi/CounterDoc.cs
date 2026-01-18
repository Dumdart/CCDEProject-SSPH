namespace PortfolioApi;

public record CounterDoc {
    public CounterDoc() { }
    public CounterDoc(string id, string pageId, int viewCount, string? lastUpdated) {
        this.id = id;
        this.pageId = pageId;
        this.viewCount = viewCount;
        this.lastUpdated = lastUpdated;
    }

    public string id { get; set; } = string.Empty;
    public string pageId { get; set; } = string.Empty; 
    public int viewCount { get; set; } 
    public string? lastUpdated { get; set; }
}