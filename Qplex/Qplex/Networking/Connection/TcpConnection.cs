using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages.Networking;

namespace Qplex.Networking.Connection
{
    public class TcpConnection : Broadcaster, IConnection
    {
        /// <summary>
        /// Header size
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Tcp client
        /// </summary>
        private readonly TcpClient _tcpClient;

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
        public TcpConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
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
            _tcpClient = new TcpClient();
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            if (_tcpClient.Connected)
            {
                Log(LogLevel.Warn, "Tried to connect, when already connected");
            }
            else
            {
                Log(LogLevel.Info, "Begin async connection...");
                var asyncResult = _tcpClient.BeginConnect(_ip, _port, null, null);
                //TODO: Receive timeout from configuration
                var successConnection = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10000));
                if (!successConnection)
                {
                    throw new Exception("Tcp client failed to connect");
                }
                _tcpClient.EndConnect(asyncResult);
                Log(LogLevel.Debug, $"Connected successfully to {_ip}:{_port}");
            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            Log(LogLevel.Debug, "Closing connection...");
            _tcpClient.Close();
        }

        /// <summary>
        /// Start receiving message, and connect if not connected
        /// </summary>
        /// <returns>Operation status</returns>
        public bool Start()
        {
            if (!_tcpClient.Connected)
            {
                Connect();
            }

            ReceiveMessage();
            return true;
        }

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        /// <param name="buffer">Buffer</param>
        public void Send(byte[] buffer)
        {
            if (_tcpClient.Connected)
            {
                Log(LogLevel.Debug, $"Begin async write of size {buffer.Length} to {_ip}:{_port}");
                _tcpClient.GetStream().BeginWrite(buffer, 0, buffer.Length, SendComplete, null);
            }
            else
            {
                Log(LogLevel.Error, "Tried to send buffer when socket is not connected!");
            }
        }

        #region Async callback
        /// <summary>
        /// Async sending completed
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void SendComplete(IAsyncResult asyncResult)
        {
            Log(LogLevel.Debug, "Send complete");
            _tcpClient.GetStream().EndWrite(asyncResult);
            //TODO: Maybe broadcast message?
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
                Log(LogLevel.Warn, "Received empty header");
                ReceiveMessage();
                return;
            }

            var readBytes = _tcpClient.GetStream().EndRead(asyncResult);
            var messageSize = ConvertLittleEndian(_headerArray);
            Log(LogLevel.Debug, $"Read {readBytes} bytes, and Received header of size {messageSize}");
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
            var bytesRead = _tcpClient.GetStream().EndRead(asyncResult);
            Log(LogLevel.Debug, $"Receive message complete. Read {bytesRead} bytes.");
            _headerArray.CopyTo(_bufferArray, 0);
            Broadcast(new BufferReceivedMessage(_bufferArray));

            ReceiveMessage();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Receive message
        /// </summary>
        private void ReceiveMessage()
        {
            Log(LogLevel.Debug, "Waiting for header...");
            _headerArray = new byte[HeaderSize];
            _tcpClient.GetStream().BeginRead(_headerArray, 0, HeaderSize, ReceivedHeader, null);
        }

        /// <summary>
        /// Convert arbitray size of byte array to ulong
        /// </summary>
        /// <param name="array">Byte array</param>
        /// <returns>ulong</returns>
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
