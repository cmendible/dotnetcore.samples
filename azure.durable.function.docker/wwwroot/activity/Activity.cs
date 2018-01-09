using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace DurableFunctions
{
    public class Activity
    {
        public static string Run(string name)
        {
            return $"Hello {name}!";
        }
    }
}