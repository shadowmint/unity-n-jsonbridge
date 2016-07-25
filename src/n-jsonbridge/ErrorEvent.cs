using System;
using N.Package.Events;

namespace N.Package.JsonBridge
{
    public class ErrorEvent : IEvent
    {
        public IEventApi Api { get; set; }

        public Exception Exception;
    }
}