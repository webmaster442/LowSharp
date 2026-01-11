using System.Text;

namespace Lowsharp.Server.Lowering.Decompilers;

internal class NomnomClassTreeDecompiler : VisualizingDecompilerBase
{
    protected override RendererBase CreateRenderer()
        => new NomnomRenderer();

    internal sealed class NomnomRenderer : RendererBase
    {
        private static string Escape(string str)
        {
            if (str.Contains('['))
            {
                str = str.Replace("[", "\\[");
            }
            if (str.Contains(']'))
            {
                str = str.Replace("]", "\\]");
            }
            return str;
        }

        private static string Symbol(Relation relation)
        {
            if (relation.Base.IsInterface)
            {
                return "<:--";
            }
            return "<:-";
        }

        public override string Render()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var item in Items)
            {
                if (item.IsInterface)
                {
                    buffer.AppendLine($"[<reference> {Escape(item.Name)}]");
                }
                else if (item.IsAbstract)
                {
                    buffer.AppendLine($"[<abstract> {Escape(item.Name)}]");
                }
                else
                {
                    buffer.AppendLine($"[{Escape(item.Name)}]");
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
    }
}
