using Grpc.Core;

using Lowsharp.Server.Interactive;

using LowSharp.ApiV1.Evaluate;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class EvaluatorService : Evaluator.EvaluatorBase
{
    private readonly CsharpEvaluator _evaluator;
    private readonly SessionManager _sessions;

    public EvaluatorService(CsharpEvaluator evaluator, SessionManager sessions)
    {
        _evaluator = evaluator;
        _sessions = sessions;
    }

    public override async Task<InitializationResponse> Initialize(InitializationRequest request, ServerCallContext context)
    {
        Guid id = await _evaluator.CreateSession();
        return new InitializationResponse { Id = id.ToString() };
    }

    public override Task<UninitializationResponse> Uninitialize(UninitializationRequest request, ServerCallContext context)
    {
        Guid id = Guid.Parse(request.Id);
        _sessions.Remove(id);
        return Task.FromResult(new UninitializationResponse());
    }

    public override async Task Evaluate(EvaluationRequest request, IServerStreamWriter<EvaluationResponse> responseStream, ServerCallContext context)
    {
        var result = _evaluator.EvaluateAsync(Guid.Parse(request.Id), request.UserInput, context.CancellationToken);
        await foreach(var output in result)
        {
            var response = new EvaluationResponse { Result = output };
            await responseStream.WriteAsync(response);
        }
    }
}
