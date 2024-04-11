package com.example.baseballnews;

import android.os.Bundle;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.bumptech.glide.Glide;

public class DetailActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detail);

        ImageView pImage = findViewById(R.id.pImage);
        ImageView tImage = findViewById(R.id.tImage);
        TextView tName = findViewById(R.id.tName);
        TextView pName = findViewById(R.id.pName);
        TextView pPosition = findViewById(R.id.pPosition);
        TextView nTitle = findViewById(R.id.nTitle);
        TextView nContents = findViewById(R.id.nContents);
        TextView nDate = findViewById(R.id.nDate);

        Bundle bundle = getIntent().getExtras();

        String playerImage = bundle.getString("playerImage");
        String teamImage = bundle.getString("teamImage");
        String playerName = bundle.getString("playerName");
        String playerPosition = bundle.getString("playerPosition");
        String teamName = bundle.getString("teamName");
        String newsTitle = bundle.getString("newsTitle");
        String newsContents = bundle.getString("newsContents");
        String newsDate = bundle.getString("newsDate");

        Glide.with(this).load(playerImage).into(pImage);
        Glide.with(this).load(teamImage).into(tImage);
        tName.setText(teamName);
        pName.setText(playerName);
        pPosition.setText(playerPosition);
        nTitle.setText(newsTitle);
        nContents.setText(newsContents);
        nDate.setText(newsDate);
    }
}
