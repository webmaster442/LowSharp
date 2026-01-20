using System;

using Lowsharp.Server;
using Lowsharp.Server.CodeExamples;
using Lowsharp.Server.Infrastructure;
using Lowsharp.Server.Lowering;
using Lowsharp.Server.Visualization;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ExampleProvider>();
builder.Services.AddSingleton<TimeProvider>((services) => TimeProvider.System);
builder.Services.AddSingleton<LoweringEngine>();
builder.Services.AddSingleton<RequestCache>();
builder.Services.AddScoped<RazorViewRenderer>();

var app = builder.Build();

app.MapGrpcService<Lowsharp.Server.Services.ApiV1.LowererService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.HealthCheckService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.RegexService>();
app.MapGrpcService<Lowsharp.Server.Services.ApiV1.ExamplesService>();

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
