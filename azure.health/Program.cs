/// <summary>
/// Query Azure.Health Events using .NET Core
/// The code was inspired on: How to Retrieve Azure Service Health Event Logs (https://code.msdn.microsoft.com/windowsapps/How-To-Programmatically-49df487d/view/Reviews)
/// by Matt Loflin
/// </summary>
namespace azure.health
{
    using System;
    using System.Linq;
    using Microsoft.Azure.Insights;
    using Microsoft.Azure.Insights.Models;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Rest.Azure.OData;

    class Program
    {
        static void Main(string[] args)
        {
            // The file with the Azure Service Principal Credentials.
            var authFile = "my.azureauth";

            // Parse the credentials from the file.
            var credentials = SdkContext.AzureCredentialsFactory.FromFile(authFile);

            // Authenticate with Azure
            var azure = Azure
                .Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();

            // Create an InsightsClient instance.
            var client = new InsightsClient(credentials);

            // If we subscription is not set the API call will fail. 
            client.SubscriptionId = credentials.DefaultSubscriptionId;

            // Create the OData filter for a time interval and the Azure.Health Provider.
            // Search back one day.
            var days = -1;
            var endDateTime = DateTime.Now;
            var startDateTime = endDateTime.AddDays(days);
            string filterString = FilterString.Generate<EventDataForFilter>(eventData =>
                (eventData.EventTimestamp >= startDateTime) &&
                (eventData.EventTimestamp <= endDateTime) &&
                (eventData.ResourceProvider == "Azure.Health"));

            // Get the Events from Azure.
            var response = client.Events.List(filterString);
            while (response != null && response.Any())
            {
                foreach (var eventData in response)
                {
                    // Set the Console Color according to the Event Status. 
                    if (eventData.Status.Value != "Resolved" &&
                        eventData.Level <= EventLevel.Warning)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (eventData.Status.Value == "Resolved")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    // Write event data to the console
                    Console.WriteLine($"{eventData.EventTimestamp.ToLocalTime()} - {eventData.ResourceProviderName.Value} - {eventData.OperationName.Value}");
                    Console.WriteLine($"Status:\t {eventData.Status.Value}");
                    Console.WriteLine($"Level:\t {eventData.Level.ToString()}");
                    Console.WriteLine($"CorrelationId:\t {eventData.CorrelationId}");
                    Console.WriteLine($"Resource Type:\t {eventData.ResourceType.Value}");
                    Console.WriteLine($"Description:\t {eventData.Description}");
                }

                // Get more events if available.
                if (!string.IsNullOrEmpty(response.NextPageLink))
                {
                    response = client.Events.ListNext(response.NextPageLink);
                }
                else
                {
                    response = null;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("No more events...");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    // EventData Filter. 
    public class EventDataForFilter
    {
        /// <summary>
        /// Event Timestamp
        /// </summary>
        public DateTime EventTimestamp { get; set; }

        /// <summary>
        /// Resource Provider
        /// </summary>
        public string ResourceProvider { get; set; }
    }
}