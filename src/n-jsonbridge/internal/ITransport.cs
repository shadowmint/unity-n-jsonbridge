using N.Package.Events;

namespace N.Package.JsonBridge.Internal
{
    /// <summary>
    /// ITransport abstracts the transport implementation for testing.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// The event handler for making callbacks on.
        /// </summary>
        EventHandler EventHandler { get; set; }

        /// <summary>
        /// Return true if the transport is not doing anything and not connected.
        /// </summary>
        bool Idle { get; }

        /// <summary>
        /// Make an out going connection
        /// </summary>
        void Connect(string host, int port);

        /// <summary>
        /// Write the instance of T to the stream.
        /// </summary>
        void Write<T>(T target);

        /// <summary>
        /// Close the connection.
        /// </summary>
        void Close();
    }
}