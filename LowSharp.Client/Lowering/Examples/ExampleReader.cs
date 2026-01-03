using System.IO;
using System.Text;

namespace LowSharp.Client.Lowering.Examples;

internal sealed class ExampleReader : IDisposable
{
    private readonly Stream _stream;
    private bool _disposed;

    private const string ResourceName = "LowSharp.Client.Lowering.Examples.Examples.txt";

    public ExampleReader()
    {
        var stream = typeof(ExamplesViewModel).Assembly.GetManifestResourceStream(ResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource '{ResourceName}'.");
        }
        _stream = stream;
    }

    public void ReadExamples(Action<Example> sorter)
    {
        static void TryRunSorter(Action<Example> sorter, string[] current, StringBuilder currentContent)
        {
            if (string.IsNullOrEmpty(current[0]))
            {
                return;
            }

            var example = new Example
            {
                Name = current[0],
                Language = current[1],
                Content = currentContent.ToString()
            };

            current[0] = string.Empty;
            current[1] = string.Empty;
            currentContent.Clear();

            sorter(example);
        }

        ObjectDisposedException.ThrowIf(_disposed, nameof(ExampleReader));
        using var reader = new StreamReader(_stream, leaveOpen: true);
        string? line;

        string[] current = new string[2];
        StringBuilder currentContent = new StringBuilder();

        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.StartsWith("# "))
            {
                TryRunSorter(sorter, current, currentContent);

                string[] nameAndLanguage = line.Split(['#', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                current[0] = nameAndLanguage[0];
                current[1] = nameAndLanguage[1];
            }
            else
            {
                currentContent.AppendLine(line);
            }
        }

        TryRunSorter(sorter, current, currentContent);
    }

    public void Dispose()
    {
        _stream.Dispose();
        _disposed = true;
    }
}