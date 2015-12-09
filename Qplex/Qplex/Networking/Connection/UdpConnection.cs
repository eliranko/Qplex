using System;
using System.Net;
using System.Net.Sockets;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages.Networking.Connection;
using Qplex.Networking.Connection.Adapters.Udp;

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
        private readonly IUdpClient _udpClient;

        /// <summary>
        /// Connection's ip
        /// </summary>
        private readonly IPAddress _ip;

        /// <summary>
        /// Connection's port
        /// </summary>
        private readonly int _port;

        private IPEndPoint _endPoint;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="udpClient">Client</param>
        public UdpConnection(IUdpClient udpClient)
        {
            _ip = udpClient.Ip;
            _port = udpClient.Port;
            _endPoint = new IPEndPoint(_ip, _port);
            _udpClient = udpClient;
        }

        /// <summary>
        /// Connect and start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public ConnectionConnectStatus ConnectAndReceive()
        {
            Log(LogLevel.Trace, $"Udp client trying to set default remote host to {_ip}:{_port}");
            try
            {
                _udpClient.Connect(_endPoint);
                Log(LogLevel.Debug, $"Udp client set remote host {_ip}:{_port} successfully");
                BeginReceiveMessage();
                return ConnectionConnectStatus.Success;
            }
            catch (SocketException)
            {
                Log(LogLevel.Error, $"SocketException thrown when tried to set default remote {_ip}:{_port}");
                return ConnectionConnectStatus.SocketError;
            }
            catch (ArgumentNullException)
            {
                Log(LogLevel.Error, $"ArgumentNullException thrown when tried to set default remote {_ip}:{_port}");
                return ConnectionConnectStatus.NullAddress;
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException thrown when tried to set default remote {_ip}:{_port}");
                return ConnectionConnectStatus.ClientDisposed;
            }
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
                var bytes = _udpClient.EndReceive(asyncResult, ref _endPoint);
                Log(LogLevel.Debug, $"Received buffer of length: {bytes.Length}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Success, bytes));
            }
            catch (ArgumentNullException)
            {
                Log(LogLevel.Error, $"ArgumentNullException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null));
            }
            catch (ArgumentException)
            {
                Log(LogLevel.Error, $"ArgumentException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null));
            }
            catch (SocketException)
            {
                Log(LogLevel.Error, $"SocketException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.SocketClosed, null));
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.ClientDisposed, null));
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Error, $"InvalidOperationException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Error, null));
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
