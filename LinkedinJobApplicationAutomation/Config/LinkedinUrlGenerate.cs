using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedinJobApplicationAutomation.Config
{
    public class LinkedinUrlGenerate
    {
        public List<string> generateUrlLinks()
        {
            var path = new List<string>();
            foreach (var location in Config.Location)
            {
                int jobExperienceIndex = Config.ExperienceLevels.Count;
                foreach (var keyword in Config.Keywords)
                {
                    for (int i = 0; i < jobExperienceIndex; i++)
                    {
                        var url = Constants.LinkJobUrl + "?f_AL=true&keywords=" + keyword + this.jobType() + this.remote() + this.checkJobLocation(location) + this.jobExp(i) + this.datePosted() + this.salary() + this.sortBy();
                        path.Add(url);
                    }
                }
            }
            return path;
        }
        public string isEasyApply(bool isEasyApply)
        {
            if (isEasyApply)
            {
                return "f_AL=true";
            }
            else
            {
                return "";
            }
        }
        public string checkJobLocation(string job)
        {
            var jobLoc = "&location=" + job;
            switch (job.ToLower())
            {
                case "asia":
                    jobLoc += "&geoId=102393603";
                    break;
                case "europe":
                    jobLoc += "&geoId=100506914";
                    break;
                case "northamerica":
                    jobLoc += "&geoId=102221843&";
                    break;
                case "southamerica":
                    jobLoc += "&geoId=104514572";
                    break;
                case "australia":
                    jobLoc += "&geoId=101452733";
                    break;
                case "africa":
                    jobLoc += "&geoId=103537801";
                    break;
            }

            //return jobLoc;
            return "";
        }

        public string jobExp(int jobIndex)
        {
            var jobtExpArray = Config.ExperienceLevels;
            var firstJobExp = jobtExpArray[jobIndex];
            var jobExp = "";
            switch (firstJobExp)
            {
                case "Internship":
                    jobExp = "&f_E=1";
                    break;
                case "Entry level":
                    jobExp = "&f_E=2";
                    break;
                case "Associate":
                    jobExp = "&f_E=3";
                    break;
                case "Mid-Senior level":
                    jobExp = "&f_E=4";
                    break;
                case "Director":
                    jobExp = "&f_E=5";
                    break;
                case "Executive":
                    jobExp = "&f_E=6";
                    break;
            }
            return jobExp;
        }

        public string datePosted()
        {
            var datePostedArray = Config.DatePosted;
            var firstDatePosted = datePostedArray[0];
            var datePosted = "";
            switch (firstDatePosted)
            {
                case "24 hours":
                    datePosted = "&f_TPR=r86400";
                    break;
                case "1 week":
                    datePosted = "&f_TPR=r604800";
                    break;
                case "1 month":
                    datePosted = "&f_TPR=r2419200";
                    break;
                case "Any time":
                    datePosted = "&f_TPR=r0";
                    break;
            }
            return datePosted;
        }

        public string jobType()
        {
            var jobTypeArray = Config.JobType;
            var firstJobType = jobTypeArray[0];
            var jobType = "";
            switch (firstJobType)
            {
                case "Full-time":
                    jobType = "&f_JT=F";
                    break;
                case "Part-time":
                    jobType = "&f_JT=P";
                    break;
                case "Temporary":
                    jobType = "&f_JT=T";
                    break;
                case "Contract":
                    jobType = "&f_JT=C";
                    break;
                case "Internship":
                    jobType = "&f_JT=I";
                    break;
                case "Commission":
                    jobType = "&f_JT=CO";
                    break;
                case "Volunteer":
                    jobType = "&f_JT=V";
                    break;
                case "Remote":
                    jobType = "&remoteJob=true";
                    break;
            }
            return jobType;
        }

        public string remote()
        {
            var jobTypeArray = Config.JobType;
            var firstJobType = jobTypeArray[0];
            var remote = "";
            switch (firstJobType)
            {
                case "Remote":
                    remote = "&remoteJob=true";
                    break;
                default:
                    remote = "";
                    break;
            }
            return remote;
        }

        public string salary()
        {
            var salaryArray = Config.Salary;
            var firstSalary = salaryArray[0];
            var salary = "";
            switch (firstSalary)
            {
                case "Range":
                    salary = "&salary=0";
                    break;
                case "0 - 30K":
                    salary = "&salary=1";
                    break;
                case "30K - 60K":
                    salary = "&salary=2";
                    break;
                case "60K - 90K":
                    salary = "&salary=3";
                    break;
                case "90K - 120K":
                    salary = "&salary=4";
                    break;
                case "120K - 150K":
                    salary = "&salary=5";
                    break;
                case "150K - 200K":
                    salary = "&salary=6";
                    break;
                case "200K - 250K":
                    salary = "&salary=7";
                    break;
                case "250K+":
                    salary = "&salary=8";
                    break;
            }
            //return salary;
            return "";
        }

        public string sortBy()
        {
            var sortByArray = Config.Sort;
            var firstSortBy = sortByArray[0];
            var sortBy = "";
            switch (firstSortBy)
            {
                case "Relevance":
                    sortBy = "";
                    break;
                case "Date":
                    sortBy = "&sortBy=R";
                    break;
            }
            return sortBy;
        }

    }


}
