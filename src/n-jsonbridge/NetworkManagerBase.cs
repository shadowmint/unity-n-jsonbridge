using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N.Package.JsonBridge.Internal;
using UnityEngine;

namespace N.Package.JsonBridge
{
    public class NetworkManagerBase : MonoBehaviour
    {
        [Tooltip("The host to open a connection to")] public string Host = "127.0.0.1";

        [Tooltip("The port to open a connection to")] public int Port = 50000;

        [Tooltip("Open a connection if possible?")] public bool Connect = false;

        [Tooltip("Debug messages?")] public bool Debug = false;

        public bool runUpdate = false;

        private JsonBridge _bridge;

        public void Awake()
        {
            _bridge = new JsonBridge(new TcpTransport());
            _bridge.AddEventHandler<ConnectionOpenedEvent>(OnConnected);
            _bridge.AddEventHandler<ConnectionClosedEvent>(OnDisconnected);
            _bridge.AddEventHandler<DataReceivedEvent>(OnData);
            _bridge.AddEventHandler<ErrorEvent>(OnError);
        }

        public void Update()
        {
            if (runUpdate)
            {
                //runUpdate = false;
                if (Connect && _bridge.Idle)
                {
                    _bridge.Connect(Host, Port);
                }
                _bridge.Update();
            }
        }

        protected void Write<T>(T target)
        {
            _bridge.Write(target);
        }

        public virtual void OnConnected(ConnectionOpenedEvent e)
        {
            if (Debug)
            {
                _.Log(e);
            }
        }

        public virtual void OnDisconnected(ConnectionClosedEvent e)
        {
            if (Debug)
            {
                _.Log(e);
            }
        }

        public virtual void OnData(DataReceivedEvent e)
        {
            if (Debug)
            {
                _.Log(e);
            }
        }

        public virtual void OnError(ErrorEvent e)
        {
            if (Debug)
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