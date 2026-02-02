using NJsonSchema.CodeGeneration.CSharp;

namespace Lowsharp.Server.JsonSchema;

internal static class CsharpCodeGenerator
{
    private static CSharpJsonLibrary Map(JsonLibrary jsonLibrary)
    {
        return jsonLibrary switch
        {
            JsonLibrary.SystemTextJson => CSharpJsonLibrary.SystemTextJson,
            JsonLibrary.NewtonsoftJson => CSharpJsonLibrary.NewtonsoftJson,
            _ => throw new ArgumentOutOfRangeException(nameof(jsonLibrary), jsonLibrary, null),
        };
    }

    private static CSharpClassStyle Map(ClassStyle classStyle)
    {
        return classStyle switch
        {
            ClassStyle.PocoClass => CSharpClassStyle.Poco,
            ClassStyle.RecordClass => CSharpClassStyle.Record,
            _ => throw new ArgumentOutOfRangeException(nameof(classStyle), classStyle, null),
        };
    }

    private static string Map(TimeType timeType)
    {
        return timeType switch
        {
            TimeType.TimeOnly => nameof(TimeOnly),
            TimeType.TimeSpan => nameof(TimeSpan),
            _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null),
        };
    }

    private static string Map(DateType dateType)
    {
        return dateType switch
        {
            DateType.DateOnly => nameof(DateOnly),
            DateType.DateTime => nameof(DateTime),
            DateType.DateTimeOffset => nameof(DateTimeOffset),
            _ => throw new ArgumentOutOfRangeException(nameof(dateType), dateType, null),
        };
    }

    private static string Map(DateTimeType dateTimeType)
    {
        return dateTimeType switch
        {
            DateTimeType.DateTime => nameof(DateTime),
            DateTimeType.DateTimeOffset => nameof(DateTimeOffset),
            _ => throw new ArgumentOutOfRangeException(nameof(dateTimeType), dateTimeType, null),
        };
    }

    private static string Map(TypeAccessModifier accessModifier)
    {
        return accessModifier switch
        {
            TypeAccessModifier.Public => "public",
            TypeAccessModifier.Internal => "internal",
            _ => throw new ArgumentOutOfRangeException(nameof(accessModifier), accessModifier, null),
        };
    }

    private static CSharpGeneratorSettings Map(CodeGenerationSettings settings)
    {
        return new CSharpGeneratorSettings()
        {
            Namespace = settings.Namespace,
            DateTimeType = Map(settings.DateTimeType),
            DateType = Map(settings.DateType),
            TimeType = Map(settings.TimeType),
            ClassStyle = Map(settings.ClassStyle),
            JsonLibrary = Map(settings.JsonLibrary),
            JsonPolymorphicSerializationStyle = settings.JsonLibrary == JsonLibrary.SystemTextJson ? CSharpJsonPolymorphicSerializationStyle.SystemTextJson : CSharpJsonPolymorphicSerializationStyle.NJsonSchema,
            UseRequiredKeyword = settings.UseRequiredKeyword,
            TypeAccessModifier = Map(settings.AccessModifier),
            JsonLibraryVersion = 10.0M,
            GenerateNullableReferenceTypes = settings.Nullable,
        };
    }

    public static async Task<CodeGenerationResponse> GenerateCodeAsync(CodeGenerationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            CSharpGeneratorSettings generationSettings = Map(request.Settings);
            NJsonSchema.JsonSchema schema = await NJsonSchema.JsonSchema.FromJsonAsync(request.JsonSchema);
            var generator = new CSharpGenerator(schema, generationSettings);
            return new CodeGenerationResponse()
            {
                CsharpCode = generator.GenerateFile()
            };
        }
        catch (Exception ex)
        {
            return new CodeGenerationResponse()
            {
                CsharpCode = $"// Error generating code: {ex.Message}"
            };
        }
    }
}
