using Microsoft.IO;

namespace LowSharp.Core.Internals;

internal interface IDecompiler
{
    bool TryDecompile(RecyclableMemoryStream assemblyStream,
                      RecyclableMemoryStream pdbStream,
                      out string result);
}