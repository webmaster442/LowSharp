using System.Reflection;
using System.Runtime.Loader;

using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Decompilers;

internal abstract class VisualizingDecompilerBase : IDecompiler
{
    internal sealed class Item : IEquatable<Item?>
    {
        public required string Name { get; init; }
        public required bool IsInterface { get; init; }
        public required bool IsAbstract { get; init; }

        public List<string> Methods { get; set; } = new();
        public List<string> Properties { get; set; } = new();
        public List<string> Fields { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals(Item? other)
        {
            return other is not null &&
                   Name == other.Name &&
                   IsInterface == other.IsInterface &&
                   IsAbstract == other.IsAbstract;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, IsInterface, IsAbstract);
        }
    }

    internal sealed record class Relation(Item Base, Item Derived, bool AreInSameNamespace);

    protected abstract RendererBase CreateRenderer();

    public bool TryDecompile(RecyclableMemoryStream assemblyStream,
                             RecyclableMemoryStream pdbStream,
                             out string result)
    {
        try
        {
            using (var customLoadContext = new DisposableAssemblyLoadContext())
            {
                Assembly assembly = customLoadContext.LoadFromStream(assemblyStream);

                var renderer = CreateRenderer();

                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    renderer.AddType(type);

                    Type[] implementations = type.GetInterfaces();
                    Type? baseClass = type.BaseType;
                    WalkBaseClass(type, baseClass, renderer);
                    foreach (var implementation in implementations)
                    {
                        renderer.AddRelation(implementation, type);
                    }
                }
                result = renderer.Render();
            }
            return true;
        }
        catch (Exception ex)
        {
            result = ex.Message;
            return false;
        }
    }

    private static void WalkBaseClass(Type type, Type? baseClass, RendererBase renderer)
    {
        if (baseClass is null || baseClass.IsValueType || baseClass == typeof(object))
            return;

        renderer.AddRelation(baseClass, type);

        WalkBaseClass(baseClass, baseClass.BaseType, renderer);
    }

}
