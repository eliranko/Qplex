using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;
using Qplex.Messages.Networking;
using Qplex.Networking.Connection;
using Qplex.Networking.Connection.Adapters;
using Qplex.Networking.FramingAlgorithms;
using Qplex.Networking.MessageFactories;
using Qplex.Networking.Parsers;

namespace Qplex.Networking.Listeners
{
    /// <summary>
    /// Tcp listener
    /// </summary>
    public class TcpListener<T, TU> : Listener where T : IMessageFactory, new() where TU : IFramingAlgorithm, new()
    {
        /// <summary>
        /// Tcp listener
        /// </summary>
        private readonly TcpListener _tcpListener;

        /// <summary>
        /// Indicates to stop listening
        /// </summary>
        private bool _stopListening;

        /// <summary>
        /// Tcp client conncted event
        /// </summary>
        private readonly ManualResetEvent _tcpClientConnectedEvent;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <param name="port">Port</param>
        public TcpListener(IPAddress ip, int port)
        {
            _tcpListener = new TcpListener(ip, port);
            _stopListening = false;
            _tcpClientConnectedEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Stop listener thread
        /// </summary>
        public override void Stop()
        {
            Log(LogLevel.Debug, "Stopping...");
            _stopListening = true;
            //Signal the listener thread to continue without a client connection, inorder to exit.
            _tcpClientConnectedEvent.Set();
            ListeningThread.Join();
            _tcpListener.Stop();
        }

        /// <summary>
        /// Listener thread
        /// </summary>
        protected override void Listen()
        {
            _tcpListener.Start();

            while (!_stopListening)
            {
                //Reset previous connected client signal
                _tcpClientConnectedEvent.Reset();
                _tcpListener.BeginAcceptTcpClient(AcceptTcpClient, _tcpListener);

                Log(LogLevel.Debug, "Waiting for connection on " +
                                    $"{IPAddress.Parse(((IPEndPoint)_tcpListener.LocalEndpoint).Address.ToString())}" +
                                    ":" +
                                    $"{((IPEndPoint)_tcpListener.LocalEndpoint).Port}");
                _tcpClientConnectedEvent.WaitOne();
            }
        }

        /// <summary>
        /// Async callback for accepted tcp clients
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void AcceptTcpClient(IAsyncResult asyncResult)
        {
            Log(LogLevel.Debug, "Accepted connection on " +
                                    $"{IPAddress.Parse(((IPEndPoint)_tcpListener.LocalEndpoint).Address.ToString())}" +
                                    ":" +
                                    $"{((IPEndPoint)_tcpListener.LocalEndpoint).Port}");
            // ReSharper disable once PossibleNullReferenceException
            var tcpClient = (asyncResult.AsyncState as TcpListener).EndAcceptTcpClient(asyncResult);
            Broadcast(new NewConnectionMessage(new Parser(
                new TcpConnection(new TcpClientAdaptee(tcpClient)), new T(), new TU())));
            _tcpClientConnectedEvent.Set();
        }
    }
}
