using Confluent.Kafka;
using ProductManagementSystem.Logic;
using System.Text.Json;

namespace ProductManagementSystem.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string[] topic = new string[] { "UpdateStockEvent" };
        private readonly IServiceProvider _serviceProvider;
        private readonly string _kafkaBootstrapServers;

        private IProductManager ProductManager { get; set; }

        public KafkaConsumer(IServiceProvider serviceProvider, string kafkaBootstrapServers)
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
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                ProductManager = scope.ServiceProvider.GetRequiredService<IProductManager>();
                Console.WriteLine(_kafkaBootstrapServers);
                var conf = new ConsumerConfig
                {
                    GroupId = "st_consumer_group",
                    BootstrapServers = "localhost:9092",
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
                            if (consumer.Topic == topic[0])
                            {
                                Console.WriteLine($"{consumer.Message.Value} recieved on topic {consumer.Topic}");
                                Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                                ProductManager.UpdateStock(product.Id, product.AmountInStorage);
                                continue;
                            }
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
