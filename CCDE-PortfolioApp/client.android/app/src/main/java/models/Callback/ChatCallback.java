package models.Callback;

import models.DataObjects.ChatResponse;

public interface ChatCallback {
    void onSuccess(ChatResponse response);
    void onError(Throwable t);
}