namespace Lowsharp.Server.Interactive.Formating;

internal sealed class ObjectPropertiesFormatter : IObjectFormatter
{
    private static bool HasCustomToString(object obj)
    {
        var toStringMethod = obj.GetType().GetMethod(
            "ToString",
            Type.EmptyTypes
        );

        return toStringMethod != null &&
               toStringMethod.DeclaringType != typeof(object);
    }

    public bool CanFormat(object obj)
        => obj != null && !HasCustomToString(obj);

    IEnumerable<TextWithFormat> IObjectFormatter.Format(object obj, IObjectFormatter parent)
    {
        yield return new TextWithFormat
        {
            Text = obj.GetType().Name,
            Color = ForegroundColor.Yellow,
            Italic = true,
        };
        yield return " {";
        yield return Environment.NewLine;
        var properties = obj.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (!property.CanRead)
                continue;

            yield return "   ";
            yield return new TextWithFormat
            {
                Text = property.Name,
                Color = ForegroundColor.Green,
                Bold = true
            };
            yield return ": ";
            var formattedValue = parent.Format(property.GetValue(obj) ?? "null", parent);
            foreach (var item in formattedValue)
            {
                if (item.Text == Environment.NewLine)
                    yield return " ";
                else
                    yield return item;
            }
            yield return Environment.NewLine;
        }
        yield return "}";
        yield return Environment.NewLine;
    }
}
