using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Logic
{
    public class Order
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public OrderStatus Status { get; set; }
    }
}
