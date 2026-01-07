namespace Lowsharp.Server.Interactive.Formating;

internal sealed class IFormattableFormatter : ObjectFormatter<IFormattable>
{
    protected override IEnumerable<TextWithFormat> Format(IFormattable obj, IObjectFormatter parent)
    {
        yield return FormatType(obj);
        yield return Environment.NewLine;
        yield return new TextWithFormat
        {
            Text = obj.ToString(null, System.Globalization.CultureInfo.InvariantCulture)
        };
        yield return Environment.NewLine;
    }
}
