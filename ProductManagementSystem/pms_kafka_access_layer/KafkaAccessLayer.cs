 using ProductManagementSystem.Logic;
using System.Text.Json;

namespace ProductManagementSystem.KafkaAccessLayer
{
    public class KafkaAccessLayer : IProductEventSender
    {
        public KafkaProducer CreateProductProducer { get; set; }
        public KafkaProducer DeleteProductProducer { get; set; }
        public KafkaProducer UpdateProductProducer { get; set; }

        public KafkaAccessLayer(string kafkaBootstrapServers)
        {
            CreateProductProducer = new KafkaProducer("CreateProductEvent", kafkaBootstrapServers);
            DeleteProductProducer = new KafkaProducer("DeleteProductEvent", kafkaBootstrapServers);
            UpdateProductProducer = new KafkaProducer("UpdateProductEvent", kafkaBootstrapServers);
        }

        public void SendProductCreatedEvent(Product product)
        {
            string message = JsonSerializer.Serialize(product);
            CreateProductProducer.SendMessage(message);
            Console.WriteLine($"Send {message} to MessageBus ProductCreated");
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