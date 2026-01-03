using CommunityToolkit.Mvvm.Messaging.Messages;

using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Common;

internal static class RequestMessages
{
    public sealed class GetInputCodeRequest : RequestMessage<string>;
}
