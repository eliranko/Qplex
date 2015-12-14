using System;
using System.Net;
using System.Net.Sockets;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages.Networking.Connection;

namespace Qplex.Networking.Connection
{
    /// <summary>
    /// Udp connection
    /// </summary>
    public class UdpConnection : Broadcaster, IConnection
    {
        /// <summary>
        /// Header size
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Udp client
        /// </summary>
        private readonly UdpClient _udpClient;

        /// <summary>
        /// Local end point
        /// </summary>
        private readonly IPEndPoint _localEndPoint;

        /// <summary>
        /// Connection's ip
        /// </summary>
        private readonly IPAddress _ip;

        /// <summary>
        /// Connection's port
        /// </summary>
        private readonly int _port;

        private IPEndPoint _refEndPoint;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="udpClient">Client</param>
        public UdpConnection(UdpClient udpClient)
        {
            _udpClient = udpClient;
            _localEndPoint = _udpClient.Client.LocalEndPoint as IPEndPoint;
            _ip = _localEndPoint.Address;
            _port = _localEndPoint.Port;
            _refEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        /// <summary>
        /// Connect and start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public ConnectionConnectStatus ConnectAndReceive()
        {
            BeginReceiveMessage();
            return ConnectionConnectStatus.Success;
        }

        /// <summary>
        /// Closes connection
        /// </summary>
        public void Close()
        {
            Log(LogLevel.Debug, $"Stopping udp client on {_ip}:{_port}");
            _udpClient.Close();
        }

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        public void Send(byte[] buffer)
        {
            Log(LogLevel.Trace, $"Begin async write of size {buffer.Length} to {_ip}:{_port}");
            try
            {
                _udpClient.Send(buffer, buffer.Length);
                Log(LogLevel.Trace, $"Sent {buffer.Length} bytes on to {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.Success));
            }
            catch (ArgumentNullException)
            {
                Log(LogLevel.Error, $"ArgumentNullException resulted when wrote on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.Error));
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException resulted when wrote on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.ClientDisposed));
            }
            catch (SocketException)
            {
                Log(LogLevel.Error, $"SocketException resulted when wrote on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.SocketClosed));
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Error, $"InvalidOperationException resulted when wrote on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.Error));
            }
        }

        #region AsyncCallbacks

        private void ReceivedMessage(IAsyncResult asyncResult)
        {
            Log(LogLevel.Trace, "Handling received message");
            try
            {
                var bytes = _udpClient.EndReceive(asyncResult, ref _refEndPoint);
                Log(LogLevel.Debug, $"Received buffer of length: {bytes.Length}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Success, bytes, _localEndPoint,
                    _refEndPoint));
            }
            catch (ArgumentNullException)
            {
                Log(LogLevel.Error, $"ArgumentNullException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null, null, null));
            }
            catch (ArgumentException)
            {
                Log(LogLevel.Error, $"ArgumentException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null, null, null));
            }
            catch (SocketException)
            {
                Log(LogLevel.Error, $"SocketException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.SocketClosed, null, null, null));
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.ClientDisposed, null, null, null));
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Error, $"InvalidOperationException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null, null, null));
            }
        }

        #endregion

        #region Helpers

        private void BeginReceiveMessage()
        {
            _udpClient.BeginReceive(ReceivedMessage, null);
        }

        #endregion
    }
}
