using System.Collections;
using System.Collections.Specialized;

namespace LowSharp.Client.Common;

internal class ObservableList<T> : IList<T>, IReadOnlyList<T>, INotifyCollectionChanged
{
    private readonly List<T> _items;
    private bool _notificationsBlocked;

    public ObservableList()
    {
        _items = new List<T>();
        _notificationsBlocked = false;
    }

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public void Add(T item)
    {
        _items.Add(item);
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
    }

    public void AddRange(IEnumerable<T> items)
    {
        _items.AddRange(items);
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items)));
    }

    public void ReplaceAll(IEnumerable<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void Clear()
    {
        _items.Clear();
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(T item)
        => _items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
        => _items.CopyTo(array, arrayIndex);

    public int IndexOf(T item)
        => _items.IndexOf(item);

    public void Insert(int index, T item)
    {
        _items.Insert(index, item);
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }

    public bool Remove(T item)
    {
        int index = _items.IndexOf(item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            if (!_notificationsBlocked)
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        T item = _items[index];
        _items.RemoveAt(index);
        if (!_notificationsBlocked)
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
    }
    public IEnumerator<T> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _items.GetEnumerator();

    public void BlockNotifications()
        => _notificationsBlocked = true;

    public void UnblockAndFireNotifications()
    {
        _notificationsBlocked = false;
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
