namespace LowSharp.Client.Comon.Views;

internal interface IDialogs
{
    Task Error(string title, string message);
}
