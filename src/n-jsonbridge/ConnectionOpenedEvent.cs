using N.Package.Events;

namespace N.Package.JsonBridge
{
    public class ConnectionOpenedEvent : IEvent
    {
        public IEventApi Api { get; set; }
    }
}