using Microsoft.IO;

namespace LowSharp.Core.Internals.Decompilers;

internal interface IDecompiler
{
    bool TryDecompile(RecyclableMemoryStream assemblyStream,
                      RecyclableMemoryStream pdbStream,
                      out string result);
}
