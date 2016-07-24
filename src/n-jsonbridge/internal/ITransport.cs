using N.Package.Events;

namespace N.Package.JsonBridge.Internal
{
    /// <summary>
    /// ITransport abstracts the transport implementation for testing.
    /// </summary>
    public interface ITransport
    {
        EventHandler EventHandler { get; set; }

        void Connect(string host, int port);

        void Write<T>(T target);
    }
}