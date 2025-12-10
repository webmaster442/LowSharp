using Spectre.Console.Cli;

namespace LowSharp.Cli.Commands;

internal sealed class GrpcCommand : AsyncCommand<GrpcCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {

    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();

        var app = builder.Build();
        app.MapGrpcService<LoweringService>();

        await app.RunAsync(cancellationToken);
        return ExitCodes.Success;
    }
}