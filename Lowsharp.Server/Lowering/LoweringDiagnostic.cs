namespace Lowsharp.Server.Lowering;

public sealed class LoweringDiagnostic
{
    public required string Message { get; init; }
    public required MessageSeverity Severity { get; init; }
}