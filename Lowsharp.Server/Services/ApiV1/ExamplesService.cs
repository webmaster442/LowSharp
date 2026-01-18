using Grpc.Core;

using Lowsharp.Server.CodeExamples;

using LowSharp.ApiV1.Examples;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class ExamplesService : Examples.ExamplesBase
{
    private readonly ExampleProvider _exampleProvider;

    public ExamplesService(ExampleProvider exampleProvider)
    {
        _exampleProvider = exampleProvider;
    }

    public override async Task GetExamples(GetExamplesRequest request, IServerStreamWriter<LowSharp.ApiV1.Examples.Example> responseStream, ServerCallContext context)
    {
        foreach (var example in _exampleProvider)
        {
            await responseStream.WriteAsync(new LowSharp.ApiV1.Examples.Example
            {
                Name = example.Name,
                Language = example.Lang,
                Code = example.Value
            });
        }
    }
}
