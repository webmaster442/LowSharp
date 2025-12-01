namespace LowSharp.Core;

public sealed class LowerResponse
{
    public string LoweredCode { get; private set; }
    public List<LoweringDiagnostic> Diagnostics { get; }
    public bool HasErrors => Diagnostics.Any(d => d.Severity == MessageSeverity.Error);

    public LowerResponse()
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
