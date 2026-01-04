namespace LowSharp.Client.Lowering.Examples;

internal record class Example
{
    public required string Name { get; init; }
    public required string Language { get; init; }
    public required string Content { get; init; }
}
