using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.KafkaAccessLayer
{
    public class DecreaseStockEventPayload
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
