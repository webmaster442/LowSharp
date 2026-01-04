using System.Globalization;

using FSharp.Compiler.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Lowsharp.Server.Lowering;

internal static class Mappers
{
    public static Microsoft.CodeAnalysis.CSharp.LanguageVersion ToLanguageVersion(this CsharpLanguageVersion csharpLanguageVersion)
    {
        return csharpLanguageVersion switch
        {
            CsharpLanguageVersion.CSharp1 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp1,
            CsharpLanguageVersion.CSharp2 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp2,
            CsharpLanguageVersion.CSharp3 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp3,
            CsharpLanguageVersion.CSharp4 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp4,
            CsharpLanguageVersion.CSharp5 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp5,
            CsharpLanguageVersion.CSharp6 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp6,
            CsharpLanguageVersion.CSharp7 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7,
            CsharpLanguageVersion.CSharp7_1 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_1,
            CsharpLanguageVersion.CSharp7_2 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_2,
            CsharpLanguageVersion.CSharp7_3 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_3,
            CsharpLanguageVersion.CSharp8 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8,
            CsharpLanguageVersion.CSharp9 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp9,
            CsharpLanguageVersion.CSharp10 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp10,
            CsharpLanguageVersion.CSharp11 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp11,
            CsharpLanguageVersion.CSharp12 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp12,
            CsharpLanguageVersion.CSharp13 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp13,
            CsharpLanguageVersion.CSharp14 => Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp14,
            CsharpLanguageVersion.LatestMajor => Microsoft.CodeAnalysis.CSharp.LanguageVersion.LatestMajor,
            CsharpLanguageVersion.Preview => Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview,
            CsharpLanguageVersion.Latest => Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest,
            CsharpLanguageVersion.Default => Microsoft.CodeAnalysis.CSharp.LanguageVersion.Default,
            _ => throw new ArgumentOutOfRangeException(nameof(csharpLanguageVersion), csharpLanguageVersion, null),
        };
    }

    public static Microsoft.CodeAnalysis.VisualBasic.LanguageVersion ToLanguageVersion(this VisualBasicLanguageVersion visualBasicLanguageVersion)
    {
        return visualBasicLanguageVersion switch
        {
            VisualBasicLanguageVersion.VisualBasic9 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9,
            VisualBasicLanguageVersion.VisualBasic10 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic10,
            VisualBasicLanguageVersion.VisualBasic11 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic11,
            VisualBasicLanguageVersion.VisualBasic12 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic12,
            VisualBasicLanguageVersion.VisualBasic14 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic14,
            VisualBasicLanguageVersion.VisualBasic15 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15,
            VisualBasicLanguageVersion.VisualBasic15_3 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3,
            VisualBasicLanguageVersion.VisualBasic15_5 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_5,
            VisualBasicLanguageVersion.VisualBasic16 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16,
            VisualBasicLanguageVersion.VisualBasic16_9 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9,
            VisualBasicLanguageVersion.VisualBasic17_13 => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic17_13,
            VisualBasicLanguageVersion.Latest => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest,
            VisualBasicLanguageVersion.Default => Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Default,
            _ => throw new ArgumentOutOfRangeException(nameof(visualBasicLanguageVersion), visualBasicLanguageVersion, null),
        };
    }

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
