using System.Text;

using CommunityToolkit.Mvvm.Input;

using LowSharp.Client.Lowering;

namespace LowSharp.Client.Common.Views;

internal sealed partial class StartPageViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    public StartPageViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
    }

    [RelayCommand]
    public void StartLowering()
        => ReplaceContents("Lowering", new LoweringViewModel(_client, _dialogs));

    [RelayCommand]
    public void StartRepl()
    {

    }

    [RelayCommand]
    public async Task StartVersions()
    {
        var result = await _client.GetComponentVersions();
        StringBuilder resultText = new StringBuilder();
        resultText.AppendLine($"Operating System: {result.OperatingSystem} {result.OperatingSystemVersion}");
        resultText.AppendLine($"Runtime Version: {result.RuntimeVersion}");
        resultText.AppendLine("-----------------------------------------");
        resultText.AppendLine("Component Versions:");
        foreach (var component in result.ComponentVersions)
        {
            resultText.AppendLine($"{component.Name}: {component.VersionString}");
        }
        await _dialogs.Info("Component Versions", resultText.ToString());
    }
}
