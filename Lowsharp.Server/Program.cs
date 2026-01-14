using Lowsharp.Server;
using Lowsharp.Server.Infrastructure;
using Lowsharp.Server.Lowering;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<TimeProvider>((services) => TimeProvider.System);
builder.Services.AddSingleton<LoweringEngine>();
builder.Services.AddSingleton<RequestCache>();

var app = builder.Build();

app.MapGrpcService<Lowsharp.Server.Services.ApiV1.LowererService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.HealthCheckService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.RegexService>();

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
