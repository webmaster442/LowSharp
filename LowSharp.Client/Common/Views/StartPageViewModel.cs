using System.Text;

using CommunityToolkit.Mvvm.Input;

using LowSharp.ApiV1.HealthCheck;
using LowSharp.Client.Lowering;
using LowSharp.Client.RegexTesting;
using LowSharp.ClientLib;

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
    public async Task StartRegex()
    {
        var vm = new RegexTestingViewModel(_client, _dialogs);
        await vm.InitializeAsync();
        ReplaceContents("Regex Testing", vm);
    }

    [RelayCommand]
    public async Task StartVersions()
    {
        var result = await _client.HealtCheck.GetComponentVersionsAsync();
        
        if (result.TryGetFailure(out Exception? ex))
        {
            await _dialogs.Error("Error Getting Versions", ex?.Message ?? "Unknown error");
            return;
        }

        if (!result.TryGetSuccess(out GetComponentVersionsRespnse? response))
        {
            await _dialogs.Error("Error Getting Versions", "Unknown error");
            return;
        }

        StringBuilder resultText = new StringBuilder();
        resultText.AppendLine($"Operating System: {response.OperatingSystem} {response.OperatingSystemVersion}");
        resultText.AppendLine($"Runtime Version: {response.RuntimeVersion}");
        resultText.AppendLine("-----------------------------------------");
        resultText.AppendLine("Component Versions:");
        foreach (var component in response.ComponentVersions)
        {
            resultText.AppendLine($"{component.Name}: {component.VersionString}");
        }
        await _dialogs.Info("Component Versions", resultText.ToString());
    }
}
