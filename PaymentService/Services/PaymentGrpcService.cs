using Grpc.Core;
using PaymentService.Models;
using PaymentService.Protos;

namespace PaymentService.Services {
    public class PaymentGrpcService : PaymentProtoService.PaymentProtoServiceBase {
        private readonly UsersBL users = new UsersBL();

        public override Task<DeductPaymentResponse> DeductPayment(DeductPaymentRequest request, ServerCallContext context) {
            var storedUser = users.GetUserById(request.Id);
            if (storedUser == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User {request.Id} not found."));

            if (storedUser.Amount < request.Amount)
                throw new RpcException(new Status(StatusCode.OutOfRange, $"Amount for user {request.Id} is too much"));

            storedUser.Amount -= request.Amount;

            return Task.FromResult(new DeductPaymentResponse() {
                Success = true,
                Message = "Payment deducted successfully."
            });
        }

        public override Task<GetPaymentResponse> GetPayment(GetPaymentRequest request, ServerCallContext context) {
            var storedUser = users.GetUserById(request.Id);
            if (storedUser == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User {request.Id} not found."));

            return Task.FromResult(new GetPaymentResponse() {
                Amount = storedUser.Amount
            });
        }
    }
}
