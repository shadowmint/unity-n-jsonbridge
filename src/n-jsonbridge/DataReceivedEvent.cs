using N.Package.Events;
using UnityEngine;

public class DataReceivedEvent : IEvent
{
    public IEventApi Api { get; set; }

    private readonly string _raw;

    public DataReceivedEvent(string raw)
    {
        _raw = raw;
    }

    /// <summary>
    /// Attempt to deserialize the internal data as a T.
    /// </summary>
    public T Read<T>()
    {
        return JsonUtility.FromJson<T>(_raw);
    }
}