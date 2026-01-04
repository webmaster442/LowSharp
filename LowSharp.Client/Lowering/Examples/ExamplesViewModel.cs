using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Lowering.Examples;

internal sealed partial class ExamplesViewModel : ViewModelWithMenus
{
    private readonly List<Example> _exampleList;

    public ExamplesViewModel()
    {
        using var exampleReader = new ExampleReader();
        _exampleList = new List<Example>();
        exampleReader.ReadExamples(example =>
        {
            _exampleList.Add(example);
        });
    }

    [RelayCommand]
    public void LoadExample(Example example)
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetInputCode(example.Content));
        WeakReferenceMessenger.Default.Send(new Messages.SetInputLanguage(example.Language));
    }

    internal MenuViewModel GenerateMenu()
    {
        var exampleMenu = new MenuViewModel { Header = "Examples" };
        var csharpMenuItem = new MenuViewModel { Header = "C#" };
        var fSharpMenuItem = new MenuViewModel { Header = "F#" };

        foreach (var example in _exampleList)
        {
            var menuItem = new MenuCommandViewModel
            {
                Header = example.Name,
                Command = LoadExampleCommand,
                CommandParameter = example
            };

            if (string.Equals(example.Language, "csharp", StringComparison.OrdinalIgnoreCase))
            {
                csharpMenuItem.Children.Add(menuItem);

            }
            else if (string.Equals(example.Language, "fsharp", StringComparison.OrdinalIgnoreCase))
            {
                fSharpMenuItem.Children.Add(menuItem);
            }
        }

        exampleMenu.Children.Add(csharpMenuItem);
        exampleMenu.Children.Add(fSharpMenuItem);
        return exampleMenu;
    }
}
