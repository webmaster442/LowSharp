using System.Collections;
using System.Diagnostics;

using static Lowsharp.Server.Lowering.Decompilers.VisualizingDecompilerBase;

namespace Lowsharp.Server.Lowering.Decompilers;

internal abstract class RendererBase
{
    protected HashSet<Item> Items { get; }

    protected HashSet<Relation> Relations { get; }

    private static string GetName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        // Remove arity suffix from the type name (e.g., "Action`1" -> "Action")
        var baseName = type.Name;
        var tickIndex = baseName.IndexOf('`');
        if (tickIndex >= 0)
        {
            baseName = baseName.Substring(0, tickIndex);
        }

        var genericArgs = type.GetGenericArguments();
        // For generic type definitions, args are generic parameters (e.g., T, T1).
        // For constructed types, args are actual types and should be formatted recursively.
        var argNames = genericArgs
            .Select(a => a.IsGenericParameter ? a.Name : GetName(a));

        return $"{baseName}<{string.Join(", ", argNames)}>";
    }

    public RendererBase()
    {
        Items = new();
        Relations = new();
    }

    public void AddType(Type type)
    {
        if (type == typeof(IEnumerable))
        {
            Debugger.Break();
        }
        Items.Add(new Item(GetName(type), type.IsInterface, type.IsAbstract));
    }

    public void AddRelation(Type baseType, Type derivedType)
    {
        Item @base = new Item(GetName(baseType), baseType.IsInterface, baseType.IsAbstract);
        Item derived = new Item(GetName(derivedType), derivedType.IsInterface, derivedType.IsAbstract);
        Items.Add(@base);
        Items.Add(derived);
        Relations.Add(new Relation(@base, derived, baseType.Namespace == derivedType.Namespace));
    }

    public abstract string Render();
}
