namespace InventoryManagementSystem.API.Models
{
    public class DecreaseStockEventModel
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
