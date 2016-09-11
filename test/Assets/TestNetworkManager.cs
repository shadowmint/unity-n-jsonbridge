using N.Package.JsonBridge;

public class NetworkTestMessage
{
  public string Message;
}

public class TestNetworkManager : NetworkManagerBase
{
  public bool dispatchHelloMessage = false;

  public new void Update()
  {
    base.Update();
    if (!dispatchHelloMessage) return;
    dispatchHelloMessage = false;
    Write(new NetworkTestMessage() {Message = "Hello World"});
  }

  public override void OnConnected(ConnectionOpenedEvent e)
  {
    base.OnConnected(e);
    dispatchHelloMessage = true;
  }

  public override void OnData(DataReceivedEvent e)
  {
    base.OnData(e);
    var message = e.Read<NetworkTestMessage>();
    if (message != null)
    {
      _.Log("Incoming message: {0}", message.Message);
    }
  }
}