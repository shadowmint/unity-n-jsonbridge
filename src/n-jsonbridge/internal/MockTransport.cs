using N.Package.Events;
using UnityEngine;

namespace N.Package.JsonBridge.Internal
{
  /// Mock testing transport implementation
  public class MockTransport : ITransport
  {
    public EventHandler EventHandler { get; set; }
    public bool Idle { get; set; }

    public void Connect(string host, int port)
    {
      Idle = false;
    }

    public void Write<T>(T target)
    {
    }

    public void Close()
    {
      Idle = true;
    }

    public void TriggerConnection()
    {
      EventHandler.Trigger(new ConnectionOpenedEvent());
    }

    public void TriggerDisconnect()
    {
      EventHandler.Trigger(new ConnectionClosedEvent());
    }

    public void TriggerData<T>(T message)
    {
      var raw = JsonUtility.ToJson(message);
      EventHandler.Trigger(new DataReceivedEvent(raw));
    }
  }
}