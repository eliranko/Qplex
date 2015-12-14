using System.Net;

namespace Qplex.Messages.Networking.Parser
{
    /// <summary>
    /// New message received from IConnection
    /// </summary>
    public class NewIncomingMessage : Message
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Local end point the buffer was delivered too
        /// </summary>
        public IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Remote end point the buffer was delivered from
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public NewIncomingMessage(Message message, IPEndPoint localEndPoint, IPEndPoint remotEndPoint)
        {
            Message = message;
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remotEndPoint;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var newMessage = obj as NewIncomingMessage;
            return newMessage != null && Message.Equals(newMessage.Message);
        }

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"NewIncomingMessage received from remote {RemoteEndPoint.Address}:{RemoteEndPoint.Port} to local {LocalEndPoint.Address}:{LocalEndPoint.Port} with message: {Message}";
        }
    }
}
