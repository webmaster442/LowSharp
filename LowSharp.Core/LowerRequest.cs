namespace LowSharp.Core;

public sealed class LowerRequest
{
    public required string Code { get; init; }
    public required InputLanguage InputLanguage { get; init; }
    public CsharpLanguageVersion CsharpLanguageVersion { get; init; } = CsharpLanguageVersion.Preview;
    public VisualBasicLanguageVersion VisualBasicLanguageVersion { get; init; } = VisualBasicLanguageVersion.Latest;
    public OutputOptimizationLevel OutputOptimizationLevel { get; init; } = OutputOptimizationLevel.Debug;
    public OutputLanguage OutputType { get; init; } = OutputLanguage.Csharp;

}