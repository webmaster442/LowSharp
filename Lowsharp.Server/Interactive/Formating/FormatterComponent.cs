namespace Lowsharp.Server.Interactive.Formating;

internal sealed class FormatterComponent : IObjectFormatter
{
    private readonly IObjectFormatter[] _formatters;

    public FormatterComponent()
    {
        _formatters = new IObjectFormatter[]
        {
            new IFormattableFormatter(),
        };
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
