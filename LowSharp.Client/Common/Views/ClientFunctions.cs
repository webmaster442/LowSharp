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

    public async Task<bool> DoHealthCheckAsync()
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

    public async Task InvalidateCache()
    {
        _clientViewModel.ThrowIfCantContinue();
        var client = new ApiV1.HealthCheck.Health.HealthClient(_clientViewModel.Channel);
        try
        {
            _clientViewModel.IsBusy = true;
            await client.InvalidateCacheAsync(new ApiV1.HealthCheck.Empty());
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }


    public async Task<ApiV1.HealthCheck.GetComponentVersionsRespnse> GetComponentVersionsAsync()
    {
        _clientViewModel.ThrowIfCantContinue();

        var client = new ApiV1.HealthCheck.Health.HealthClient(_clientViewModel.Channel);
        try
        {
            _clientViewModel.IsBusy = true;
            return await client.GetComponentVersionsAsync(new ApiV1.HealthCheck.Empty());
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

    public async Task<ApiV1.Regex.RegexMatchResponse> RegexMatchAsync(string input,
                                                                      string pattern,
                                                                      ApiV1.Regex.RegexOptions options)
    {
        _clientViewModel.ThrowIfCantContinue();
        var client = new ApiV1.Regex.RegexTester.RegexTesterClient(_clientViewModel.Channel);
        _clientViewModel.IsBusy = true;
        try
        {
            var result = await client.MatchAsync(new ApiV1.Regex.RegexRequest
            {
                Input = input,
                Pattern = pattern,
                Options = options,
            });
            return result;
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return new ApiV1.Regex.RegexMatchResponse();
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }

    public async Task<(string result, long time)> RegexReplaceAsync(string input,
                                                                    string replacement,
                                                                    string pattern,
                                                                    ApiV1.Regex.RegexOptions options)
    {
        _clientViewModel.ThrowIfCantContinue();
        var client = new ApiV1.Regex.RegexTester.RegexTesterClient(_clientViewModel.Channel);
        _clientViewModel.IsBusy = true;
        try
        {
            var result = await client.ReplaceAsync(new ApiV1.Regex.RegexReplaceRequest
            {
                Input = input,
                Options = options,
                Pattern = pattern,
                Replacement = replacement,
            });

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return (result.ErrorMessage, result.ExtecutionTimeMs);
            }

            return (result.Result, result.ExtecutionTimeMs);
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return (ex.Message, 0);
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }

    public async Task<(IList<string> results, long time)> RegexSplitAsync(string input,
                                                                          string pattern,
                                                                          ApiV1.Regex.RegexOptions options)
    {
        _clientViewModel.ThrowIfCantContinue();
        var client = new ApiV1.Regex.RegexTester.RegexTesterClient(_clientViewModel.Channel);
        _clientViewModel.IsBusy = true;
        try
        {
            var result = await client.SplitAsync(new ApiV1.Regex.RegexRequest
            {
                Input = input,
                Pattern = pattern,
                Options = options,
            });

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return ([result.ErrorMessage], result.ExtecutionTimeMs);
            }

            return (result.Results, result.ExtecutionTimeMs);
        }
        catch (Exception ex)
        {
            await _dialogs.Error("Server failed to reply", ex.Message);
            return ([ex.Message], 0);
        }
        finally
        {
            _clientViewModel.IsBusy = false;
        }
    }
}
