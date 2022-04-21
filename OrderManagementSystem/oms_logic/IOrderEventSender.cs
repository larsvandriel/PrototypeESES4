using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Logic
{
    public interface IOrderEventSender
    {
        void SendDecreaseStockEvent(Guid orderId, Guid productId, int amount);
    }
}
