using LinkedinJobApplicationAutomation.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedinJobApplicationAutomation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Linkedin linkedin = new Linkedin();
            linkedin.LinkJobApply();
        }
    }
}
