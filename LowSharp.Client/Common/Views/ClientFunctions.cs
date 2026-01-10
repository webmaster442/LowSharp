namespace LowSharp.Client.Common.Views;

internal class ClientFunctions : IClient
{
    private readonly ClientViewModel _clientViewModel;
    private readonly IDialogs _dialogs;

    public ClientFunctions(ClientViewModel clientViewModel, IDialogs dialogs)
    {
        _clientViewModel = clientViewModel;
        _dialogs = dialogs;
    }

    public bool IsBusy => _clientViewModel.IsBusy;

    public async Task<bool> DoHealthCheck()
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.HealthCheck.Health.HealthClient(_clientViewModel.Channel);
        try
        {
            int number1 = Random.Shared.Next();
            int number2 = Random.Shared.Next();

            long expectedSum = (long)number1 + (long)number2;

            _clientViewModel.IsBusy = true;
            var response = await client.CheckAsync(new ApiV1.HealthCheck.HealthCheckRequest
            {
                Number1 = number1,
                Number2 = number2,
            });

            return response.Sum == expectedSum;
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return false;
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }


    public async Task<ApiV1.HealthCheck.GetComponentVersionsRespnse> GetComponentVersions()
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.HealthCheck.Health.HealthClient(_clientViewModel.Channel);
        try
        {
            _clientViewModel.IsBusy = true;
            return await client.GetComponentVersionsAsync(new ApiV1.HealthCheck.GetComponentVersionsRequest());
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return new ApiV1.HealthCheck.GetComponentVersionsRespnse();
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }

    public void HideIsBusy()
        => _clientViewModel.IsBusy = false;

    public async Task<Guid> InitializeReplSession()
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.Evaluate.Evaluator.EvaluatorClient(_clientViewModel.Channel);
        try
        {
            _clientViewModel.IsBusy = true;
            var result = await client.InitializeAsync(new ApiV1.Evaluate.InitializationRequest());
            return Guid.Parse(result.Id);
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return Guid.Empty;
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }

    public async Task<ApiV1.Lowering.LoweringResponse> LowerCodeAsync(string code,
                                                                      ApiV1.Lowering.InputLanguage inputLanguage,
                                                                      ApiV1.Lowering.Optimization optimization,
                                                                      ApiV1.Lowering.OutputCodeType outputCodeType)
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.Lowering.Lowerer.LowererClient(_clientViewModel.Channel);

        try
        {
            _clientViewModel.IsBusy = true;
            var result = await client.ToLowerCodeAsync(new ApiV1.Lowering.LoweringRequest()
            {
                Code = code,
                Language = inputLanguage,
                OptimizationLevel = optimization,
                OutputType = outputCodeType
            });
            return result;
        }
        catch (Exception ex)
        {
            return new ApiV1.Lowering.LoweringResponse()
            {
                ResultCode = string.Empty,
                Diagnostics =
                {
                    new ApiV1.Lowering.Diagnostic()
                    {
                        Severity = ApiV1.Lowering.DiagnosticSeverity.Error,
                        Message = $"Failed to lower code: {ex.Message}"
                    }
                }
            };
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }

    public async IAsyncEnumerable<ApiV1.Evaluate.FormattedText> SendReplInput(Guid session, string input)
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.Evaluate.Evaluator.EvaluatorClient(_clientViewModel.Channel);
        _clientViewModel.IsBusy = true;
        var response = client.Evaluate(new ApiV1.Evaluate.EvaluationRequest
        {
            Id = session.ToString(),
            UserInput = input,
        });

        while (await response.ResponseStream.MoveNext(CancellationToken.None))
        {
            var current = response.ResponseStream.Current;
            yield return current;
        }

        _clientViewModel.IsBusy = false;
    }
}
