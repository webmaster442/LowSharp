using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Decompilers;

internal interface IDecompiler
{
    bool TryDecompile(RecyclableMemoryStream assemblyStream,
                      RecyclableMemoryStream pdbStream,
                      out string result);
}
