using System.Windows.Input;

namespace LowSharp.Client.Comon.Views;

internal sealed class MenuCommandViewModel : MenuViewModel
{
    public required ICommand Command { get; set; }
}
