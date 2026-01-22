using System.Text;

namespace Lowsharp.Server.Lowering.Decompilers;

internal sealed class MermaidClassTreeDecompiler : VisualizingDecompilerBase
{
    protected override RendererBase CreateRenderer()
        => new MermaidRenderer();

    internal sealed class MermaidRenderer : RendererBase
    {
        private static string Escape(string str)
        {
            if (str.Contains('<'))
            {
                str = str.Replace('<', '~');
            }
            if (str.Contains('>'))
            {
                str = str.Replace('>', '~');
            }
            return str;
        }

        private static string Symbol(Relation relation)
        {
            if (relation.Base.IsInterface)
            {
                if (relation.AreInSameNamespace)
                    return "<|..";
                else
                    return "()--";
            }
            return "<|--";
        }

        public override string Render()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine("classDiagram");

            foreach (var item in Items)
            {
                if (item.IsInterface)
                {
                    buffer.Append($"  class {Escape(item.Name)}").Append("{ <<Interface>> }");
                }
                else if (item.IsAbstract)
                {
                    buffer.Append($"  class {Escape(item.Name)}").Append("{ <<Abstract>> }");
                }
                else
                {
                    buffer.Append($"  class {Escape(item.Name)}").Append("{ }");
                }
            }

            foreach (var relation in Relations)
            {
                buffer
                    .Append("  ")
                    .Append(Escape(relation.Base.Name))
                    .Append(' ')
                    .Append(Symbol(relation))
                    .Append(' ')
                    .AppendLine(Escape(relation.Derived.Name));
            }

            return buffer.ToString();
        }
    }
}
