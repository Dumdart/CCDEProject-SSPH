package ui.Main;

import android.os.Bundle;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import models.Callback.CounterCallback;
import models.DataObjects.CounterDoc;
import ui.Main.ccde_sshp.R;

import repository.CounterDocRepository;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);


        // Fabsoft

        CounterDocRepository counterRepo = new CounterDocRepository();

        counterRepo.GetCounterDoc(new CounterCallback() {
            @Override
            public void onSuccess(CounterDoc doc) {
                TextView view = findViewById(R.id.txtView);
                view.setText(doc.getPageId() + " " + doc.getViewCount());
            }

            @Override
            public void onError(Throwable t) {
                TextView view = findViewById(R.id.txtView);
                view.setText(t.getMessage());
            }
        });

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });
    }
}