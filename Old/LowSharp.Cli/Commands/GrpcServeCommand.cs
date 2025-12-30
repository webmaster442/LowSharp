using LowSharp.Cli.Services;
using LowSharp.Core;

using Spectre.Console.Cli;

namespace LowSharp.Cli.Commands;

internal sealed class GrpcServeCommand : AsyncCommand<GrpcServeCommand.Settings>
{
    private const string msg = """
        Communication with gRPC endpoints must be made through a gRPC client.
        To get the proto file definition, navigate to /proto
        """;

    public sealed class Settings : CommandSettings
    {

    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddSingleton<ILowerer>(new CachedLowerer(new Lowerer()));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<LoweringService>();
        app.MapGet("/", () => msg);
        app.MapGet("/proto", () => GrpcProtoCommand.GetProtoDefinition());

        await app.RunAsync(cancellationToken);

        return ExitCodes.Success;
    }
}