using Lowsharp.Server.Interactive;
using Lowsharp.Server.Lowering;

using LowSharp.ApiV1.Evaluate;
using LowSharp.ApiV1.Lowering;

namespace Lowsharp.Server.Services.ApiV1;

internal static class Mapper
{
    public static LoweringResponse Map(EngineOutput result)
    {
        var returnValue = new LoweringResponse
        {
            ResultCode = result.LoweredCode
        };
        returnValue.Diagnostics.AddRange(result.Diagnostics.Select(d => new Diagnostic
        {
            Message = d.Message,
            Severity = d.Severity switch
            {
                MessageSeverity.Info => DiagnosticSeverity.Info,
                MessageSeverity.Warning => DiagnosticSeverity.Warning,
                MessageSeverity.Error => DiagnosticSeverity.Error,
                _ => throw new InvalidOperationException("Unknown severity")
            }
        }));
        return returnValue;
    }

    public static EngineInput Map(LoweringRequest request)
    {
        return new EngineInput
        {
            Code = request.Code,
            InputLanguage = request.Language switch
            {
                LowSharp.ApiV1.Lowering.InputLanguage.Csharp => Lowering.InputLanguage.Csharp,
                LowSharp.ApiV1.Lowering.InputLanguage.Fsharp => Lowering.InputLanguage.FSharp,
                LowSharp.ApiV1.Lowering.InputLanguage.VisualBasic => Lowering.InputLanguage.VisualBasic,
                _ => throw new InvalidOperationException("Unknown Input language")
            },
            OutputType = request.OutputType switch
            {
                OutputCodeType.Il => OutputLanguage.IL,
                OutputCodeType.JitAsm => OutputLanguage.JitAsm,
                OutputCodeType.LoweredCsharp => OutputLanguage.Csharp,
                _ => throw new InvalidOperationException("Unknown Output type")
            },
            OutputOptimizationLevel = request.OptimizationLevel switch
            {
                Optimization.Debug => OutputOptimizationLevel.Debug,
                Optimization.Release => OutputOptimizationLevel.Release,
                _ => throw new InvalidOperationException("Unknown optimization level")
            }
        };
    }

    public static FormattedText Map(TextWithFormat output)
    {
        return new FormattedText
        {
            Text = output.Text,
            IsBold = output.Bold,
            IsItalic = output.Italic,
            Color = output.Color switch
            {
                Interactive.ForegroundColor.Default => LowSharp.ApiV1.Evaluate.ForegroundColor.Default,
                Interactive.ForegroundColor.Black => LowSharp.ApiV1.Evaluate.ForegroundColor.Black,
                Interactive.ForegroundColor.Blue => LowSharp.ApiV1.Evaluate.ForegroundColor.Blue,
                Interactive.ForegroundColor.Cyan => LowSharp.ApiV1.Evaluate.ForegroundColor.Cyan,
                Interactive.ForegroundColor.Green => LowSharp.ApiV1.Evaluate.ForegroundColor.Green,
                Interactive.ForegroundColor.Purple => LowSharp.ApiV1.Evaluate.ForegroundColor.Purple,
                Interactive.ForegroundColor.Red => LowSharp.ApiV1.Evaluate.ForegroundColor.Red,
                Interactive.ForegroundColor.White => LowSharp.ApiV1.Evaluate.ForegroundColor.White,
                Interactive.ForegroundColor.Yellow => LowSharp.ApiV1.Evaluate.ForegroundColor.Yellow,
                Interactive.ForegroundColor.BrightBlack => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightBlack,
                Interactive.ForegroundColor.BrightBlue => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightBlue,
                Interactive.ForegroundColor.BrightCyan => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightCyan,
                Interactive.ForegroundColor.BrightGreen => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightGreen,
                Interactive.ForegroundColor.BrightPurple => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightPurple,
                Interactive.ForegroundColor.BrightRed => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightRed,
                Interactive.ForegroundColor.BrightWhite => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightWhite,
                Interactive.ForegroundColor.BrightYellow => LowSharp.ApiV1.Evaluate.ForegroundColor.BrightYellow,
                _ => throw new InvalidOperationException("Unknown color")
            }
        };
    }
}
