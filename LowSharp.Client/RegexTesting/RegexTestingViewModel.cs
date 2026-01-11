using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LowSharp.ApiV1.Regex;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client.RegexTesting;

internal sealed partial class RegexTestingViewModel : ViewModelWithMenus
{
    private readonly IClient _client;

    public RegexTestingViewModel(IClient client)
    {
        Options = new RegexOptionViewModel();
        IsMatchMode = true;
        Input = string.Empty;
        Pattern = string.Empty;
        Replacement = string.Empty;
        Result = string.Empty;
        _client = client;
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
        if (IsMatchMode)
        {
            var results = await _client.RegexMatchAsync(Input, Pattern, Options.GetOptions());
            Result = FormatResults(results);
            ExecutionTimeInMs = results.ExtecutionTimeMs;
        }
        else if (IsReplaceMode)
        {
            (string result, long time) = await _client.RegexReplaceAsync(Input, Pattern, Replacement, Options.GetOptions());
            Result = result;
            ExecutionTimeInMs = time;
        }
        else if (IsSplitMode)
        {
            (IList<string> results, long time) = await _client.RegexSplitAsync(Input, Pattern, Options.GetOptions());
            Result = string.Join(Environment.NewLine, results);
            ExecutionTimeInMs = time;
        }
    }

    private string FormatResults(RegexMatchResponse results)
    {
        StringBuilder result = new();
        foreach (var match in results.Matches)
        {
            result.AppendLine($"Match Name: {match.Name}, Success?: {match.Success}, Value: {match.Value}, Index: {match.Index}, Length: {match.Length}");
        }
        return result.ToString();
    }
}
