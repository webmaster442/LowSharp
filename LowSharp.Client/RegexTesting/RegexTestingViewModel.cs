using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LowSharp.ApiV1.Regex;
using LowSharp.Client.Common.Views;
using LowSharp.ClientLib;

namespace LowSharp.Client.RegexTesting;

internal sealed partial class RegexTestingViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    public RegexTestingViewModel(IClient client, IDialogs dialogs)
    {
        Options = new RegexOptionViewModel();
        IsMatchMode = true;
        Input = string.Empty;
        Pattern = string.Empty;
        Replacement = string.Empty;
        Result = string.Empty;
        _client = client;
        _dialogs = dialogs;
    }

    [ObservableProperty]
    public partial bool IsMatchMode { get; set; }

    [ObservableProperty]
    public partial bool IsReplaceMode { get; set; }

    [ObservableProperty]
    public partial bool IsSplitMode { get; set; }

    public RegexOptionViewModel Options { get; }

    [ObservableProperty]
    public partial string Pattern { get; set; }

    [ObservableProperty]
    public partial string Input { get; set; }

    [ObservableProperty]
    public partial string Replacement { get; set; }

    [ObservableProperty]
    public partial string Result { get; set; }

    [ObservableProperty]
    public partial long ExecutionTimeInMs { get; set; }

    [RelayCommand]
    public async Task Execute()
    {
        var options = Options.GetOptions();
        if (IsMatchMode)
        {
            await DoMatch(Input, Pattern, options);
        }
        else if (IsReplaceMode)
        {
            await DoReplace(Input, Pattern, Replacement, options);
        }
        else if (IsSplitMode)
        {
            await DoSplit(Input, Pattern, options);
        }
    }

    private async Task DoSplit(string input, string pattern, RegexOptions options)
    {
        var response = await _client.Regex.SplitAsync(input, pattern, options);

        if (response.TryGetFailure(out Exception? failure))
        {
            await _dialogs.ClientError(failure);
            return;
        }

        if (response.TryGetSuccess(out var success))
        {
            Result = string.Join(Environment.NewLine, success.Results);
            ExecutionTimeInMs = success.ExtecutionTimeMs;
        }
    }

    private async Task DoReplace(string input, string pattern, string replacement, RegexOptions options)
    {
        var response = await _client.Regex.ReplaceAsync(input, replacement, pattern, options);

        if (response.TryGetFailure(out Exception? failure))
        {
            await _dialogs.ClientError(failure);
            return;
        }

        if (response.TryGetSuccess(out var success))
        {
            Result = success.Result;
            ExecutionTimeInMs = success.ExtecutionTimeMs;
        }
    }

    private async Task DoMatch(string input, string pattern, RegexOptions options)
    {
        var response = await _client.Regex.MatchAsync(input, pattern, options);

        if (response.TryGetFailure(out Exception? failure))
        {
            await _dialogs.ClientError(failure);
            return;
        }

        if (response.TryGetSuccess(out var success))
        {
            ExecutionTimeInMs = success.ExtecutionTimeMs;
            StringBuilder result = new();
            foreach (var match in success.Matches)
            {
                result.AppendLine($"Match Name: {match.Name}, Success?: {match.Success}, Value: {match.Value}, Index: {match.Index}, Length: {match.Length}");
            }
            Result = result.ToString();
        }
    }
}
