using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Lowsharp.Server.Lowering.Syntax;

internal static class SyntaxNodeFactory
{
    public static NodeOrTokenDto ConvertCsharp(SyntaxTree tree,
                                               bool includeTrivia = true,
                                               bool includeTokenText = true)
    {
        if (tree == null)
        {
            throw new ArgumentNullException(nameof(tree));
        }

        SyntaxNode root = tree.GetRoot();
        NodeOrTokenDto dto = ConvertNode(root, includeTrivia, includeTokenText);

        return dto;
    }

    private static NodeOrTokenDto ConvertNode(SyntaxNode node, bool includeTrivia, bool includeTokenText)
    {
        NodeOrTokenDto dto = new NodeOrTokenDto
        {
            Type = "Node",
            Kind = node.Kind().ToString(),
            Span = new TextSpanDto
            {
                Start = node.SpanStart,
                Length = node.Span.Length,
                End = node.Span.End
            },
            Children = new List<NodeOrTokenDto>()
        };

        foreach (SyntaxNodeOrToken child in node.ChildNodesAndTokens())
        {
            if (child.IsNode)
            {
                dto.Children!.Add(ConvertNode(child.AsNode()!, includeTrivia, includeTokenText));
            }
            else
            {
                dto.Children!.Add(ConvertToken(child.AsToken(), includeTrivia, includeTokenText));
            }
        }

        return dto;
    }

    private static NodeOrTokenDto ConvertToken(SyntaxToken token, bool includeTrivia, bool includeTokenText)
    {
        NodeOrTokenDto dto = new NodeOrTokenDto
        {
            Type = "Token",
            Kind = token.Kind().ToString(),
            Span = new TextSpanDto
            {
                Start = token.SpanStart,
                Length = token.Span.Length,
                End = token.Span.End
            },
            Text = includeTokenText ? token.Text : null,
            ValueText = includeTokenText ? token.ValueText : null
        };

        if (includeTrivia)
        {
            dto.LeadingTrivia = ConvertTriviaList(token.LeadingTrivia);
            dto.TrailingTrivia = ConvertTriviaList(token.TrailingTrivia);
        }

        return dto;
    }

    private static List<TriviaDto> ConvertTriviaList(SyntaxTriviaList triviaList)
    {
        List<TriviaDto> list = new List<TriviaDto>(triviaList.Count);
        foreach (SyntaxTrivia trivia in triviaList)
        {
            list.Add(new TriviaDto
            {
                Kind = trivia.Kind().ToString(),
                Span = new TextSpanDto
                {
                    Start = trivia.SpanStart,
                    Length = trivia.Span.Length,
                    End = trivia.Span.End
                },
                Text = trivia.ToString()
            });
        }

        return list;
    }
}
