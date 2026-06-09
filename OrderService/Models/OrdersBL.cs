namespace OrderService.Models {
    public class OrdersBL {
        List<Order> orders = [];

        public OrdersBL() {
            orders.AddRange(new List<Order> {
                new Order {
                    Id = 1,
                    CustomerId = 1,
                    Items = new List<OrderItem> {
                        new OrderItem { Id = 1, Quantity = 2, Price = 10 },
                        new OrderItem { Id = 2, Quantity = 1, Price = 20 }
                    },
                    CreatedAt = DateTime.Now
                },
                new Order {
                    Id = 2,
                    CustomerId = 2,
                    Items = new List<OrderItem> {
                        new OrderItem { Id = 3, Quantity = 1, Price = 15 },
                        new OrderItem { Id = 4, Quantity = 3, Price = 5 }
                    },
                    CreatedAt = DateTime.Now
                }
            });
        }

        public List<Order> GetAllOrders() {
            return orders;
        }

        public Order GetById(int id) {
            return orders.FirstOrDefault(o => o.Id == id);
        }
    }
}
