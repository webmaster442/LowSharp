namespace Lowsharp.Server.Lowering.Syntax;

internal sealed class NodeOrTokenDto
{
    public string Type { get; set; } = "";
    public string Kind { get; set; } = "";
    public TextSpanDto Span { get; set; } = new TextSpanDto();
    public List<NodeOrTokenDto>? Children { get; set; }
    public string? Text { get; set; }
    public string? ValueText { get; set; }
    public List<TriviaDto>? LeadingTrivia { get; set; }
    public List<TriviaDto>? TrailingTrivia { get; set; }
}
