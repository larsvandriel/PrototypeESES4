using Confluent.Kafka;
using OrderManagementSystem.Logic;
using System.Text.Json;

namespace OrderManagementSystem.API.Kafka
{
    public class KafkaConsumerDeclineOrder: IHostedService
    {
        private readonly string topic = "DeclineOrderEvent";
        private IOrderManager OrderManager { get; set; }

        public KafkaConsumerDeclineOrder(IOrderManager orderManager)
        {
            OrderManager = orderManager;
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
                        Guid orderId = (Guid)JsonSerializer.Deserialize(consumer.Message.Value, typeof(Guid));
                        OrderManager.DeclineOrder(orderId);
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
