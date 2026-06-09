using InventoryService.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using PaymentService.Protos;

namespace OrderService.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase {
        private readonly InventoryProtoService.InventoryProtoServiceClient _inventoryClient;
        private readonly PaymentProtoService.PaymentProtoServiceClient _paymentClient;

        public OrdersController(InventoryProtoService.InventoryProtoServiceClient inventoryClient,
                PaymentProtoService.PaymentProtoServiceClient paymentClient) {
            _inventoryClient = inventoryClient;
            _paymentClient = paymentClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO request) {
            var inventoryRequest = new DeductQuantityRequest();
            inventoryRequest.Items.AddRange(request.Items.Select(i => new Item {
                Id = i.Id,
                Quantity = i.Quantity
            }));
            var inventoryResponse = await _inventoryClient.DeductQuantityAsync(inventoryRequest);

            var paymentRequest = new DeductPaymentRequest();
            paymentRequest.Id = request.CustomerId;
            paymentRequest.Amount = request.Amount;
            var paymentResponse = await _paymentClient.DeductPaymentAsync(paymentRequest);

            return Ok("Order created successfully.");
        }
    }
}
