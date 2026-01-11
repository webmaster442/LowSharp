using CommunityToolkit.Mvvm.Input;

using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Lowering.Examples;

internal sealed partial class ExamplesViewModel : ViewModelWithMenus
{
    private readonly List<Example> _exampleList;
    private readonly LoweringViewModel _loweringViewModel;

    public ExamplesViewModel(LoweringViewModel loweringViewModel)
    {
        using var exampleReader = new ExampleReader();
        _exampleList = new List<Example>();
        exampleReader.ReadExamples(example =>
        {
            _exampleList.Add(example);
        });
        _loweringViewModel = loweringViewModel;
    }

    [RelayCommand]
    public void LoadExample(Example example)
    {
        _loweringViewModel.InputCode = example.Content;
        _loweringViewModel.SelectedInputLanguageIndex = GetLanguageIndex(example.Language);
    }

    private int GetLanguageIndex(string language)
    {
        if (language.Contains("cs", StringComparison.OrdinalIgnoreCase))
        {
            return _loweringViewModel.InputLanguages.IndexOf(InputLanguage.Csharp);
        }
        else if (language.Contains("v", StringComparison.OrdinalIgnoreCase))
        {
            return _loweringViewModel.InputLanguages.IndexOf(InputLanguage.Visualbasic);
        }
        else if (language.Contains("fs", StringComparison.OrdinalIgnoreCase))
        {
            return _loweringViewModel.InputLanguages.IndexOf(InputLanguage.Fsharp);
        }
        throw new ArgumentException($"Unknown language: {language}", nameof(language));
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
