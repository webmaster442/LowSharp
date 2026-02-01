using System.Collections.ObjectModel;

namespace LowSharp.Common.Controls;

internal class EnumViewModel<T> : EnumViewModel where T : struct, Enum
{
    private readonly Func<T, string?> _descriptionProvider;

    public EnumViewModel(Func<T, string?> descriptionProvider, T selectedValue)
    {
        _descriptionProvider = descriptionProvider;
        GetItems();
        SelectValue(selectedValue);
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

    protected override void GetItems()
    {
        var collection = Enum.GetValues<T>()
            .Where(v => !v.ToString().Contains("unspecified", StringComparison.OrdinalIgnoreCase))
            .Select(value => new EnumItem
        {
            Value = value,
            Description = _descriptionProvider(value) ?? value.ToString()!,
        });
        Items = new ObservableCollection<EnumItem>(collection);
    }

    public void SelectValue(T value)
    {
        foreach (var (index, item) in Items.Select((item, index) => (index, item)))
        {
            if (EqualityComparer<T>.Default.Equals((T)item.Value, value))
            {
                SelectedIndex = index;
                return;
            }
        }
    }

    internal void SelectValueByStringName(string language, bool ignoreCase = true)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (string.Equals(item.Value.ToString(), language, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                SelectedIndex = i;
                return;
            }
        }
    }
}
