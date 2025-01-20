using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLibrary.Producers
{
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(string bootstrapServers)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task<JObject> SendMessage(string topic, string key, string value)
        {
            try
            {
                var message = new Message<string, string> { Key = key, Value = value };
                var taskCompletionSource = new TaskCompletionSource<JObject>();

                _producer.Produce(topic, message, (deliveryReport) =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        taskCompletionSource.SetResult(null);
                    }
                    else
                    {
                        Console.WriteLine($"Message delivered to {deliveryReport.TopicPartitionOffset}");
                        var result = JsonConvert.DeserializeObject<JObject>(deliveryReport.Value);
                        taskCompletionSource.SetResult(result);
                    }
                });
                return await taskCompletionSource.Task;
            }
            catch (ProduceException<string, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                return null;
            }
        }

        public void Close()
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
