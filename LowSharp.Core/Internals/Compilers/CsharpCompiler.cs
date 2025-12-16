using LowSharp.Core.Internals;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace LowSharp.Core.Internals.Compilers;

internal sealed class CsharpCompiler
{
    private readonly CSharpCompilationOptions _compilerOptions;
    private readonly IEnumerable<PortableExecutableReference> _references;
    private readonly EmitOptions _emitOptions;

    public CsharpCompiler(IEnumerable<PortableExecutableReference> references, EmitOptions emitOptions)
    {
        _emitOptions = emitOptions;
        _references = references;
        _compilerOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithPlatform(Platform.AnyCpu)
            .WithAllowUnsafe(true)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithUsings("System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Threading", "System.Threading.Tasks")
            .WithConcurrentBuild(true)
            .WithDeterministic(true);
    }

    public (bool result, IEnumerable<LoweringDiagnostic> diagnostics) CompileCsharp(string code,
                                                                                    CsharpLanguageVersion languageVersion,
                                                                                    OutputOptimizationLevel outputOptimizationLevel,
                                                                                    RecyclableMemoryStream assemblyStream,
                                                                                    RecyclableMemoryStream pdbStream,
                                                                                    CancellationToken cancellationToken)
    {
        try
        {
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(code, CSharpParseOptions.Default.WithLanguageVersion(languageVersion.ToLanguageVersion()));

            CSharpCompilation compilation = CSharpCompilation.Create("inMemory")
                .WithOptions(_compilerOptions.WithOptimizationLevel(outputOptimizationLevel.ToOptimizationLevel()))
                .AddReferences(_references)
                .AddSyntaxTrees(syntaxTree);

            EmitResult result = compilation.Emit(assemblyStream, pdbStream, options: _emitOptions, cancellationToken: cancellationToken);

            var messages = result.Diagnostics
                .Where(d => d.Severity != DiagnosticSeverity.Hidden)
                .Select(Mappers.ToLoweringDiagnostic);

            assemblyStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            return (result.Success, messages);
        }
        catch (Exception ex)
        {
            var diagnostic = new LoweringDiagnostic
            {
                Message = $"Compilation failed with exception: {ex.Message}",
                Severity = MessageSeverity.Error
            };
            return (false, new[] { diagnostic });
        }
    }
}