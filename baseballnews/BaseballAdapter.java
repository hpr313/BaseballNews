package com.example.baseballnews;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.widget.SearchView;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.GlideBuilder;

import java.util.ArrayList;
import java.util.List;

public class BaseballAdapter extends RecyclerView.Adapter<BaseballAdapter.BaseballHolder> {

    private Context context;
//    String title = "abc";
    private List<playerNews> playerNewsList;

//    private List<playerNews> newsList = filter();

//    public List<playerNews> filter() {
//        List<playerNews> filter = new ArrayList<>();
//        for (int i = 0; i < playerNewsList.size(); i++) {
//            if (playerNewsList.get(i).getNewsTitle().contains(title)) {
//                filter.add(playerNewsList.get(i));
//            }
//        }
//        return  filter;
//    }


    public BaseballAdapter(Context context, List<playerNews> playersNews){
        this.context = context;
        playerNewsList = playersNews;
    }

    public void setFilteredNews(List<playerNews> filteredNews){
        this.playerNewsList = filteredNews;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public BaseballHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.items, parent,false);
        return new BaseballHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull BaseballHolder holder, int position) {
        playerNews playersNews = playerNewsList.get(position);
        holder.teamName.setText(playersNews.getPlayerTeam());
        holder.playerName.setText(playersNews.getPlayerName());
        holder.newsTitle.setText(playersNews.getNewsTitle());
        holder.newsContents.setText(playersNews.getNewsContents());
        holder.newsDate.setText(playersNews.getNewsDate());
        holder.playerPosition.setText(playersNews.getPlayerPosition());
        Glide.with(context).load(playersNews.getPlayerImage()).into(holder.playerPhoto);
        Glide.with(context).load(playersNews.getTeamImage()).into(holder.teamPhoto);

        holder.constraintLayout.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Intent intent = new Intent(context, DetailActivity.class);

                Bundle bundle = new Bundle();
                bundle.putString("teamName", playersNews.getPlayerTeam());
                bundle.putString("playerName", playersNews.getPlayerName());
                bundle.putString("playerPosition", playersNews.getPlayerPosition());
                bundle.putString("newsTitle", playersNews.getNewsTitle());
                bundle.putString("newsContents", playersNews.getNewsContents());
                bundle.putString("newsDate", playersNews.getNewsDate());
                bundle.putString("teamImage", playersNews.getTeamImage());
                bundle.putString("playerImage", playersNews.getPlayerImage());

                intent.putExtras(bundle);
                context.startActivity(intent);
            }
        });
    }

    @Override
    public int getItemCount() {
        return playerNewsList.size();
    }

    public class BaseballHolder extends RecyclerView.ViewHolder{

        ImageView playerPhoto, teamPhoto;
        TextView newsTitle, newsContents, newsDate, playerName, teamName, playerPosition;
        ConstraintLayout constraintLayout;

        public BaseballHolder(@NonNull View itemView) {
            super(itemView);
            playerPhoto = itemView.findViewById(R.id.playerPhoto);
            teamPhoto = itemView.findViewById(R.id.teamPhoto);
            newsTitle = itemView.findViewById(R.id.newsTitle);
            newsContents = itemView.findViewById(R.id.newsContents);
            newsDate = itemView.findViewById(R.id.newsDate);
            playerName = itemView.findViewById(R.id.playerName);
            teamName = itemView.findViewById(R.id.teamName);
            playerPosition = itemView.findViewById(R.id.playerPosition);
            constraintLayout = itemView.findViewById(R.id.main_layout);
        }

    }

}
