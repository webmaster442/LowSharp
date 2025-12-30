using Spectre.Console;
using Spectre.Console.Cli;

namespace LowSharp.Cli.Commands;

internal sealed class GrpcProtoCommand : Command
{
    public static string GetProtoDefinition()
    {
        using var stream = typeof(GrpcProtoCommand).Assembly.GetManifestResourceStream("LowSharp.Cli.Protos.ApiV1.proto");
        if (stream == null)
        {
            throw new InvalidOperationException("Could not find embedded proto resource.");
        }
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        AnsiConsole.WriteLine(GetProtoDefinition());
        return ExitCodes.Success;
    }
}