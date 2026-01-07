namespace Lowsharp.Server.Interactive.Formating;

internal abstract class ObjectFormatter<T> : IObjectFormatter
{
    public bool CanFormat(object obj)
        => obj is T;

    public IEnumerable<TextWithFormat> Format(object obj, IObjectFormatter parent)
    {
        return obj is T tObj
            ? Format(tObj, parent)
            : throw new ArgumentException($"Object is not of type {typeof(T).FullName}", nameof(obj));
    }

    protected static TextWithFormat FormatType(object o)
    {
        return new TextWithFormat
        {
            Text = o.GetType().Name,
            Color = ForegroundColor.Yellow,
            Italic = true
        };
    }

    protected abstract IEnumerable<TextWithFormat> Format(T obj, IObjectFormatter parent);
}
