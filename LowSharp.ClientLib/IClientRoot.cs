namespace LowSharp.ClientLib;

internal interface IClientRoot
{
    bool IsBusy { get; set; }
    string HttpUrl { get; }

    void ThrowIfCantContinue();
}
