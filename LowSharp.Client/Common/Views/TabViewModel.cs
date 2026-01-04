using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Client.Common.Views;

[DebuggerDisplay("{TabTitle}")]
internal partial class TabViewModel : ObservableObject
{
    [ObservableProperty]
    public required partial string TabTitle { get; set; }

    [ObservableProperty]
    public required partial ViewModelWithMenus Content { get; set; }
}
