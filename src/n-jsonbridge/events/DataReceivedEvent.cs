using N.Package.Events;
using UnityEngine;

namespace N.Package.JsonBridge
{
  public class DataReceivedEvent : INetworkEvent
  {
    private readonly string _raw;

    public DataReceivedEvent(string raw)
    {
      _raw = raw;
    }

    /// Attempt to deserialize the internal data as a T.
    public T Read<T>()
    {
      return JsonUtility.FromJson<T>(_raw);
    }
  }
}