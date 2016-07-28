using System;
using System.Collections;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

/*********************************************************************************** 
   _________________________________GitHubScraper_________________________________
 * This program uses Selenium's WebDriver & phantomJS, in order to scrape the 
 * contributions, fills, and active dates from a Github user's profile.
 *
 * To configure this file for your own use, replace the "Username" variable with 
 * your own GitHub username.

 ***********************************************************************************/

namespace GitHubScraper {

    class GraphScraper {

        private static readonly string Username = "JonSn0w";  /* <--- Change this -- */
        private static readonly string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        private static readonly string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public static DateTime Today { get; }
        public static DateTime ThisDay = DateTime.Today;
        private static int count;

        static void Main(){
            string currentPath = GetDirectory();
            currentPath.Substring(0, currentPath.Length - 9);
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(driverService);
            driver.Navigate().GoToUrl("http://github.com/" + Username);
            ArrayList list = new ArrayList();
            // Stack stack = new Stack();
            try {
                var calendarElements = driver.FindElement(By.Id("contributions-calendar"))
                                             .FindElement(By.ClassName("js-calendar-graph"))
                                             .FindElements(By.TagName("g"));
                int remainder = 0;
                for (int i = 0; i < Days.Length; i++){
                    if (Days[i] == DateTime.Now.ToString("dddd"))
                        remainder = 6 - i;
                }
                foreach (var calElement in calendarElements){
                    if (count < calendarElements.Count - 10)
                        count++;
                    else {
                        var rectElements = calElement.FindElements(By.TagName("rect"));
                        foreach (var rect in rectElements){
                            string color = rect.GetAttribute("fill");
                            int contribCount = int.Parse(rect.GetAttribute("data-count"));
                            string dataDate = rect.GetAttribute("data-date");
                            Block b = new Block(contribCount, color, dataDate);
                            // stack.Push(b);
                            list.Add(b);
                        }
                    }
                }
                driver.Quit(); //close the webdriver & phantomJS

                // create placeholders for the rest of the week
                while (remainder > 0){
                    Block b = new Block(0, "#eeeee", "TBD");
                    // stack.Push(b);
                    list.Add(b);
                    remainder--;
                }
                WriteToFile(list, "/data.txt");
                // WriteToFile(stack, "/data.txt");
            }
            catch (NoSuchElementException e){
                
            }
            driver.Quit();
            driver.Dispose();
        }

        private static string GetDirectory(){
            string path = Directory.GetCurrentDirectory();
            return path.Substring(0, path.Length - 9);
        }

        private static void WriteToFile(ArrayList list, string filename) {
            string path = GetDirectory();
            path = path.Substring(0, path.Length - 28) + filename;
            StreamWriter file = new StreamWriter(path);
            foreach (Block b in list){
                file.WriteLine(b.ToDataString());
            }
            file.Close();
        }


        class Block{
            private int contributions;
            private string fill;
            private int day;
            private string dayOfWeek;
            private int month;
            private int year;

            public Block(int contributions, string fill, string date){
                this.contributions = contributions;
                this.fill = fill;
                GetInfo(date);
            }

            private void GetInfo(string dataDate){
                if (dataDate.Equals("TBD")){
                    year = -1;
                }
                else {
                    year = int.Parse(dataDate.Substring(0, 4));
                    month = int.Parse(dataDate.Substring(5, 2));
                    day = int.Parse(dataDate.Substring(8, 2));
                }
            }

            private string GetDate(){
                if (year < 0)
                    return "TBD";
                return month + "/" + day + "/" + year;
            }

            public string ToDataString(){
                StringBuilder sb = new StringBuilder("");
                sb.Append(contributions);
                sb.Append(Environment.NewLine);
                sb.Append(fill);
                sb.Append(Environment.NewLine);
                sb.Append(GetDate());
                return sb.ToString();
            }

        }

    }

}
