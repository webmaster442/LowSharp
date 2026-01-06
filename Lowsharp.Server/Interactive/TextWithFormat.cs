namespace Lowsharp.Server.Interactive;

public class TextWithFormat
{
    public required string Text { get; init; }
    public ForegroundColor Color { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }

    public TextWithFormat()
    {
        Bold = false;
        Italic = false;
        Color = ForegroundColor.Default;
    }

    public static implicit operator TextWithFormat(string text)
        => new() { Text = text };
}
