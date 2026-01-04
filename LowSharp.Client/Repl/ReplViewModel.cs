using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;

using static System.Net.Mime.MediaTypeNames;

namespace LowSharp.Client.Repl;

internal sealed partial class ReplViewModel : ViewModelWithMenus
{
    private readonly IClient _client;

    private List<string> History { get; }
    private int _historyIndex;

    public ReplViewModel(IClient client)
    {
        _client = client;
        History = new List<string>();
    }

    [RelayCommand]
    public async Task Send()
    {
        var input = WeakReferenceMessenger.Default.Send(new RequestMessages.GetReplInputCodeRequest());
        History.Add(input.Response);
        _historyIndex = History.Count - 1;
    }

    [RelayCommand]
    public void SetInput(string text)
        => WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(text));

    [RelayCommand]
    public void PreviousHistory()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(History[_historyIndex]));
        int index = _historyIndex - 1 > -1 ? _historyIndex - 1 : History.Count - 1;
        _historyIndex = index;

    }

    [RelayCommand]
    public void NextHistory()
    {
        WeakReferenceMessenger.Default.Send(new Messages.SetReplInputCode(History[_historyIndex]));
        int index = _historyIndex + 1 < History.Count - 1 ? _historyIndex + 1 : 0;
        _historyIndex = index;
    }
}
