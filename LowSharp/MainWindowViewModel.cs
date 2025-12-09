using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Core;
using LowSharp.Examples;

namespace LowSharp;

internal sealed partial class MainWindowViewModel : ObservableObject
{
    private const int BaseFontSize = 16;

    private readonly CachedLowerer _lowerer;
    private readonly IDialogs _dialogs;

    public ObservableCollection<double> ZoomLevels { get; }

    public ObservableCollection<InputLanguage> Languages { get; }

    public ObservableCollection<OutputOptimizationLevel> OptimizationLevels { get; }

    public ObservableCollection<OutputLanguage> OutputTypes { get; }

    public ObservableList<LoweringDiagnostic> Diagnostics { get; }

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

    public MainWindowViewModel(IDialogs dialogs)
    {
        _lowerer = new CachedLowerer(new Lowerer());
        _dialogs = dialogs;
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        OptimizationLevels = new ObservableCollection<OutputOptimizationLevel>([OutputOptimizationLevel.Debug, OutputOptimizationLevel.Release]);
        Languages = new ObservableCollection<InputLanguage>([InputLanguage.Csharp, InputLanguage.VisualBasic, InputLanguage.FSharp]);
        OutputTypes = new ObservableCollection<OutputLanguage>([OutputLanguage.Csharp, OutputLanguage.IL, OutputLanguage.JitAsm]);
        SelectedZoomIndex = ZoomLevels.IndexOf(1.0);
        Diagnostics = new ObservableList<LoweringDiagnostic>();
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
    public void ComponentVersions()
    {
        var versions = _lowerer.GetComponentVersions();
        _dialogs.Information("Component versions", versions.Select(v => $"{v.Name}: {v.Version}"));
    }

    [RelayCommand]
    public void LoadExample(Example example)
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetInputCodeRequest(example.Value));
    }

    [RelayCommand]
    public async Task LowerCode()
    {
        IsInProgress = true;
        var result = await _lowerer.ToLowerCodeAsync(new LowerRequest
        {
            Code = WeakReferenceMessenger.Default.Send<Messages.GetInputCodeRequest>(),
            InputLanguage = Languages[SelectedLanguageIndex],
            OutputOptimizationLevel = OptimizationLevels[SelectedOptimizationLevelIndex],
            OutputType = OutputTypes[SelectedOutputTypeIndex],
        }, CancellationToken.None);
        IsInProgress = false;

        OutputTabIndex = result.HasErrors ? 1 : 0;

        LoweredCode = result.LoweredCode;
        Diagnostics.ReplaceAll(result.Diagnostics);
    }

    [RelayCommand]
    public async Task Export()
    {
        IsInProgress = true;

        var exportPath = _dialogs.ExportDialog();
        if (!string.IsNullOrEmpty(exportPath))
        {
            string? html = await _lowerer.CreateExport(new LowerRequest
            {
                Code = WeakReferenceMessenger.Default.Send<Messages.GetInputCodeRequest>(),
                InputLanguage = Languages[SelectedLanguageIndex],
                OutputOptimizationLevel = OptimizationLevels[SelectedOptimizationLevelIndex],
                OutputType = OutputTypes[SelectedOutputTypeIndex],
            }, CancellationToken.None);

            if (html is null)
                _dialogs.Error("Export failed", "An error occurred while creating the export.");

            await File.WriteAllTextAsync(exportPath, html);
        }
        IsInProgress = false;
    }
}