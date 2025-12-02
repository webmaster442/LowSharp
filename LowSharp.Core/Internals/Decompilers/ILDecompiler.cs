using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using Microsoft.IO;

namespace LowSharp.Core.Internals.Decompilers;

internal sealed class ILDecompiler : IDecompiler
{
    private static bool IsNonUserCode(MetadataReader metadata, TypeDefinition type)
    {
        return !type.NamespaceDefinition.IsNil && type.IsCompilerGenerated(metadata);
    }

    private void DecompileTypes(PEFile assemblyFile,
                                PlainTextOutput output,
                                ReflectionDisassembler disassembler,
                                MetadataReader metadata)
    {
        const int maxNonUserTypeHandles = 100;
        var nonUserTypeHandlesLease = default(MemoryLease<TypeDefinitionHandle>);
        var nonUserTypeHandlesCount = -1;

        try
        {
            // user code (first)                
            foreach (var typeHandle in metadata.TypeDefinitions)
            {
                var type = metadata.GetTypeDefinition(typeHandle);
                if (!type.GetDeclaringType().IsNil)
                    continue; // not a top-level type

                if (IsNonUserCode(metadata, type) && nonUserTypeHandlesCount < maxNonUserTypeHandles)
                {
                    if (nonUserTypeHandlesCount == -1)
                    {
                        nonUserTypeHandlesLease = MemoryPoolSlim<TypeDefinitionHandle>.Shared.RentExact(25);
                        nonUserTypeHandlesCount = 0;
                    }

                    nonUserTypeHandlesLease.AsSpan()[nonUserTypeHandlesCount] = typeHandle;
                    nonUserTypeHandlesCount += 1;
                    continue;
                }

                disassembler.DisassembleType(assemblyFile, typeHandle);
                output.WriteLine();
            }

            // non-user code (second)
            if (nonUserTypeHandlesCount > 0)
            {
                foreach (var typeHandle in nonUserTypeHandlesLease.AsSpan().Slice(0, nonUserTypeHandlesCount))
                {
                    disassembler.DisassembleType(assemblyFile, typeHandle);
                    output.WriteLine();
                }
            }
        }
        finally
        {
            nonUserTypeHandlesLease.Dispose();
        }
    }


    public bool TryDecompile(RecyclableMemoryStream assemblyStream, RecyclableMemoryStream pdbStream, out string result)
    {
        try
        {
            using var assemblyFile = new PEFile("", assemblyStream);
            using var pdbFile = new PdbDebugInfoProvider(pdbStream);
            using var codeWriter = new StringWriter();

            var output = new PlainTextOutput(codeWriter)
            {
                IndentationString = new string(' ', 4),
            };

            var disassembler = new ReflectionDisassembler(output, CancellationToken.None)
            {
                DebugInfo = pdbFile,
                ShowSequencePoints = true
            };

            disassembler.WriteAssemblyHeader(assemblyFile);

            output.WriteLine();

            var metadata = assemblyFile.Metadata;
            DecompileTypes(assemblyFile, output, disassembler, metadata);

            result = codeWriter.ToString();
            return true;

        }
        catch (Exception ex)
        {
            result = ex.Message;
            return false;
        }
    }
}