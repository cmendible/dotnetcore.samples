using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace DurableFunctions
{
    public class Orchestrator
    {

        public static async Task<List<string>> Run(
            DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>("activity", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("activity", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("activity", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }
    }
}