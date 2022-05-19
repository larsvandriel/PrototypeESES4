using Confluent.Kafka;
using OrderManagementSystem.Logic;
using System.Text.Json;

namespace OrderManagementSystem.API.Kafka
{
    public class KafkaConsumerAcceptOrder: BackgroundService
    {
        private readonly string topic = "AcceptOrderEvent";
        private readonly IServiceProvider _serviceProvider;
        private readonly string _kafkaBootstrapServers;

        private IOrderManager OrderManager { get; set; }

        public KafkaConsumerAcceptOrder(IServiceProvider serviceProvider, string kafkaBootstrapServers)
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
                OrderManager = scope.ServiceProvider.GetRequiredService<IOrderManager>();

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
                        while (true)
                        {
                            var consumer = builder.Consume(cancelToken.Token);
                            Console.WriteLine();
                            Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                            Console.WriteLine();
                            Guid orderId = (Guid)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Guid));
                            OrderManager.AcceptOrder(orderId);
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
