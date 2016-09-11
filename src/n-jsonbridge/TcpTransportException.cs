namespace N.Package.JsonBridge
{
  public class TcpTransportException : System.Exception
  {
    public readonly TcpTransportErrors Error;

    public TcpTransportException(TcpTransportErrors error, string message) : base(message)
    {
      Error = error;
    }
  }
}