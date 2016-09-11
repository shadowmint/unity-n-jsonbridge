using System;
using N.Package.Events;

namespace N.Package.JsonBridge
{
  public class ErrorEvent : INetworkEvent
  {
    public Exception Exception;
  }
}