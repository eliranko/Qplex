using System.Net;

namespace Qplex.Messages.Networking.Listener
{
    /// <summary>
    /// New protocol created message
    /// </summary>
    public class NewProtocolMessage : Message
    {
        /// <summary>
        /// Local ip end point of the connection
        /// </summary>
        public IPEndPoint LocalIpEndPoint { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="localIpEndPoint">Local end point</param>
        public NewProtocolMessage(IPEndPoint localIpEndPoint)
        {
            LocalIpEndPoint = localIpEndPoint;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"NewProtocolMessage with end point {LocalIpEndPoint.Address}:{LocalIpEndPoint.Port}";
        }
    }
}
