using InventoryManagementSystem.Logic;
using System.Text.Json;

namespace InventoryManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IInventoryEventSender
    {
        public KafkaProducer OrderApprovedProducer { get; set; }
        public KafkaProducer OrderDeniedProducer { get; set; }
        public KafkaProducer UpdateStockProducer { get; set; }

        public KafkaAccessLayer()
        {
            OrderApprovedProducer = new KafkaProducer("OrderApprovedEvent");
            OrderDeniedProducer = new KafkaProducer("OrderDeniedEvent");
            UpdateStockProducer = new KafkaProducer("UpdateStockEvent");
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