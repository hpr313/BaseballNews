package com.example.baseballnews;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.SearchView;
import androidx.recyclerview.widget.DividerItemDecoration;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.content.Context;
import android.graphics.Rect;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonArrayRequest;
import com.loopj.android.http.AsyncHttpClient;
import com.loopj.android.http.AsyncHttpResponseHandler;
import com.loopj.android.http.JsonHttpResponseHandler;

import org.json.JSONArray;
import org.json.JSONObject;

import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import cz.msebera.android.httpclient.Header;

public class MainActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private RequestQueue requestQueue;
    private List<playerNews> playerNewsList;
    private SearchView searchView;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        searchView = findViewById(R.id.searchView);

        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                filterList(newText);
                return true;
            }

        });
        recyclerView = findViewById(R.id.recyclerview);
        recyclerView.setHasFixedSize(true);
        recyclerView.addItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.VERTICAL));
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        requestQueue = VolleySingleton.getmInstance(this).getRequestQueue();
        playerNewsList = new ArrayList<>();

        fetchNews();
        closeKeyboard();
    }
    private void closeKeyboard() {
        View view = this.getCurrentFocus();
        if (view != null) {
            InputMethodManager imm = (InputMethodManager)getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(searchView.getWindowToken(),0);
        }
    }
    private void filterList(String text) {
        List<playerNews> filteredNews = new ArrayList<>();
        for (playerNews news : playerNewsList){
            if (news.getPlayerName().contains(text.substring(0,1).toUpperCase()+text.substring(1))
                    || news.getPlayerTeam().contains(text.substring(0,1).toUpperCase()+text.substring(1))){
                filteredNews.add(news);
            }
        }
        if (filteredNews.isEmpty()){
            Toast.makeText(this, "No data found", Toast.LENGTH_SHORT).show();
        }else{
            BaseballAdapter filteredAdapter = new BaseballAdapter(MainActivity.this, filteredNews);
            recyclerView.setAdapter(filteredAdapter);

        }
    }

    private void fetchNews(){
        AsyncHttpClient client = new AsyncHttpClient();
      String url = "http://10.0.2.2:44376/api/home/getNews";
//        String url = "http://10.0.2.2:80/baseballnews/api/home/getNews";
        client.get(url, new JsonHttpResponseHandler(){
                    @Override
                    public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                        for (int i = 0; i<response.length(); i++){
                            try {
                                JSONObject jsonObject = response.getJSONObject(i);
                                String playerTeam = jsonObject.getString("playerTeam");
                                String playerPosition = jsonObject.getString("playerPosition");
                                String playerName = jsonObject.getString("playerName");
                                String newsTitle = jsonObject.getString("newsTitle");
                                String newsContents = jsonObject.getString("newsContents");
                                String newsDate = jsonObject.getString("newsDate");
                                String playerImage = jsonObject.getString("playerImage");
                                String teamImage = jsonObject.getString("teamImage");
                                playerNews playerNews =new playerNews(playerTeam, playerPosition, playerName,
                                        newsTitle, newsContents, newsDate, playerImage, teamImage);
                                playerNewsList.add(playerNews);

                            } catch (Exception e) {
                                e.printStackTrace();
                            }
                            BaseballAdapter adapter = new BaseballAdapter(MainActivity.this, playerNewsList);
                            recyclerView.setAdapter(adapter);
                        }
                    }
                });
    }
    @Override
    public boolean dispatchTouchEvent(MotionEvent event) {
        if (event.getAction() == MotionEvent.ACTION_DOWN) {
            View v = getCurrentFocus();
            if (v instanceof EditText) {
                Rect outRect = new Rect();
                v.getGlobalVisibleRect(outRect);
                if (!outRect.contains((int)event.getRawX(), (int)event.getRawY())) {
                    v.clearFocus();
                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                }
            }
        }
        return super.dispatchTouchEvent( event );
    }
}