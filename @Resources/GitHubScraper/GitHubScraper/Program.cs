using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections;
using OpenQA.Selenium.PhantomJS;
using System.Text;
using System.IO;
using System.Diagnostics;

/* 
 * This program uses Selenium's WebDriver to scrape the active dates and the number 
 * of contributions made from a Github profile in the current week.
 *
 * To configure this file for your own use, replace the "username" variable with 
 * your own GitHub Username.
 */

namespace GitHubGraph {

    class GraphScraper {

        private static string username = "JonSn0w";  /* <--- Change this */
        private static string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public static DateTime Today { get; }
        public static DateTime thisDay = DateTime.Today;
        private static int blockCount = 1;
        private static int count = 0;

        static void Main(string[] args) {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            currentPath = currentPath.Substring(0, currentPath.Length - 9);
            var driverService = PhantomJSDriverService.CreateDefaultService(currentPath + "phantomJS");
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(driverService);
            driver.Navigate().GoToUrl("http://github.com/" + username);
            ArrayList list = new ArrayList();
            ArrayList week = new ArrayList();
            try {
                var calendarElements = driver.FindElement(By.Id("contributions-calendar"))
                                            .FindElement(By.ClassName("js-calendar-graph"))
                                            .FindElements(By.TagName("g"));
                int remainder = 0;
                for (int i = 0; i < days.Length; i++) {
                    if (days[i] == System.DateTime.Now.ToString("dddd"))
                        remainder = 6 - i;
                }
                foreach (var calElement in calendarElements) {
                    if (count < calendarElements.Count - 10)
                        count++;
                    else if (calElement != calendarElements[calendarElements.Count - 1]) {
                        var rectElements = calElement.FindElements(By.TagName("rect"));
                        foreach (var rect in rectElements) {
                            string color = rect.GetAttribute("fill");
                            int contribCount = Int32.Parse(rect.GetAttribute("data-count"));
                            string dataDate = rect.GetAttribute("data-date");
                            Block b = new Block(blockCount, contribCount, color, dataDate);
                            list.Add(b);
                            blockCount++;
                        }
                    }
                    else {
                        //var thisWeek = calendarElements[calendarElements.Count - 1];
                        var rectElements = calElement.FindElements(By.TagName("rect"));
                        foreach (var rect in rectElements)
                        {
                            string color = rect.GetAttribute("fill");
                            int contribCount = Int32.Parse(rect.GetAttribute("data-count"));
                            string dataDate = rect.GetAttribute("data-date");
                            Block b = new Block(blockCount, contribCount, color, dataDate);
                            week.Add(b);
                            list.Add(b);
                            blockCount++;
                        }
                    }
                }
                // create placeholders for the rest of the week
                while (remainder > 0) { 
                    blockCount++;
                    Block b = new Block(blockCount, 0, "#eeeee"," ");
                    week.Add(b);
                    list.Add(b);
                    remainder--;
                }
                writeToFile(list, "/GraphInfo.txt");
                writeToFile(week, "/GraphInfoWeek.txt");
            }
            catch (NoSuchElementException e) {
                return;
            }
            //Console.WriteLine("Finished.");
            Environment.Exit(0);
        }


        private static void writeToFile(ArrayList list, String filename) {
            var path = System.IO.Directory.GetCurrentDirectory();
            var root = Path.GetDirectoryName(Path.GetDirectoryName(path));
            var ancestor = Path.GetDirectoryName(Path.GetDirectoryName(root));
            ancestor = ancestor + filename;
            //Console.WriteLine("PATH: " + path);
            //Console.WriteLine("TODAY: " + System.DateTime.Now.ToString("dddd"));
            System.IO.StreamWriter file = new System.IO.StreamWriter(ancestor);
            int writeCount = 1;
            foreach (Block item in list) {
               if (writeCount < 50)
                    file.WriteLine("\"block\":{");
                else
                    file.WriteLine("\"block2\":{"); 
                file.WriteLine(item.toDataString());
                writeCount++;
            }
            file.Close();
        }


        class Block {

            int index;
            int contributions;
            string fill;
            int day;
            string dayOfWeek;
            int month;
            string monthStr;
            int year;

            public Block(int index, int contributions, string fill, string date) {
                this.index = index;
                this.contributions = contributions;
                this.fill = fill;
                getInfo(date);
            }

            public void getInfo(string dataDate) {
                if (dataDate.Length < 2) {
                    year = -1;
                } else {
                    year = Int32.Parse(dataDate.Substring(0, 4));
                    month = Int32.Parse(dataDate.Substring(5, 2));
                    day = Int32.Parse(dataDate.Substring(8, 2));
                    monthStr = months[month - 1];
                }
            }

            public int getDay() { return this.day; }
            public int getMonth() { return this.month; }
            public int getYear() { return this.year; }
            public string getDate() {
                if (year < 0)
                    return " ";
                return this.month + "/" + this.day + "/" + this.year;
            }
            public int getContributions() { return this.contributions; }

            public string toReadableString() {
                return months[this.month - 1] + ". " + this.day + ", " + this.year + "\n" + "Contributions: " + this.contributions;
            }

            public string toDataString() {
                StringBuilder sb = new StringBuilder("");
                //sb.Append("\"block\":{");
                sb.Append(Environment.NewLine);
                sb.Append("\t\"contribs\": \"");
                sb.Append(contributions);
                sb.Append("\"");
                sb.Append(Environment.NewLine);
                sb.Append("\t\"fill\": \"");
                sb.Append(fill);
                sb.Append("\"");
                sb.Append(Environment.NewLine);
                sb.Append("\t\"date\": \"");
                sb.Append(getDate());
                sb.Append("\"");
                sb.Append(Environment.NewLine);
                sb.Append("},");
                return sb.ToString();
            }
        }
    }

}
