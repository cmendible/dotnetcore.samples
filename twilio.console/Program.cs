using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Send the SMS
            var messageSid = Task.Run(() => 
            { 
                return SendSms(); 
            })
            .GetAwaiter()
            .GetResult();

            // Write the message Id to the console.
            Console.WriteLine(messageSid);
            Console.Read();
        }

        // Send SMS with async feature!
        private static async Task<string> SendSms()
        {
            // Your Account SID from twilio.com/console
            var accountSid = "[Account SID]";

            // Your Auth Token from twilio.com/console
            var authToken = "[Auth Token]";

            // Initialize Twilio Client
            TwilioClient.Init(accountSid, authToken);

            // Create & Send Message (New lib supports async await)
            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber("[Target Phone Number]"),
                from: new PhoneNumber("[Your Twilio Phone Number]"),
                body: "Hello from dotnetcore");

            return message.Sid;
        }
    }
}