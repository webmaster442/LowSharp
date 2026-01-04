using System.Reflection.Metadata;

using ICSharpCode.Decompiler.DebugInfo;

namespace Lowsharp.Server.Lowering;

internal sealed class PdbDebugInfoProvider : IDebugInfoProvider, IDisposable
{
    private readonly MetadataReaderProvider _readerProvider;
    private readonly MetadataReader _reader;

    public PdbDebugInfoProvider(Stream symbolStream)
    {
        _readerProvider = MetadataReaderProvider.FromPortablePdbStream(symbolStream);
        _reader = _readerProvider.GetMetadataReader();
    }

    public void Dispose()
    {
        _readerProvider.Dispose();
    }

    public string Description => "_";

    public string SourceFileName => "";

    public IList<ICSharpCode.Decompiler.DebugInfo.SequencePoint> GetSequencePoints(MethodDefinitionHandle method)
    {
        var debugInfo = _reader.GetMethodDebugInformation(method);

        var points = debugInfo.GetSequencePoints();
        var result = new List<ICSharpCode.Decompiler.DebugInfo.SequencePoint>();
        foreach (var point in points)
        {
            result.Add(new ICSharpCode.Decompiler.DebugInfo.SequencePoint
            {
                Offset = point.Offset,
                StartLine = point.StartLine,
                StartColumn = point.StartColumn,
                EndLine = point.EndLine,
                EndColumn = point.EndColumn,
                DocumentUrl = "_",
            });
        }
        return result;
    }

    private IEnumerable<LocalVariable> EnumerateLocals(MethodDefinitionHandle method)
    {
        foreach (var scopeHandle in _reader.GetLocalScopes(method))
        {
            var scope = _reader.GetLocalScope(scopeHandle);
            foreach (var variableHandle in scope.GetLocalVariables())
            {
                yield return _reader.GetLocalVariable(variableHandle);
            }
        }
    }

    public IList<Variable> GetVariables(MethodDefinitionHandle method)
    {
        var variables = new List<Variable>();
        foreach (var local in EnumerateLocals(method))
        {
            variables.Add(new Variable(local.Index, _reader.GetString(local.Name)));
        }
        return variables;
    }

    public bool TryGetExtraTypeInfo(MethodDefinitionHandle method, int index, out PdbExtraTypeInfo extraTypeInfo)
    {
        foreach (var local in EnumerateLocals(method))
        {
            if (local.Index == index)
            {
                extraTypeInfo = new PdbExtraTypeInfo
                {
                    TupleElementNames = Array.Empty<string>(),
                    DynamicFlags = Array.Empty<bool>(),
                };
                return true;
            }

        }
        extraTypeInfo = default;
        return false;
    }

    public bool TryGetName(MethodDefinitionHandle method, int index, out string name)
    {
        name = string.Empty;
        foreach (var local in EnumerateLocals(method))
        {
            if (local.Index == index)
            {
                name = _reader.GetString(local.Name);
                return true;
            }
        }
        return false;
    }
}
