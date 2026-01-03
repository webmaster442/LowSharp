using System.Windows.Input;

namespace LowSharp.Client.Common.Views;

internal sealed class MenuCommandViewModel : MenuViewModel
{
    public required ICommand Command { get; set; }
}
