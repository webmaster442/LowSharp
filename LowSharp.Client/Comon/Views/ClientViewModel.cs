using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Grpc.Net.Client;

namespace LowSharp.Client.Comon.Views;


internal sealed partial class ClientViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherTimer _checkTimer;
    private readonly IDialogs _dialogs;
    private readonly string _serverPath;
    private GrpcChannel? _channel;
    private bool _disposed;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

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
        if (IsBusy || IsConnected)
            return;

        IsRunning = IsPortInUse(Port);

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
    public void Connect()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsConnected || _channel != null)
            return;

        _channel = GrpcChannel.ForAddress($"http://localhost:{Port}", new GrpcChannelOptions
        {
            UnsafeUseInsecureChannelCallCredentials = true,
        });
    }
}
