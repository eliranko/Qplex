using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages.Networking.Connection;
using Qplex.Networking.Connection.Adapters.Tcp;

namespace Qplex.Networking.Connection
{
    /// <summary>
    /// Tcp connection
    /// </summary>
    public class TcpConnection : Broadcaster, IConnection
    {
        /// <summary>
        /// Header size
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Tcp client
        /// </summary>
        private readonly ITcpClient _tcpClient;

        /// <summary>
        /// Ip
        /// </summary>
        private readonly IPAddress _ip;

        /// <summary>
        /// Port
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Header array
        /// </summary>
        private byte[] _headerArray;

        /// <summary>
        /// Message array
        /// </summary>
        private byte[] _bufferArray;

        /// <summary>
        /// Constructor used by listener
        /// </summary>
        /// <param name="tcpClient">Tcp client</param>
        public TcpConnection(ITcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _ip = _tcpClient.Ip;
            _port = _tcpClient.Port;
        }

        /// <summary>
        /// Constructor used by client
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <param name="port">Port</param>
        public TcpConnection(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            _tcpClient = new TcpClientAdaptee();
        }

        /// <summary>
        /// Connect and start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public ConnectionConnectStatus ConnectAndReceive()
        {
            Log(LogLevel.Trace, $"Tcp client trying to connect to {_ip}:{_port}");
            if (_tcpClient.Connected)
            {
                Log(LogLevel.Warn, "Tried to connect while tcp client is already connected");
                return ConnectionConnectStatus.SocketAlreadyConnected;
            }

            try
            {
                _tcpClient.Connect(_ip, _port);
                Log(LogLevel.Debug, $"Connected successfully to {_ip}:{_port}");
                BeginReceiveMessage();
                return ConnectionConnectStatus.Success;
            }
            catch (ArgumentNullException)
            {
                Log(LogLevel.Error, $"ArgumentNullException thrown when tried to connect to {_ip}:{_port}");
                return ConnectionConnectStatus.NullAddress;
            }
            catch (ArgumentOutOfRangeException)
            {
                Log(LogLevel.Error, $"ArgumentOutOfRangeException thrown when tried to connect to {_ip}:{_port}");
                return ConnectionConnectStatus.InvalidPort;
            }
            catch (SocketException)
            {
                Log(LogLevel.Error, $"SocketException thrown when tried to connect to {_ip}:{_port}");
                return ConnectionConnectStatus.SocketError;
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException thrown when tried to connect to {_ip}:{_port}");
                return ConnectionConnectStatus.ClientDisposed;
            }
        }

        /// <summary>
        /// Closes connection
        /// </summary>
        public void Close()
        {
            Log(LogLevel.Debug, $"Stopping tcp client on {_ip}:{_port}");
            _tcpClient.Close();
        }

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        /// <param name="buffer">Buffer</param>
        public void Send(byte[] buffer)
        {
            if (_tcpClient.Connected)
            {
                Log(LogLevel.Trace, $"Begin async write of size {buffer.Length} to {_ip}:{_port}");
                _tcpClient.GetStream().BeginWrite(buffer, 0, buffer.Length, SendComplete, null);
            }
            else
            {
                Log(LogLevel.Error, "Tried to send buffer when socket is not connected");
            }
        }

        #region Async Callbacks
        /// <summary>
        /// Async sending completed
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void SendComplete(IAsyncResult asyncResult)
        {
            try
            {
                _tcpClient.GetStream().EndWrite(asyncResult);
                Log(LogLevel.Trace, $"Send complete on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.Success));
            }
            catch (IOException)
            {
                Log(LogLevel.Error, $"IOException resulted when ended write on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.SocketClosed));
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException resulted when ended write on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.ClientDisposed));
            }
            catch (InvalidOperationException)
            {
                Log(LogLevel.Error, $"InvalidOperationException resulted when ended write on {_ip}:{_port}");
                Broadcast(new ConnectionSendStatusMessage(ConnectionSocketStatus.SocketClosed));
            }
        }

        /// <summary>
        /// Received header
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void ReceivedHeader(IAsyncResult asyncResult)
        {
            //If array contains only zeros, receive another header
            if (_headerArray.All(b => b == 0))
            {
                Log(LogLevel.Warn, "Received an empty header");
                BeginReceiveMessage();
                return;
            }

            var bytesRead = EndAsyncReadResult(asyncResult);
            if(bytesRead < 0)
                return;

            var messageSize = ConvertLittleEndian(_headerArray);
            Log(LogLevel.Trace, $"Read {bytesRead} bytes, and Received header of size {messageSize}");
            _bufferArray = new byte[messageSize + HeaderSize];

            //Receive the message
            _tcpClient.GetStream().BeginRead(_bufferArray, HeaderSize, _bufferArray.Length - HeaderSize, ReceivedMessage, null);
        }

        /// <summary>
        /// Received message
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void ReceivedMessage(IAsyncResult asyncResult)
        {
            var bytesRead = EndAsyncReadResult(asyncResult);
            if (bytesRead < 0)
                return;

            Log(LogLevel.Trace, $"Receive message complete. Read {bytesRead} bytes.");
            _headerArray.CopyTo(_bufferArray, 0);
            Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Success, _bufferArray));

            BeginReceiveMessage();
        }
        #endregion

        #region Helpers

        private void BeginReceiveMessage()
        {
            Log(LogLevel.Trace, $"Begin receiving on {_ip}:{_port}");
            _headerArray = new byte[HeaderSize];
            _tcpClient.GetStream().BeginRead(_headerArray, 0, HeaderSize, ReceivedHeader, null);
        }

        /// <summary>
        /// Returns the number of bytes read if succeeded,
        /// otherwise broadcasts error in receiving, starts receiving new message and returns -1
        /// </summary>
        private int EndAsyncReadResult(IAsyncResult asyncResult)
        {
            var bytesRead = -1;
            try
            {
                bytesRead = _tcpClient.GetStream().EndRead(asyncResult);
            }
            catch (IOException)
            {
                Log(LogLevel.Error, $"IOException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.SocketClosed, null));
            }
            catch (ObjectDisposedException)
            {
                Log(LogLevel.Error, $"ObjectDisposedException when reading socket on {_ip}:{_port}");
                Broadcast(new ConnectionBufferReceivedMessage(ConnectionSocketStatus.ClientDisposed, null));
            }

            return bytesRead;
        }

        /// <summary>
        /// Convert arbitray size of byte array to uint
        /// </summary>
        private uint ConvertLittleEndian(byte[] array)
        {
            var pos = 0;
            uint result = 0;
            foreach (var by in array)
            {
                result |= (uint)(by << pos);
                pos += 8;
            }

            return result;
        }
        #endregion
    }
}
