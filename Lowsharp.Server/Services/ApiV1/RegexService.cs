using System.Text;

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

    public override Task<RegexCodeResponse> GenerateCode(RegexRequest request, ServerCallContext context)
    {
        List<string> optionList = new();
        if (request.Options.IgnoreCase)
            optionList.Add("RegexOptions.IgnoreCase");
        if (request.Options.Multiline)
            optionList.Add("RegexOptions.Multiline");
        if (request.Options.ExplicitCapture)
            optionList.Add("RegexOptions.ExplicitCapture");
        if (request.Options.Compiled)
            optionList.Add("RegexOptions.Compiled");
        if (request.Options.Singleline)
            optionList.Add("RegexOptions.Singleline");
        if (request.Options.IgnorePatternWhitespace)
            optionList.Add("RegexOptions.IgnorePatternWhitespace");
        if (request.Options.RightToLeft)
            optionList.Add("RegexOptions.RightToLeft");
        if (request.Options.EcmaScript)
            optionList.Add("RegexOptions.ECMAScript");
        if (request.Options.CultureInvariant)
            optionList.Add("RegexOptions.CultureInvariant");
        if (request.Options.NonBackTracking)
            optionList.Add("RegexOptions.NonBacktracking");

        var output = new StringBuilder();
        output
            .Append("var myRegex = new Regex(")
            .Append($"@\"{request.Pattern}\", ");
        if (optionList.Count > 0)
            output.Append(string.Join(" | ", optionList));
        else
            output.Append("RegexOptions.None");
        
        output.Append($", TimeSpan.FromMilliseconds({request.Options.TimeoutMilliseconds}));");

        output.AppendLine("//as generated regex use this:");
        output
            .AppendLine("internal partial class CompilerGenerated")
            .AppendLine("{")
            .Append("    [")
            .Append("GeneratedRegex(")
            .Append($"@\"{request.Pattern}\", ");

        if (optionList.Count > 0)
            output.Append(string.Join(" | ", optionList));
        else
            output.Append("RegexOptions.None");

        output.AppendLine($", {request.Options.TimeoutMilliseconds})]");

        output
            .AppendLine("    internal partial Regex MyRegex { get; }")
            .AppendLine("}");

        var response = new RegexCodeResponse
        {
            ResultCode = output.ToString()
        };

        return Task.FromResult(response);
    }
}
