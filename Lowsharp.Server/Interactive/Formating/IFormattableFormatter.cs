namespace Lowsharp.Server.Interactive.Formating;

internal class IFormattableFormatter : ObjectFormatter<IFormattable>
{
    protected override IEnumerable<TextWithFormat> Format(IFormattable obj, IObjectFormatter parent)
    {
        yield return new TextWithFormat
        {
            Text = obj.GetType().FullName ?? "unknown type",
            Italic = true,
            Color = ForegroundColor.Yellow
        };
        yield return Environment.NewLine;
        yield return new TextWithFormat
        {
            Text = obj.ToString(null, System.Globalization.CultureInfo.InvariantCulture)
        };
        yield return Environment.NewLine;
    }
}

