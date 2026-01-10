using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LowSharp.Client.RegexTesting;

internal sealed partial class RegexOptionViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IgnoreCase { get; set; }

    [ObservableProperty]
    public partial bool Multiline { get; set; }

    [ObservableProperty]
    public partial bool ExplicitCapture { get; set; }

    [ObservableProperty]
    public partial bool Compiled { get; set; }

    [ObservableProperty]
    public partial bool Singleline { get; set; }

    [ObservableProperty]
    public partial bool IgnorePatternWhitespace { get; set; }

    [ObservableProperty]
    public partial bool RightToLeft { get; set; }

    [ObservableProperty]
    public partial bool ECMAScript { get; set; }

    [ObservableProperty]
    public partial bool CultureInvariant { get; set; }

    [ObservableProperty]
    public partial bool NonBacktracking { get; set; }

    [RelayCommand]
    public void ResetOptions()
    {
        IgnoreCase = false;
        Multiline = false;
        ExplicitCapture = false;
        Compiled = false;
        Singleline = false;
        IgnorePatternWhitespace = false;
        RightToLeft = false;
        ECMAScript = false;
        CultureInvariant = false;
        NonBacktracking = false;
    }
}
