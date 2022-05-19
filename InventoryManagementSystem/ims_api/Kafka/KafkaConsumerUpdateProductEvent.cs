using Confluent.Kafka;
using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.API.Kafka
{
    public class KafkaConsumerUpdateProductEvent: BackgroundService
    {
        private readonly string topic = "UpdateProductEvent";
        private readonly IServiceProvider _serviceProvider;
        private readonly string _kafkaBootstrapServers;

        private IInventoryManager InventoryManager { get; set; }

        public KafkaConsumerUpdateProductEvent(IServiceProvider serviceProvider, string kafkaBootstrapServers)
        {
            _serviceProvider = serviceProvider;
            _kafkaBootstrapServers = kafkaBootstrapServers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWorkAsync(stoppingToken);
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Update Product Event: Do some work!");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                InventoryManager = scope.ServiceProvider.GetRequiredService<IInventoryManager>();

                var conf = new ConsumerConfig
                {
                    GroupId = "st_consumer_group",
                    BootstrapServers = _kafkaBootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
                using (var builder = new ConsumerBuilder<Ignore, string>(conf).Build())
                {
                    builder.Subscribe(topic);
                    var cancelToken = new CancellationTokenSource();
                    try
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            var consumer = builder.Consume(cancelToken.Token);
                            Console.WriteLine();
                            Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                            Console.WriteLine();
                            Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                            InventoryManager.UpdateProduct(product);
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
                await Task.CompletedTask;
            }
        }
    }
}
