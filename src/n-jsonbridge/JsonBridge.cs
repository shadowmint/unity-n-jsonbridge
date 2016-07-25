using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using N.Package.Events;
using N.Package.JsonBridge.Internal;

namespace N.Package.JsonBridge
{
    /// <summary>
    /// JsonBridge is a high level construct to connect to a JSON-over-TCP server directly
    /// using the native C# communication framework.
    /// </summary>
    public class JsonBridge
    {
        private readonly ITransport _transport;
        private readonly EventHandler _eventHandler;
        private readonly EventHandler _internalHandler;
        private Queue<IEvent> _eventCache;

        public JsonBridge(ITransport transport)
        {
            _eventHandler = new EventHandler();
            _internalHandler = new EventHandler();
            _transport = transport;
            _transport.EventHandler = _internalHandler;
            _eventCache = new Queue<IEvent>();

            // Setup internal event batching
            _internalHandler.AddEventHandler<ConnectionOpenedEvent>((ep) => _eventCache.Enqueue(ep));
            _internalHandler.AddEventHandler<ConnectionClosedEvent>((ep) => _eventCache.Enqueue(ep));
            _internalHandler.AddEventHandler<DataReceivedEvent>((ep) => _eventCache.Enqueue(ep));
        }

        /// <summary>
        /// Check if the transport is busy or not.
        /// </summary>
        public bool Idle
        {
            get { return _transport.Idle; }
        }

        /// <summary>
        /// Make an out going connection to this given host on the given port.
        /// There is no return value on this call because all returns are done via callback.
        /// </summary>
        public void Connect(string host, int port)
        {
            _transport.Connect(host, port);
        }

        /// <summary>
        /// Update internal state, and dispatch any pending events.
        /// </summary>
        public void Update()
        {
            foreach (var queuedEvent in _eventCache)
            {
                _eventHandler.Trigger(queuedEvent);
            }
            _eventCache.Clear();
        }

        /// <summary>
        /// Add an event handler for a supported event type.
        /// </summary>
        public void AddEventHandler<T>(EventHandler<T> action) where T : class, IEvent
        {
            _eventHandler.AddEventHandler(action);
        }
        
        /// <summary>
        /// Convert the instance of T to json and write it to the transport layer.
        /// </summary>
        public void Write<T>(T target)
        {
            _transport.Write(target);
        }

        /// <summary>
        /// Close the connection if it's active.
        /// </summary>
        public void Close()
        {
            _transport.Close();
        }
    }
}