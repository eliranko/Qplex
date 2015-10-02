using System;
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
        /// Header array
        /// </summary>
// ReSharper disable once FieldCanBeMadeReadOnly.Local
        private byte[] _headerArray;

        /// <summary>
        /// Message array
        /// </summary>
        private byte[] _messageArray;

        /// <summary>
        /// Ctor
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
        /// Send bytes over socket
        /// </summary>
        /// <param name="bytes">Bytes</param>
        public void Send(byte[] bytes)
        {
            _tcpClient.GetStream().BeginWrite(bytes, 0, bytes.Length, SendComplete, null);
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
            Broadcast(new SerializedMessageReceivedMessage(_messageArray));

            //Start receiving
            _tcpClient.GetStream().BeginRead(_headerArray, 0, _headerArray.Length, ReceivedHeader, null);
        }

        /// <summary>
        /// Convert any size of byte array to ulong
        /// </summary>
        /// <param name="array"></param>
        /// <returns>ulong</returns>
        private ulong ConvertLittleEndian(byte[] array)
        {
            var pos = 0;
            ulong result = 0;
            foreach (var by in array)
            {
                result |= (ulong)(by << pos);
                pos += 8;
            }
            return result;
        }
    }
}
