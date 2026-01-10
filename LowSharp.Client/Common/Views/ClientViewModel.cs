using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Grpc.Net.Client;

using LowSharp.ApiV1.Evaluate;

namespace LowSharp.Client.Common.Views;

internal sealed partial class ClientViewModel :
    ObservableObject,
    IDisposable
{
    private readonly DispatcherTimer _checkTimer;
    private readonly IDialogs _dialogs;
    private readonly string _serverPath;
    private bool _disposed;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    partial void OnIsConnectedChanged(bool oldValue, bool newValue)
        => WeakReferenceMessenger.Default.Send(new Messages.IsConnectedChanged(newValue));

    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public GrpcChannel? Channel { get; private set; }

    public IClient ApiClient { get; }

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
        ApiClient = new ClientFunctions(this, dialogs);
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _checkTimer.Stop();
        Channel?.Dispose();
        Channel = null;
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
            Channel?.Dispose();
            Channel = null;
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

    public void ThrowIfCantContinue()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsRunning)
        {
            throw new InvalidOperationException("The server is not running");
        }
        if (Channel == null)
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
        Channel?.Dispose();
        Channel = null;

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

        if (IsConnected || Channel != null)
            return;

        Channel = GrpcChannel.ForAddress($"http://localhost:{Port}", new GrpcChannelOptions
        {
            UnsafeUseInsecureChannelCallCredentials = true,
        });

        bool isOk = await ApiClient.DoHealthCheck();

        IsConnected = isOk;
    }

    [RelayCommand]
    public async Task StartAndConnect()
    {
        await Start();
        IsRunning = true;
        await Connect();
    }

    public void HideIsBusy()
    {
        IsBusy = false;
    }
}
