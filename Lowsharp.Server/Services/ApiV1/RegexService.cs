using Grpc.Core;

using LowSharp.ApiV1.Regex;

using SysRegex = System.Text.RegularExpressions.Regex;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class RegexService : RegexTester.RegexTesterBase
{
    public override Task<RegexMatchResponse> Match(RegexRequest request, ServerCallContext context)
    {
        var regex = new SysRegex(request.Pattern,
                                 Mapper.Map(request.Options),
                                 TimeSpan.FromMilliseconds(request.Options.TimeoutMilliseconds));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var matches = regex.Matches(request.Input);
        stopwatch.Stop();

        var response = new RegexMatchResponse
        {
            ExtecutionTimeMs = stopwatch.ElapsedMilliseconds
        };
        response.Matches.Add(Mapper.Map(matches));

        return Task.FromResult(response);
    }

    public override Task<RegexSplitResponse> Split(RegexRequest request, ServerCallContext context)
    {
        var regex = new SysRegex(request.Pattern,
                                 Mapper.Map(request.Options),
                                 TimeSpan.FromMilliseconds(request.Options.TimeoutMilliseconds));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var splits = regex.Split(request.Input);
        stopwatch.Stop();

        var response = new RegexSplitResponse
        {
            ExtecutionTimeMs = stopwatch.ElapsedMilliseconds
        };

        response.Results.AddRange(splits);

        return Task.FromResult(response);
    }

    public override Task<RegexReplaceResponse> Replace(RegexReplaceRequest request, ServerCallContext context)
    {
        var regex = new SysRegex(request.Pattern,
                                 Mapper.Map(request.Options),
                                 TimeSpan.FromMilliseconds(request.Options.TimeoutMilliseconds));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var replaced = regex.Replace(request.Input, request.Replacement);
        stopwatch.Stop();

        var response = new RegexReplaceResponse
        {
            ExtecutionTimeMs = stopwatch.ElapsedMilliseconds,
            Result = replaced
        };

        return Task.FromResult(response);
    }
}
