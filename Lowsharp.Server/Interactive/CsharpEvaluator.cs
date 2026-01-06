using System.Runtime.CompilerServices;

using Lowsharp.Server.Interactive.Formating;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Interactive;

internal sealed class CsharpEvaluator
{
    private readonly SessionManager _sessionManager;
    private readonly FormatterComponent _formatter;
    private readonly ScriptOptions _options;

    public CsharpEvaluator(SessionManager sessionManager, FormatterComponent formatter)
    {
        _sessionManager = sessionManager;
        _formatter = formatter;
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

    public async IAsyncEnumerable<TextWithFormat> EvaluateAsync(Guid sessionId, string code, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ScriptState? state = _sessionManager.GetSessionState(sessionId);
        if (state == null)
        {
            yield return "Error: Session not Initialized";
            yield break;
        }

        object returnValue;

        try
        {
            ScriptState<object> newState = await state.ContinueWithAsync(code, _options, cancellationToken);
            _sessionManager.Update(sessionId, newState);
            returnValue = newState.ReturnValue;
        }
        catch (Exception ex)
        {
            returnValue = ex;
        }

        var formatParts = _formatter.Format(returnValue);

        foreach (var part in formatParts)
        {
            yield return part;
        }
    }
}
