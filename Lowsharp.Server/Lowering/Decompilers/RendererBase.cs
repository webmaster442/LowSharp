using System.Collections;
using System.Diagnostics;
using System.Reflection;

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
        Items.Add(new Item
        {
            Name = GetName(type),
            IsInterface = type.IsInterface,
            IsAbstract = type.IsAbstract,
            Methods = GetMethods(type),
            Fields = GetFields(type),
            Properties = GetProperties(type)

        });
    }

    public void AddRelation(Type baseType, Type derivedType)
    {
        Item @base = new Item
        {
            Name = GetName(baseType),
            IsInterface = baseType.IsInterface,
            IsAbstract = baseType.IsAbstract,
            Methods = GetMethods(baseType),
            Fields = GetFields(baseType),
            Properties = GetProperties(baseType)
        };
        Item derived = new Item
        {
            Name = GetName(derivedType),
            IsInterface = derivedType.IsInterface,
            IsAbstract = derivedType.IsAbstract,
            Methods = GetMethods(derivedType),
            Fields = GetFields(derivedType),
            Properties = GetProperties(derivedType)
        };
        Items.Add(@base);
        Items.Add(derived);
        Relations.Add(new Relation(@base, derived, baseType.Namespace == derivedType.Namespace));
    }

    private static List<string> GetProperties(Type baseType)
    {
        List<string> properties = new();
        foreach (var property in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            string readAccessor = property.CanRead ? "get;" : "";
            string writeAccessor = property.CanWrite ? "set;" : "";
            properties.Add($"{GetName(property.PropertyType)} {property.Name} {{ {readAccessor} {writeAccessor} }}");
        }
        return properties;
    }

    private static List<string> GetFields(Type baseType)
    {
        var fields = new List<string>();
        foreach (var field in baseType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            fields.Add($"{GetName(field.FieldType)} {field.Name}");
        }
        return fields;
    }

    private static List<string> GetMethods(Type derivedType)
    {
        var medhods = new List<string>();
        foreach (var method in derivedType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            if (method.IsSpecialName)
            {
                continue;
            }
            var parameters = method.GetParameters();
            string parameterList = string.Join(", ", parameters.Select(p => $"{GetName(p.ParameterType)} {p.Name}"));
            medhods.Add($"{GetName(method.ReturnType)} {method.Name}({parameterList})");
        }
        return medhods;
    }

    public abstract string Render();
}
