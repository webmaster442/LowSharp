using LowSharp.Core;

using Spectre.Console;
using Spectre.Console.Cli;

namespace LowSharp.Cli.Commands;

internal sealed class ComponentsCommand : Command
{
    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        var lowerer = new Lowerer();
        IEnumerable<ComponentVersion> versions = lowerer.GetComponentVersions();

        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn("Version");

        foreach (var version in versions)
        {
            table.AddRow(version.Name, version.Version.ToString());
        }

        AnsiConsole.Write(table);
        return ExitCodes.Success;
    }
}