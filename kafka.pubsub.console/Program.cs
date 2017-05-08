using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

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

            // Create the producer configuration
            var producerConfig = new Dictionary<string, object> { { "bootstrap.servers", kafkaEndpoint } };

            // Create the producer
            using (var producer = new Producer<Null, string>(producerConfig, null, new StringSerializer(Encoding.UTF8)))
            {
                // Send 10 messages to the topic
                for (int i = 0; i < 10; i++)
                {
                    var message = $"Event {i}";
                    var result = producer.ProduceAsync(kafkaTopic, null, message).GetAwaiter().GetResult();
                    Console.WriteLine($"Event {i} sent on Partition: {result.Partition} with Offset: {result.Offset}");
                }
            }

            // Create the consumer configuration
            var consumerConfig = new Dictionary<string, object>
            {
                { "group.id", "myconsumer" },
                { "bootstrap.servers", kafkaEndpoint },
            };

            // Create the consumer
            using (var consumer = new Consumer<Null, string>(consumerConfig, null, new StringDeserializer(Encoding.UTF8)))
            {
                // Subscribe to the OnMessage event
                consumer.OnMessage += (obj, msg) =>
                {
                    Console.WriteLine($"Received: {msg.Value}");
                };

                // Subscribe to the Kafka topic
                consumer.Subscribe(new List<string>() { kafkaTopic });

                // Handle Cancel Keypress 
                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                Console.WriteLine("Ctrl-C to exit.");

                // Poll for messages
                while (!cancelled)
                {
                    consumer.Poll();
                }
            }
        }
    }
}