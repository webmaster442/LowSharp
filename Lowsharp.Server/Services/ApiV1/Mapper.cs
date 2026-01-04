
using Lowsharp.Server.Lowering;

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
}
