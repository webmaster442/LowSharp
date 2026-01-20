using System.Collections.ObjectModel;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Google.Protobuf.Collections;

using LowSharp.ApiV1.Examples;
using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;
using LowSharp.ClientLib;

namespace LowSharp.Client.Lowering;

internal sealed partial class LoweringViewModel :
    ViewModelWithMenus,
    IRecipient<Messages.IsBusyChanged>
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    private List<Example> _exampleList = new();

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public MenuCheckableViewModel ShowLineNumbers { get; }

    public MenuCheckableViewModel WordWrap { get; }

    public ObservableCollection<InputLanguage> InputLanguages { get; }

    [ObservableProperty]
    public partial int SelectedInputLanguageIndex { get; set; }

    public ObservableCollection<OutputCodeType> OutputTypes { get; }

    [ObservableProperty]
    public partial int SelectedOutputTypeIndex { get; set; }

    public ObservableCollection<Optimization> Optimizations { get; }

    [ObservableProperty]
    public partial int SelectedOptimizationIndex { get; set; }

    [ObservableProperty]
    public partial string InputCode { get; set; }

    [ObservableProperty]
    public partial string OutputCode { get; set; }

    public LoweringViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
        IsBusy = client.IsBusy;
        InputCode = string.Empty;
        OutputCode = string.Empty;
        WeakReferenceMessenger.Default.Register<Messages.IsBusyChanged>(this);

        ShowLineNumbers = new MenuCheckableViewModel
        {
            Header = "Show Line Numbers",
            IsChecked = true
        };

        WordWrap = new MenuCheckableViewModel
        {
            Header = "Word Wrap",
            IsChecked = true
        };

        Menus.Add(new MenuViewModel
        {
            Header = "View",
            Children =
            {
                ShowLineNumbers,
                WordWrap
            }
        });

        InputLanguages = Fill<InputLanguage>();
        OutputTypes = Fill<OutputCodeType>();
        Optimizations = Fill<Optimization>();
        SelectedInputLanguageIndex = 0;
        SelectedOutputTypeIndex = OutputTypes.IndexOf(OutputCodeType.Loweredcsharp);
        SelectedOptimizationIndex = 0;
    }

    public override async Task InitializeAsync()
    {
        Either<List<Example>, Exception> examples = await _client.Examples.GetExamplesAsync();
        if (examples.TryGetFailure(out Exception? failure))
        {
            await _dialogs.ClientError(failure);
            return;
        }

        if (examples.TryGetSuccess(out List<Example>? result))
        {
            _exampleList  = result;

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

            Menus.Add(exampleMenu);
        }

    }

    private int GetLanguageIndex(string language)
    {
        if (language.Contains("cs", StringComparison.OrdinalIgnoreCase))
        {
            return InputLanguages.IndexOf(InputLanguage.Csharp);
        }
        else if (language.Contains("v", StringComparison.OrdinalIgnoreCase))
        {
            return InputLanguages.IndexOf(InputLanguage.Visualbasic);
        }
        else if (language.Contains("fs", StringComparison.OrdinalIgnoreCase))
        {
            return InputLanguages.IndexOf(InputLanguage.Fsharp);
        }
        throw new ArgumentException($"Unknown language: {language}", nameof(language));
    }

    [RelayCommand]
    public void LoadExample(Example example)
    {
       InputCode = example.Code;
       SelectedInputLanguageIndex = GetLanguageIndex(example.Language);
    }


    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpenCode(out var result))
        {
            var code = System.IO.File.ReadAllText(result.filename);
            InputCode = code;
            SelectedInputLanguageIndex = InputLanguages.IndexOf(result.language);
        }
    }


    [RelayCommand]
    public async Task Lower()
    {
        Either<LoweringResponse, Exception> response = await _client.Lowering.LowerCodeAsync(
            InputCode,
            InputLanguages[SelectedInputLanguageIndex],
            Optimizations[SelectedOptimizationIndex],
            OutputTypes[SelectedOutputTypeIndex]);

        await response.MapAsync(success =>
        {
            if (success.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                OutputCode = GetDiagnostics(success.Diagnostics);
                return Task.CompletedTask;
            }
            OutputCode = success.ResultCode;
            return Task.CompletedTask;
        },
        async failure =>
        {
            await _dialogs.ClientError(failure);
        });
    }

    [RelayCommand]
    public async Task Preview()
    {
        Either<string, Exception> response = await _client.Lowering.RenderVisualizationAsync(OutputCode, VisualType.Nomnoml);

        await response.MapAsync(async succes =>
        {
            _dialogs.OpenWebView("Nomnoml Previrew", succes);
        },
        async failure =>
        {
            await _dialogs.ClientError(failure);
        });
    }

    private static string GetDiagnostics(RepeatedField<Diagnostic> diagnostics)
    {
        var builder = new StringBuilder();
        foreach (var diagnostic in diagnostics)
        {
            builder
                .AppendLine($"{diagnostic.Severity}:")
                .AppendLine(diagnostic.Message)
                .AppendLine();
        }

        return builder.ToString();
    }

    private static ObservableCollection<T> Fill<T>() where T : struct, Enum
    {
        var values = Enum.GetValues<T>().Where(v => Enum.GetName<T>(v) != "Unspecified");
        return new(values);
    }

    void IRecipient<Messages.IsBusyChanged>.Receive(Messages.IsBusyChanged message)
        => IsBusy = message.IsBusy;
}
