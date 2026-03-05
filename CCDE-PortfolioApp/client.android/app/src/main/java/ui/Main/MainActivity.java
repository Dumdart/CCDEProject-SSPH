package ui.Main;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import models.Callback.ChatCallback;
import models.Callback.CounterCallback;
import models.DataObjects.ChatRequest;
import models.DataObjects.ChatResponse;
import models.DataObjects.CounterDoc;
import repository.LlmRepository;
import ui.Main.ccde_sshp.R;

import repository.CounterDocRepository;

public class MainActivity extends AppCompatActivity {

    private LinearLayout analyticsContainer;
    private LinearLayout chatContainer;

    private TextView txtTotalViews;
    private TextView txtPageId;
    private TextView txtChatHistory;
    private EditText editUserInput;

    private CounterDocRepository counterRepo = new CounterDocRepository();
    private LlmRepository llmRepo = new LlmRepository();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        analyticsContainer = findViewById(R.id.analyticsContainer);
        chatContainer = findViewById(R.id.chatContainer);

        txtTotalViews = findViewById(R.id.txtTotalViews);
        txtPageId = findViewById(R.id.txtPageId);
        txtChatHistory = findViewById(R.id.txtChatHistory);
        editUserInput = findViewById(R.id.editUserInput);

        Button btnTabAnalytics = findViewById(R.id.btnTabAnalytics);
        Button btnTabChat = findViewById(R.id.btnTabChat);
        Button btnTabSplit = findViewById(R.id.btnTabSplit);
        Button btnRefreshAnalytics = findViewById(R.id.btnRefreshAnalytics);
        Button btnSend = findViewById(R.id.btnSend);

        // Tabs
        btnTabAnalytics.setOnClickListener(v -> showAnalyticsOnly());
        btnTabChat.setOnClickListener(v -> showChatOnly());
        btnTabSplit.setOnClickListener(v -> showSplit());

        // Initial state
        showAnalyticsOnly();
        loadAnalytics();

        btnRefreshAnalytics.setOnClickListener(v -> loadAnalytics());

        btnSend.setOnClickListener(v -> sendChatMessage());
    }

    private void showAnalyticsOnly() {
        analyticsContainer.setVisibility(View.VISIBLE);
        chatContainer.setVisibility(View.GONE);
    }

    private void showChatOnly() {
        analyticsContainer.setVisibility(View.GONE);
        chatContainer.setVisibility(View.VISIBLE);
    }

    private void showSplit() {
        analyticsContainer.setVisibility(View.VISIBLE);
        chatContainer.setVisibility(View.VISIBLE);
    }

    private void loadAnalytics() {
        counterRepo.GetCounterDoc(new CounterCallback() {
            @Override
            public void onSuccess(CounterDoc doc) {
                runOnUiThread(() -> {
                    txtTotalViews.setText(doc.getViewCount() + " views");
                    txtPageId.setText("page: " + doc.getPageId());
                });
            }

            @Override
            public void onError(Throwable t) {
                runOnUiThread(() ->
                        Toast.makeText(MainActivity.this,
                                "Error: " + t.getMessage(),
                                Toast.LENGTH_SHORT).show());
            }
        });
    }

    private void sendChatMessage() {
        String message = editUserInput.getText().toString().trim();
        if (message.isEmpty()) return;

        // Append user message to history
        appendToHistory("You: " + message);

        ChatRequest body = new ChatRequest(message);
        editUserInput.setText("");

        llmRepo.SendChatMessage(body, new ChatCallback() {
            @Override
            public void onSuccess(ChatResponse response) {
                runOnUiThread(() ->
                        appendToHistory("Assistant: " + response.getResponse()));
            }

            @Override
            public void onError(Throwable t) {
                runOnUiThread(() ->
                        appendToHistory("Error: " + t.getMessage()));
            }
        });
    }

    private void appendToHistory(String line) {
        String current = txtChatHistory.getText().toString();
        if (!current.isEmpty()) current += "\n\n";
        txtChatHistory.setText(current + line);

        // Simple scroll-to-bottom
        txtChatHistory.post(() -> {
            int scrollAmount = txtChatHistory.getLayout().getLineTop(
                    txtChatHistory.getLineCount()) - txtChatHistory.getHeight();
            if (scrollAmount > 0) {
                txtChatHistory.scrollTo(0, scrollAmount);
            } else {
                txtChatHistory.scrollTo(0, 0);
            }
        });
    }
}