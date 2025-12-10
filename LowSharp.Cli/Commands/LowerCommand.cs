using System.ComponentModel;

using ColorCode;

using LowSharp.Core;

using Spectre.Console;
using Spectre.Console.Cli;

namespace LowSharp.Cli.Commands;

internal sealed class LowerCommand : AsyncCommand<LowerCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Input file path, to lower. Must be a file with extension .cs, .vb or .fs")]
        [CommandArgument(0, "<input-file>")]
        public string InputFile { get; set; } = string.Empty;

        [Description("Output format. Supported formats: csharp, il, jitasm")]
        [CommandOption("-f|--format")]
        public string OutputFormat { get; set; } = "csharp";

        [Description("Output file path. If not specified, output will be printed to console.")]
        [CommandOption("-o|--output")]
        public string OutputFile { get; set; } = string.Empty;

        public override ValidationResult Validate()
        {
            if (!File.Exists(InputFile))
            {
                return ValidationResult.Error($"Input file '{InputFile}' does not exist.");
            }

            var extension = Path.GetExtension(InputFile).ToLowerInvariant();
            if (extension != ".cs" && extension != ".vb" && extension != ".fs")
            {
                return ValidationResult.Error("Input file must have extension .cs, .vb or .fs");
            }

            if (!Enum.TryParse<OutputLanguage>(OutputFormat, ignoreCase: true, out var _))
            {
                return ValidationResult.Error("Output format must be either 'csharp' or 'il' or 'jitasm'");
            }

            return ValidationResult.Success();
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var lowerer = new Lowerer();
        OutputLanguage outLanguage = Enum.Parse<OutputLanguage>(settings.OutputFormat, ignoreCase: true);
        string inputCode = null!;
        LowerResponse result = null!;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Reading file...", async ctx =>
            {
                inputCode = await File.ReadAllTextAsync(settings.InputFile, cancellationToken);
            });

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync("Lowering...", async ctx =>
            {
                result = await lowerer.ToLowerCodeAsync(new LowerRequest
                {
                    Code = inputCode,
                    InputLanguage = GetInputLangugeFromExtension(settings.InputFile),
                }, cancellationToken);

            });

        if (result.Diagnostics.Count > 0)
        {
            PrintDiagnostics(result.Diagnostics);
            if (result.Diagnostics.Any(d => d.Severity == MessageSeverity.Error))
            {
                AnsiConsole.MarkupLine("[red]Lowering failed due to errors.[/]");
                return ExitCodes.Error;
            }
        }

        if (string.IsNullOrEmpty(settings.OutputFile))
        {
            ILanguage? language = GetLanguage(outLanguage);
            if (language != null)
            {
                ConsoleFormatter formatter = new();
                string highlightedCode = formatter.GetHtmlString(result.LoweredCode, language);
                AnsiConsole.MarkupLine(highlightedCode);
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]{result.LoweredCode.EscapeMarkup()}[/]");
            }
        }
        else
        {
            await File.WriteAllTextAsync(settings.OutputFile, result.LoweredCode, cancellationToken);
            AnsiConsole.MarkupLine($"[green]Lowering succeeded. Output written to '{settings.OutputFile}'.[/]");
        }

        return ExitCodes.Success;
    }

    private static void PrintDiagnostics(List<LoweringDiagnostic> diagnostics)
    {
        var table = new Table();
        table.AddColumn("Severity");
        table.AddColumn("Message");
        foreach (var diag in diagnostics)
        {
            string severityMarkup = diag.Severity switch
            {
                MessageSeverity.Info => "[blue]Info[/]",
                MessageSeverity.Warning => "[yellow]Warning[/]",
                MessageSeverity.Error => "[red]Error[/]",
                _ => diag.Severity.ToString(),
            };
            table.AddRow(severityMarkup, diag.Message);
        }
    }

    private static ILanguage? GetLanguage(OutputLanguage outLanguage)
    {
        return outLanguage switch
        {
            OutputLanguage.Csharp => ColorCode.Languages.CSharp,
            OutputLanguage.IL => null,
            OutputLanguage.JitAsm => null,
            _ => throw new InvalidOperationException("Unsupported output language."),
        };
    }

    private static InputLanguage GetInputLangugeFromExtension(string inputFile)
    {
        return Path.GetExtension(inputFile).ToLowerInvariant() switch
        {
            ".cs" => InputLanguage.Csharp,
            ".vb" => InputLanguage.VisualBasic,
            ".fs" => InputLanguage.FSharp,
            _ => throw new InvalidOperationException("Unsupported file extension."),
        };
    }
}