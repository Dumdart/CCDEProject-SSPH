package api;

import android.util.Log;

import ui.Main.ccde_sshp.BuildConfig;

import java.io.IOException;

import okhttp3.HttpUrl;
import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitClient {
    private static final String BASE_URL = BuildConfig.API_BASE_URL;
    private static Retrofit retrofit;

    public static ApiService getApiService() {
        if (retrofit == null) { // Interceptor to add API key to all requests
            OkHttpClient client = new OkHttpClient.Builder()
                    .addInterceptor(new Interceptor() {
                        @Override
                        public okhttp3.Response intercept(Chain chain) throws IOException {
                            Request original = chain.request();
                            HttpUrl originalHttpUrl = original.url();

                            // Add code parameter (API key) to every request
                            HttpUrl url = originalHttpUrl.newBuilder()
                                    .addQueryParameter("code", BuildConfig.API_KEY)
                                    .build();

                            Log.d("Interceptor", "Original: " + original.url() + "\nNew: " + url.toString());

                            Request request = original.newBuilder()
                                    .url(url)
                                    .build();

                            return chain.proceed(request);
                        }
                    })
                    .build();

            retrofit = new Retrofit.Builder()
                    .baseUrl(BuildConfig.API_BASE_URL)
                    .client(client)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }

        return retrofit.create(ApiService.class);
    }
}
