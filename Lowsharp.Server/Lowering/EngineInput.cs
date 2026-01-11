namespace Lowsharp.Server.Lowering;

public sealed record class EngineInput
{
    public required string Code { get; init; }
    public required InputLanguage InputLanguage { get; init; }
    public OutputOptimizationLevel OutputOptimizationLevel { get; init; } = OutputOptimizationLevel.Debug;
    public OutputLanguage OutputLanguage { get; init; } = OutputLanguage.Csharp;
}
