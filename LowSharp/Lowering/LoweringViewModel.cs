using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Google.Protobuf.Collections;

using LowSharp.ApiV1.Examples;
using LowSharp.ApiV1.Lowering;
using LowSharp.ClientLib;
using LowSharp.Common;
using LowSharp.Common.Controls;
using LowSharp.Common.ViewModels;

namespace LowSharp.Lowering;

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

    public EnumViewModel<InputLanguage> InputLanguages { get; }

    public EnumViewModel<OutputCodeType> OutputTypes { get; }

    public EnumViewModel<Optimization> Optimizations { get; }

    [ObservableProperty]
    public partial string InputCode { get; set; }

    partial void OnInputCodeChanged(string value)
        => IsValidLowering = false;


    [ObservableProperty]
    public partial string OutputCode { get; set; }

    [ObservableProperty]
    public partial bool IsValidLowering { get; set; }

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

        InputLanguages = new EnumViewModel<InputLanguage>(EnumMapper.ToString, InputLanguage.Csharp);
        OutputTypes = new EnumViewModel<OutputCodeType>(EnumMapper.ToString, OutputCodeType.Loweredcsharp);
        Optimizations = new EnumViewModel<Optimization>(EnumMapper.ToString, Optimization.Debug);
        OutputTypes.PropertyChanged += OnOutputTypeChanged;
    }

    private void OnOutputTypeChanged(object? sender, PropertyChangedEventArgs e)
        => IsValidLowering = false;

    public override async Task InitializeAsync()
    {
        Either<List<Example>, Exception> examples = await _client.Examples.GetExamplesAsync();
        if (examples.TryGetFailure(out Exception? failure))
        {
            _dialogs.ClientError(failure);
            return;
        }

        if (examples.TryGetSuccess(out List<Example>? result))
        {
            _exampleList = result;

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

    [RelayCommand]
    public void LoadExample(Example example)
    {
        InputCode = example.Code;
        InputLanguages.SelectValueByStringName(example.Language);
    }


    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpenCode(out var result))
        {
            string code = System.IO.File.ReadAllText(result.filename);
            InputCode = code;
            InputLanguages.SelectValue(result.language);
        }
    }


    [RelayCommand]
    public async Task Lower()
    {
        Either<LoweringResponse, Exception> response = await _client.Lowering.LowerCodeAsync(
            InputCode,
            InputLanguages.SelectedValue,
            Optimizations.SelectedValue,
            OutputTypes.SelectedValue);

        response.Map(success =>
        {
            if (success.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                OutputCode = GetDiagnostics(success.Diagnostics);
                IsValidLowering = false;
            }
            OutputCode = success.ResultCode;
            IsValidLowering = true;
        },
        failure =>
        {
            IsValidLowering = false;
            _dialogs.ClientError(failure);
        });
    }

    [RelayCommand]
    public async Task Preview()
    {
        VisualType visualType = OutputTypes.SelectedValue switch
        {
            OutputCodeType.Nonmoml => VisualType.Nomnoml,
            OutputCodeType.Mermaid => VisualType.Mermaid,
            _ => throw new InvalidOperationException("Unknown visual type"),
        };

        Either<Uri, Exception> response = await _client.Lowering.RenderVisualizationAsync(OutputCode, visualType);

        response.Map(succes =>
        {
            _dialogs.OpenWebView("Diagram Previrew", succes);
        },
        failure =>
        {
            _dialogs.ClientError(failure);
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
