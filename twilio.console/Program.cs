using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Your Account SID from twilio.com/console
            var accountSid = "[Account SID]";
            
            // Your Auth Token from twilio.com/console
            var authToken = "[Auth Token]";   

            // Initialize Twilio Client
            TwilioClient.Init(accountSid, authToken);

            // Create & Send Message
            var message = MessageResource.Create(
                to: new PhoneNumber("[Target Phone Number]"),
                from: new PhoneNumber("[Your Twilio Phone Number]"),
                body: "Hello from dotnetcore");

            Console.WriteLine(message.Sid);
            Console.Read();
        }
    }
}