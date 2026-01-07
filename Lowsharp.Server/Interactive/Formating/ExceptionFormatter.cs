namespace Lowsharp.Server.Interactive.Formating;

internal sealed class ExceptionFormatter : ObjectFormatter<Exception>
{
    protected override IEnumerable<TextWithFormat> Format(Exception obj, IObjectFormatter parent)
    {
        yield return FormatType();
        yield return Environment.NewLine;
        yield return new TextWithFormat
        {
            Text = obj.Message,
            Color = ForegroundColor.Red
        };
        yield return Environment.NewLine;
    }
}
