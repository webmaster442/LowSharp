namespace LowSharp.Common.Controls;

internal class EnumViewModel<T> : EnumViewModel where T : struct, Enum
{
    private readonly Func<T, string?> _descriptionProvider;

    public EnumViewModel(Func<T, string?> descriptionProvider, T selectedValue)
    {
        _descriptionProvider = descriptionProvider;
        SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(T)), selectedValue);
    }

    public override int SelectedIndex
    {
        get => base.SelectedIndex;
        set
        {
            base.SelectedIndex = value;
            OnPropertyChanged(nameof(SelectedValue));
        }
    }

    public T SelectedValue
    {
        get => (T)Items[SelectedIndex].Value;
    }

    protected override IEnumerable<EnumItem> GetItems()
    {
        return Enum.GetValues<T>().Select(value => new EnumItem
        {
            Value = value,
            Description = _descriptionProvider(value) ?? value.ToString()!,
        });
    }
}
