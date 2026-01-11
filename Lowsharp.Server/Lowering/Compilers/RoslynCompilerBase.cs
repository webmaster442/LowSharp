using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using Lowsharp.Server.Lowering.Syntax;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Compilers;

internal abstract class RoslynCompilerBase : ICompiler
{
    protected readonly IEnumerable<PortableExecutableReference> _references;
    protected readonly EmitOptions _emitOptions;

    protected RoslynCompilerBase(IEnumerable<PortableExecutableReference> references,
                                 EmitOptions emitOptions)
    {
        _references = references;
        _emitOptions = emitOptions;
    }

    public abstract Task<CompilerOutput> CompileAsync(string code,
                                                      OutputOptimizationLevel outputOptimizationLevel,
                                                      RecyclableMemoryStream assemblyStream,
                                                      RecyclableMemoryStream pdbStream,
                                                      CancellationToken cancellationToken);

    public abstract Task<string> CompileToSyntaxTreeJsonAsync(string code,
                                                              CancellationToken cancellationToken);

    protected string SerializeSyntaxTree(SyntaxTree syntax)
    {
        NodeOrTokenDto dto = RoslynSyntaxNodeFactory.Create(syntax);
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(dto, options);
    }
}
