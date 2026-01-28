using System.Windows.Input;

namespace LowSharp.Common.ViewModels;

internal sealed class MenuCommandViewModel : MenuViewModel
{
    public required ICommand Command { get; set; }
    public object? CommandParameter { get; set; }
}
