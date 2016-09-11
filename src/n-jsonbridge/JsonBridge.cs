using System;
using System.Collections.Generic;
using N.Package.JsonBridge.Internal;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.JsonBridge
{
  /// JsonBridge is a high level construct to connect to a JSON-over-TCP server directly
  /// using the native C# communication framework.
  public class JsonBridge
  {
    private readonly ITransport _transport;
    private readonly EventHandler _eventHandler;
    private readonly EventHandler _internalHandler;
    private readonly Queue<INetworkEvent> _eventCache;

    public JsonBridge(ITransport transport)
    {
      _eventHandler = new EventHandler();
      _internalHandler = new EventHandler();
      _transport = transport;
      _transport.EventHandler = _internalHandler;
      _eventCache = new Queue<INetworkEvent>();

      // Setup internal event batching
      _internalHandler.AddEventHandler<ConnectionOpenedEvent>(ep => _eventCache.Enqueue(ep));
      _internalHandler.AddEventHandler<ConnectionClosedEvent>(ep => _eventCache.Enqueue(ep));
      _internalHandler.AddEventHandler<DataReceivedEvent>(ep => _eventCache.Enqueue(ep));
    }

    /// Check if the transport is busy or not.
    public bool Idle
    {
      get { return _transport.Idle; }
    }

    /// Make an out going connection to this given host on the given port.
    /// There is no return value on this call because all returns are done via callback.
    public void Connect(string host, int port)
    {
      _transport.Connect(host, port);
    }

    /// Update internal state, and dispatch any pending events.
    public void Update()
    {
      foreach (var queuedEvent in _eventCache)
      {
        _eventHandler.Trigger(queuedEvent);
      }
      _eventCache.Clear();
    }

    /// Add an event handler for a supported event type.
    public void AddEventHandler<T>(Action<T> action) where T : class, INetworkEvent
    {
      _eventHandler.AddEventHandler(action);
    }

    /// Convert the instance of T to json and write it to the transport layer.
    public void Write<T>(T target)
    {
      _transport.Write(target);
    }

    /// Close the connection if it's active.
    public void Close()
    {
      _transport.Close();
    }
  }
}