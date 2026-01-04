using Lowsharp.Server;
using Lowsharp.Server.Data;
using Lowsharp.Server.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<ServerDbContext>(options => {
    options.UseSqlite($"Data Source={GetDatabasePath()}");
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<JsonDbContextCache>(factory =>
{
    return new JsonDbContextCache(
        factory.GetRequiredService<ServerDbContext>(),
        TimeSpan.FromMinutes(10),
        factory.GetRequiredService<IMemoryCache>(),
        factory.GetRequiredService<ILoggerFactory>());
});

builder.Services.AddHostedService<CacheCleanupService>();

var app = builder.Build();

app.MapGrpcService<Lowsharp.Server.Services.ApiV1.LowererService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.HealthCheckService>();

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


var lst = VersionCollector.LoadedAsssemblyVersions().ToList();

app.Run();

static string GetDatabasePath()
{
    string? dataDir = Environment.GetEnvironmentVariable("DATA_DIR");
    if (string.IsNullOrEmpty(dataDir))
    {
        dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
    }
    if (!Directory.Exists(dataDir))
    {
        Directory.CreateDirectory(dataDir);
    }
    string dbPath = Path.Combine(dataDir, "lowsharp_server.db");
    return dbPath;
}