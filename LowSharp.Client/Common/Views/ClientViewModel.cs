using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Grpc.Net.Client;

using LowSharp.ApiV1.HealthCheck;

using Windows.Media.Protection.PlayReady;

namespace LowSharp.Client.Common.Views;

internal sealed partial class ClientViewModel :
    ObservableObject,
    IDisposable,
    IClient
{
    private readonly DispatcherTimer _checkTimer;
    private readonly IDialogs _dialogs;
    private readonly string _serverPath;
    private GrpcChannel? _channel;
    private bool _disposed;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    partial void OnIsConnectedChanged(bool oldValue, bool newValue)
        => WeakReferenceMessenger.Default.Send(new Messages.IsConnectedChanged(newValue));

    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    partial void OnIsBusyChanged(bool oldValue, bool newValue)
        => WeakReferenceMessenger.Default.Send(new Messages.IsBusyChanged(newValue));

    public ClientViewModel(IDialogs dialogs)
    {
        _dialogs = dialogs;
        _serverPath = Path.Combine(AppContext.BaseDirectory, "Server", "LowSharp.Server.exe");
        _checkTimer = new DispatcherTimer()
        {
            IsEnabled = true,
            Interval = TimeSpan.FromSeconds(2),
        };
        _checkTimer.Tick += CheckStatus;
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _checkTimer.Stop();
        _channel?.Dispose();
        _channel = null;
        Stop();
        _disposed = true;
    }

    private const int Port = 11483;

    private void CheckStatus(object? sender, EventArgs e)
    {
        if (IsBusy)
            return;

        IsRunning = IsPortInUse(Port);

        if (!IsRunning)
        {
            IsConnected = false;
            _channel?.Dispose();
            _channel = null;
            return;
        }

        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private static bool IsPortInUse(int port)
    {
        IPGlobalProperties ipGP = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] endpoints = ipGP.GetActiveTcpListeners();
        if (endpoints == null || endpoints.Length == 0) return false;
        for (int i = 0; i < endpoints.Length; i++)
        {
            if (endpoints[i].Port == port)
            {
                return true;
            }
        }
        return false;
    }

    private void ThrowIfCantContinue()
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("The server is not running");
        }
        if (_channel == null)
        {
            throw new InvalidOperationException("Not connected to the server");
        }
    }

    private static async Task WaitTillStartedOrTimeout(string file, TimeSpan timeSpan)
    {
        const int decrement = 250;
        double totalMilliseconds = timeSpan.TotalMilliseconds;
        do
        {
            var process = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(file));
            if (process.Length > 0)
            {
                return;
            }
            await Task.Delay(decrement);
            totalMilliseconds -= decrement;

        }
        while (totalMilliseconds > 0);

        throw new TimeoutException("Timed out waiting for server to start");
    }

    [RelayCommand]
    public async Task Start()
    {
        if (!File.Exists(_serverPath))
        {
            await _dialogs.Error("Missing server", "The server executable can't be found");
            return;
        }

        try
        {
            IsBusy = true;
            Process.Start(new ProcessStartInfo()
            {
                FileName = _serverPath,
                WorkingDirectory = Path.GetDirectoryName(_serverPath) ?? AppContext.BaseDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            await WaitTillStartedOrTimeout(_serverPath, TimeSpan.FromSeconds(5));
            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            await _dialogs.Error("Failed to start server", ex.Message);
        }
    }

    [RelayCommand]
    public void Stop()
    {
        _channel?.Dispose();
        _channel = null;

        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsRunning)
            return;

        foreach (var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(_serverPath)))
        {
            proc.Kill();
        }
    }

    [RelayCommand]
    public async Task Connect()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsConnected || _channel != null)
            return;

        _channel = GrpcChannel.ForAddress($"http://localhost:{Port}", new GrpcChannelOptions
        {
            UnsafeUseInsecureChannelCallCredentials = true,
        });

        bool isOk = await DoHealthCheck();

        IsConnected = isOk;
    }

    public async Task<bool> DoHealthCheck()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ThrowIfCantContinue();

        var client = new LowSharp.ApiV1.HealthCheck.Health.HealthClient(_channel);
        try
        {
            int number1 = Random.Shared.Next();
            int number2 = Random.Shared.Next();

            long expectedSum = (long)number1 + (long)number2;

            IsBusy = true;
            var response = await client.CheckAsync(new LowSharp.ApiV1.HealthCheck.HealthCheckRequest
            {
                Number1 = number1,
                Number2 = number2,
            });

            return response.Sum == expectedSum;
        }
        catch
        {
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<GetComponentVersionsRespnse> GetComponentVersions()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ThrowIfCantContinue();

        var client = new LowSharp.ApiV1.HealthCheck.Health.HealthClient(_channel);
        try
        {
            IsBusy = true;
            return await client.GetComponentVersionsAsync(new GetComponentVersionsRequest());
        }
        catch
        {
            return new GetComponentVersionsRespnse();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<LowSharp.ApiV1.Lowering.LoweringResponse> LowerCodeAsync(string code,
                                                                               LowSharp.ApiV1.Lowering.InputLanguage inputLanguage,
                                                                               LowSharp.ApiV1.Lowering.Optimization optimization,
                                                                               LowSharp.ApiV1.Lowering.OutputCodeType outputCodeType)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ThrowIfCantContinue();

        var client = new LowSharp.ApiV1.Lowering.Lowerer.LowererClient(_channel);

        try
        {
            IsBusy = true;
            var result = await client.ToLowerCodeAsync(new LowSharp.ApiV1.Lowering.LoweringRequest()
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
            return new LowSharp.ApiV1.Lowering.LoweringResponse()
            {
                ResultCode = string.Empty,
                Diagnostics =
                {
                    new LowSharp.ApiV1.Lowering.Diagnostic()
                    {
                        Severity = LowSharp.ApiV1.Lowering.DiagnosticSeverity.Error,
                        Message = $"Failed to lower code: {ex.Message}"
                    }
                }
            };
        }
        finally
        {
            IsBusy = false;
        }
    }
}
