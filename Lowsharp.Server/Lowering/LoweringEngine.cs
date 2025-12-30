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

        _csCompiler = new CsharpCompiler(_referenceProvider.References, emitOptions);
        _vbCompiler = new VisualBasicCompiler(_referenceProvider.References, emitOptions);
        _fsCompiler = new FsharpCompiler();
    }

    private async Task<(bool result, IEnumerable<LoweringDiagnostic> diagnostics)> Compile(EngineInput request,
                                                                                           RecyclableMemoryStream assemblyStream,
                                                                                           RecyclableMemoryStream pdbStream,
                                                                                           CancellationToken cancellationToken)
    {


        return request.InputLanguage switch
        {
            InputLanguage.Csharp => _csCompiler.CompileCsharp(request.Code,
                                                              request.CsharpLanguageVersion,
                                                              request.OutputOptimizationLevel,
                                                              assemblyStream,
                                                              pdbStream,
                                                              cancellationToken),

            InputLanguage.VisualBasic => _vbCompiler.CompileVisualBasic(request.Code,
                                                                        request.VisualBasicLanguageVersion,
                                                                        request.OutputOptimizationLevel,
                                                                        assemblyStream,
                                                                        pdbStream,
                                                                        cancellationToken),

            InputLanguage.FSharp => await _fsCompiler.Compile(request.Code,
                                                             request.OutputOptimizationLevel,
                                                             assemblyStream,
                                                             pdbStream,
                                                             cancellationToken),

            _ => ((bool result, IEnumerable<LoweringDiagnostic> diagnostics))(false, new List<LoweringDiagnostic>
                {
                    new() {
                        Message = "Unsupported input language.",
                        Severity = MessageSeverity.Error
                    }
                }),
        };
    }

    private static IDecompiler CreateDecompiler(OutputLanguage outputType)
    {
        return outputType switch
        {
            OutputLanguage.Csharp => new CsharpDecompiler(),
            OutputLanguage.IL => new ILDecompiler(),
            OutputLanguage.JitAsm => new JitDecompiler(),
            _ => throw new NotSupportedException("Unsupported output type."),
        };
    }

    public async Task<EngineOutput> ToLowerCodeAsync(EngineInput request, CancellationToken cancellationToken)
    {
        await using var pdbStream = _memoryStreamManager.GetStream();
        await using var assemblyStream = _memoryStreamManager.GetStream();

        var response = new EngineOutput();

        (bool result, IEnumerable<LoweringDiagnostic> diagnostics) = await Compile(request, assemblyStream, pdbStream, cancellationToken);

        response.AppendDiagnostics(diagnostics);

        if (!result)
        {
            response.SetDecompilation(string.Empty);
            return response;
        }

        IDecompiler decompiler = CreateDecompiler(request.OutputType);

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