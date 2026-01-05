using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Interactive;

internal sealed class CsharpEvaluator
{
    private readonly SessionManager _sessionManager;
    private readonly ScriptOptions _options;

    public CsharpEvaluator(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
        _options = ScriptOptions.Default
            .WithImports("System", "System.IO", "System.Collections.Generic", "System.Linq", "System.Threading.Tasks")
            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)));
    }

    public async Task<Guid> CreateSession()
    {
        var id = Guid.NewGuid();
        ScriptState state = await CSharpScript.RunAsync("", _options);
        _sessionManager.Create(id, state);
        return id;
    }

    public async IAsyncEnumerable<string> EvaluateAsync(Guid sessionId, string code, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ScriptState? state = _sessionManager.GetSessionState(sessionId);
        if (state == null)
        {
            yield return "Error: Session not Initialized";
            yield break;
        }

        var newState = await state.ContinueWithAsync(code, _options, cancellationToken);
        
        _sessionManager.Update(sessionId, newState);

        yield return newState.ReturnValue?.ToString() ?? "null";
    }
}
