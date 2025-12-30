namespace Lowsharp.Server.Lowering;

public sealed class EngineOutput
{
    public string LoweredCode { get; private set; }
    public List<LoweringDiagnostic> Diagnostics { get; }
    public bool HasErrors => Diagnostics.Any(d => d.Severity == MessageSeverity.Error);

    public EngineOutput()
    {
        LoweredCode = string.Empty;
        Diagnostics = new List<LoweringDiagnostic>();
    }

    internal void AppendDiagnostics(IEnumerable<LoweringDiagnostic> diagnostics)
    {
        Diagnostics.AddRange(diagnostics);
    }

    internal void SetDecompilation(string loweredCode)
    {
        LoweredCode = loweredCode;
    }
}