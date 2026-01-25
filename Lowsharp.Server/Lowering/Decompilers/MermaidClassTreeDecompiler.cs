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
            var builder = new StringBuilder(str.Length + 10);
            foreach (char c in str)
            {
                switch (c)
                {
                    case '<':
                    case '>':
                        builder.Append('~');
                        break;
                    case '{':
                        builder.Append('(');
                        break;
                    case '}':
                        builder.Append(')');
                        break;
                    default:
                        builder.Append(c);
                        break;
                }

            }
            return builder.ToString();
        }

        private static string Symbol(Relation relation)
        {
            return relation.Base.IsInterface
                ? "<|.."
                : "<|--";
        }

        public override string Render()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine("classDiagram");
            foreach (Item item in Items)
            {

                buffer.AppendLine($"class {Escape(item.Name)} {{");
                RenderItemContents(buffer, item);
                buffer.AppendLine("}");
            }

            buffer.AppendLine();

            foreach (var relation in Relations)
            {
                buffer
                    .Append(Escape(relation.Base.Name))
                    .Append(' ')
                    .Append(Symbol(relation))
                    .Append(' ')
                    .AppendLine(Escape(relation.Derived.Name));
            }

            return buffer.ToString();
        }

        private static void RenderItemContents(StringBuilder buffer, Item item)
        {
            foreach (var field in item.Fields)
            {
                buffer.AppendLine($"  + {Escape(field)}");
            }

            foreach (var property in item.Properties)
            {
                buffer.AppendLine($"  + {Escape(property)}");
            }

            foreach (var method in item.Methods)
            {
                buffer.AppendLine($"  + {Escape(method)}");
            }
        }
    }
}
