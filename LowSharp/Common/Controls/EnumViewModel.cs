using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Common.Controls;

internal abstract class EnumViewModel : ObservableObject
{
    public class EnumItem
    {
        public required string Description { get; init; }
        public required object Value { get; init; }
    }

    public ObservableCollection<EnumItem> Items { get; }

    public virtual int SelectedIndex
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public EnumViewModel()
    {
        Items = new ObservableCollection<EnumItem>(GetItems());
        SelectedIndex = 0;
    }

    protected abstract IEnumerable<EnumItem> GetItems();
}
