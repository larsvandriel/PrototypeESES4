using Confluent.Kafka;
using InventoryManagementSystem.API.Models;
using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string[] topic = new string[] { "CreateProductEvent", "UpdateProductEvent", "DeleteProductEvent", "DecreaseStockEvent" };
        private readonly IServiceProvider _serviceProvider;
        private readonly string _kafkaBootstrapServers;

        private IInventoryManager InventoryManager { get; set; }

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
                            if (consumer.Topic == topic[0])
                            {
                                Console.WriteLine($"{consumer.Message.Value} recieved on topic {consumer.Topic}");
                                Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                                InventoryManager.AddProduct(product);
                                continue;
                            }
                            if (consumer.Topic == topic[1])
                            {
                                Console.WriteLine($"{consumer.Message.Value} recieved on topic {consumer.Topic}");
                                Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                                InventoryManager.UpdateProduct(product);
                                continue;
                            }
                            if (consumer.Topic == topic[2])
                            {
                                Console.WriteLine($"{consumer.Message.Value} recieved on topic {consumer.Topic}");
                                Product product = (Product)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Product));
                                InventoryManager.RemoveProduct(product);
                                continue;
                            }
                            if (consumer.Topic == topic[3])
                            {
                                Console.WriteLine($"{consumer.Message.Value} recieved on topic {consumer.Topic}");
                                DecreaseStockEventModel decreaseStockEventModel = (DecreaseStockEventModel)JsonSerializer.Deserialize(consumer.Message.Value, typeof(DecreaseStockEventModel));
                                InventoryManager.DecreaseStorage(decreaseStockEventModel.ProductId, decreaseStockEventModel.OrderId, decreaseStockEventModel.Amount);
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
