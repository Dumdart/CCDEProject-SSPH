package api;

import models.DataObjects.ChatRequest;
import models.DataObjects.ChatResponse;
import models.DataObjects.CounterDoc;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Query;

public interface LlmService {
    @POST("chat")
    Call<ChatResponse> sendChatMessage(@Body ChatRequest request);
}
