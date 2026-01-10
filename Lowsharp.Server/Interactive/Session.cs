using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Interactive;

internal sealed class Session
{
    public required DateTimeOffset LastAccessUtc { get; set; }
    public required ScriptState ScriptState { get; set; }
    public required Document Document { get; set; }
}
