using System;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

/*******************************************************************************
   ______________________________GitHubScraper__________________________________
 * This program uses Selenium's WebDriver & phantomJS, in order to scrape the
 * contributions, fills, and active dates from a Github user's profile.
 *
 * To configure this file for your own use, replace the "Username" variable with
 * your own GitHub username.

 ******************************************************************************/

namespace GitHubScraper {

    class GraphScraper {

        private static readonly int numWeeks = 10;

        static void Main() {
            Console.SetWindowSize(50, 15);
            string username = File.ReadAllText(GetDirectory("/config.txt"));
            Console.WriteLine("Username: " + username);
            var driverService = PhantomJSDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(driverService);
            driver.Navigate().GoToUrl("http://github.com/" + username);
            Console.WriteLine("Connected.");
            Block[,] data = new Block[numWeeks, 7];
            try {
                var calendar = driver.FindElement(By.ClassName("js-calendar-graph"))
                                     .FindElements(By.TagName("g"));
                int range = calendar.Count - numWeeks;
                for (int i = range; i < calendar.Count; i++) {
                    int week = i - range;
                    Console.Write('\r' + "Grabbing data..." + (i - range) * 10 + "%");
                    var rectElements = calendar[i].FindElements(By.TagName("rect"));
                    int day = 0;
                    foreach (var rect in rectElements) {
                        string fill = rect.GetAttribute("fill");
                        int contribCount = int.Parse(rect.GetAttribute("data-count"));
                        string date = rect.GetAttribute("data-date");
                        data[week, day] = new Block(contribCount, fill, date);
                        day++;
                    }
                }
                Console.WriteLine('\r' + "Grabbing data...100%");
            } catch (NoSuchElementException e) {
                Console.WriteLine(e);
            }
            driver.Quit();   // close the webdriver
            driver.Dispose();
            FillWeek(data);  // add placeholders to fill current week
            Console.WriteLine("Done.");
            for(int i = 0; i < numWeeks; i++) // write data to file 
                WriteToFile(GetWeek(data, i), GetDirectory("/data.txt"));
            WriteToFile(GetWeek(data, numWeeks-1), GetDirectory("/week.txt"));
            Environment.Exit(0); // Exit program
        }


        /* returns @Resource directory + filename */
        private static string GetDirectory(string filename) { 
            string path = Directory.GetCurrentDirectory();
            return path.Substring(0, path.Length - 37) + filename; 
        }


        /* create placeholders for the rest of the week */
        private static void FillWeek(Block[,] data) {
            int tbd = 6 - (int)DateTime.Now.DayOfWeek;
            while(tbd > 0) {
                int x = 6 - tbd;
                data[numWeeks-1, x] = new Block(0, "#eeeee", "TBD");
                tbd--;
            }
        }

        /* returns the week at the given index */
        private static Block[] GetWeek(Block[,] data, int index) {
            Block[] weekData = new Block[7];
            if(index > 0 && index < numWeeks)
                for(int i = 0; i < 7; i++)
                    weekData[i] = data[index, i];
            return weekData;
        }

        /* Writes the data to the provided .txt file */
        private static void WriteToFile(Block[] data, string dir) {
            StreamWriter file = new StreamWriter(dir);
            foreach(Block b in data)
                if(b != null)
                    file.WriteLine(b.ToString());
            file.Close();
        }


        private class Block {

            private readonly int _contribs;
            private readonly string _fill;
            private readonly string _date;

            public Block(int contribs, string fill, string date) {
                _contribs = contribs;
                _fill = fill;
                _date = FormatDate(date);
            }

            private string FormatDate(string dataDate) {
                if (dataDate.Equals("TBD"))
                    return "TBD";
                int year = int.Parse(dataDate.Substring(2, 2));
                int month = int.Parse(dataDate.Substring(5, 2));
                int day = int.Parse(dataDate.Substring(8, 2));
                return month + "/" + day + "/" + year;
            }

            public override string ToString() {
                StringBuilder sb = new StringBuilder("");
                sb.Append(_contribs);
                sb.Append(Environment.NewLine);
                sb.Append(_fill);
                sb.Append(Environment.NewLine);
                sb.Append(_date);
                return sb.ToString();
            }
        }

    }
}
