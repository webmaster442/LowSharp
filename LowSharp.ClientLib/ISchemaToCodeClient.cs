using LowSharp.ApiV1.SchemaCodeGen;

namespace LowSharp.ClientLib;

public interface ISchemaToCodeClient
{
    Task<Either<string, Exception>> JsonSchemaToCsharpAsync(string schema, JsonSchemaToCsharpOptions options, CancellationToken cancellation = default);
}
