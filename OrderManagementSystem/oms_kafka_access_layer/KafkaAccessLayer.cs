using OrderManagementSystem.Logic;
using System.Text.Json;

namespace OrderManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IOrderEventSender
    {
        public KafkaProducer DecreaseStockProducer { get; set; }

        public KafkaAccessLayer()
        {
            DecreaseStockProducer = new KafkaProducer("DecreaseStockEvent");
        }

        public void SendDecreaseStockEvent(Guid orderId, Guid productId, int amount)
        {
            string message = JsonSerializer.Serialize(new DecreaseStockEventPayload() { OrderId = orderId, ProductId = productId, Amount = amount });
            DecreaseStockProducer.SendMessage(message);
        }
    }
}