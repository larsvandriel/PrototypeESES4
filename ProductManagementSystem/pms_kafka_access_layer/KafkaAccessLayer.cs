 using ProductManagementSystem.Logic;
using System.Text.Json;

namespace ProductManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IProductEventSender
    {
        public KafkaProducer CreateProductProducer { get; set; }
        public KafkaProducer DeleteProductProducer { get; set; }
        public KafkaProducer UpdateProductProducer { get; set; }

        public KafkaAccessLayer()
        {
            CreateProductProducer = new KafkaProducer("CreateProductEvent");
            DeleteProductProducer = new KafkaProducer("DeleteProductEvent");
            UpdateProductProducer = new KafkaProducer("UpdateProductEvent");
        }

        public void SendProductCreatedEvent(Product product)
        {
            string message = JsonSerializer.Serialize(product);
            CreateProductProducer.SendMessage(message);
        }

        public void SendProductDeletedEvent(Product product)
        {
            string message = JsonSerializer.Serialize(product);
            DeleteProductProducer.SendMessage(message);
        }

        public void SendProductUpdatedEvent(Product product)
        {
            string message = JsonSerializer.Serialize(product);
            UpdateProductProducer.SendMessage(message);
        }
    }
}