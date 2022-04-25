using Confluent.Kafka;
using InventoryManagementSystem.API.Models;
using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.API.Kafka
{
    public class KafkaConsumerDecreaseStockEvent: IHostedService
    {
        private readonly string topic = "DecreaseStockEvent";
        private IInventoryManager InventoryManager { get; set; }

        public KafkaConsumerDecreaseStockEvent(IInventoryManager inventoryManager)
        {
            InventoryManager = inventoryManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = "kafka:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                builder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);
                        Console.WriteLine();
                        Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                        Console.WriteLine();
                        DecreaseStockEventModel message = (DecreaseStockEventModel)JsonSerializer.Deserialize(consumer.Message.Value, typeof(DecreaseStockEventModel));
                        InventoryManager.DecreaseStorage(message.OrderId, message.ProductId, message.Amount);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failure occurred");
                    Console.WriteLine(e.GetType());
                    Console.WriteLine(e.Message);
                    builder.Close();
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
