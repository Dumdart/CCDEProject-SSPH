package repository;

import api.ApiService;
import api.RetrofitClient;
import models.Callback.CounterCallback;
import models.DataObjects.CounterDoc;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CounterDocRepository {
    private final ApiService api = RetrofitClient.getApiService();

    public void GetCounterDoc(CounterCallback callback){
        api.getCounter("home").enqueue(new Callback<CounterDoc>() {
            @Override
            public void onResponse(Call<CounterDoc> call, Response<CounterDoc> response) {
                if (response.isSuccessful() && response.body() != null)
                    callback.onSuccess(response.body());
                else
                    callback.onError(new Exception("Response not successful: " + response.code()));
            }

            @Override
            public void onFailure(Call<CounterDoc> call, Throwable t) {
                callback.onError(t);
            }
        });
    }
}