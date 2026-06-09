using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrderService.Models;
using OrderService.Protos;

namespace OrderService.Services {
    public class OrderGrpcService : OrderProtoService.OrderProtoServiceBase {
        private readonly OrdersBL orders = new OrdersBL();

        public override Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context) {
            var storedOrder = orders.GetById(request.Id);

            var response = new GetOrderResponse();
            response.CustomerId = storedOrder.CustomerId;
            response.Items.AddRange(storedOrder.Items.Select(o => new Protos.OrderItem {
                ItemId = o.Id,
                Price = o.Price,
                Quantity = o.Quantity
            }));
            response.TotalPrice = response.Items.Sum(i => i.Price * i.Quantity);
            response.CreatedAt = Timestamp.FromDateTime(storedOrder.CreatedAt.ToUniversalTime());
            return Task.FromResult(response);
        }

        public override Task<GetOrdersResponse> GetOrders(Empty request, ServerCallContext context) {
            var response = new GetOrdersResponse();
            response.Orders.AddRange(orders.GetAllOrders().Select(o => {
                var orderResponse = new GetOrderResponse {
                    CustomerId = o.CustomerId,
                    TotalPrice = o.Items.Sum(i => i.Price * i.Quantity),
                    CreatedAt = Timestamp.FromDateTime(o.CreatedAt.ToUniversalTime())
                };
                orderResponse.Items.AddRange(o.Items.Select(i => new Protos.OrderItem {
                    ItemId = i.Id,
                    Price = i.Price,
                    Quantity = i.Quantity
                }));
                return orderResponse;
            }));

            return Task.FromResult(response);
        }
    }
}
