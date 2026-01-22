using Lowsharp.Server.Lowering.Compilers;
using Lowsharp.Server.Lowering.Decompilers;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace Lowsharp.Server.Lowering;

public sealed class LoweringEngine
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly CsharpCompiler _csCompiler;
    private readonly VisualBasicCompiler _vbCompiler;
    private readonly FsharpCompiler _fsCompiler;
    private readonly ReferenceProvider _referenceProvider;

    public LoweringEngine()
    {
        _referenceProvider = new ReferenceProvider();
        _memoryStreamManager = new RecyclableMemoryStreamManager();

        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

        _csCompiler = new CsharpCompiler(_referenceProvider.MetadataReferences, _referenceProvider.SourceGeneratgors, emitOptions);
        _vbCompiler = new VisualBasicCompiler(_referenceProvider.MetadataReferences, emitOptions);
        _fsCompiler = new FsharpCompiler();
    }

    private static IDecompiler CreateDecompiler(OutputLanguage outputType)
    {
        return outputType switch
        {
            OutputLanguage.Csharp => new CsharpDecompiler(),
            OutputLanguage.IL => new ILDecompiler(),
            OutputLanguage.JitAsm => new JitDecompiler(),
            OutputLanguage.NomnommlClassTree => new NomnomClassTreeDecompiler(),
            OutputLanguage.MermaidClassTree => new MermaidClassTreeDecompiler(),
            _ => throw new NotSupportedException("Unsupported output type."),
        };
    }

    public async Task<EngineOutput> ToLowerCodeAsync(EngineInput request, CancellationToken cancellationToken)
    {
        await using var pdbStream = _memoryStreamManager.GetStream();
        await using var assemblyStream = _memoryStreamManager.GetStream();

        var response = new EngineOutput();

        ICompiler compiler = request.InputLanguage switch
        {
            InputLanguage.Csharp => _csCompiler,
            InputLanguage.VisualBasic => _vbCompiler,
            InputLanguage.FSharp => _fsCompiler,
            _ => throw new NotSupportedException("Unsupported input language."),
        };

        if (request.OutputLanguage == OutputLanguage.SyntaxTreeJson)
        {
            string json = await compiler.CompileToSyntaxTreeJsonAsync(request.Code, cancellationToken);
            response.SetDecompilation(json);
            return response;
        }

        var compileResult = await compiler.CompileAsync(request.Code,
                                                        request.OutputOptimizationLevel,
                                                        assemblyStream,
                                                        pdbStream,
                                                        cancellationToken);

        response.AppendDiagnostics(compileResult.Diagnostics);

        if (!compileResult.Success)
        {
            response.SetDecompilation(string.Empty);
            return response;
        }

        IDecompiler decompiler = CreateDecompiler(request.OutputLanguage);

        if (!decompiler.TryDecompile(assemblyStream, pdbStream, out string loweredCode))
        {
            response.AppendDiagnostics(new List<LoweringDiagnostic>
            {
                new() {
                    Message = "Lowering failed.",
                    Severity = MessageSeverity.Error
                },
                new() {
                    Message = loweredCode,
                    Severity = MessageSeverity.Error
                }
            });
            response.SetDecompilation(string.Empty);
            return response;
        }

        response.SetDecompilation(loweredCode);
        return response;
    }
}
