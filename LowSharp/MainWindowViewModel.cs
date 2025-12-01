using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LowSharp.Core;

namespace LowSharp;

internal sealed partial class MainWindowViewModel : ObservableObject
{
    private const int BaseFontSize = 16;

    private readonly Lowerer _lowerer;
    private readonly IDialogs _dialogs;

    public ObservableCollection<double> ZoomLevels { get; }

    public ObservableCollection<InputLanguage> Languages { get; }

    public ObservableCollection<OutputOptimizationLevel> OptimizationLevels { get; }

    public ObservableCollection<OutputType> OutputTypes { get; }

    public ObservableList<LoweringDiagnostic> Diagnostics { get; }

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
    public partial string InputCode { get; set; }

    [ObservableProperty]
    public partial string LoweredCode { get; set; }

    [ObservableProperty]
    public partial bool IsInProgress { get; set; }

    [ObservableProperty]
    public partial int OutputTabIndex { get; set; }

    public double ComputedFontSize { get; private set; }

    public MainWindowViewModel(IDialogs dialogs)
    {
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        OptimizationLevels = new ObservableCollection<OutputOptimizationLevel>([OutputOptimizationLevel.Debug, OutputOptimizationLevel.Release]);
        Languages = new ObservableCollection<InputLanguage>([InputLanguage.Csharp, InputLanguage.VisualBasic]);
        OutputTypes = new ObservableCollection<OutputType>([OutputType.Csharp, OutputType.IL]);
        SelectedZoomIndex = ZoomLevels.IndexOf(1.0);
        _lowerer = new Lowerer();
        Diagnostics = new ObservableList<LoweringDiagnostic>();
        InputCode = string.Empty;
        LoweredCode = string.Empty;
        _dialogs = dialogs;
    }

    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpenCode(out var result))
        {
            InputCode = System.IO.File.ReadAllText(result.filename);
            SelectedLanguageIndex = Languages.IndexOf(result.language);
        }
    }

    [RelayCommand]
    public void Exit()
    {
        Environment.Exit(0);
    }

    public async Task LowerCode()
    {
        IsInProgress = true;
        var result = await _lowerer.ToLowerCodeAsync(new LowerRequest
        {
            Code = InputCode,
            InputLanguage = Languages[SelectedLanguageIndex],
            OutputOptimizationLevel = OptimizationLevels[SelectedOptimizationLevelIndex],
            OutputType = OutputTypes[SelectedOutputTypeIndex],
        }, CancellationToken.None);
        IsInProgress = false;

        OutputTabIndex = result.HasErrors ? 1 : 0;

        LoweredCode = result.LoweredCode;
        Diagnostics.ReplaceAll(result.Diagnostics);
    }
}
