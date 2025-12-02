using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LowSharp;

public static class Messages
{
    public class GetInputCodeRequest : RequestMessage<string>;
    public record class SetInputCodeRequest(string Code);
}