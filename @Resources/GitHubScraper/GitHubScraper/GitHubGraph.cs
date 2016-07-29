using System;
using System.Collections;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

/************************************************************************************ 
   _________________________________GitHubScraper__________________________________
 * This program uses Selenium's WebDriver & phantomJS, in order to scrape the 
 * contributions, fills, and active dates from a Github user's profile.
 *
 * To configure this file for your own use, replace the "Username" variable with 
 * your own GitHub username.

 ************************************************************************************/

namespace GitHubScraper {

    class GraphScraper {

        private static readonly string[] DaysofWeek = { "Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sat" };
        public static DateTime ThisDay = DateTime.Today;

        static void Main() {
            string username = File.ReadAllText(GetDirectory() + "/config.txt");
            Console.WriteLine("Username: " + username);
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(driverService);
            driver.Navigate().GoToUrl("http://github.com/" + username);
            ArrayList list = new ArrayList();
            try {
                Console.WriteLine("Connected.");
                var contribCalendar = driver.FindElement(By.Id("contributions-calendar"))
                                            .FindElement(By.ClassName("js-calendar-graph"))
                                            .FindElements(By.TagName("g"));
                Console.WriteLine("Grabbing data...");
                for (int i = contribCalendar.Count - 10; i < contribCalendar.Count; i++){
                    Console.Write("*");
                    var week = contribCalendar[i];
                    var rectElements = week.FindElements(By.TagName("rect"));
                    foreach (var rect in rectElements){
                        string fill = rect.GetAttribute("fill");
                        int contribCount = int.Parse(rect.GetAttribute("data-count"));
                        string date = rect.GetAttribute("data-date");
                        Block b = new Block(contribCount, fill, date);
                        list.Add(b);
                    }
                }
                Console.WriteLine("");
            } catch (NoSuchElementException e) {
                Console.WriteLine(e);
            }
            driver.Quit(); //close the webdriver & phantomJS  
            driver.Dispose();

            FillWeek(list);
            ArrayList weekData = GetCurrentWeek(list); 
            WriteToFile(list, "/data.txt");
            WriteToFile(weekData, "/week.txt");
            Console.WriteLine("Done.");
            Environment.Exit(0);
        }


        private static string GetDirectory(){
            string path = Directory.GetCurrentDirectory();
            return path.Substring(0, path.Length - 37);
        }

        private static void FillWeek(ArrayList list) {
            int tba = 6;
            for(int i = 0; i < DaysofWeek.Length; i++)
                if (DaysofWeek[i] == DateTime.Now.ToString("ddd"))
                    tba -= i;
            while(tba > 0){
                // create placeholders for the rest of the week
                Block b = new Block(0, "#eeeee", "TBD");
                list.Add(b);
                tba--;
            }
        }

        private static ArrayList GetCurrentWeek(ArrayList list){
            ArrayList weekData = new ArrayList();
            for(int i = list.Count-8; i < list.Count; i++)
                weekData.Add(list[i]);
            return weekData;
        }

        private static void WriteToFile(ArrayList list, string filename) {
            StreamWriter file = new StreamWriter(GetDirectory() + filename);
            foreach (Block b in list)
                file.WriteLine(b.ToDataString());
            file.Close();
        }


        private class Block {
            private readonly int _contributions;
            private readonly string _fill;
            private int _day;
            private int _month;
            private int _year;

            public Block(int contributions, string fill, string date){
                _contributions = contributions;
                _fill = fill;
                GetInfo(date);
            }

            private void GetInfo(string dataDate){
                if (dataDate.Equals("TBD")){
                    _year = -1;
                }
                else {
                    _year = int.Parse(dataDate.Substring(0, 4));
                    _month = int.Parse(dataDate.Substring(5, 2));
                    _day = int.Parse(dataDate.Substring(8, 2));
                }
            }

            private string GetDate(){
                if (_year < 0)
                    return "TBD";
                return _month + "/" + _day + "/" + _year;
            }

            public string ToDataString(){
                StringBuilder sb = new StringBuilder("");
                sb.Append(_contributions);
                sb.Append(Environment.NewLine);
                sb.Append(_fill);
                sb.Append(Environment.NewLine);
                sb.Append(GetDate());
                return sb.ToString();
            }
        }

    }
}
