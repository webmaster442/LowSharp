using Google.Protobuf.WellKnownTypes;

namespace LowSharp.Common;

internal static class EnumMapper
{
    public static string ToString(ApiV1.Lowering.InputLanguage inputLanguage)
    {
        return inputLanguage switch
        {
            ApiV1.Lowering.InputLanguage.Csharp => "C#",
            ApiV1.Lowering.InputLanguage.Visualbasic => "VB.NET",
            ApiV1.Lowering.InputLanguage.Fsharp => "F#",
            _ => throw new ArgumentOutOfRangeException(nameof(inputLanguage), inputLanguage, null),
        };
    }

    public static string ToString(ApiV1.Lowering.OutputCodeType outputLanguage)
    {
        return outputLanguage switch
        {
            ApiV1.Lowering.OutputCodeType.Il => "IL",
            ApiV1.Lowering.OutputCodeType.Loweredcsharp => "Lowered C#",
            ApiV1.Lowering.OutputCodeType.Jitasm => "JIT ASM",
            ApiV1.Lowering.OutputCodeType.Syntaxtreejson => "Syntax Tree (JSON)",
            ApiV1.Lowering.OutputCodeType.Nonmoml => "Nonmoml Diagram",
            ApiV1.Lowering.OutputCodeType.Mermaid => "Mermaid Diagram",
            _ => throw new ArgumentOutOfRangeException(nameof(outputLanguage), outputLanguage, null),
        };
    }

    public static string ToString(ApiV1.Lowering.Optimization optimization)
    {
        return optimization switch
        {
            ApiV1.Lowering.Optimization.Release => "Release",
            ApiV1.Lowering.Optimization.Debug => "Debug",
            _ => throw new ArgumentOutOfRangeException(nameof(optimization), optimization, null),
        };
    }
}
