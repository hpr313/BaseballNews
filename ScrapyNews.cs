using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedProject.Model;
using OpenQA.Selenium.Firefox;

namespace Baseball_Project.Model
{
    public class ScrapyNews
    {
        string path0 = "//*[@id=\"Page-content\"]/div[2]/main/bsp-player-news-module/form/div/ul";
        string pathNews = "//*[@id=\"Page-content\"]/div[2]/main/bsp-player-news-module/form/div/ul/li";
        public string apiURL(string _name)
        {
            //return $"http://localhost:80/baseballnews/api/home/{_name}/";
            return $"http://localhost:32361/api/home/{_name}/";
        }
        internal WebDriver webDriver()
        {
            FirefoxOptions options = new FirefoxOptions();
            options.AddArguments("--headless");
            WebDriver driver = new FirefoxDriver(@"firefox\geckodriver.exe", options);
            //WebDriver driver = new FirefoxDriver();
            return driver;
        }
        public void goNewsURL(IWebDriver driver, int totalPage)
        {
            string url = $"https://www.nbcsports.com/fantasy/baseball/player-news?p={totalPage}";      
            driver.Navigate().GoToUrl(url);
        }

        public IList<IWebElement> getNewsList(IWebDriver driver)
        {
            Thread.Sleep(3000);           
            IWebElement news = driver.FindElement(By.XPath(path0));            
            IList<IWebElement> newsList = news.FindElements(By.XPath(pathNews));
            return newsList;
        }
        public string getTitle(IWebDriver driver, int i)
        {
            ScrapyNews scrapy = new ScrapyNews();
            string xpath_title = path0 + $"/li[{i}]/div/div[2]/div[1]";
            string newsTitle = scrapy.getNewsTitle(driver, xpath_title); 
            return newsTitle;
        }
        public void getNewsAndInsert(IWebDriver driver, int i)
        {
            ScrapyNews scrapy = new ScrapyNews();
            playerNews pNews = new playerNews();
            pNews.newsTitle = scrapy.getTitle(driver, i);
            pNews.playerName = scrapy.getPlayerName(driver, i);
            pNews.teamImage = scrapy.getTeamImage(driver, i);
            pNews.playerTeam = scrapy.getPlayerTeam(driver, i);
            pNews.playerPosition = scrapy.getPlayerPosition(driver, i);
            pNews.playerTeam = scrapy.getPlayerTeam(driver, i);
            pNews.newsContents = scrapy.getNewsContent(driver, i);
            pNews.playerImage = scrapy.getPlayerImage(driver, i);
            pNews.newsDate = scrapy.getDatetime(driver, i);
            insert_postAPI(pNews);
        }
        public List<playerNews> getNews()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(apiURL("getNews")).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            List<playerNews> data = JsonConvert.DeserializeObject<List<playerNews>>(responseBody);
            return data;
        }
        public void insert_postAPI(playerNews xxx)
        {
            HttpClient client = new HttpClient();
            string jsonText = JsonConvert.SerializeObject(xxx);
            var requestContent = new StringContent(jsonText, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiURL("insertData"), requestContent).Result; //雖然沒有response，但需要post出去
        }

        public HashSet<string> getTitles()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(apiURL("getNewsTitle")).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            List<string> data = JsonConvert.DeserializeObject<List<string>>(responseBody);

            HashSet<string> hs = new HashSet<string>();
            for (int i = 0; i < data.Count; i++) { hs.Add(data[i]); }
            return hs;
        }
        public bool checkTitle(HashSet<string> hs, string title)
        {
            if (!hs.Contains(title)) { return true; }
            else { return false; }
        }
        public string getPlayerImage(IWebDriver webDriver, int i)
        {
            string xpath_playerImage = path0 + $"/li[{i}]/div/div[1]/div[1]/a/picture/img";
            string playerImage;
            try { IWebElement pImage = webDriver.FindElement(By.XPath(xpath_playerImage));
                playerImage = pImage.GetAttribute("src").ToString();
            } catch { playerImage = "https://www.nbcsportsedge.com/sites/default/files/players/124/8798.jpg"; }
            return playerImage;
        }
        public string getNewsTitle(IWebDriver webDriver, string path_nTitle)
        {
            IWebElement player_newsTitle = webDriver.FindElement(By.XPath(path_nTitle));
            return player_newsTitle.Text;
        }

        public string getNewsContent(IWebDriver webDriver, int i)
        {
            string xpath_content = path0 + $"/li[{i}]/div/div[2]/div[2]";
            string newsContent = webDriver.FindElement(By.XPath(xpath_content)).Text;
            return newsContent;
        }
        public string getPlayerName(IWebDriver webDriver, int i)
        {
            string xpath_firstName = path0 + $"/li[{i}]/div/div[1]/div[2]/div[1]/a/span[1]";
            string xpath_lastName = path0 + $"/li[{i}]/div/div[1]/div[2]/div[1]/a/span[2]";

            string alt_xpath_firstName = path0 + $"/li[{i}]/div/div[1]/div[2]/div[1]/span[1]";
            string alt_xpath_lastName = path0 + $"/li[{i}]/div/div[1]/div[2]/div[1]/span[2]";

            string firstName;
            string lastName;
            try { firstName = webDriver.FindElement(By.XPath(xpath_firstName)).Text; }
            catch { firstName = webDriver.FindElement(By.XPath(alt_xpath_firstName)).Text; }
            try { lastName = webDriver.FindElement(By.XPath(xpath_lastName)).Text; }
            catch { lastName = webDriver.FindElement(By.XPath(alt_xpath_lastName)).Text; }

            return firstName + " " + lastName;
        }

        public string getPlayerTeam(IWebDriver webDriver, int i)
        {
            string team;
            IWebElement list = singleList(webDriver, i);
            try { team = list.FindElement(By.ClassName("PlayerNewsPost-team-abbr")).Text; }
            catch { team = ""; }
            return team;
        }
        public string getPlayerPosition(IWebDriver webDriver, int i)
        {
            string position;
            IWebElement list = singleList(webDriver, i);
            try { position = list.FindElement(By.ClassName("PlayerNewsPost-position")).Text; }
            catch { position = ""; }
            return position;
        }
        public string getTeamImage(IWebDriver webDriver, int i)
        {
            string tImage;
            IWebElement list = singleList(webDriver, i);
            try
            {
                IWebElement team_image = list.FindElement(By.ClassName("PlayerNewsPost-team-logo"));
                IWebElement image = team_image.FindElement(By.ClassName("Image"));
                tImage = image.GetAttribute("src").ToString();
            }
            catch { tImage = ""; }
            return tImage;
        }
        public string getDatetime(IWebDriver webDriver, int i)
        {
            IWebElement list = singleList(webDriver, i);
            IWebElement news_date = list.FindElement(By.ClassName("PlayerNewsPost-date"));
            string[] date = news_date.GetAttribute("data-date").ToString().Split('.');
            string newsDate = date[0];
            return newsDate;
        }
        public IWebElement singleList(IWebDriver webDriver, int i)
        {
            string path_list = path0 + $"/li[{i}]";
            IWebElement list = webDriver.FindElement(By.XPath(path_list));
            return list;
        }
    }
}
