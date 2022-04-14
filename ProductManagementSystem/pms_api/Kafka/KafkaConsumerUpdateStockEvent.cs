using Confluent.Kafka;
using ProductManagementSystem.Logic;
using System.Text.Json;

namespace ProductManagementSystem.API.Kafka
{
    public class KafkaConsumerUpdateStockEvent : IHostedService
    {
        private readonly string topic = "UpdateStockEvent";
        private IProductManager ProductManager { get; set; }

        public KafkaConsumerUpdateStockEvent(ProductManager productManager)
        {
            ProductManager = productManager;
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
                        Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                        ProductManager.UpdateStock(product.Id, product.AmountInStorage);
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
            throw new NotImplementedException();
        }
    }
}
