namespace Lowsharp.Server.Interactive.Formating;

internal sealed class TextWithFormatFactory : IObjectFormatter
{
    private readonly IObjectFormatter[] _formatters;

    public TextWithFormatFactory()
    {
        _formatters =
        [
            new ExceptionFormatter(),
            new IFormattableFormatter(),
        ];
    }

    public IEnumerable<TextWithFormat> Format(object obj)
    {
        foreach (var formatter in _formatters)
        {
            if (formatter.CanFormat(obj))
            {
                return formatter.Format(obj, this);
            }
        }
        return
        [
            new TextWithFormat
            {
                Text = obj?.ToString() ?? "null"
            }
        ];
    }

    bool IObjectFormatter.CanFormat(object obj)
        => true;

    IEnumerable<TextWithFormat> IObjectFormatter.Format(object obj, IObjectFormatter parent)
        => Format(obj);
}
