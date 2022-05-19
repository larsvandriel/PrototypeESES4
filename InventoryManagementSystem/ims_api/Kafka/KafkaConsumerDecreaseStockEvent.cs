using Confluent.Kafka;
using InventoryManagementSystem.API.Models;
using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.API.Kafka
{
    public class KafkaConsumerDecreaseStockEvent : BackgroundService
    {

        private readonly string topic = "DecreaseStockEvent";
        private readonly IServiceProvider _serviceProvider;
        private readonly string _kafkaBootstrapServers;

        private IInventoryManager InventoryManager { get; set; }

        public KafkaConsumerDecreaseStockEvent(IServiceProvider serviceProvider, string kafkaBootstrapServers)
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
            Console.WriteLine("Decrease Stock Event: Do some work!");
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
                    while (true)
                    {
                        builder.Subscribe(topic);
                        var cancelToken = new CancellationTokenSource();
                        try
                        {
                            while (true)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Ready for consuming next message.");
                                Console.WriteLine();
                                var consumer = builder.Consume(cancelToken.Token);
                                Console.WriteLine("Consumed message");
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

                }
                await Task.CompletedTask;
            }
        }
    }
}
