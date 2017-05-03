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
            using (Producer producer = new Producer("127.0.0.1:9092"))
            using (Topic topic = producer.Topic("testtopic"))
            {
                byte[] data = Encoding.UTF8.GetBytes("Hello RdKafka");
                DeliveryReport deliveryReport = topic.Produce(data).GetAwaiter().GetResult();
                Console.WriteLine($"Produced to Partition: {deliveryReport.Partition}, Offset: {deliveryReport.Offset}");
            }

            var config = new Config() { GroupId = "myconsumer" };
            config["api.version.request"] = "true";
            using (var consumer = new EventConsumer(config, "127.0.0.1:9092"))
            {
                consumer.OnMessage += (obj, msg) =>
                {
                    string text = Encoding.UTF8.GetString(msg.Payload, 0, msg.Payload.Length);
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {text}");
                };

                consumer.Subscribe(new List<string> { "testtopic" });
                consumer.Start();

                Console.WriteLine("Started consumer, press enter to stop consuming");
                Console.ReadLine();
            }
        }
    }
}