using Grpc.Net.Client;

namespace LowSharp.ClientLib;

public sealed class Client : IDisposable, IClientRoot, IClient
{
    private GrpcChannel? _channel;
    private bool _disposed;

    public Client()
    {
        HealtCheck = null!;
        Lowering = null!;
        Regex = null!;
        Examples = null!;
    }

    public IHealtCheckClient HealtCheck
    {
        get => IsConnected ? field : throw new InvalidOperationException("Client is not connected.");
        private set => field = value;
    }

    public ILoweringClient Lowering
    {
        get => IsConnected ? field : throw new InvalidOperationException("Client is not connected.");
        private set => field = value;
    }

    public IRegexClient Regex
    {
        get => IsConnected ? field : throw new InvalidOperationException("Client is not connected.");
        private set => field = value;
    }

    public IExamplesClient Examples
    {
        get => IsConnected ? field : throw new InvalidOperationException("Client is not connected.");
        private set => field = value;
    }

    public bool IsConnected
    {
        get => field;
        private set
        {
            field = value;
            IsConnectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsBusy
    {
        get => field;
        set
        {
            field = value;
            IsBusyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? IsBusyChanged;

    public event EventHandler? IsConnectedChanged;

    public async Task<Either<bool, Exception>> Connect(Uri server)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (IsConnected)
            throw new InvalidOperationException("Client is already connected.");
        try
        {
            _channel = GrpcChannel.ForAddress(server, new GrpcChannelOptions
            {
                UnsafeUseInsecureChannelCallCredentials = true,
            });

            HealtCheck = new HealtCheckClient(_channel, this);
            Lowering = new LoweringClient(_channel, this);
            Regex = new RegexClient(_channel, this);
            Examples = new ExamplesClient(_channel, this);

            IsConnected = true;

            var healthCeckResult = await HealtCheck.DoHealthCheckAsync();

            IsConnected = healthCeckResult.TryGetSuccess(out var success) && success;

            return healthCeckResult;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public void ThrowIfCantContinue()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsConnected)
            throw new InvalidOperationException("Client is not connected.");
    }

    public void CloseConnection()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _channel?.Dispose();
        _channel = null;
    }

    public void Dispose()
    {
        CloseConnection();
        _disposed = true;
    }
}
