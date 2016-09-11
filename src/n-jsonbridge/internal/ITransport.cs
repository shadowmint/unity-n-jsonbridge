using N.Package.Events;

namespace N.Package.JsonBridge.Internal
{
  /// ITransport abstracts the transport implementation for testing.
  public interface ITransport
  {
    /// The event handler for making callbacks on.
    EventHandler EventHandler { get; set; }

    /// Return true if the transport is not doing anything and not connected.
    bool Idle { get; }

    /// Make an out going connection
    void Connect(string host, int port);

    /// Write the instance of T to the stream.
    void Write<T>(T target);

    /// Close the connection.
    void Close();
  }
}