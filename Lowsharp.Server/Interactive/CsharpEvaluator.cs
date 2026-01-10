using System.Runtime.CompilerServices;

using Lowsharp.Server.Interactive.Formating;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;

namespace Lowsharp.Server.Interactive;

internal sealed class CsharpEvaluator
{
    private readonly SessionManager _sessionManager;
    private readonly TextWithFormatFactory _formatter;
    private readonly ScriptOptions _options;
    private readonly AdhocWorkspace _workspace;

    public CsharpEvaluator(SessionManager sessionManager, TextWithFormatFactory formatter)
    {
        _sessionManager = sessionManager;
        _formatter = formatter;
        _workspace = new AdhocWorkspace(MefHostServices.DefaultHost);
        _options = ScriptOptions.Default
            .WithImports("System", "System.IO", "System.Collections.Generic", "System.Linq", "System.Threading.Tasks")
            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)));
    }

    public async Task<Guid> CreateSession()
    {
        var id = Guid.NewGuid();
        ScriptState state = await CSharpScript.RunAsync("", _options);
        Document document = CreateDocumentForHighlight(state, id, "");
        _sessionManager.Create(id, state, document);
        return id;
    }

    public async IAsyncEnumerable<TextWithFormat> EvaluateAsync(Guid sessionId, string code, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        (ScriptState state, Document document)? session = _sessionManager.GetSessionState(sessionId);
        if (session == null)
        {
            yield return "Error: Session not Initialized";
            yield break;
        }

        object returnValue;

        Document newDoc = session.Value.document.WithText(SourceText.From(code));
        try
        {
            ScriptState<object> newState = await session.Value.state.ContinueWithAsync(code, _options, cancellationToken);
            _sessionManager.Update(sessionId, newState, newDoc);
            returnValue = newState.ReturnValue;
        }
        catch (Exception ex)
        {
            returnValue = ex;
        }

        IAsyncEnumerable<TextWithFormat> formattedPrompt = CsharpFormatter.Format(newDoc); 
        IEnumerable<TextWithFormat> resultParts = _formatter.Format(returnValue);

        await foreach(var promptPart in formattedPrompt)
        {
            yield return promptPart;
        }
        yield return Environment.NewLine;
        yield return new string('-', 80);
        yield return Environment.NewLine;
        foreach (var resultPart in resultParts)
        {
            yield return resultPart;
        }
    }

    private Document CreateDocumentForHighlight(ScriptState state, Guid sessionId, string code)
    {
        ProjectId project = ProjectId.CreateFromSerialized(sessionId);
        DocumentId documentId = DocumentId.CreateNewId(project);
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var parseOptions = new CSharpParseOptions(kind: SourceCodeKind.Script);
        var projectInfo = ProjectInfo.Create(project,
                                             VersionStamp.Create(),
                                             name: "ScriptProject",
                                             assemblyName: "ScriptProject",
                                             language: LanguageNames.CSharp,
                                             compilationOptions: compilationOptions,
                                             parseOptions: parseOptions);

        Solution solution = _workspace.CurrentSolution.AddProject(projectInfo);

        foreach (MetadataReference reference in state.Script.Options.MetadataReferences)
        {
            solution = solution.AddMetadataReference(project, reference);
        }

        var sourceText = SourceText.From(code);

        solution = solution.AddDocument(
            documentId,
            name: "Submission.csx",
            text: sourceText);

        return solution.GetDocument(documentId)!;
    }
}
