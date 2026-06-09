namespace OrderService.DTOs {
    public class OrderDTO {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public IEnumerable<ItemDTO> Items { get; set; }
        public int Amount { get; set; }
    }
}
