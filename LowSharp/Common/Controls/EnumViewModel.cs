using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Common.Controls;

internal abstract class EnumViewModel : ObservableObject
{
    public class EnumItem
    {
        public required string Description { get; init; }
        public required object Value { get; init; }
    }

    public ObservableCollection<EnumItem> Items { get; protected set; } = null!;

    public virtual int SelectedIndex
    {
        get => field; 
        set => SetProperty(ref field, value);
    }

    [MemberNotNull(nameof(Items))]
    protected abstract void GetItems();
}
