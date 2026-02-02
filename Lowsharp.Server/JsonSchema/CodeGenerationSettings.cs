namespace Lowsharp.Server.JsonSchema;

internal sealed class CodeGenerationSettings
{
    public string Namespace { get; init; } = "GeneratedNamespace";
    public DateTimeType DateTimeType { get; init; } = DateTimeType.DateTimeOffset;
    public DateType DateType { get; init; } = DateType.DateTimeOffset;
    public TimeType TimeType { get; init; } = TimeType.TimeSpan;
    public ClassStyle ClassStyle { get; init; } = ClassStyle.RecordClass;
    public JsonLibrary JsonLibrary { get; init; } = JsonLibrary.SystemTextJson;
    public bool UseRequiredKeyword { get; init; } = true;
    public TypeAccessModifier AccessModifier { get; init; } = TypeAccessModifier.Public;
    public bool Nullable { get; init; } = true;
}
