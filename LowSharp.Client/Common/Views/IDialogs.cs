namespace LowSharp.Client.Common.Views;

internal interface IDialogs
{
    Task Error(string title, string message);
}
