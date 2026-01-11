using System.Threading;

using Lowsharp.Server.Lowering;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Compilers;

internal sealed class VisualBasicCompiler : RoslynCompilerBase
{
    private readonly VisualBasicCompilationOptions _compilerOptions;

    public VisualBasicCompiler(IEnumerable<PortableExecutableReference> references, EmitOptions emitOptions)
        : base(references, emitOptions)
    {
        _compilerOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithOptionStrict(OptionStrict.On)
            .WithOptionInfer(true)
            .WithPlatform(Platform.AnyCpu);
    }

    public override async Task<CompilerOutput> CompileAsync(string code,
                                                            OutputOptimizationLevel outputOptimizationLevel,
                                                            RecyclableMemoryStream assemblyStream,
                                                            RecyclableMemoryStream pdbStream,
                                                            CancellationToken cancellationToken)
    {
        try
        {
            var syntaxTree = VisualBasicSyntaxTree.ParseText(code, VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

            VisualBasicCompilation compilation = VisualBasicCompilation.Create("inMemory")
                .WithOptions(_compilerOptions.WithOptimizationLevel(outputOptimizationLevel.ToOptimizationLevel()))
                .AddReferences(_references)
                .AddSyntaxTrees(syntaxTree);

            EmitResult result = compilation.Emit(assemblyStream, pdbStream, options: _emitOptions, cancellationToken: cancellationToken);

            var messages = result.Diagnostics
                .Where(d => d.Severity != DiagnosticSeverity.Hidden)
                .Select(Mappers.ToLoweringDiagnostic)
                .ToArray();

            assemblyStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            return new(result.Success, messages);
        }
        catch (Exception ex)
        {
            var diagnostic = new LoweringDiagnostic
            {
                Message = $"An unexpected error occurred during compilation: {ex.Message}",
                Severity = MessageSeverity.Error
            };
            return new(false, [diagnostic]);
        }
    }

    public override Task<string> CompileToSyntaxTreeJsonAsync(string code, CancellationToken cancellationToken)
    {
        var syntaxTree = VisualBasicSyntaxTree.ParseText(code, VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
        return Task.FromResult(SerializeSyntaxTree(syntaxTree));
    }
}
