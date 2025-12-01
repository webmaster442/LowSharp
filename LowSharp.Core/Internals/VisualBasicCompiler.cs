using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.IO;

namespace LowSharp.Core.Internals;

internal sealed class VisualBasicCompiler
{
    private readonly VisualBasicCompilationOptions _compilerOptions;
    private readonly IEnumerable<PortableExecutableReference> _references;
    private readonly EmitOptions _emitOptions;

    public VisualBasicCompiler(IEnumerable<PortableExecutableReference> references, EmitOptions emitOptions)
    {
        _references = references;
        _emitOptions = emitOptions;
        _compilerOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOptionStrict(OptionStrict.On)
            .WithOptionInfer(true)
            .WithPlatform(Platform.AnyCpu);
    }

    public (bool result, IEnumerable<LoweringDiagnostic> diagnostics) CompileVisualBasic(string code,
                                                                                         VisualBasicLanguageVersion languageVersion,
                                                                                         OutputOptimizationLevel outputOptimizationLevel,
                                                                                         RecyclableMemoryStream assemblyStream,
                                                                                         RecyclableMemoryStream pdbStream,
                                                                                         CancellationToken cancellationToken)
    {
        try
        {
            var syntaxTree = VisualBasicSyntaxTree.ParseText(code, VisualBasicParseOptions.Default.WithLanguageVersion(languageVersion.ToLanguageVersion()));
            
            VisualBasicCompilation compilation = VisualBasicCompilation.Create("inMemory")
                .WithOptions(_compilerOptions.WithOptimizationLevel(outputOptimizationLevel.ToOptimizationLevel()))
                .AddReferences(_references)
                .AddSyntaxTrees(syntaxTree);

            EmitResult result = compilation.Emit(assemblyStream, pdbStream, options: _emitOptions, cancellationToken: cancellationToken);

            var messages = result.Diagnostics
                .Where(d => d.Severity != DiagnosticSeverity.Hidden)
                .Select(Mappers.ToCompilerMessage);

            assemblyStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            return (result.Success, messages);
        }
        catch (Exception ex)
        {
            var diagnostic = new LoweringDiagnostic
            {
                Message = $"An unexpected error occurred during compilation: {ex.Message}",
                Severity = MessageSeverity.Error
            };
            return (false, new[] { diagnostic });
        }
    }
}
