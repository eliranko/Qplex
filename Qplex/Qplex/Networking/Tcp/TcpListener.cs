using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Qplex.FramingAlgorithms;
using Qplex.MessageFactories;
using Qplex.Messages.Networking;

namespace Qplex.Networking.Tcp
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
        /// Header size
        /// </summary>
        private readonly int _headerSize;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <param name="port">Port</param>
        /// <param name="headerSize">Header size</param>
        public TcpListener(IPAddress ip, int port, int headerSize = 4)
        {
            _tcpListener = new TcpListener(ip, port);
            _stopListening = false;
            _tcpClientConnectedEvent = new ManualResetEvent(false);
            _headerSize = headerSize;
        }

        /// <summary>
        /// Stop listener thread
        /// </summary>
        public override void Stop()
        {
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
                //TODO: Log
                //Reset previous connected client signal
                _tcpClientConnectedEvent.Reset();
                _tcpListener.BeginAcceptTcpClient(AcceptTcpClient, _tcpListener);

                //Wait for a connection
                _tcpClientConnectedEvent.WaitOne();
            }
        }

        /// <summary>
        /// Async callback for accepted tcp clients
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void AcceptTcpClient(IAsyncResult asyncResult)
        {
// ReSharper disable once PossibleNullReferenceException
            var tcpClient = (asyncResult.AsyncState as TcpListener).EndAcceptTcpClient(asyncResult);
            Broadcast(new NewConnectionMessage(new Agent(new Parser(
                new TcpConnection(tcpClient,_headerSize), new T(), new TU()))));
            _tcpClientConnectedEvent.Set();
        }
    }
}
