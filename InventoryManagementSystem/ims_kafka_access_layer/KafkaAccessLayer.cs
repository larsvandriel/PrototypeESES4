using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IInventoryEventSender
    {
        public KafkaProducer OrderApprovedProducer { get; set; }
        public KafkaProducer OrderDeniedProducer { get; set; }
        public KafkaProducer UpdateStockProducer { get; set; }

        public KafkaAccessLayer(string kafkaBootstrapServers)
        {
            OrderApprovedProducer = new KafkaProducer("OrderApprovedEvent", kafkaBootstrapServers);
            OrderDeniedProducer = new KafkaProducer("OrderDeniedEvent", kafkaBootstrapServers);
            UpdateStockProducer = new KafkaProducer("UpdateStockEvent", kafkaBootstrapServers);
        }

        public void SendOrderApprovedEvent(Guid orderId)
        {
            string message = JsonSerializer.Serialize(orderId);
            OrderApprovedProducer.SendMessage(message);
        }

        public void SendOrderDeniedEvent(Guid orderId)
        {
            string message = JsonSerializer.Serialize(orderId);
            OrderDeniedProducer.SendMessage(message);
        }

        public void SendUpdateStockEvent(Product product)
        {
            string message = JsonSerializer.Serialize(product);
            UpdateStockProducer.SendMessage(message);
        }
    }
}