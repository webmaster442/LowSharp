using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace LowSharp.Core.Internals;

internal abstract class CompilerBase
{
    protected readonly IEnumerable<PortableExecutableReference> _references;
    protected readonly EmitOptions _emitOptions;

    protected CompilerBase(IEnumerable<PortableExecutableReference> references,
                           EmitOptions emitOptions)
    {
        _references = references;
        _emitOptions = emitOptions;
    }
}
