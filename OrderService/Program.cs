using InventoryService.Protos;
using OrderService.Services;
using PaymentService.Protos;

namespace OrderService {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddGrpc().AddJsonTranscoding();
            builder.Services.AddGrpcReflection();

            builder.Services.AddCors(options =>
                options.AddPolicy("Default", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")
                )
            );

            builder.Services.AddGrpcClient<InventoryProtoService.InventoryProtoServiceClient>(options => {
                options.Address = new Uri("http://localhost:5266");
            });

            builder.Services.AddGrpcClient<PaymentProtoService.PaymentProtoServiceClient>(options => {
                options.Address = new Uri("http://localhost:5189");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.MapGrpcReflectionService();
            }

            app.UseAuthorization();

            app.UseCors("Default");

            app.MapGrpcService<OrderGrpcService>().RequireCors("Default");

            app.MapControllers();

            app.Run();
        }
    }
}
