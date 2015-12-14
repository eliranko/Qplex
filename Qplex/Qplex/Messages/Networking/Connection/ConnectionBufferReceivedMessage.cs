using System.Net;
using Qplex.Networking.Connection;

namespace Qplex.Messages.Networking.Connection
{
    /// <summary>
    /// New buffer received over socket message
    /// </summary>
    public class ConnectionBufferReceivedMessage : Message
    {
        /// <summary>
        /// Socket status
        /// </summary>
        public ConnectionSocketStatus ConnectionSocketStatus { get; }

        /// <summary>
        /// Buffer
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Local end point the buffer was delievered too
        /// </summary>
        public IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Remote end point
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionSocketStatus">Connection socket status</param>
        /// <param name="buffer">Buffer received</param>
        /// <param name="localEndPoint">Local end point</param>
        /// <param name="remotEndPoint">Remote end point</param>
        public ConnectionBufferReceivedMessage(ConnectionSocketStatus connectionSocketStatus, byte[] buffer,
            IPEndPoint localEndPoint, IPEndPoint remotEndPoint)
        {
            ConnectionSocketStatus = connectionSocketStatus;
            Buffer = buffer;
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remotEndPoint;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = $"ConnectionBufferReceivedMessage with status {ConnectionSocketStatus}";
            if (LocalEndPoint != null)
                str += $" and local end point {LocalEndPoint.Address}:{LocalEndPoint.Port}";
            if (RemoteEndPoint != null)
                str += $" and remote end point {RemoteEndPoint.Address}:{RemoteEndPoint.Port}";

            return str;
        }
    }
}
