using System.Globalization;

using FSharp.Compiler.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Lowsharp.Server.Lowering;

internal static class Mappers
{
    public static MessageSeverity ToMessageSeverity(DiagnosticSeverity severity)
    {
        return severity switch
        {
            DiagnosticSeverity.Info => MessageSeverity.Info,
            DiagnosticSeverity.Warning => MessageSeverity.Warning,
            DiagnosticSeverity.Error => MessageSeverity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null),
        };
    }

    public static MessageSeverity ToMessageSeverity(this FSharpDiagnosticSeverity severity)
    {
        if (severity.IsInfo)
            return MessageSeverity.Info;

        if (severity.IsWarning)
            return MessageSeverity.Warning;

        if (severity.IsError)
            return MessageSeverity.Error;

        throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
    }

    public static LoweringDiagnostic ToLoweringDiagnostic(this Diagnostic diagnostic)
    {
        return new LoweringDiagnostic
        {
            Message = $"{diagnostic.Location.SourceSpan.Start}: {diagnostic.GetMessage(CultureInfo.InvariantCulture)}",
            Severity = ToMessageSeverity(diagnostic.Severity)
        };
    }

    public static LoweringDiagnostic ToLoweringDiagnostic(this FSharpDiagnostic diagnostic)
    {
        return new LoweringDiagnostic
        {
            Message = diagnostic.Message,
            Severity = diagnostic.Severity.ToMessageSeverity()
        };
    }

    public static OptimizationLevel ToOptimizationLevel(this OutputOptimizationLevel outputOptimizationLevel)
    {
        return outputOptimizationLevel switch
        {
            OutputOptimizationLevel.Debug => OptimizationLevel.Debug,
            OutputOptimizationLevel.Release => OptimizationLevel.Release,
            _ => throw new ArgumentOutOfRangeException(nameof(outputOptimizationLevel), outputOptimizationLevel, null),
        };
    }
}
