using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace LowSharp.Core.Internals.Compilers;

internal abstract class RoslynCompilerBase
{
    protected readonly IEnumerable<PortableExecutableReference> _references;
    protected readonly EmitOptions _emitOptions;

    protected RoslynCompilerBase(IEnumerable<PortableExecutableReference> references,
                                 EmitOptions emitOptions)
    {
        _references = references;
        _emitOptions = emitOptions;
    }
}