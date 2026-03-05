package repository;

import api.ApiService;
import api.LlmService;
import api.RetrofitClient;
import models.Callback.ChatCallback;
import models.Callback.CounterCallback;
import models.DataObjects.ChatRequest;
import models.DataObjects.ChatResponse;
import models.DataObjects.CounterDoc;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LlmRepository {
    private final LlmService api = RetrofitClient.getLlmService();

    public void SendChatMessage(ChatRequest request, ChatCallback callback){
        api.sendChatMessage(request).enqueue(new Callback<ChatResponse>() {
            @Override
            public void onResponse(Call<ChatResponse> call, Response<ChatResponse> response) {
                if (response.isSuccessful() && response.body() != null)
                    callback.onSuccess(response.body());
                else
                    callback.onError(new Exception("Response not successful: " + response.code()));
            }

            @Override
            public void onFailure(Call<ChatResponse> call, Throwable t) {
                callback.onError(t);
            }
        });
    }
}
