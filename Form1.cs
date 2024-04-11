using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Baseball_Project.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SharedProject.Model;

namespace Baseball_Project
{
    public partial class Form1 : Form
    {
        //public string season; //Cross thread....
        IList<IWebElement> iList; // List of ScrapyStacast 
        IList<IWebElement> newsList; // List of news
        int totalPage; //total page to get news
        List<string> playerNames;

        public Form1()
        {
            InitializeComponent();
            getcbBoxName();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            setController(false);
            newsBgWorker.RunWorkerAsync();
            labelStuation.Text = "Getting News.....";
            labelTotal.Text = "News Amount: " + ((int)numBoxPages.Value * 10).ToString();
        }
        private string getType()
        {
            if (radioBtn_pitcher.Checked) { return radioBtn_pitcher.Text; }
            else { return radioBtn_batter.Text; }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            if (txtBox_id.Text.Length == 6)
            {
                try
                {
                    setController(false);
                    int.Parse(txtBox_id.Text);
                    ScrapyStacast stacast = new ScrapyStacast();
                    WebDriver driver = stacast.webDriver();
                    labelStuation.Text = "Searching Player.....";
                    stacast.searchCond(driver, getType(), cbBox_season.Text);
                    Thread.Sleep(5000);
                    labelStuation.Text = "Openning the Data Table.....";
                    iList = stacast.getDataBodyList(driver, txtBox_id.Text);
                    labelTotal.Text = "Data Amount: " + iList.Count().ToString();
                    labelStuation.Text = "Scrapying Data.....";
                    backgroundWorker1.RunWorkerAsync();
                }
                catch { display("Work cancelled or input correct ID format"); }//txtBox_id.Clear(); }
            }
            else { display("Please input correct ID format"); }//txtBox_id.Clear(); }
        }                                              

        private void button3_Click(object sender, EventArgs e)
        {
            ScrapyStacast stacast = new ScrapyStacast();
            WebDriver driver = stacast.webDriver();
            stacast.searchCond(driver, getType(), cbBox_season.Text);
            Thread.Sleep(5000);
            List<int> id = stacast.getIDs(driver);
            List<string> name = stacast.getNames(driver, getType());
            List<string> playerImage = stacast.getImage(driver);
            for (int i = 0; i < id.Count; i++)
            {
                stacast.insertPlayerInfo(id[i], name[i], playerImage[i]);
            }
            getcbBoxName();
            MessageBox.Show("Update Complete!");
        }

        private void setID()
        {
            ScrapyStacast statcast = new ScrapyStacast();
            string id = statcast.getID(@cbBox_Name.Text);
            if(id == "") { display($"There is no such player: {cbBox_Name.Text}. Please try again"); }
            else { txtBox_id.Text = id; }
        }

        private List<string> initail_name()
        {
            ScrapyStacast statcast = new ScrapyStacast();
            List<string> names = statcast.getAllName();
            return names;
        }

