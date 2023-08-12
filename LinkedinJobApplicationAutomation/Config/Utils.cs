using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LinkedinJobApplicationAutomation.Config
{
    public static class Utils
    {
        public static FirefoxOptions browserOptions()
        {
            var options = new FirefoxOptions();
            var firefoxProfileRootDir = Config.FirefoxProfileRootDir;
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-gpu");
            if (Config.Headless)
            {
                options.AddArgument("--headless");
            }

            options.AddArgument("--disable-blink-features");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--incognito");
            options.AddArgument("-profile");
            options.AddArgument(firefoxProfileRootDir);

            return options;
        }

        public static void prRed(string prt)
        {
            Console.WriteLine($"\033[91m{prt}\033[00m");
        }

        public static void prGreen(string prt)
        {
            Console.WriteLine($"\033[92m{prt}\033[00m");
        }

        public static void prYellow(string prt)
        {
            Console.WriteLine($"\033[93m{prt}\033[00m");
        }

        public static List<string> getUrlDataFile()
        {
            var urlData = new List<string>();
            try
            {
                var file = new StreamReader("data/urlData.txt");
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    urlData.Add(line);
                }
                file.Close();
            }
            catch (FileNotFoundException)
            {
                var text = "FileNotFound:urlData.txt file is not found. Please run ./data folder exists and check config.py values of yours. Then run the bot again";
                prRed(text);
            }
            return urlData;
        }

        public static int jobsToPages(string numOfJobs)
        {
            var number_of_pages = 1;

            if (numOfJobs.Contains(" "))
            {
                var spaceIndex = numOfJobs.IndexOf(' ');
                var totalJobs = numOfJobs.Substring(0, spaceIndex);
                var totalJobs_int = int.Parse(totalJobs.Replace(",", ""));
                number_of_pages = (int)Math.Ceiling((double)(totalJobs_int));
                if (number_of_pages > 40)
                {
                    number_of_pages = 40;
                }
            }
            else
            {
                number_of_pages = int.Parse(numOfJobs);
            }

            return number_of_pages;
        }

        public static List<string> urlToKeywords(string url)
        {
            var keywordUrl = url.Substring(url.IndexOf("keywords=") + 9);
            var keyword = keywordUrl.Substring(0, keywordUrl.IndexOf("&"));
            var locationUrl = url.Substring(url.IndexOf("location=") + 9);
            var location = locationUrl.Substring(0, locationUrl.IndexOf("&"));
            return new List<string> { keyword, location };
        }

        public static void writeResults(string text)
        {
            var timeStr = DateTime.Now.ToString("yyyyMMdd");
            var fileName = "Applied Jobs DATA - " + timeStr + ".txt";
            try
            {
                var lines = new List<string>();
                using (var file = new StreamReader("data/" + fileName, System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (!line.Contains("----"))
                        {
                            lines.Add(line);
                        }
                    }
                }

                using (var f = new StreamWriter("data/" + fileName, false, System.Text.Encoding.UTF8))
                {
                    f.WriteLine("---- Applied Jobs Data ---- created at: " + timeStr);
                    f.WriteLine("---- Number | Job Title | Company | Location | Work Place | Posted Date | Applications | Result");
                    foreach (var line in lines)
                    {
                        f.WriteLine(line);
                    }
                    f.WriteLine(text);
                }
            }
            catch (FileNotFoundException)
            {
                using (var f = new StreamWriter("data/" + fileName, false, System.Text.Encoding.UTF8))
                {
                    f.WriteLine("---- Applied Jobs Data ---- created at: " + timeStr);
                    f.WriteLine("---- Number | Job Title | Company | Location | Work Place | Posted Date | Applications | Result");
                    f.WriteLine(text);
                }
            }
        }

        public static void printInfoMes(string bot)
        {
            prYellow("ℹ️ " + bot + " is starting soon... ");
        }

        public static void donate()
        {
            //prYellow("If you like the project, please support me so that I can make more such projects, thanks!");
            //try
            //{
            //    driver.Navigate().GoToUrl("https://commerce.coinbase.com/checkout/576ee011-ba40-47d5-9672-ef7ad29b1e6c");
            //}
            //catch (Exception e)
            //{
            //    prRed("Error in donate: " + e.Message);
            //}
        }


    }
}
