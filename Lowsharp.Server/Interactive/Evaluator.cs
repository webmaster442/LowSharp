using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Interactive;

internal sealed class Evaluator
{
    private readonly SessionManager _sessionManager;
    private readonly ScriptOptions _options;

    public Evaluator(SessionManager sessionManager)
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

    public async Task<string> EvaluateAsync(Guid sessionId, string code, CancellationToken cancellationToken)
    {
        ScriptState? state = _sessionManager.GetSessionState(sessionId);
        if (state == null)
        {
            return "Error: Session not Initialized";
        }

        var newState = await state.ContinueWithAsync(code, _options, cancellationToken);
        _sessionManager.Update(sessionId, newState);

        return newState.ReturnValue?.ToString() ?? "null";
    }
}
