using System.Collections;
using System.Collections.Concurrent;

using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Interactive;

public class SessionManager : IEnumerable<(Guid sessionId, DateTimeOffset lastaccessUtc)>
{
    private readonly ConcurrentDictionary<Guid, Session> _sessions;
    private readonly TimeProvider _timeProvider;

    public SessionManager(TimeProvider timeProvider)
    {
        _sessions = new ConcurrentDictionary<Guid, Session>();
        _timeProvider = timeProvider;
    }

    public ScriptState? GetSessionState(Guid sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            return session.ScriptState;
        }
        return null;
    }

    public void Update(Guid sessionId, ScriptState newState)
    {
        _sessions[sessionId] = new Session
        {
            LastAccessUtc = _timeProvider.GetUtcNow(),
            ScriptState = newState
        };
    }

    public void Create(Guid sessionId, ScriptState initialState)
    {
        var session = new Session
        {
            LastAccessUtc = _timeProvider.GetUtcNow(),
            ScriptState = initialState
        };
        _sessions[sessionId] = session;
    }

    public void Remove(Guid sessionId)
    {
        _sessions.TryRemove(sessionId, out _);
    }

    public IEnumerator<(Guid sessionId, DateTimeOffset lastaccessUtc)> GetEnumerator()
    {
        foreach (var kvp in _sessions)
        {
            yield return (kvp.Key, kvp.Value.LastAccessUtc);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
