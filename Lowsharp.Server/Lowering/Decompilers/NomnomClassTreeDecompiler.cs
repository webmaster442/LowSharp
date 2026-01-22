using System.Text;

namespace Lowsharp.Server.Lowering.Decompilers;

internal sealed class NomnomClassTreeDecompiler : VisualizingDecompilerBase
{
    protected override RendererBase CreateRenderer()
        => new NomnomRenderer();

    internal sealed class NomnomRenderer : RendererBase
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
                    case '[':
                    case ']':
                    case ';':
                        builder.Append('\\');
                        builder.Append(c);
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
                ? "<:--" 
                : "<:-";
        }

        public override string Render()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (Item item in Items)
            {
                if (item.IsInterface)
                {
                    buffer.AppendLine($"[<reference> {Escape(item.Name)}|");
                    RenderItemContents(buffer, item);
                    buffer.AppendLine("]");
                }
                else if (item.IsAbstract)
                {
                    buffer.AppendLine($"[<abstract> {Escape(item.Name)}|");
                    RenderItemContents(buffer, item);
                    buffer.AppendLine("]");
                }
                else
                {
                    buffer.AppendLine($"[{Escape(item.Name)}|");
                    RenderItemContents(buffer, item);
                    buffer.AppendLine("]");
                }
            }

            buffer.AppendLine();

            foreach (var relation in Relations)
            {
                if (!Items.Contains(relation.Base))
                {
                    if (relation.Base.IsInterface)
                    {
                        buffer.AppendLine($"[<lollipop> {Escape(relation.Base.Name)}]");
                    }
                    else if (relation.Base.IsAbstract)
                    {
                        buffer.AppendLine($"[<abstract> {Escape(relation.Base.Name)}]");
                    }
                    else
                    {
                        buffer.AppendLine($"[{Escape(relation.Base.Name)}]");
                    }
                }

                buffer
                    .Append($"[{Escape(relation.Base.Name)}]")
                    .Append(' ')
                    .Append(Symbol(relation))
                    .Append(' ')
                    .AppendLine($"[{Escape(relation.Derived.Name)}]");
            }

            return buffer.ToString();
        }

        private static void RenderItemContents(StringBuilder buffer, Item item)
        {
            foreach (var field in item.Fields)
            {
                buffer.AppendLine($"  + {Escape(field)}");
            }

            if (item.Fields.Count > 0)
                buffer.AppendLine("|");

            foreach (var property in item.Properties)
            {
                buffer.AppendLine($"  + {Escape(property)}");
            }

            if (item.Properties.Count > 0)
                buffer.AppendLine("|");

            foreach (var method in item.Methods)
            {
                buffer.AppendLine($"  + {Escape(method)}");
            }
        }
    }
}
