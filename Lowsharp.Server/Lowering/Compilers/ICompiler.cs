using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Compilers;

internal interface ICompiler
{
    Task<CompilerOutput> CompileAsync(string code,
                                      OutputOptimizationLevel outputOptimizationLevel,
                                      RecyclableMemoryStream assemblyStream,
                                      RecyclableMemoryStream pdbStream,
                                      CancellationToken cancellationToken);
    Task<string> CompileToSyntaxTreeJsonAsync(string code,
                                              CancellationToken cancellationToken);
}
