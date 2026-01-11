using System.Text;

using CommunityToolkit.Mvvm.Input;

using LowSharp.Client.Lowering;
using LowSharp.Client.RegexTesting;
using LowSharp.Client.Repl;

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
    public async Task StartLowering()
    {
        var vm = new LoweringViewModel(_client, _dialogs);
        await vm.InitializeAsync();
        ReplaceContents("Lowering", vm);
    }

    [RelayCommand]
    public async Task StartRepl()
    {
        var vm = new ReplViewModel(_client, _dialogs);
        await vm.InitializeAsync();
        ReplaceContents("REPL", vm);
    }

    [RelayCommand]
    public async Task StartRegex()
    {
        var vm = new RegexTestingViewModel(_client);
        await vm.InitializeAsync();
        ReplaceContents("Regex Testing", vm);
    }

    [RelayCommand]
    public async Task StartVersions()
    {
        var result = await _client.GetComponentVersionsAsync();
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
