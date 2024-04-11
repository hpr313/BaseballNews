package com.example.baseballnews;

public class playerNews {
    private String playerTeam, playerPosition, playerName, newsTitle, newsContents, newsDate, playerImage, teamImage;

    public  playerNews(String playerTeam, String playerPosition, String playerName,
                       String newsTitle, String newsContents, String newsDate,
                       String playerImage, String teamImage){
        this.playerName = playerName;
        this.playerPosition = playerPosition;
        this.playerTeam = playerTeam;
        this.newsTitle = newsTitle;
        this.newsContents = newsContents;
        this.newsDate = newsDate;
        this.playerImage = playerImage;
        this.teamImage = teamImage;
    }

    public String getPlayerTeam() { return playerTeam; }

    public String getPlayerPosition() { return playerPosition; }

    public String getPlayerName() { return playerName; }

    public String getNewsTitle() { return newsTitle; }

    public String getNewsContents() { return newsContents; }

    public String getNewsDate() {  return newsDate; }

    public String getPlayerImage() {  return playerImage; }

    public String getTeamImage() {  return teamImage; }
}
