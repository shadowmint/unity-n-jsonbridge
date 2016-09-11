using N.Package.JsonBridge.Internal;
using UnityEngine;

namespace N.Package.JsonBridge
{
  [System.Serializable]
  public class NetworkManagerStatus
  {
    [Tooltip("Currently connected? (read-only)")] public bool Connected;

    [Tooltip("Count of sent messages (read-only)")] public int Sent;

    [Tooltip("Count of received messages (read-only)")] public int Received;

    [Tooltip("Count of errors (read-only)")] public int Errors;
  }

  [System.Serializable]
  public class NetworkManagerProfile
  {
    [Tooltip("The host to open a connection to")] public string Host = "127.0.0.1";

    [Tooltip("The port to open a connection to")] public int Port = 50000;

    [Tooltip("Show debug messages?")] public bool Debug = false;

    [Tooltip("Reconnect after this long if idle?")] public float ReconnectInterval = 1f;
  }

  public class NetworkManagerBase : MonoBehaviour
  {
    [Tooltip("Open a connection if possible?")] public bool Connect = false;

    public NetworkManagerProfile Profile;

    public NetworkManagerStatus Status;

    private JsonBridge _bridge;
    private float _elapsed;

    public void Awake()
    {
      _bridge = new JsonBridge(new TcpTransport());
      _bridge.AddEventHandler<ConnectionOpenedEvent>(OnConnectedInternal);
      _bridge.AddEventHandler<ConnectionClosedEvent>(OnDisconnectedInternal);
      _bridge.AddEventHandler<DataReceivedEvent>(OnDataInternal);
      _bridge.AddEventHandler<ErrorEvent>(OnErrorInternal);
    }

    public void Update()
    {
      if (Connect && _bridge.Idle)
      {
        if (_elapsed > Profile.ReconnectInterval)
        {
          _bridge.Connect(Profile.Host, Profile.Port);
          _elapsed = 0f;
        }
        else
        {
          _elapsed += Time.deltaTime;
        }
      }
      if (!Connect && !_bridge.Idle)
      {
        _bridge.Close();
      }
      _bridge.Update();
    }

    protected void Write<T>(T target)
    {
      Status.Sent += 1;
      _bridge.Write(target);
    }

    private void OnConnectedInternal(ConnectionOpenedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
      Status.Connected = true;
      OnConnected(e);
    }

    private void OnDisconnectedInternal(ConnectionClosedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
      Status.Connected = false;
      OnDisconnected(e);
    }

    private void OnDataInternal(DataReceivedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
      Status.Received += 1;
      OnData(e);
    }

    private void OnErrorInternal(ErrorEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
      Status.Errors += 1;
      OnError(e);
    }

    public virtual void OnConnected(ConnectionOpenedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
    }

    public virtual void OnDisconnected(ConnectionClosedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
    }

    public virtual void OnData(DataReceivedEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
    }

    public virtual void OnError(ErrorEvent e)
    {
      if (Profile.Debug)
      {
        _.Log(e);
      }
    }

    public void OnApplicationQuit()
    {
      if (!_bridge.Idle)
      {
        _bridge.Close();
      }
    }
  }
}