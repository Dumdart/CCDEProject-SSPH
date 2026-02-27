package models.DataObjects;

import com.google.gson.annotations.SerializedName;

public class CounterDoc {
    @SerializedName("Id")
    private String id;

    @SerializedName("PageId")
    private String pageId;

    @SerializedName("ViewCount")
    private int viewCount;

    @SerializedName("LastUpdated")
    private String lastUpdated;

    public CounterDoc(){    }
    public CounterDoc(String id, String pageId, int viewCount, String lastUpdated){
        this.id = id;
        this.pageId = pageId;
        this.viewCount = viewCount;
        this.lastUpdated = lastUpdated;
    }

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getPageId() { return pageId; }
    public void setPageId(String pageId) { this.pageId = pageId; }

    public int getViewCount() { return viewCount; }
    public void setViewCount(int viewCount) { this.viewCount = viewCount; }

    public String getLastUpdated() { return lastUpdated; }
    public void setLastUpdated(String lastUpdated) { this.lastUpdated = lastUpdated; }
}
