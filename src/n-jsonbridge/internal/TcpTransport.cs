using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.JsonBridge.Internal
{
    enum TcpTransportState
    {
        Idle,
        Connecting,
        Connected
    }

    class TcpTransport : ITransport
    {
        /// <summary>
        /// Only allow connections if not currently connected.
        /// </summary>
        private TcpTransportState _state = TcpTransportState.Idle;

        /// <summary>
        /// The currently active network thread, if any.
        /// </summary>
        private Thread _thread;

        private string _host;
        private int _port;
        private TcpClient _client;
        private NetworkStream _stream;
        private StreamWriter _writer;

        public EventHandler EventHandler { get; set; }

        public bool Idle
        {
            get { return _state == TcpTransportState.Idle; }
        }

        public void Connect(string host, int port)
        {
            lock (this)
            {
                if (_state == TcpTransportState.Idle)
                {
                    _host = host;
                    _port = port;
                    _state = TcpTransportState.Connecting;
                    _thread = new Thread(Connect);
                    _thread.Start();
                    return;
                }
            }
            throw new TcpTransportException(TcpTransportErrors.ConnectionBusy, "A connection is already in progress");
        }

        public void Write<T>(T target)
        {
            lock (this)
            {
                if (_state == TcpTransportState.Connected)
                {
                    var output = JsonUtility.ToJson(target);
                    try
                    {
                        _writer.WriteLine(output);
                        _writer.Flush();
                    }
                    catch (Exception error)
                    {
                        EventHandler.Trigger(new ErrorEvent() {Exception = error});
                    }
                    return;
                }
                throw new TcpTransportException(TcpTransportErrors.NotConencted, "No active connection");
            }
        }

        public void Close()
        {
            lock (this)
            {
                if (_state == TcpTransportState.Connected)
                {
                    _client.Close();
                    return;
                }
                throw new TcpTransportException(TcpTransportErrors.NotConencted, "No active connection");
            }
        }

        /// <summary>
        /// Open a connection to the remote host or dispatch an error event.
        /// </summary>
        private void Connect()
        {
            lock (this)
            {
                _client = new TcpClient();
                try
                {
                    _client.Connect(_host, _port);
                }
                catch (Exception error)
                {
                    EventHandler.Trigger(new ErrorEvent() {Exception = error});
                    _state = TcpTransportState.Idle;
                    return;
                }

                _state = TcpTransportState.Connected;
                EventHandler.Trigger(new ConnectionOpenedEvent());
            }

            _stream = _client.GetStream();
            _writer = new StreamWriter(_stream);
            ReadStreamData();
        }

        /// <summary>
        /// Block on the stream forever, reading data as soon as we can.
        /// </summary>
        private void ReadStreamData()
        {
            using (var streamReader = new StreamReader(_stream))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    lock (this)
                    {
                        EventHandler.Trigger(new DataReceivedEvent(line));
                    }
                }
            }
            lock (this)
            {
                _state = TcpTransportState.Idle;
                _writer = null;
                _stream = null;
                EventHandler.Trigger(new ConnectionClosedEvent());
            }
        }
    }
}