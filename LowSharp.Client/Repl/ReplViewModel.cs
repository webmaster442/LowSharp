using CommunityToolkit.Mvvm.Input;

using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Repl;

internal sealed partial class ReplViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    

    public ReplViewModel(IClient client)
    {
        _client = client;
    }

    [RelayCommand]
    public async Task Send()
    {

    }

    [RelayCommand]
    public void SetInput(string text)
    {

    }
}
