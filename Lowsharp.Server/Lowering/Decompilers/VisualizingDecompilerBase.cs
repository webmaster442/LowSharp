using System.Reflection;
using System.Runtime.Loader;

using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Decompilers;

internal abstract class VisualizingDecompilerBase : IDecompiler
{
    internal sealed record class Item(string Name, bool IsInterface, bool IsAbstract);

    internal sealed record class Relation(Item Base, Item Derived, bool AreInSameNamespace);


    private class CustomLoadContext : AssemblyLoadContext
    {
        public CustomLoadContext() : base(isCollectible: true)
        {
        }
    }

    protected abstract RendererBase CreateRenderer();

    public bool TryDecompile(RecyclableMemoryStream assemblyStream,
                             RecyclableMemoryStream pdbStream,
                             out string result)
    {
        try
        {
            CustomLoadContext customLoadContext = new CustomLoadContext();

            Assembly assembly = customLoadContext.LoadFromStream(assemblyStream);

            var renderer = CreateRenderer();

            var types = assembly.GetTypes().Where(t => t.IsVisible);
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