        private void cbBox_Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter) { setID(); }
        }

        public void getcbBoxName()
        {
            playerNames = initail_name();
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            for (int i = 0; i < playerNames.Count; i++)
            {
                if (playerNames[i].Contains(cbBox_Name.Text.ToUpper())) { col.Add(playerNames[i]); cbBox_Name.Items.Add(playerNames[i]); }
            }
            cbBox_Name.AutoCompleteCustomSource = col;
        }

        private void btn_searchID_Click(object sender, EventArgs e)
        {
            setID();
            //try { setID(); }
            ////catch { initialForm("There is no such player. Please try again"); }
            //catch { display("There is no such player. Please try again"); }
        }

        private void btn_selsql_Click(object sender, EventArgs e)
        {
            ExportData data = new ExportData();
            DataTable ds = data.dataToView(txtBox_id.Text);
            dataGridView.DataSource = ds;
        }

        private void btn_toCSV_Click(object sender, EventArgs e)
        {
            ExportData data = new ExportData();
            DataTable dt = data.dataToView(txtBox_id.Text);
            string name = cbBox_Name.Text;
            string path = $"C:\\Users\\user\\PycharmProjects\\BaseballProject\\{name}.csv";
            data.toCSV(dt, path);
            MessageBox.Show($"Successfullly Exported to {path}!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string url = $"https://www.nbcsportsedge.com/baseball/mlb/player-news?category=all&page=5";
            
            try { WebDriver driver = new ChromeDriver(@"chromedriver");
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(5000);
                IWebElement ctsBtn1 = driver.FindElement(By.ClassName("_2Sbg_-vS"));
                ctsBtn1.Click();
                Thread.Sleep(5000);
            }
            catch { MessageBox.Show("Please Update the Version of Driver"); }


            // 使用 HTTP 請求函數獲取網站回傳的 HTML 內容
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            // 使用正則表達式解析 HTML 內容，找到標題
            string pattern = @"<title>(.*?)</title>";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(html);

            // 提取標題內容，並輸出到控制台
            string title = match.Groups[1].Value;

            //ScrapyNews scrapy = new ScrapyNews();
            //IWebDriver driver = scrapy.webDriver();
            //for (int j = totalPage; j >= 1; j--)
            //{
            //    scrapy.goNewsURL(driver, j);
            //    Thread.Sleep(5000);
            //    IWebElement ctsBtn1 = driver.FindElement(By.ClassName("_2Sbg_-vS"));
            //    ctsBtn1.Click();
            //    Thread.Sleep(5000);

            //    IWebElement _ctsBtn2 = driver.FindElement(By.ClassName("btn-label"));
            //    ((IJavaScriptExecutor)driver).ExecuteScript("argument[0].click();", _ctsBtn2);
                
            //    newsList = scrapy.getNewsList(driver);
            //    //scrapy.getInsertData();
            //    for (int i = newsList.Count(); i >= 1; i--)
            //    {

            //        scrapy.getNewsAndInsert(driver, i);

            //    }
            //}
        }
        public void scrapyFunc(int i)
        {
            ScrapyStacast stacast = new ScrapyStacast();
            stacast.getData(iList[i-1], txtBox_id.Text, i);
            //ScrapyStacast stacast = new ScrapyStacast();
            //for (int i = 1; i <= iList.Count; i++)
            //{
            //    stacast.getData(iList[i], txtBox_id.Text, i);
               
            //}
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ScrapyStacast stacast = new ScrapyStacast();
            HashSet<string> videoLinks = stacast.getVideoLinks(int.Parse(txtBox_id.Text));
            for (int i = 1; i <= iList.Count; i++)
            {
                if (backgroundWorker1.CancellationPending) { e.Cancel = true; } // cancel
                else {
                    if(stacast.checkVideoLink(stacast.getLink(iList[i - 1], txtBox_id.Text, i), videoLinks)) { 
                        scrapyFunc(i); backgroundWorker1.ReportProgress(i); }
                    else { backgroundWorker1.ReportProgress(i); }
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Maximum = iList.Count();
            progressBar1.Value = e.ProgressPercentage;
            labelGetting.Text = "Getting Data Count: " + e.ProgressPercentage.ToString();

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) { initialize(); display("Work cancelled"); }
            else { initialize(); display("Work Completed successfully!"); }
            setController(true);
        }

        public void display(string text) { MessageBox.Show(text); }

        private void initialize()
        {
            progressBar1.Value = 0;
            labelTotal.Text = "Data Amount: ";
            labelGetting.Text = "Getting Data Count: ";
            labelStuation.Text = "Stand By.....";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            newsBgWorker.CancelAsync();
        }

        private void newsBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            totalPage = (int)numBoxPages.Value;
            ScrapyNews scrapy = new ScrapyNews();
            HashSet<string> titles = scrapy.getTitles();
            //try { 
                IWebDriver driver = scrapy.webDriver();
                int k = 0;
                if (newsBgWorker.CancellationPending) { e.Cancel = true; } // cancel
                else //{ scrapyFunc(i); backgroundWorker1.ReportProgress(i); }
                {
                    for (int j = totalPage; j >= 1; j--)
                    {
                        if (newsBgWorker.CancellationPending) { e.Cancel = true; }
                        else
                        {
                            scrapy.goNewsURL(driver, j);
                            newsList = scrapy.getNewsList(driver);
                            //scrapy.getInsertData();
                            for (int i = newsList.Count(); i >= 1; i--)
                            {
                                if (newsBgWorker.CancellationPending) { e.Cancel = true; }
                                else
                                {
                                    //string title = scrapy.getTitle(driver, i);
                                    bool check = scrapy.checkTitle(titles, scrapy.getTitle(driver, i));
                                    if (scrapy.checkTitle(titles, scrapy.getTitle(driver, i)))
                                    {
                                        scrapy.getNewsAndInsert(driver, i);
                                        k += 1;
                                        newsBgWorker.ReportProgress(k);
                                    }
                                    else
                                    {
                                        k += 1;
                                        newsBgWorker.ReportProgress(k);
                                    }

                                }
                            }
                        }
                    }
                }
            //}
            //catch { MessageBox.Show("Please Update the Version of Driver");}
        }

        private void newsBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Maximum = totalPage * 10;
            progressBar1.Value = e.ProgressPercentage;
            labelGetting.Text = "Getting News Count: " + e.ProgressPercentage.ToString();
        }

        private void newsBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) { initialize(); display("Work cancelled"); }
            else { initialize(); display("Work Completed successfully!"); }
            setController(true);
        }
        private void setController(bool b)
        {
            button1.Enabled = b;
            cbBox_Name.Enabled = b;
            cbBox_season.Enabled = b;
            btn_searchID.Enabled = b;
            txtBox_id.Enabled = b;
            button2.Enabled = b;
            button3.Enabled = b;
            btn_selsql.Enabled = b;
            numBoxPages.Enabled = b;
            btn_toCSV.Enabled = b;
            button4.Enabled = b;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playerNames = initail_name();
        }
    }
}
