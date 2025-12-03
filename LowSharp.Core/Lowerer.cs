

using System.Threading.Tasks;

using LowSharp.Core.Internals;
using LowSharp.Core.Internals.Compilers;
using LowSharp.Core.Internals.Decompilers;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace LowSharp.Core;

public sealed class Lowerer
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly CsharpCompiler _csCompiler;
    private readonly VisualBasicCompiler _vbCompiler;
    private readonly FsharpCompiler _fsCompiler;
    private readonly ReferenceProvider _referenceProvider;

    public Lowerer()
    {
        _referenceProvider = new ReferenceProvider();
        _memoryStreamManager = new RecyclableMemoryStreamManager();

        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

        _csCompiler = new CsharpCompiler(_referenceProvider.References, emitOptions);
        _vbCompiler = new VisualBasicCompiler(_referenceProvider.References, emitOptions);
        _fsCompiler = new FsharpCompiler();
    }

    public IEnumerable<ComponentVersion> GetComponentVersions()
        => VersionCollector.GetComponentVersions();

    private async Task<(bool result, IEnumerable<LoweringDiagnostic> diagnostics)> Compile(LowerRequest request,
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

            InputLanguage.FSharp=> await _fsCompiler.Compile(request.Code,
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
            _ => throw new NotSupportedException("Unsupported output type."),
        };
    }

    public async Task<LowerResponse> ToLowerCodeAsync(LowerRequest request, CancellationToken cancellationToken)
    {
        await using var pdbStream = _memoryStreamManager.GetStream();
        await using var assemblyStream = _memoryStreamManager.GetStream();

        var response = new LowerResponse();

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