using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.ApiV1.Evaluate;
using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Repl;

internal sealed partial class ReplViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;
    private readonly List<string> _history;

    private int _historyIndex;

    [ObservableProperty]
    public partial Guid Session { get; set; }

    public ReplViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
        _history = new List<string>();
        Session = Guid.Empty;
    }

    public override async Task InitializeAsync()
    {
        Session = await _client.InitializeReplSessionAsync();
    }

    [RelayCommand]
    public async Task Send()
    {
        var input = WeakReferenceMessenger.Default.Send(new RequestMessages.GetReplInputCodeRequest());

        _history.Add(input.Response);
        WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(string.Empty));

        var results = _client.SendReplInputAsync(Session, input);
        try
        {
            await foreach (FormattedText result in results)
            {
                WeakReferenceMessenger.Default.Send(new Messages.AppendReplOutput(result));
            }
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Evaluation communication error", ex.Message);
            _client.HideIsBusy();
        }
        _historyIndex = _history.Count - 1;
    }

    [RelayCommand]
    public void SetInput(string text)
        => WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(text));

    [RelayCommand]
    public void PreviousHistory()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(_history[_historyIndex]));
        int index = _historyIndex - 1 > -1 ? _historyIndex - 1 : _history.Count - 1;
        _historyIndex = index;

    }

    [RelayCommand]
    public void ClearOutput()
        => WeakReferenceMessenger.Default.Send(new Messages.ClearReplOutput());

    [RelayCommand]
    public void NextHistory()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(_history[_historyIndex]));
        int index = _historyIndex + 1 < _history.Count - 1 ? _historyIndex + 1 : 0;
        _historyIndex = index;
    }
}
