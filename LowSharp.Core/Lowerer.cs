

using LowSharp.Core.Internals;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace LowSharp.Core;

public sealed class Lowerer
{
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly CsharpCompiler _csCompiler;
    private readonly VisualBasicCompiler _vbCompiler;
    private readonly ReferenceProvider _referenceProvider;

    public Lowerer()
    {
        _referenceProvider = new ReferenceProvider();
        _memoryStreamManager = new RecyclableMemoryStreamManager();

        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

        _csCompiler = new CsharpCompiler(_referenceProvider.References, emitOptions);
        _vbCompiler = new VisualBasicCompiler(_referenceProvider.References, emitOptions);
    }

    public Task<LowerResponse> ToLowerCodeAsync(LowerRequest request, CancellationToken cancellationToken)
    {
        return Task.Run(() => ToLowerCode(request, cancellationToken), cancellationToken);
    }

    private (bool result, IEnumerable<LoweringDiagnostic> diagnostics) Compile(LowerRequest request,
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
            _ => ((bool result, IEnumerable<LoweringDiagnostic> diagnostics))(false, new List<LoweringDiagnostic>
                {
                    new() {
                        Message = "Unsupported input language.",
                        Severity = MessageSeverity.Error
                    }
                }),
        };
    }

    private static IDecompiler CreateDecompiler(OutputType outputType)
    {
        return outputType switch
        {
            OutputType.Csharp => new CsharpDecompiler(),
            OutputType.IL => new ILDecompiler(),
            _ => throw new NotSupportedException("Unsupported output type."),
        };
    }

    private LowerResponse ToLowerCode(LowerRequest request, CancellationToken cancellationToken)
    {
        using var pdbStream = _memoryStreamManager.GetStream();
        using var assemblyStream = _memoryStreamManager.GetStream();

        var response = new LowerResponse();

        (bool result, IEnumerable<LoweringDiagnostic> diagnostics) = Compile(request,
                                                                             assemblyStream,
                                                                             pdbStream,
                                                                             cancellationToken);




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