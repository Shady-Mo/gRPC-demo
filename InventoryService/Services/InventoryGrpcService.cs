using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using InventoryService.Models;
using InventoryService.Protos;

namespace InventoryService.Services {
    public class InventoryGrpcService : InventoryProtoService.InventoryProtoServiceBase {
        private readonly ItemsBL items = new ItemsBL();

        public override Task<DeductQuantityResponse> DeductQuantity(DeductQuantityRequest request, ServerCallContext context) {
            var requestItemsIds = request.Items.Select(i => i.Id).ToList();
            var storedItems = items.GetRequestedItems(requestItemsIds);
            var storedItemsMap = storedItems.ToDictionary(i => i.Id);

            foreach (var requestItem in request.Items) {
                if (!storedItemsMap.TryGetValue(requestItem.Id, out var storedItem))
                    throw new RpcException(new Status(StatusCode.NotFound, $"Item {requestItem.Id} not found."));

                if (storedItem.Quantity < requestItem.Quantity)
                    throw new RpcException(new Status(StatusCode.OutOfRange, $"Quantity for item {requestItem.Id} is too much."));
            }

            foreach (var requestItem in request.Items) {
                bool ok = storedItemsMap.TryGetValue(requestItem.Id, out var storedItem);
                storedItem.Quantity -= requestItem.Quantity;
            }

            return Task.FromResult(new DeductQuantityResponse() {
                Success = true,
                Message = "Quantity deducted successfully."
            });
        }

        public override Task<GetAllInventoriesResponse> GetAllInventories(Empty request, ServerCallContext context) {
            var response = new GetAllInventoriesResponse();
            response.Items.AddRange(items.GetAllItems().Select(i => new Protos.Item {
                Id = i.Id,
                Quantity = i.Quantity
            }));

            return Task.FromResult(response);
        }

        public override Task<GetInventoryResponse> GetInventory(GetInventoryRequest request, ServerCallContext context) {
            var storedItem = items.GetItemById(request.ItemId);
            if (storedItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Item {request.ItemId} not found."));

            return Task.FromResult(new GetInventoryResponse() {
                ItemId = storedItem.Id,
                Quantity = storedItem.Quantity
            });
        }
    }
}
