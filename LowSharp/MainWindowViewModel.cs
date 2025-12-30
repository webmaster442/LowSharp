using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Examples;
using LowSharp.Server.ApiV1;

using static LowSharp.Server.ApiV1.Lowerer;

namespace LowSharp;

internal sealed partial class MainWindowViewModel : ObservableObject
{
    private const int BaseFontSize = 16;

    private readonly IDialogs _dialogs;
    private readonly LowererClient _client;

    public ObservableCollection<double> ZoomLevels { get; }

    public ObservableCollection<InputLanguage> Languages { get; }

    public ObservableCollection<Optimization> OptimizationLevels { get; }

    public ObservableCollection<OutputCodeType> OutputTypes { get; }

    public ObservableList<Diagnostic> Diagnostics { get; }

    public ExamplesViewModel Examples { get; }

    [ObservableProperty]
    public partial int SelectedZoomIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedLanguageIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedOutputTypeIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedOptimizationLevelIndex { get; set; }

    partial void OnSelectedZoomIndexChanged(int value)
    {
        ComputedFontSize = BaseFontSize * ZoomLevels[SelectedZoomIndex];
        OnPropertyChanged(nameof(ComputedFontSize));
    }

    [ObservableProperty]
    public partial string LoweredCode { get; set; }

    [ObservableProperty]
    public partial bool IsInProgress { get; set; }

    [ObservableProperty]
    public partial int OutputTabIndex { get; set; }

    public double ComputedFontSize { get; private set; }

    public MainWindowViewModel(IDialogs dialogs, LowererClient client)
    {
        _dialogs = dialogs;
        _client = client;
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        OptimizationLevels = new ObservableCollection<Optimization>([Optimization.Debug, Optimization.Release]);
        Languages = new ObservableCollection<InputLanguage>([InputLanguage.Csharp, InputLanguage.VisualBasic, InputLanguage.Fsharp]);
        OutputTypes = new ObservableCollection<OutputCodeType>([OutputCodeType.LoweredCsharp, OutputCodeType.Il, OutputCodeType.JitAsm]);
        SelectedZoomIndex = ZoomLevels.IndexOf(1.0);
        Diagnostics = new ObservableList<Diagnostic>();
        LoweredCode = string.Empty;
        Examples = new ExamplesViewModel();
    }

    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpenCode(out var result))
        {
            var code = System.IO.File.ReadAllText(result.filename);
            WeakReferenceMessenger.Default.Send(new Messages.SetInputCodeRequest(code));
            SelectedLanguageIndex = Languages.IndexOf(result.language);
        }
    }

    [RelayCommand]
    public void Exit()
        => Environment.Exit(0);

    [RelayCommand]
    public async Task ComponentVersions()
    {
        var versions = await _client.GetComponentVersionsAsync(new GetComponentVersionsRequest());
        _dialogs.Information("Component versions", versions.ComponentVersions.Select(v => $"{v.Name}: {v.VersionString}"));
    }

    [RelayCommand]
    public void LoadExample(Example example)
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetInputCodeRequest(example.Content));
    }

    [RelayCommand]
    public async Task LowerCode()
    {
        IsInProgress = true;

        var result = await _client.ToLowerCodeAsync(new LoweringRequest
        {
            Code = WeakReferenceMessenger.Default.Send<Messages.GetInputCodeRequest>(),
            Language = Languages[SelectedLanguageIndex],
            OptimizationLevel = OptimizationLevels[SelectedOptimizationLevelIndex],
            OutputType = OutputTypes[SelectedOutputTypeIndex],
        });

        IsInProgress = false;

        OutputTabIndex = result.Diagnostics.Count != 0 ? 1 : 0;
        LoweredCode = result.ResultCode;
        Diagnostics.ReplaceAll(result.Diagnostics);
    }
}