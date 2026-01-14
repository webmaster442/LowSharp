using System.Collections.Immutable;

using Lowsharp.Server.Lowering;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Compilers;

internal sealed class CsharpCompiler : RoslynCompilerBase
{
    private readonly CSharpCompilationOptions _compilerOptions;
    private readonly List<ISourceGenerator> _sourceGenerators;

    public CsharpCompiler(IEnumerable<MetadataReference> references,
                          IEnumerable<IIncrementalGenerator> generators,
                          EmitOptions emitOptions)
        : base(references, emitOptions)
    {
        _compilerOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithPlatform(Platform.AnyCpu)
            .WithAllowUnsafe(true)
            .WithNullableContextOptions(NullableContextOptions.Enable)
            .WithUsings("System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Threading", "System.Threading.Tasks")
            .WithConcurrentBuild(true)
            .WithDeterministic(true);

        _sourceGenerators = generators.Select(g => g.AsSourceGenerator()).ToList();
    }

    public override async Task<CompilerOutput> CompileAsync(string code,
                                                            OutputOptimizationLevel outputOptimizationLevel,
                                                            RecyclableMemoryStream assemblyStream,
                                                            RecyclableMemoryStream pdbStream,
                                                            CancellationToken cancellationToken)
    {
        try
        {
            var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);

            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(code, parseOptions);

            CSharpCompilation compilation = CSharpCompilation.Create("inMemory")
                .WithOptions(_compilerOptions.WithOptimizationLevel(outputOptimizationLevel.ToOptimizationLevel()))
                .AddReferences(_references)
                .AddSyntaxTrees(syntaxTree);

            // Run source generators
            CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(_sourceGenerators, parseOptions: parseOptions);

            driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation,
                                                                                     out Compilation? updatedCompilation,
                                                                                     out ImmutableArray<Diagnostic> diagnostics,
                                                                                     cancellationToken);
            compilation = (CSharpCompilation)updatedCompilation;


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
                Message = $"Compilation failed with exception: {ex.Message}",
                Severity = MessageSeverity.Error
            };
            return new(false, [diagnostic]);
        }
    }

    public override Task<string> CompileToSyntaxTreeJsonAsync(string code,
                                                              CancellationToken cancellationToken)
    {
        SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));
        return Task.FromResult(SerializeSyntaxTree(syntaxTree));
    }
}
