using System;
using System.Net;
using System.Net.Sockets;
using Qplex.Communication.Handlers;
using Qplex.Messages.Networking;

namespace Qplex.Networking.Tcp
{
    public class TcpConnection : Broadcaster, IConnection
    {
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
// ReSharper disable once FieldCanBeMadeReadOnly.Local
        private byte[] _headerArray;

        /// <summary>
        /// Message array
        /// </summary>
        private byte[] _messageArray;

        /// <summary>
        /// Constructor used by listener
        /// </summary>
        /// <param name="tcpClient">Tcp client</param>
        /// <param name="headerSize">Header size</param>
        public TcpConnection(TcpClient tcpClient, int headerSize)
        {
            _tcpClient = tcpClient;
            _headerArray = new byte[headerSize];
            
            //Start receiving
            _tcpClient.GetStream().BeginRead(_headerArray, 0, _headerArray.Length, ReceivedHeader, null);
        }

        /// <summary>
        /// Constructor used by client
        /// </summary>
        /// <param name="ip">Ip</param>
        /// <param name="port">Port</param>
        /// <param name="headerSize">Header</param>
        public TcpConnection(IPAddress ip, int port, int headerSize)
        {
            _ip = ip;
            _port = port;
            _tcpClient = new TcpClient();
            _headerArray = new byte[headerSize];

            //Start receiving
            _tcpClient.GetStream().BeginRead(_headerArray, 0, _headerArray.Length, ReceivedHeader, null);
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            if (_tcpClient.Connected || _ip == null || _port == 0)
            {
                //TODO: Log
            }
            else
            {
                _tcpClient.Connect(_ip, _port);
            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            _tcpClient.Close();
        }

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        /// <param name="buffer">Buffer</param>
        public void Send(byte[] buffer)
        {
            _tcpClient.GetStream().BeginWrite(buffer, 0, buffer.Length, SendComplete, null);
        }

        /// <summary>
        /// Async sending completed
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void SendComplete(IAsyncResult asyncResult)
        {
            //TODO: Log
            _tcpClient.GetStream().EndWrite(asyncResult);
        }

        /// <summary>
        /// Received header
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void ReceivedHeader(IAsyncResult asyncResult)
        {
            _messageArray = new byte[ConvertLittleEndian(_headerArray)];
            _tcpClient.GetStream().EndRead(asyncResult);

            //Receive the message
            _tcpClient.GetStream().BeginRead(_messageArray, 0, _messageArray.Length, ReceivedMessage, null);
        }

        /// <summary>
        /// Received message
        /// </summary>
        /// <param name="asyncResult">Async result</param>
        private void ReceivedMessage(IAsyncResult asyncResult)
        {
            //TODO: Log
            _tcpClient.GetStream().EndRead(asyncResult);
            Broadcast(new BufferReceivedMessage(_messageArray));

            //Start receiving
            _tcpClient.GetStream().BeginRead(_headerArray, 0, _headerArray.Length, ReceivedHeader, null);
        }

        /// <summary>
        /// Convert any size of byte array to ulong
        /// </summary>
        /// <param name="array"></param>
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
    }
}
