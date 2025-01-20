using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLibrary.Consumers
{
    public class KafkaConsumer
    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(string bootstrapServers, string groupId, string topic)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(topic);
        }

        public void StartListening<T>(Func<T, Task> messageHandler) where T : class
        {
            try
            {
                while (true)
                {
                    var consumeResult = _consumer.Consume();
                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");

                    var message = JsonConvert.DeserializeObject<T>(consumeResult.Message.Value);
                    if (message != null)
                    {
                        messageHandler(message).Wait();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consuming operation was canceled.");
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"Error occurred: {ex.Error.Reason}");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
