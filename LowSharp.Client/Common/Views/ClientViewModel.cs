using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


using LowSharp.ClientLib;

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

    public IClient Client { get; }

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
        Client = new ClientLib.Client();
        Client.IsBusyChanged += OnIsBusyChanged;
        Client.IsConnectedChanged += OnIsConnectedChanged;
    }

    private void OnIsConnectedChanged(object? sender, EventArgs e)
        => IsConnected = Client.IsConnected;

    private void OnIsBusyChanged(object? sender, EventArgs e)
        => IsBusy = Client.IsBusy;

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Stop();
        _checkTimer.Stop();
        Client.IsBusyChanged -= OnIsBusyChanged;
        Client.IsConnectedChanged -= OnIsConnectedChanged;
        Client.Dispose();
        _disposed = true;
    }

    private const int Port = 11483;
    private const int HttpPort = 11484;

    private void CheckStatus(object? sender, EventArgs e)
    {
        if (IsBusy)
            return;

        IsRunning = IsPortInUse(Port);

        if (!IsRunning)
        {
            IsConnected = false;
            Client.CloseConnection();
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
        ObjectDisposedException.ThrowIf(_disposed, this);
        Client.CloseConnection();
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

        if (IsConnected)
            return;

        var result = await Client.Connect("http://localhost", Port, HttpPort);

        if (result.TryGetFailure(out Exception? ex))
        {
            await _dialogs.Error("Connection failed", ex.Message);
            return;
        }

        if (result.TryGetSuccess(out bool success) && !success)
        {
            await _dialogs.Error("Connection failed", "Unknown error");
            return;
        }

        IsConnected = success;
    }

    [RelayCommand]
    public async Task StartAndConnect()
    {
        await Start();
        IsRunning = true;
        await Connect();
    }

    [RelayCommand]
    public async Task InvalidateCache()
    {
        if (await _dialogs.Confirm("Confirm", "Invalidate server side cache?"))
        {
            await Client.HealtCheck.InvalidateCacheAsync();
        }
    }

    public void HideIsBusy()
    {
        IsBusy = false;
    }
}
