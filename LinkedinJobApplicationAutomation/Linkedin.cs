
using LinkedinJobApplicationAutomation.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinkedinJobApplicationAutomation.Config
{
    public class Linkedin
    {
        private IWebDriver driver;

        public Linkedin()
        {
            string browser = Config.Browser.ToLower();
            string linkedinEmail = Config.Email;

            if (browser == "firefox")
            {
                if (string.IsNullOrEmpty(linkedinEmail))
                {
                    Console.WriteLine("On Linux you need to define profile path to run the bot with Firefox. Go about:profiles find root directory of your profile paste in line 8 of config file next to firefoxProfileRootDir");
                    Environment.Exit(0);
                }
                else
                {
                    FirefoxOptions options = new FirefoxOptions();
                    options.Profile = new FirefoxProfile();
                    driver = new FirefoxDriver(OpenQA.Selenium.Firefox.FirefoxDriverService.CreateDefaultService(), options);
                }
            }
            else if (browser == "chrome")
            {
                driver = new ChromeDriver();
            }
            else
            {
                Console.WriteLine("Invalid browser specified in config.");
                Environment.Exit(0);
            }

            driver.Navigate().GoToUrl("https://www.linkedin.com/login?trk=guest_homepage-basic_nav-header-signin");
            Console.WriteLine("Trying to log in to LinkedIn.");

            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //var usernameField = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("username")));
                var usernameField = driver.FindElement(By.Id("username"));
                usernameField.SendKeys(linkedinEmail);

                var passwordField = driver.FindElement(By.Id("password"));
                passwordField.SendKeys(Config.Password);

                var loginButton = driver.FindElement(By.XPath("//*[@id='organic-div']/form/div[3]/button"));
                loginButton.Click();

                Console.WriteLine("Logged in to LinkedIn.");
                Console.WriteLine("CAPTCHA DELAY");
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(Constants.CaptaTime));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GenerateUrls()
        {
            if (!System.IO.Directory.Exists("data"))
            {
                System.IO.Directory.CreateDirectory("data");
            }

            try
            {
                using (var file = new System.IO.StreamWriter("data/urlData.txt", false, System.Text.Encoding.UTF8))
                {
                    LinkedinUrlGenerate linkedinUrlGenerate = new LinkedinUrlGenerate();
                    var linkedinJobLinks = linkedinUrlGenerate.generateUrlLinks();
                    foreach (var url in linkedinJobLinks)
                    {
                        file.WriteLine(url);
                    }
                }

                Console.WriteLine("URLs are created successfully. Now the bot will visit those URLs.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't generate URL. Make sure you have the 'data' folder and modified 'config.cs' file for your preferences.");
                Console.WriteLine(e.Message);
            }
        }
        public bool AppliedBefore(IWebElement offer)
        {
            try
            {
                IWebElement appliedElement = offer.FindElement(By.XPath(".//li[contains(@class, 'job-card-container__footer-item') and contains(@class, 'inline-flex') and contains(@class, 'align-items-center')]"));

                string innerText = appliedElement.Text;

                if (innerText.Contains("Applied"))
                {
                    Console.WriteLine("Applied");
                    return true;
                }
                else
                {
                    Console.WriteLine("not Applied");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("not Applied exception");
                return false;
            }

        }
        public void LinkJobApply()
        {
            try
            {
                GenerateUrls();
                int countApplied = 0;
                int countJobs = 0;

                List<string> urlData = Utils.getUrlDataFile();

                foreach (string url in urlData)
                {
                    List<string> urlWords = new List<string>();
                    try
                    {
                        driver.Url = url;

                        string totalJobs = driver.FindElement(By.XPath("//small")).Text;
                        int totalPages = Utils.jobsToPages(totalJobs);

                        urlWords = Utils.urlToKeywords(url);
                        string lineToWrite = "\n Category: " + urlWords[0] + ", Location: " + urlWords[1] + ", Applying " + totalJobs + " jobs.";
                        //DisplayWriteResults(lineToWrite);

                        for (int page = 0; page < totalPages; page++)
                        {
                            int currentPageJobs = Constants.JobsPerPage * page;
                            var currentUrl = url + "&start=" + currentPageJobs;
                            driver.Url = currentUrl;
                            Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));

                            //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                            ReadOnlyCollection<IWebElement> offersPerPage = wait.Until(ct => SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//li[@data-occludable-job-id]")).Invoke(driver));

                            List<long> offerIds = new List<long>();
                            foreach (IWebElement offer in offersPerPage)
                            {
                                string offerId = offer.GetAttribute("data-occludable-job-id");
                                if(!AppliedBefore(offer))
                                    offerIds.Add(long.Parse(offerId.Split(':').Last()));
                            }

                            foreach (long jobID in offerIds)
                            {
                                string offerPage = "https://www.linkedin.com/jobs/view/" + jobID;
                                driver.Url = offerPage;
                                Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));

                                countJobs++;

                                string jobProperties = GetJobProperties(countJobs);
                                IWebElement button = EasyApplyButton();
                                IWebElement linkApplyButton = LinkApplyButton();
                                if (button != null)
                                {
                                    button.Click();
                                    Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
                                    countApplied++;
                                    try
                                    {
                                        bool isApplied = SubmitApplication(jobProperties, offerPage);
                                        if (!isApplied)
                                        {
                                            int counter = 0;
                                            string currentValueOfPercent = "";
                                            string previousValueOfPercent = "";
                                            for (int i = 0; i < 15; i++)
                                            {
                                                Console.WriteLine($"---->{i + 1}.Page<----");
                                                currentValueOfPercent = ContinueNextStep();
                                                EnterCityName();
                                                if (currentValueOfPercent != previousValueOfPercent)
                                                {
                                                    previousValueOfPercent = currentValueOfPercent;
                                                }
                                                else
                                                {
                                                    break;
                                                }

                                                if (ReviewTheApplication(jobProperties, offerPage, ref counter))
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    Console.WriteLine("----> Already applied! Job");
                                }
                                if (linkApplyButton != null)
                                {
                                    lineToWrite = jobProperties + " | " + "* EXTERNAL LINK APPLICATION: " + offerPage;
                                    DisplayWriteResults(lineToWrite);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("XXXXXXXXXXXXXXX //SMALL EXCEPTION: " + url);
                        Console.WriteLine("Error: " + e.ToString());
                    }

                    Console.WriteLine("Category: " + urlWords[0] + "," + urlWords[1] + " applied: " + countApplied +
                        " jobs out of " + countJobs + ".");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                Console.WriteLine(ex.Message);
            }


            //Utils.Donate(this);
        }

        private void EnterCityName()
        {
            try
            {
                IWebElement comboboxElement = driver.FindElement(By.CssSelector("input[role='combobox'][id*='city']"));
                string desiredValue = "Yalova, Turkey";

                if (comboboxElement != null)
                {
                    comboboxElement.Clear();
                    comboboxElement.SendKeys(desiredValue);
                    comboboxElement.SendKeys(Keys.Tab);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("##Error while Entring City");
            }
            
        }


        public string ContinueNextStep()
        {
            string currentValue = "";
            IReadOnlyCollection<IWebElement> progressElements = driver.FindElements(By.CssSelector("progress.artdeco-completeness-meter-linear__progress-element"));
            
            driver.FindElement(By.CssSelector("button[aria-label='Continue to next step']")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
            if (progressElements.Count>0)
            {
                IWebElement progressElement = driver.FindElement(By.CssSelector("progress.artdeco-completeness-meter-linear__progress-element"));
                currentValue = progressElement.GetAttribute("aria-valuenow");
            }
            return currentValue;
        }
        public bool SubmitApplication(string jobProperties, string offerPage)
        {
            bool isApplied = false;
            Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
            IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.CssSelector("button[aria-label='Submit application']"));
            if (elements.Count > 0)
            {
                driver.FindElement(By.CssSelector("button[aria-label='Submit application']")).Click();
                Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.Slow));
                
                string lineToWrite = jobProperties + " | " + "* ## SUCCESS ##: " + offerPage;
                DisplayWriteResults(lineToWrite);
                
                Console.WriteLine(("Successful Job App Count ##-->") + (++Config.successfulJobApplicationCounter)+"<--##");
                isApplied= true;

            }
            return isApplied;
        }
        public bool ReviewTheApplication(string jobProperties, string offerPage, ref int counter)
        {
            Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
            ReadOnlyCollection<IWebElement> buttons = driver.FindElements(By.CssSelector("button[aria-label='Review your application']"));

            if (buttons.Count>0)
            {
                counter++;
                driver.FindElement(By.CssSelector("button[aria-label='Review your application']")).Click();
            }
            Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
            return SubmitApplication(jobProperties, offerPage);
        }
        
        public string CheckRadioButtons()
        {
            try
            {
                var radioExistence = driver.FindElement(By.CssSelector("label.t-14"));
                if (radioExistence != null)
                {
                    radioExistence.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
                }
                if (!Config.FollowCompanies)
                {
                    driver.FindElement(By.CssSelector("label[for='follow-company-checkbox']")).Click();
                    Thread.Sleep(TimeSpan.FromSeconds(new Random().NextDouble() * Constants.BotSpeed));
                }

                return "";
            }
            catch (Exception)
            {
                return "%%% xXx -- Exception while selecting Radio Button";
            }




        }
       
        public IWebElement EasyApplyButton()
        {
            try
            {
                IWebElement button = driver.FindElement(By.XPath("//button[contains(@class, 'jobs-apply-button')]"));
                return button;
            }
            catch
            {
                return null;
            }
        }
        public IWebElement LinkApplyButton()
        {
            try
            {
                IWebElement linkApplyButton = driver.FindElement(By.XPath("//span[text()='Apply']"));
                return linkApplyButton;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public string GetJobProperties(int count)
        {
            string textToWrite = "";
            string jobTitle = "";
            string jobCompany = "";
            string jobLocation = "";
            string jobWorkPlace = "";
            string jobPostedDate = "";
            string jobApplications = "";

            try
            {
                jobTitle = driver.FindElement(By.XPath("//h1[contains(@class, 'job-title')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobTitle: " + e.Message.Substring(0, 50));
                jobTitle = "";
            }
            try
            {
                jobCompany = driver.FindElement(By.XPath("//a[contains(@class, 'ember-view t-black t-normal')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobCompany: " + e.Message.Substring(0, 50));
                jobCompany = "";
            }
            try
            {
                jobLocation = driver.FindElement(By.XPath("//span[contains(@class, 'bullet')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobLocation: " + e.Message.Substring(0, 50));
                jobLocation = "";
            }
            try
            {
                jobWorkPlace = driver.FindElement(By.XPath("//span[contains(@class, 'workplace-type')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobWorkPlace: " + e.Message.Substring(0, 50));
                jobWorkPlace = "";
            }
            try
            {
                jobPostedDate = driver.FindElement(By.XPath("//span[contains(@class, 'posted-date')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobPostedDate: " + e.Message.Substring(0, 50));
                jobPostedDate = "";
            }
            try
            {
                jobApplications = driver.FindElement(By.XPath("//span[contains(@class, 'applicant-count')]")).GetAttribute("innerHTML").Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning in getting jobApplications: " + e.Message.Substring(0, 50));
                jobApplications = "";
            }

            textToWrite = count + " | " + jobTitle + " | " + jobCompany + " | " + jobLocation + " | " + jobWorkPlace + " | " + jobPostedDate + " | " + jobApplications;
            return textToWrite;
        }
        public void DisplayWriteResults(string lineToWrite)
        {
            try
            {
                Console.WriteLine(lineToWrite);
                Utils.writeResults(lineToWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in DisplayWriteResults: " + e.Message);
            }
        }

    }
}






