package models.DataObjects;

public class ChatRequest {
    private String Message;

    public ChatRequest(String message) {
        this.Message = message;
    }

    public String getMessage() {
        return Message;
    }

    public void setMessage(String message) {
        Message = message;
    }
}
