
using Grpc.Core;

using LowSharp.Core;

namespace LowSharp.Cli.Services;

public class LoweringService : LowSharp.Grpc.Api.LoweringService.LoweringServiceBase
{
    private readonly ILogger<LoweringService> _logger;
    private readonly ILowerer _lowerer;

    public LoweringService(ILowerer lowerer, ILogger<LoweringService> logger)
    {
        _lowerer = lowerer;
        _logger = logger;
    }

    public override async Task<Grpc.Api.LoweringResponse> ToLowerCode(Grpc.Api.LoweringRequest request, ServerCallContext context)
    {
        LowerResponse result = await _lowerer.ToLowerCodeAsync(Map(request), context.CancellationToken);
        return Map(result);
    }

    public override async Task<Grpc.Api.GetComponentVersionsRespnse> GetComponentVersions(Grpc.Api.GetComponentVersionsRequest request, ServerCallContext context)
    {
        var versions = _lowerer.GetComponentVersions();
        var response = new Grpc.Api.GetComponentVersionsRespnse();

        foreach (var version in versions)
        {
            response.ComponentVersions.Add(new Grpc.Api.ComponentVersion
            {
                Name = version.Name,
                Version = version.Version.ToString()
            });
        }
        return response;
    }


    private static Grpc.Api.LoweringResponse Map(LowerResponse result)
    {
        var returnValue = new Grpc.Api.LoweringResponse
        {
            ResultCode = result.LoweredCode
        };
        returnValue.Diagnostics.AddRange(result.Diagnostics.Select(d => new Grpc.Api.Diagnostics
        {
            Message = d.Message,
            Severity = d.Severity switch
            {
                MessageSeverity.Info => Grpc.Api.DiagnosticSeverity.Info,
                MessageSeverity.Warning => Grpc.Api.DiagnosticSeverity.Warning,
                MessageSeverity.Error => Grpc.Api.DiagnosticSeverity.Error,
                _ => throw new InvalidOperationException("Unknown severity")
            }
        }));
        return returnValue;
    }

    private static LowerRequest Map(Grpc.Api.LoweringRequest request)
    {
        return new LowerRequest
        {
            Code = request.Code,
            InputLanguage = request.Language switch
            {
                Grpc.Api.InputLanguage.Csharp => InputLanguage.Csharp,
                Grpc.Api.InputLanguage.Fsharp => InputLanguage.FSharp,
                Grpc.Api.InputLanguage.VisualBasic => InputLanguage.VisualBasic,
                _ => throw new InvalidOperationException("Unknown Input language")
            },
            OutputType = request.OutputType switch
            {
                Grpc.Api.OutputCodeType.Il => OutputLanguage.IL,
                Grpc.Api.OutputCodeType.JitAsm => OutputLanguage.JitAsm,
                Grpc.Api.OutputCodeType.LoweredCsharp => OutputLanguage.Csharp,
                _ => throw new InvalidOperationException("Unknown Output type")
            },
            OutputOptimizationLevel = request.OptimizationLevel switch
            {
                Grpc.Api.Optimization.Debug => OutputOptimizationLevel.Debug,
                Grpc.Api.Optimization.Release => OutputOptimizationLevel.Release,
                _ => throw new InvalidOperationException("Unknown optimization level")
            }
        };
    }
}