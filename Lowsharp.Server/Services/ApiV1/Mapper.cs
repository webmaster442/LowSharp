using System.Text.RegularExpressions;

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
                LowSharp.ApiV1.Lowering.InputLanguage.Visualbasic => Lowering.InputLanguage.VisualBasic,
                _ => throw new InvalidOperationException("Unknown Input language")
            },
            OutputLanguage = request.OutputType switch
            {
                OutputCodeType.Il => OutputLanguage.IL,
                OutputCodeType.Jitasm => OutputLanguage.JitAsm,
                OutputCodeType.Loweredcsharp => OutputLanguage.Csharp,
                OutputCodeType.Syntaxtreejson => OutputLanguage.SyntaxTreeJson,
                OutputCodeType.Nonmoml => OutputLanguage.NomnommlClassTree,
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

    public static RegexOptions Map(LowSharp.ApiV1.Regex.RegexOptions options)
    {
        RegexOptions result = RegexOptions.None;

        if (options.IgnoreCase)
            result |= RegexOptions.IgnoreCase;
        if (options.Multiline)
            result |= RegexOptions.Multiline;
        if (options.ExplicitCapture)
            result |= RegexOptions.ExplicitCapture;
        if (options.Compiled)
            result |= RegexOptions.Compiled;
        if (options.Singleline)
            result |= RegexOptions.Singleline;
        if (options.IgnorePatternWhitespace)
            result |= RegexOptions.IgnorePatternWhitespace;
        if (options.RightToLeft)
            result |= RegexOptions.RightToLeft;
        if (options.EcmaScript)
            result |= RegexOptions.ECMAScript;
        if (options.CultureInvariant)
            result |= RegexOptions.CultureInvariant;
        if (options.NonBackTracking)
            result |= RegexOptions.NonBacktracking;

        return result;
    }

    public static IEnumerable<LowSharp.ApiV1.Regex.RegexMatch> Map(MatchCollection matches)
    {
        foreach (Match m in matches)
        {
            yield return new LowSharp.ApiV1.Regex.RegexMatch
            {
                Value = m.Value,
                Success = m.Success,
                Index = m.Index,
                Length = m.Length,
                Name = m.Name,
            };
        }
    }
}
