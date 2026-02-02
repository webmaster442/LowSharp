namespace Lowsharp.Server.JsonSchema;

internal sealed class CodeGenerationRequest
{
    public CodeGenerationSettings Settings { get; init; } = new();
    public required string JsonSchema { get; init; }
}
