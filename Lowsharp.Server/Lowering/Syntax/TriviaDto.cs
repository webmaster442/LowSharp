namespace Lowsharp.Server.Lowering.Syntax;

internal sealed class TriviaDto
{
    public string Kind { get; set; } = "";
    public TextSpanDto Span { get; set; } = new TextSpanDto();
    public string Text { get; set; } = "";
}
