using Grpc.Core;

using Lowsharp.Server.JsonSchema;

using LowSharp.ApiV1.SchemaCodeGen;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class SchemaToCodeService : SchemaToCode.SchemaToCodeBase
{
    public override async Task<JsonSchemaToCsharpResponse> JsonSchemaToCsharp(JsonSchemaToCsharpRequest request, ServerCallContext context)
    {
        var result = await CsharpCodeGenerator.GenerateCodeAsync(Map(request), context.CancellationToken);
        return new JsonSchemaToCsharpResponse
        {
            GeneratedCode = result.CsharpCode
        };

    }

    private CodeGenerationRequest Map(JsonSchemaToCsharpRequest request)
    {
        return new CodeGenerationRequest
        {
            JsonSchema = request.JsonSchema,
            Settings = new CodeGenerationSettings
            {
                Namespace = request.Options.Namespace,
                DateTimeType = Map(request.Options.DateTimeType),
                DateType = Map(request.Options.DateType),
                TimeType = Map(request.Options.TimeType),
                ClassStyle = Map(request.Options.ClassStyle),
                JsonLibrary = Map(request.Options.JsonLibary),
                UseRequiredKeyword = request.Options.UseRequired,
                AccessModifier = Map(request.Options.AccessModifier),
                Nullable = request.Options.Nullable,
            }
        };
    }

    private TypeAccessModifier Map(AccessModifier accessModifier)
    {
        return accessModifier switch
        {
            AccessModifier.Internal => TypeAccessModifier.Internal,
            AccessModifier.Public => TypeAccessModifier.Public,
            _ => throw new InvalidCastException($"Unknown AccessModifier value: {accessModifier}"),
        };
    }

    private JsonLibrary Map(JsonLibary jsonLibary)
    {
        return jsonLibary switch
        {
            JsonLibary.Newtonsoft => JsonLibrary.NewtonsoftJson,
            JsonLibary.Systemtext => JsonLibrary.SystemTextJson,
            _ => throw new InvalidCastException($"Unknown JsonLibary value: {jsonLibary}"),
        };
    }

    private JsonSchema.ClassStyle Map(LowSharp.ApiV1.SchemaCodeGen.ClassStyle classStyle)
    {
        return classStyle switch
        {
            LowSharp.ApiV1.SchemaCodeGen.ClassStyle.Poco => JsonSchema.ClassStyle.PocoClass,
            LowSharp.ApiV1.SchemaCodeGen.ClassStyle.Record => JsonSchema.ClassStyle.RecordClass,
            _ => throw new InvalidCastException($"Unknown ClassStyle value: {classStyle}"),
        };
    }

    private JsonSchema.TimeType Map(LowSharp.ApiV1.SchemaCodeGen.TimeType timeType)
    {
        return timeType switch
        {
            LowSharp.ApiV1.SchemaCodeGen.TimeType.Timespan => JsonSchema.TimeType.TimeSpan,
            LowSharp.ApiV1.SchemaCodeGen.TimeType.Timeonly => JsonSchema.TimeType.TimeOnly,
            _ => throw new InvalidCastException($"Unknown TimeType value: {timeType}"),
        };
    }

    private JsonSchema.DateType Map(LowSharp.ApiV1.SchemaCodeGen.DateType dateType)
    {
        return dateType switch
        {
            LowSharp.ApiV1.SchemaCodeGen.DateType.Dateonly => JsonSchema.DateType.DateOnly,
            LowSharp.ApiV1.SchemaCodeGen.DateType.Datetime => JsonSchema.DateType.DateTime,
            LowSharp.ApiV1.SchemaCodeGen.DateType.Datetimeoffset => JsonSchema.DateType.DateTimeOffset,
            _ => throw new InvalidCastException($"Unknown DateType value: {dateType}"),
        };
    }

    private JsonSchema.DateTimeType Map(LowSharp.ApiV1.SchemaCodeGen.DateTimeType dateTimeType)
    {
        return dateTimeType switch
        {
            LowSharp.ApiV1.SchemaCodeGen.DateTimeType.Datetime => JsonSchema.DateTimeType.DateTime,
            LowSharp.ApiV1.SchemaCodeGen.DateTimeType.Datetimeoffset => JsonSchema.DateTimeType.DateTimeOffset,
            _ => throw new InvalidCastException($"Unknown DateTimeType value: {dateTimeType}"),
        };
    }
}
