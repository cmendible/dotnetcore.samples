namespace ConsoleApplication
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;
    using Amqp;
    using Amqp.Framing;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // build the configuration
            var configuration = builder.Build();

            // Azure service bus SAS key
            var policyName = WebUtility.UrlEncode(configuration["ServiceBus:PolicyName"]);
            var key = WebUtility.UrlEncode(configuration["ServiceBus:Key"]);

            // Azure service bus namespace
            var namespaceUrl = configuration["ServiceBus:NamespaceUrl"];

            // Create the AMQP connection string
            var connnectionString = $"amqps://{policyName}:{key}@{namespaceUrl}/";

            // Create the AMQP connection
            var connection = new Connection(new Address(connnectionString));

            // Create the AMQP session
            var amqpSession = new Session(connection);

            // Give a name to the sender
            var senderSubscriptionId = "codeityourself.amqp.sender";
            // Give a name to the receiver
            var receiverSubscriptionId = "codeityourself.amqp.receiver";

            // Name of the topic you will be sending messages
            var topic = "codeityourself";

            // Name of the subscription you will receive messages from
            var subscription = "codeityourself.listener";

            // Create the AMQP sender
            var sender = new SenderLink(amqpSession, senderSubscriptionId, topic);

            for (var i = 0; i < 10; i++)
            {
                // Create message
                var message = new Message($"Received message {i}");

                // Add a meesage id
                message.Properties = new Properties() { MessageId = Guid.NewGuid().ToString() };

                // Add some message properties
                message.ApplicationProperties = new ApplicationProperties();
                message.ApplicationProperties["Message.Type.FullName"] = typeof(string).FullName;

                // Send message
                sender.Send(message);
            }

            // Create the AMQP consumer
            var consumer = new ReceiverLink(amqpSession, receiverSubscriptionId, $"{topic}/Subscriptions/{subscription}");

            // Start listening
            consumer.Start(5, OnMessageCallback);

            // Wait for a key to close the program
            Console.Read();
        }

        /// <summary>
        /// Callback method that will be called every time a message is received.
        /// </summary>
        /// <param name="receiver">The AMQP receiver</param>
        /// <param name="message">The message you just received</param>
        static void OnMessageCallback(ReceiverLink receiver, Message message)
        {
            try
            {
                // You can read the custom property
                var messageType = message.ApplicationProperties["Message.Type.FullName"];

                // Variable to save the body of the message.
                string body = string.Empty;

                // Get the body
                var rawBody = message.GetBody<object>();

                // If the body is byte[] asume it was sent as a BrokeredMessage  
                // adn deserialize it using a XmlDictionaryReader
                if (rawBody is byte[])
                {
                    using (var reader = XmlDictionaryReader.CreateBinaryReader(
                        new MemoryStream(rawBody as byte[]),
                        null,
                        XmlDictionaryReaderQuotas.Max))
                    {
                        var doc = new XmlDocument();
                        doc.Load(reader);
                        body = doc.InnerText;
                    }
                }
                else // Asume the body is a string
                {
                    body = rawBody.ToString();
                }

                // Write the body to the Console.
                Console.WriteLine(body);

                // Accept the messsage.
                receiver.Accept(message);
            }
            catch (Exception ex)
            {
                receiver.Reject(message);
                Console.WriteLine(ex);
            }
        }
    }
}
