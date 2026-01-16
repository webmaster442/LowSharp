namespace LowSharp.ClientLib;

internal interface IClientRoot
{
    bool IsBusy { get; set; }
    void ThrowIfCantContinue();
}
