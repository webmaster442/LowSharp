using System.Collections.ObjectModel;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Google.Protobuf.Collections;

using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;
using LowSharp.Client.Lowering.Examples;

namespace LowSharp.Client.Lowering;

internal sealed partial class LoweringViewModel :
    ViewModelWithMenus,
    IRecipient<Messages.IsBusyChanged>,
    IRecipient<Messages.SetInputLanguage>
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public MenuCheckableViewModel ShowLineNumbers { get; }

    public MenuCheckableViewModel WordWrap { get; }

    public ExamplesViewModel Examples { get; }

    public ObservableCollection<InputLanguage> InputLanguages { get; }

    [ObservableProperty]
    public partial int SelectedInputLanguageIndex { get; set; }

    public ObservableCollection<OutputCodeType> OutputTypes { get; }

    [ObservableProperty]
    public partial int SelectedOutputTypeIndex { get; set; }

    public ObservableCollection<Optimization> Optimizations { get; }

    [ObservableProperty]
    public partial int SelectedOptimizationIndex { get; set; }

    public LoweringViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
        IsBusy = client.IsBusy;
        Examples = new ExamplesViewModel();
        WeakReferenceMessenger.Default.Register<Messages.IsBusyChanged>(this);
        WeakReferenceMessenger.Default.Register<Messages.SetInputLanguage>(this);

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

        Menus.Add(Examples.GenerateMenu());

        InputLanguages = Fill<InputLanguage>();
        OutputTypes = Fill<OutputCodeType>();
        Optimizations = Fill<Optimization>();
        SelectedInputLanguageIndex = 0;
        SelectedOutputTypeIndex = 0;
        SelectedOptimizationIndex = 0;
    }

    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpenCode(out var result))
        {
            var code = System.IO.File.ReadAllText(result.filename);
            WeakReferenceMessenger.Default.Send(new Messages.SetInputCode(code));
            SelectedInputLanguageIndex = InputLanguages.IndexOf(result.language);
        }
    }


    [RelayCommand]
    public async Task Lower()
    {
        string inputCode = WeakReferenceMessenger.Default.Send<RequestMessages.GetLoweringInputCodeRequest>();

        LoweringResponse result = await _client.LowerCodeAsync(inputCode,
                                                               InputLanguages[SelectedInputLanguageIndex],
                                                               Optimizations[SelectedOptimizationIndex],
                                                               OutputTypes[SelectedOutputTypeIndex]);

        if (result.Diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
        {
            WeakReferenceMessenger.Default.Send(new Messages.SetOutputCodeRequest(GetDiagnostics(result.Diagnostics)));
            return;
        }

        WeakReferenceMessenger.Default.Send(new Messages.SetOutputCodeRequest(result.ResultCode));
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
        => new(Enum.GetValues<T>());

    void IRecipient<Messages.IsBusyChanged>.Receive(Messages.IsBusyChanged message)
        => IsBusy = message.IsBusy;

    void IRecipient<Messages.SetInputLanguage>.Receive(Messages.SetInputLanguage message)
    {
        if (message.Language.Contains("cs", StringComparison.OrdinalIgnoreCase))
        {
            SelectedInputLanguageIndex = InputLanguages.IndexOf(InputLanguage.Csharp);
        }
        else if (message.Language.Contains("v", StringComparison.OrdinalIgnoreCase))
        {
            SelectedInputLanguageIndex = InputLanguages.IndexOf(InputLanguage.Visualbasic);
        }
        else if (message.Language.Contains("fs", StringComparison.OrdinalIgnoreCase))
        {
            SelectedInputLanguageIndex = InputLanguages.IndexOf(InputLanguage.Fsharp);
        }
    }
}
