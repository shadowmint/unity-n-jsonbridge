using N.Package.Events;

public class ConnectionOpenedEvent : IEvent
{
    public IEventApi Api { get; set; }
}