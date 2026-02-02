using LowSharp.ClientLib;
using LowSharp.Common;
using LowSharp.Common.ViewModels;

namespace LowSharp.Schema;

internal sealed partial class JsonSchemaToCsharpViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    public JsonSchemaToCsharpViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
    }
}
