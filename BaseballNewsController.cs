using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace News_WebApplication.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class BaseballNewsController : Controller
    {
        [HttpGet]
        [Route("Test")]
        public DateTime getDate()
        {
            return DateTime.Now;
        }
        [HttpPost]
        [Route("insertData")]
        public void insertData(playerNews xxx)
        {
            SqlServer sql = new SqlServer();
            SqlConnection cnn = new SqlConnection(sql.connectTo());
            cnn.Open();
            SqlCommand myCommand = new SqlCommand();
            myCommand = cnn.CreateCommand();
            myCommand.CommandType = CommandType.Text;//宣告為SP
                                                     //myCommand.CommandText = "SelectExample"; //SP名稱
            myCommand.CommandText = "Insert into dbo.playerNews (playerTeam, playerPosition, playerName, newsTitle, newsContents, newsDate, playerImage, teamImage) " +
                "VALUES (@playerTeam, @playerPosition, @playerName, @newsTitle, @newsContents, @newsDate, @playerImage, @teamImage)";
            myCommand.Parameters.AddWithValue("@playerTeam", xxx.playerTeam);
            myCommand.Parameters.AddWithValue("@playerPosition", xxx.playerPosition);
            myCommand.Parameters.AddWithValue("@playerName", xxx.playerName);
            myCommand.Parameters.AddWithValue("@newsTitle", xxx.newsTitle);
            myCommand.Parameters.AddWithValue("@newsContents", xxx.newsContents);
            myCommand.Parameters.AddWithValue("@newsDate", xxx.newsDate);
            myCommand.Parameters.AddWithValue("@playerImage", xxx.playerImage);
            myCommand.Parameters.AddWithValue("@teamImage", xxx.teamImage);
            int recordsAffected = myCommand.ExecuteNonQuery();  //執行以上的程式
            cnn.Close();
        }

        [HttpGet]
        [Route("getNewsTitle")]
        public List<string> getNewsTitle()
        {
            SqlServer sql = new SqlServer();
            SqlConnection cnn = new SqlConnection(sql.connectTo());
            cnn.Open();
            SqlCommand myCommand = new SqlCommand();
            myCommand = cnn.CreateCommand();
            myCommand.CommandType = CommandType.StoredProcedure;//宣告為SP
            myCommand.CommandText = "getNewsTitle";
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            cnn.Close();
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            List<string> newsTitle = new List<string>();
            foreach (DataRow myRow in myDataSet.Tables[0].Rows)
            {
                newsTitle.Add(myRow[0].ToString());
            }
            return newsTitle;
        }
        
        [HttpGet]
        [Route("getNews")]
        // use in APP
        public List<playerNews> getNews()
        {
            SqlServer sql = new SqlServer();
            SqlConnection cnn = new SqlConnection(sql.connectTo());
            cnn.Open();
            SqlCommand myCommand = new SqlCommand();
            myCommand = cnn.CreateCommand();
            myCommand.CommandType = CommandType.StoredProcedure;//宣告為SP
            myCommand.CommandText = "getNews";
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            cnn.Close();
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            List<playerNews> newsList = new List<playerNews>();
            foreach (DataRow myRow in myDataSet.Tables[0].Rows)
            {
                // The order of myRow totally followed by the same as in SQL
                playerNews player = new playerNews();
                player.recID = Convert.ToInt32(myRow[0]);
                player.playerTeam = myRow[1].ToString();
                player.playerPosition = myRow[2].ToString();
                player.playerName = myRow[3].ToString();
                player.newsTitle = myRow[4].ToString();
                player.newsContents = myRow[5].ToString();
                player.newsDate = myRow[6].ToString();
                player.playerImage = myRow[7].ToString();
                player.teamImage = myRow[8].ToString();
                newsList.Add(player);
            }
            return newsList;
        }
    }
    public class SqlServer
    {
        public string connectTo()
        {
            string connetionString = $@"data source=localhost;
                                initial catalog=Baseball;user id=sa;
                                persist security info=True;
                                password=1qaz2wsx;
                                workstation id=LAPTOP-CSG847EV;
                                packet size=4096;";
            return connetionString;
        }
        public DataSet getDataByText(string commandText)
        {
            SqlServer sql = new SqlServer();
            SqlConnection cnn = new SqlConnection(sql.connectTo());
            cnn.Open();
            SqlCommand myCommand = new SqlCommand();
            myCommand = cnn.CreateCommand();
            myCommand.CommandType = CommandType.Text;
            myCommand.CommandText = @commandText;
            myCommand.ExecuteNonQuery();
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            cnn.Close();
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet);
            return myDataSet;
        }
    }
}
