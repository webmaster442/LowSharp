using CommunityToolkit.Mvvm.ComponentModel;

using LowSharp.Client.Common;

namespace LowSharp.Client.Lowering.Examples;

internal sealed class ExamplesViewModel : ObservableObject
{
    public ObservableList<Example> Csharp { get; }

    public ObservableList<Example> Fsharp { get; }

    public ExamplesViewModel()
    {
        Csharp = new ObservableList<Example>();
        Fsharp = new ObservableList<Example>();
        using var exampleReader = new ExampleReader();
        Csharp.BlockNotifications();
        Fsharp.BlockNotifications();
        exampleReader.ReadExamples(example =>
        {
            if (string.Equals(example.Language, nameof(Csharp), StringComparison.OrdinalIgnoreCase))
            {
                Csharp.Add(example);
            }
            else if (string.Equals(example.Language, nameof(Fsharp), StringComparison.OrdinalIgnoreCase))
            {
                Fsharp.Add(example);
            }
        });
        Csharp.UnblockAndFireNotifications();
        Fsharp.UnblockAndFireNotifications();
    }
}
