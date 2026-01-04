using CommunityToolkit.Mvvm.Messaging.Messages;

using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Common;

internal static class RequestMessages
{
    public sealed class GetLoweringInputCodeRequest : RequestMessage<string>;
    public sealed class GetReplInputCodeRequest : RequestMessage<string>;
}
