using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LowSharp.Core.Internals;

internal sealed class ReferenceProvider
{
    public IReadOnlyList<PortableExecutableReference> References { get; }

    public ReferenceProvider()
    {
        if (!(AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trusted))
            throw new InvalidOperationException("Can't locate Trusted platform assemblies");

        string[]? trustedAssembliesPaths = trusted.Split(Path.PathSeparator);

        References = trustedAssembliesPaths
                .Distinct()
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();
    }
}
