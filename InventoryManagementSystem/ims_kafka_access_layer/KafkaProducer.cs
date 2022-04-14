﻿using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.KafkaAccessLayer
{
    public class KafkaProducer
    {
        public string Topic { get; set; }

        public KafkaProducer(string topic)
        {
            Topic = topic;
        }

        public void SendMessage(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092"
            };

            Action<DeliveryReport<Null, string>> handler = r =>
             Console.Out.WriteLine(!r.Error.IsError
             ? $"Delivered message to {r.TopicPartitionOffset}"
             : $"Delivery Error: {r.Error.Reason}");

            using var p = new ProducerBuilder<Null, string>(config).Build();
            p.Produce(Topic, new Message<Null, string> { Value = message }, handler);

            p.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
