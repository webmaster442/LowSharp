using System.Linq.Expressions;

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

    [ObservableProperty]
    public partial long TimeoutMilliseconds { get; set; } = 5000;

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
        TimeoutMilliseconds = 5000;
    }

    public ApiV1.Regex.RegexOptions GetOptions()
    {
        return new ApiV1.Regex.RegexOptions
        {
            IgnoreCase = IgnoreCase,
            Compiled = Compiled,
            CultureInvariant = CultureInvariant,
            EcmaScript = ECMAScript,
            ExplicitCapture = ExplicitCapture,
            IgnorePatternWhitespace = IgnorePatternWhitespace,
            Multiline = Multiline,
            NonBackTracking = NonBacktracking,
            RightToLeft = RightToLeft,
            Singleline = Singleline,
            TimeoutMilliseconds = TimeoutMilliseconds,
        };
    }
}
