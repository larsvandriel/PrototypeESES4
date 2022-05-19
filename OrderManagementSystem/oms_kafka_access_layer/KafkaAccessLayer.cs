using OrderManagementSystem.Logic;
using System.Text.Json;

namespace OrderManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IOrderEventSender
    {
        public KafkaProducer DecreaseStockProducer { get; set; }

        public KafkaAccessLayer(string kafkaBootstrapServers)
        {
            DecreaseStockProducer = new KafkaProducer("DecreaseStockEvent", kafkaBootstrapServers);
        }

        public void SendDecreaseStockEvent(Guid orderId, Guid productId, int amount)
        {
            string message = JsonSerializer.Serialize(new DecreaseStockEventPayload() { OrderId = orderId, ProductId = productId, Amount = amount });
            DecreaseStockProducer.SendMessage(message);
        }
    }
}