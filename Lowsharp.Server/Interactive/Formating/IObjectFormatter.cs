namespace Lowsharp.Server.Interactive.Formating;

internal interface IObjectFormatter
{
    public bool CanFormat(object obj);
    public IEnumerable<TextWithFormat> Format(object obj, IObjectFormatter parent);
}
