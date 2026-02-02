using System;
using System.Collections.Generic;
using System.Text;

using Grpc.Net.Client;

using LowSharp.ApiV1.SchemaCodeGen;

namespace LowSharp.ClientLib;

internal sealed class SchemaToCodeClient : ISchemaToCodeClient
{
    private readonly IClientRoot _root;
    private readonly SchemaToCode.SchemaToCodeClient _client;

    public SchemaToCodeClient(GrpcChannel channel, IClientRoot root)
    {
        _client = new SchemaToCode.SchemaToCodeClient(channel);
        _root = root;
    }

    public async Task<Either<string, Exception>> JsonSchemaToCsharpAsync(string schema, JsonSchemaToCsharpOptions options, CancellationToken cancellation = default)
    {
        _root.ThrowIfCantContinue();
        try
        {
            _root.IsBusy = true;
            var response = await _client.JsonSchemaToCsharpAsync(new JsonSchemaToCsharpRequest
            {
                JsonSchema = schema,
                Options = options
            });
            return response.GeneratedCode;
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            _root.IsBusy = false;
        }
    }
}
