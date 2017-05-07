using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RdKafka;

namespace kafka.pubsub.console
{
    class Program
    {
        static void Main(string[] args)
        {
            // The Kafka endpoint address
            string kafkaEndpoint = "127.0.0.1:9092";

            // The Kafka topic we'll be using
            string kafkaTopic = "testtopic";

            // Create a producer and connec to topic
            using (Producer producer = new Producer(kafkaEndpoint))
            using (Topic topic = producer.Topic(kafkaTopic))
            {
                // Send 10 messages to the topic
                for (int i = 0; i < 10; i++)
                {
                    byte[] data = Encoding.UTF8.GetBytes($"Event {1}");
                    DeliveryReport deliveryReport = topic.Produce(data).GetAwaiter().GetResult();
                    Console.WriteLine($"Event {i} sent...");
                }
            }

            // Create an Event Consumer Config
            var config = new Config() { GroupId = "myconsumer" };
            config["api.version.request"] = "true";
            // Create an Event Consumer
            using (var consumer = new EventConsumer(config, kafkaEndpoint))
            {
                // Subscribe to the OnMessage event
                consumer.OnMessage += (obj, msg) =>
                {
                    string text = Encoding.UTF8.GetString(msg.Payload, 0, msg.Payload.Length);
                    Console.WriteLine($"Received: {text}");
                };

                // Subscribe to the Kafka topic
                consumer.Subscribe(new List<string> { kafkaTopic });

                // Start listening
                consumer.Start();

                // Wait for key to stop the app
                Console.ReadLine();
            }
        }
    }
}