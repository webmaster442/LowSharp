using Lowsharp.Server.CodeExamples;
using Lowsharp.Server.Infrastructure;
using Lowsharp.Server.Lowering;
using Lowsharp.Server;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ExampleProvider>();
builder.Services.AddSingleton<TimeProvider>((services) => TimeProvider.System);
builder.Services.AddSingleton<LoweringEngine>();
builder.Services.AddSingleton<RequestCache>();

var app = builder.Build();

app.MapGrpcService<Lowsharp.Server.Services.ApiV1.LowererService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.HealthCheckService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.RegexService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.ExamplesService>();

// Configure the HTTP request pipeline.
var contentTypeProvider = new FileExtensionContentTypeProvider();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapEmbeddedFile("script/graphere.js", "Lowsharp.Server.Visualization.script.graphere.js", contentTypeProvider);
app.MapEmbeddedFile("script/nomnoml.js", "Lowsharp.Server.Visualization.script.nomnoml.js", contentTypeProvider);


if (!HasReferencePacks())
{
    Console.WriteLine("WARNING: .NET reference packs not found. Lowering will work correctly.");
    Console.WriteLine("Please install .NET SDK instead of Runtime");
    return;
}

app.Run();


static bool HasReferencePacks()
{
    var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "dotnet");

    var packsPath = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");

    return Directory.Exists(packsPath);
}
