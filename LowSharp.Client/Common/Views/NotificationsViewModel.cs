using System.Collections.Concurrent;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace LowSharp.Client.Common.Views;

internal sealed partial class NotificationsViewModel 
    : ObservableObject, IRecipient<Messages.Notification>
{
    private readonly ConcurrentQueue<Messages.Notification> _messages;
    private readonly DispatcherTimer _timer;
    private DateTime? _visibleTill;

    public NotificationsViewModel()
    {
        _messages = new ConcurrentQueue<Messages.Notification>();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1),
        };
        _timer.Tick += RunTimer;
        Latest = string.Empty;
        WeakReferenceMessenger.Default.Register(this);
    }

    [ObservableProperty]
    public partial string Latest { get; set; }

    public void Receive(Messages.Notification message)
    {
        if (_visibleTill is not null)
        {
            _messages.Enqueue(message);
            return;
        }
        Latest = message.Message;
        _visibleTill = DateTime.UtcNow + message.Validity;
    }

    private void RunTimer(object? sender, EventArgs e)
    {
        if (_visibleTill is null)
        {
            if (_messages.TryDequeue(out var nextMessage))
            {
                Latest = nextMessage.Message;
                _visibleTill = DateTime.UtcNow + nextMessage.Validity;
            }
        }
        else if (DateTime.UtcNow >= _visibleTill)
        {
            _visibleTill = null;
            Latest = string.Empty;
        }
    }
}
