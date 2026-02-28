package api;

import models.DataObjects.CounterDoc;
import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface LlmService {
    @GET("visitor-count")
    Call<CounterDoc> getCounter(@Query("pageId") String pageId);
}
