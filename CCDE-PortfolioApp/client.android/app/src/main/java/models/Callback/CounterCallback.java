package models.Callback;

import models.DataObjects.CounterDoc;

public interface CounterCallback {
    void onSuccess(CounterDoc doc);
    void onError(Throwable t);
}
