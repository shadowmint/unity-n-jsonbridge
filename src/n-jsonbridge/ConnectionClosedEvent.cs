using N.Package.Events;

public class ConnectionClosedEvent : IEvent
{
    public IEventApi Api { get; set; }
}